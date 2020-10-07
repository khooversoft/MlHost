<#
.DESCRIPTION
	CLear, build, publish, and build test ML model used for testing
#>

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

# ..\..\
Write-Host "Clear our all bin and obj folders in the ML Host Server projects..."
Get-ChildItem ..\..\ -Directory -Include bin,obj -Recurse | Remove-Item -Recurse -Force

Write-Host "Rebuild all release and public fake server..."
Write-Host "***"
Write-Host "***"

& dotnet build ..\..\MlHost.sln --configuration release;
& dotnet publish ..\..\..\Tools\FakeModelServer\FakeModelServer.csproj --configuration release

$CliBinPath = [System.IO.Path]::Combine($currentLocation, "..\bin\Release\netcoreapp3.1\MlHostCli.exe");
$SpecFile = [System.IO.Path]::Combine($currentLocation, "..\..\MlHost\MlPackage\test-model.mlPackageSpec.json");
$PackageFile = [System.IO.Path]::Combine($currentLocation, "..\..\MlHost\MlPackage\RunModel.mlPackage");
$ModelExePath = [System.IO.Path]::Combine($currentLocation, "..\..\..\Tools\FakeModelServer\bin\Release\netcoreapp3.1\win-x64\publish\FakeModelServer.exe");


# Write out spec json file
$spec = @{
    "PackageFile" = $PackageFile
    "Manifest" = @{
        "ModelName" = "test"
        "VersionId" = "fakeModel_v1"
        "RunCmd" = "FakeModelServer.exe"
        "StartSignal" = "Running on"
    }
    "Copy" = @(
        @{
            "Source" = $ModelExePath
            "Destination" = "FakeModelServer.exe"
        }
    )
}

$json = $spec | ConvertTo-Json;
$json | Out-File $specFile;

& $CliBinPath Build SpecFile=$specFile;

