@echo off

cd Mp3CoverDroper.Extension\build
regasm /u Mp3CoverDroper.Extension.dll
cd ..\..

@echo on
