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
	[Parameter(Mandatory)]
	[int] $Port,
	
	[string] $Configuration = "debug"
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

$frontEndBin = [System.IO.Path]::Combine($currentLocation, "..\..\MlFrontEnd\bin\$Configuration\netcoreapp3.1\MlFrontEnd.exe");

if( !(Test-Path $frontEndBin) )
{
	Write-Error "Cannot find $frontEndBin";
	return;
}

& $frontEndBin Port=$Port;

