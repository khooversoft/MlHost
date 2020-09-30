<#
.DESCRIPTION
	Generate ML Test model to be used with intergration tests, uses fake model

.PARAMETER CliBinPath
	Path where ML Host CLI exceute is

.PARAMETER SpecFile
	Path where ML Package Specification file is store

.PARAMETER PackageFile
	Where to write ML Package

.PARAMETER ModelExePath
	Path of execute for fake model server
#>

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

# Clean all bin and obj folders in the ML Host Server projects



& dotnet build ..\..\MlHost.sln --configuration release;
& dotnet publish ..\..\..\Tools\FakeModelServer\FakeModelServer.csproj --configuration release

# ..\..\
Get-ChildItem ..\.. -Directory -Include bin,obj -Recurse | Remove-Item -Recurse -Force


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

