<#
.DESCRIPTION
	Clear, rebuild, publish and run n number of ML host.  Used for testing ML Frontend
#>

Param(
    [switch] $Rebuild,
	
	[string] $Configuration = "debug",

	[switch] $All,
	[switch] $RunIdcard,
	[switch] $RunIntent,
	[switch] $RunEmotional,
	[switch] $RunFrontEnd
)

$debugpreference = "Continue";
$ErrorActionPreference = "Stop";

if( $Rebuild )
{
	Write-Host "Rebuild all release and public fake server..."
	Write-Host "***"
	Write-Host "***"

	Get-ChildItem ..\..\ -Directory -Include bin,obj -Recurse | Remove-Item -Recurse -Force;
	& dotnet build ..\..\MlHost.sln --configuration $Configuration;
}

Write-Host "Starting ML services..."
Write-Host "***"
Write-Host "***"

if( $All )
{
	$RunIdcard = $true;
	$RunIntent = $true;
	$RunEmotional = $true;
	$RunFrontEnd = $true;
}

$cmds = $(
	$($RunIdcard, '.\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\idcard-classifier-v1\idcard-classifier-v1a.mlPackage" -Port 5010 -ModelPort 5020'),
	$($RunIntent, '.\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\intent-classifier-v3\intent-classifier-v1a.mlPackage" -Port 5011 -ModelPort 5021'),
	$($RunEmotional, '.\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\emotional-sentiment-v1\emotional-sentiment-v1a.mlPackage" -Port 5012 -ModelPort 5004'),
	$($RunFrontEnd, '.\Start-MlFrontEnd.ps1 -Port 5015')
)

foreach($item in $cmds)
{
	if( $item[0] )
	{
		$exeCmd = "-NoExit -File $($item[1])";
		Start-Process -File "pwsh" -ArgumentList $exeCmd;
	}
}

# if( $RunIdcard )
# {
# 	Start-Process -File "pwsh" -ArgumentList '-NoExit -File .\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\idcard-classifier-v1\idcard-classifier-v1a.mlPackage" -Port 5010 -ModelPort 5020';
# }

# if( $RunIdcard )
# {
# Start-Process -File "pwsh" -ArgumentList '-NoExit -File .\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\intent-classifier-v3\intent-classifier-v1a.mlPackage" -Port 5011 -ModelPort 5021';
# }

# if( $RunIdcard )
# {
# Start-Process -File "pwsh" -ArgumentList '-NoExit -File .\Start-ModelHost.ps1 -PackageFile "D:\ML-Models\V2-Packages\emotional-sentiment-v1\emotional-sentiment-v1a.mlPackage" -Port 5012 -ModelPort 5004';
# }

# if( $RunIdcard )
# {
# Start-Process -File "pwsh" -ArgumentList '-NoExit -File .\Start-MlFrontEnd.ps1 -Port 5015';
# }
