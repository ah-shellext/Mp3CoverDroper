@echo off

:: Build Mp3CoverDroperApp.exe
rm app/bin/ app/obj/ -rf
dotnet publish app/ -c Release

:: Move ./bin/Release/net48/Mp3CoverDroperApp.exe to PATH
:: Or Modify DropHandlerService.cs app_path variable to the path of Mp3CoverDroperApp.exe

@echo on
