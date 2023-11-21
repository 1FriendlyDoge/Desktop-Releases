cd /d %~dp0
dotnet publish -r win-x64 --configuration Release -p:UseAppHost=true --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
echo Built Windows.
dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64 -property:Configuration=Release -p:UseAppHost=true
echo Built MacOS.