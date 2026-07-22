# OpenStoreUI Source - Compiling
run `dotnet build -c Release /p:Platform=x64` inside powershell
then run `dotnet publish -c Release -r win-x64 /p:Platform=x64 --self-contained true --no-restore -o ./publish`
then `wix heat dir ./publish -cg PublishedFiles -dr INSTALLFOLDER -srd -gg -out Files.wxs` to make it a .msi
then completely make a .msi `wix build Package.wxs Files.wxs -ext WixToolset.UI.wixext -o OpenStoreUI-Setup.msi`

