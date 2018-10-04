DEBUG_FLAG = Debug


.PHONY: configure build debug

configure:
	dotnet restore

build:
	dotnet build

debug: build
	dotnet run -c $(DEBUG_FLAG)