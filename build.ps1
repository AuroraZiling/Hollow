param(
    [string] $Version = "0.0.1.0"
)

$ErrorActionPreference = "Stop";

Write-Output "Start building withRuntime...";

dotnet publish Hollow/Hollow.csproj -o "build/$Version/withRuntime" -p:EnableCompressionInSingleFile=true -p:PublishSingleFile=true -p:Platform=$Architecture -p:PublishReadyToRun=true -p:SelfContained=true -p:AssemblyVersion=$Version -p:Configuration=Release;

Rename-Item -Path "build/$Version/withRuntime/Hollow.exe" -NewName "Hollow_withRuntime.exe"

Write-Output "Start building withoutRuntime...";

dotnet publish Hollow/Hollow.csproj -o "build/$Version/withoutRuntime" -p:PublishSingleFile=true -p:Platform=$Architecture -p:PublishReadyToRun=true -p:SelfContained=false -p:AssemblyVersion=$Version -p:Configuration=Release;

Write-Output "Build Finished";

[Console]::ReadKey()