<#
.DESCRIPTION
	Build swagger and deploy API to APIM

.PARAMETER ModelName
	Name of the model (lower alpha numeric, '-').  Example: "sentiment"

.PARAMETER Environment
	Name of environment, valid names are "dev", "acpt", "prod"

.PARAMETER CliBinPath
	Path to MlHostCli executable path
#>

Param(
	[Parameter(Mandatory)]
	[string] $ModelName,

    [Parameter(Mandatory)]
    [ValidateSet("dev", "acpt", "prod")]
	[string] $Environment,

	[string] $CliBinPath
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
$currentLocation = $PSScriptRoot;

if( !$CliBinPath )
{
	$CliBinPath = [System.IO.Path]::Combine($currentLocation, "..\MlHostCli.exe");
}

if( !(Test-Path $CliBinPath) )
{
	Write-Error "Cannot find MlHostCli.exe at $CliBinPath for deployment";
	return;
}

$swaggerFile = "$Environment-$ModelName.swagger.json";

Write-Host "Creating swagger";
& $currentLocation\New-Swagger.ps1 -ModelName $ModelName -Environment $Environment -SwaggerFile $SwaggerFile -CliBinPath $CliBinPath;

Write-Host "Deploying Model API to APIM";
& $currentLocation\Deploy-ApiManagementApi.ps1 -Environment $Environment -SwaggerPath $SwaggerFile;

Write-Host "Completed deploying ML Model API";
