using System;
using System.Linq;
using System.Collections.Generic;

using Markdig.Syntax;
using Inlines = Markdig.Syntax.Inlines;
using System.IO;

namespace punk_tex_backend.Utils
{
    public static class Markdown
    {
        static Dictionary<Type, Func<Block, string>> KnownBlocks = new Dictionary<Type, Func<Block, string>>() {
            { typeof(HeadingBlock), (Block block) => {
                var heading = block as HeadingBlock;
                string subword;
                switch (heading.Level) {
                    case 1:
                        subword = "section";
                        break;
                    case 2:
                        subword = "subsection";
                        break;
                    default:
                        subword = "subsubsection";
                        break;
                }
                return $"\\{subword}{{{ProcessInline(heading.Inline)}}}\n";
            } },
            { typeof(ParagraphBlock), (Block block) => {
                var paragraph = block as ParagraphBlock;
                if (paragraph.Parent is MarkdownDocument)
                    return ProcessInline(paragraph.Inline) + "\n\n";
                return ProcessInline(paragraph.Inline);
            } },
            { typeof(ListBlock), (Block block) => {
                var list = block as ListBlock;
                string body = string.Empty, type = "enumerate";
                switch (list.BulletType) {
                    case '+':
                    case '-':
                    case '*':
                        type = "itemize";
                        break;
                    default:
                        break;
                }
                
                foreach (ListItemBlock item in list.Descendants().OfType<ListItemBlock>()) {
                    body += $"\\item {KnownBlocks[item.GetType()].Invoke(item)}\n";
                }
                return $"\\begin{{{type}}}[]\n{body}\\end{{{type}}}\n\n";
            } },
            { typeof(ListItemBlock), (Block block) => {
                var item = block as ListItemBlock;
                string body = string.Empty;
                foreach (MarkdownObject s in item.Descendants().OfType<ParagraphBlock>())
                    body += KnownBlocks[s.GetType()].Invoke(s as ParagraphBlock);
                return body;
            } },
            { typeof(ThematicBreakBlock), (Block block) => {
                return "\\par\\vspace{-.5\\ht\\strutbox}\\noindent\\hrulefill\\par\n";
            } },
            { typeof(FencedCodeBlock), (Block block) => {
                var code = block as FencedCodeBlock;
                string body = string.Empty;
                for (int i = 0; i < code.Lines.Count; ++i)
                    body += code.Lines.Lines[i].Slice.ToString() + '\n';
                    
                return $"\\begin{{lstlisting}}\n{body}\n\\end{{lstlisting}}\n\n";
            } },
            { typeof(QuoteBlock), (Block block) => {
                var quote = block as QuoteBlock;
                return $"\\begin{{quote}}\n{ProcessBlock(quote)}\n\\end{{quote}}\n\n";
            } },
        };

        static Dictionary<Type, Func<Inlines.Inline, string>> KnownInlines = new Dictionary<Type, Func<Inlines.Inline, string>>() {
            { typeof(Inlines.LiteralInline), (Inlines.Inline i) => i.ToString() },
            { typeof(Inlines.EmphasisInline), (Inlines.Inline i) => {
                var emph = i as Inlines.EmphasisInline;
                string action, inner = string.Empty;
                switch(emph.DelimiterChar) {
                    case '_':
                    case '*':
                        if (emph.IsDouble) {
                            action = "textbf";
                            break;
                        }
                        goto default;
                    case '~':
                        action = "sout";
                        break;
                    default:
                        action = "emph";
                        break;
                }

                inner += ProcessInlineElem(emph.FirstChild);

                return $"\\{action}{{{inner}}}";
            } },
            { typeof(Inlines.HtmlInline), (Inlines.Inline i) => {
                // ! This is here to prevent HTML faggotry.
                return string.Empty;
            } },
            { typeof(Inlines.LineBreakInline), (Inlines.Inline i) => {
                return "\\\\";
            } },
            { typeof(Inlines.LinkInline), (Inlines.Inline i) => {
                var link = i as Inlines.LinkInline;
                return $"\\href{{{link.Url}}}{{{ProcessInlineElem(link.FirstChild)}}}";
            } },
            { typeof(Inlines.CodeInline), (Inlines.Inline i) => {
                var code = i as Inlines.CodeInline;
                return $"\\hl{{{code.Content}}}";
            } },
        };

        static string ProcessInline(IEnumerable<Inlines.Inline> e) {
            if (e == null)
                return string.Empty;

            string result = string.Empty;
            foreach (var b in e) {
                var type = b.GetType();
                if (KnownInlines.ContainsKey(type)) {
                    result += KnownInlines[type].Invoke(b);
                } else {
                    Console.WriteLine($"% WARNING: inline element not handled ({type})");
                }
            }
            return result;
        }

        static string ProcessInlineElem(Inlines.Inline inline) {
            string result = string.Empty;
            while (inline != null) {
                var type = inline.GetType();
                if (KnownInlines.ContainsKey(type)) {
                    result += KnownInlines[type].Invoke(inline);
                } else {
                    Console.WriteLine($"% WARNING: inline element not handled ({type})");
                }
                inline = inline.NextSibling;
            }
            return result;
        }

        static string ProcessBlock(IEnumerable<Block> blocks) {
            string result = string.Empty;
            foreach (Block b in blocks) {
                var type = b.GetType();
                if (KnownBlocks.ContainsKey(type)) {
                    result += KnownBlocks[type].Invoke(b);
                } else {
                    Console.WriteLine($"% WARNING: block element ({type}) not handled at {b.Line}-{b.Column}");
                }
            }
            return result;
        }

        public static void ToLatex(this MarkdownDocument markdown, TextWriter writer) {
            for (int i = 0; i < markdown.Count; ++i) {
                var mk = markdown[i];
                var type = mk.GetType();
                if (KnownBlocks.ContainsKey(type)) {
                    writer.Write(KnownBlocks[type].Invoke(mk));
                } else {
                    Console.WriteLine($"% WARNING: block element ({type}) not handled at {mk.Line}-{mk.Column}");
                }
            }
        }
    }
}