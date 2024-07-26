param(
    [string] $Version = "0.0.4.0"
)

$ErrorActionPreference = "Stop";

Write-Output "Start building launcher...";

cargo build --manifest-path .\hollow-launcher\Cargo.toml -r

Write-Output "Start building withRuntime...";

dotnet publish Hollow.Windows/Hollow.Windows.csproj -o "build/$Version/withRuntime/hollow_app" -r win-x64 -p:SelfContained=true -p:AssemblyVersion=$Version -p:Configuration=Release;

Write-Output "Removing unused files of withRuntime...";

Remove-Item -Path ./build/$Version/withRuntime/hollow_app/Microsoft.Web.WebView2.Core.xml
Remove-Item -Path ./build/$Version/withRuntime/hollow_app/Microsoft.Web.WebView2.WinForms.dll
Remove-Item -Path ./build/$Version/withRuntime/hollow_app/Microsoft.Web.WebView2.WinForms.xml
Remove-Item -Path ./build/$Version/withRuntime/hollow_app/Microsoft.Web.WebView2.Wpf.dll
Remove-Item -Path ./build/$Version/withRuntime/hollow_app/Microsoft.Web.WebView2.Wpf.xml

Copy-Item -Path ".\hollow-launcher\target\release\Hollow.exe" -Destination ".\build\$Version\withRuntime\Hollow.exe"

Write-Output "Start building withoutRuntime...";

dotnet publish Hollow.Windows/Hollow.Windows.csproj -o "build/$Version/withoutRuntime/hollow_app" -r win-x64 -p:SelfContained=false -p:AssemblyVersion=$Version -p:Configuration=Release;

Write-Output "Removing unused files of withoutRuntime...";

Remove-Item -Path ./build/$Version/withoutRuntime/hollow_app/Microsoft.Web.WebView2.Core.xml
Remove-Item -Path ./build/$Version/withoutRuntime/hollow_app/Microsoft.Web.WebView2.WinForms.dll
Remove-Item -Path ./build/$Version/withoutRuntime/hollow_app/Microsoft.Web.WebView2.WinForms.xml
Remove-Item -Path ./build/$Version/withoutRuntime/hollow_app/Microsoft.Web.WebView2.Wpf.dll
Remove-Item -Path ./build/$Version/withoutRuntime/hollow_app/Microsoft.Web.WebView2.Wpf.xml

Copy-Item -Path ".\hollow-launcher\target\release\Hollow.exe" -Destination ".\build\$Version\withoutRuntime\Hollow.exe"

Write-Output "Build Finished";

[Console]::ReadKey()