# OpenStoreUI Source - Compiling
To build and compile the app(Either edited or you just want to rebuild):
```pwsh
dotnet build -c Release /p:Platform=x64
dotnet publish -c Release -r win-x64 /p:Platform=x64 --self-contained true --no-restore -o ./publish
wix build Package.wxs -ext WixToolset.UI.wixext -o OpenStoreUI-Setup.msi
```
