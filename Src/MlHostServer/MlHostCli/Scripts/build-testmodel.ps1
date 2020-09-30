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

Param(
    [string] $CliBinPath,

	[string] $SpecFile,

	[string] $PackageFile,

    [string] $ModelExePath
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

if( !$CliBinPath )
{
	$CliBinPath = [System.IO.Path]::Combine($currentLocation, "..\bin\Release\netcoreapp3.1\MlHostCli.exe");
}

if( !(Test-Path $CliBinPath) )
{
	Write-Error "Cannot find MlHostCli.exe at $CliBinPath";
	return;
}

if( !$SpecFile )
{
	$SpecFile = [System.IO.Path]::Combine($currentLocation, "..\..\MlHost\MlPackage\test-model.mlPackageSpec.json");
}

if( !$PackageFile )
{
	$PackageFile = [System.IO.Path]::Combine($currentLocation, "..\..\MlHost\MlPackage\RunModel.mlPackage");
}

if( !$ModelExePath )
{
	$ModelExePath = [System.IO.Path]::Combine($currentLocation, "..\..\..\Tools\FakeModelServer\bin\Release\netcoreapp3.1\win-x64\publish\FakeModelServer.exe");
}


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

& dotnet build ..\..\MlHost.sln --configuration release;
& dotnet publish ..\..\..\Tools\FakeModelServer\FakeModelServer.csproj --configuration release

& $CliBinPath Build SpecFile=$specFile;

