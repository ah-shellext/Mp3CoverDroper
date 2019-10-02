@echo off

:: Build app exe
cd ../app
rm bin/ obj/ -rf
dotnet publish -c Release

:: Move ./bin/Release/net48/Mp3CoverDroperApp.exe to PATH
:: Or Modify DropHandlerService.cs app_path variable to the path of Mp3CoverDroperApp.exe

@echo on