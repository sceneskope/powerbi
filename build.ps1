[CmdletBinding(PositionalBinding=$false)]
param(
    [string]$BuildNumber="dev",
    [bool] $CreatePackages=$true,
    [bool] $RunTests = $true
)

Write-Host "BuildNumber: $BuildNumber"
Write-Host "CreatePackages: $CreatePackages"
Write-Host "RunTests: $RunTests"

Get-ChildItem -Recurse -Filter bin | Remove-Item -Recurse
Get-ChildItem -Recurse -Filter obj | Remove-Item -Recurse

$packageOutputFolder = "$PSScriptRoot\.nupkgs"

$semVer = Get-Content (Join-Path $PSScriptRoot "semver.txt")
$version = "$semVer-$BuildNumber"

Write-Host "Restore"
dotnet restore "/p:Version=$version"

if ($RunTests) {
    Write-Host "Running tests"
    Push-Location "$PSScriptRoot\PowerBIClientTests\"
    dotnet test "/p:Version=$version"
    Pop-Location
}

Write-Host "Building"
dotnet build -c Release "/p:Version=$version"


if ($CreatePackages) {
    Write-Host "Packing"
    mkdir -Force $packageOutputFolder | Out-Null

    Get-ChildItem $packageOutputFolder | Remove-Item
    dotnet pack --no-build -c Release "/p:Version=$version"
}
Write-Host "Complete"

