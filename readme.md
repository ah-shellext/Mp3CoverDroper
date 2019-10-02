# Mp3CoverDroper
+ Windows Shell Extension
+ Personal tool to update mp3 cover through drag and drop files

### Environment
+ `.NET Core cli 2.2.401`
+ `.NET Framework 4.8`
+ `VS Code`
+ `Windows 10 Version 1803`

### Path
+ `Program Files\Microsoft SDKs\Windows\vx.xA\bin\NETFX x.x.x Tools` -> `sn.exe` / `ildasm.exe`
+ `Windows\Microsoft.NET\Framework64\vx.x.x` -> `ilasm.exe` / `regasm.exe`
+ ( `PATH` -> `Mp3CoverDroperApp.exe` )

### Depedences
+ Service

```bash
# Add depedence
dotnet add package SharpShell
```

+ App

```bash
# Add depedence
dotnet add package ID3
dotnet add package System.Windows.Forms
dotnet add package System.Drawing.Common
```

### Build
+ Service

```bash
# register.cmd

cd services

# Build service dll
rm bin/ obj/ -rf
dotnet publish -c Release

# Generate Key
cd ./bin/Release/net48/
sn -k key.snk

# ReCompile
ildasm Mp3CoverDroper.dll /OUTPUT=Mp3CoverDroper.il
ilasm Mp3CoverDroper.il /DLL /OUTPUT=Mp3CoverDroper.dll /KEY=key.snk

# Backup dll
cp ./Mp3CoverDroper.dll ./../../../build/Mp3CoverDroper.dll
cp ./publish/SharpShell.dll ./../../../build/SharpShell.dll
cd ../../../build

# Register
regasm /codebase ./Mp3CoverDroper.dll
```

+ App

```bash
# build.cmd

cd app

# Build app exe
rm bin/ obj/ -rf
dotnet publish -c Release

# Move ./bin/Release/net48/Mp3CoverDroperApp.exe to PATH
# Or modify DropHandlerService.cs app_path variable to the path of Mp3CoverDroperApp.exe
```

### UnRegister
```bash
# unregister.cmd

regasm /u ./services/build/Mp3CoverDroper.dll

# Restart explorer.exe
```

### Problem
+ Nuget package `ID3` could not add to GAC directly -> couldn't use `ID3` directly in `SharpDropHandler`

![dllException](./assets/dllException.jpg)

### ScreenShot

![desktop](./assets/desktop.jpg) ![dialog](./assets/dialog.jpg)

### References
+ [Shell Drop Handlers](http://www.codeproject.com/Articles/529515/NET-Shell-Extensions-Shell-Drop-Handlers)
+ [Id3](https://github.com/JeevanJames/Id3)
+ [C#自定义MessageBox 按钮的Text](https://www.cnblogs.com/code1992/p/9719856.html)
+ [installing.md#install-using-the-gac](https://github.com/dwmkerr/sharpshell/blob/master/docs/installing/installing.md#install-using-the-gac)
+ [sharpshell-issue](https://github.com/dwmkerr/sharpshell/issues/278)