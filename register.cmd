@echo off

:: Build service dll
cd services
rm bin/ obj/ -rf

dotnet publish -c Release

:: Generate Key
cd ./bin/Release/net48/
sn -k key.snk

:: ReCompile
ildasm Mp3CoverDroper.dll /OUTPUT=Mp3CoverDroper.il
ilasm Mp3CoverDroper.il /DLL /OUTPUT=Mp3CoverDroper.dll /KEY=key.snk

:: Backup dll
cp ./Mp3CoverDroper.dll ./../../../build/Mp3CoverDroper.dll
cp ./publish/SharpShell.dll ./../../../build/SharpShell.dll
cd ../../..

:: Register
regasm /codebase ./build/Mp3CoverDroper.dll

cd ..

@echo on