<#
.DESCRIPTION
	Execute MlHostCli to download a ML Model from ADLS

.PARAMETER Environment
	Name of the environment, "dev", "acpt", "prod"

.PARAMETER SwaggerPath
	Path to the swagger json file.  Will read "title" and "x-apiPath" from this file.

.PARAMETER ProductId
	Azure API Management product name
#>

Param(
    [Parameter(Mandatory)]
    [ValidateSet("dev", "acpt", "prod")]
    [string] $Environment,

    [Parameter(Mandatory)]
    [ValidateSet("Emotion", "Intent", "Sentiment")]
    [string] $Model,

    [Parameter(Mandatory)]
    [string] $Question
)

#$debugpreference = "Continue";
$ErrorActionPreference = "Stop";
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 

$environments = @{
    "dev" = @{
        "Intent" = @{
            "DirectUri" = "https://dev-va-ml-host-emotion.ase-mobile.dev.premera.net/api/question"
            "Uri" = "https://apim.ag.dev.premera.net/ML-Intent/api/question"
            "SubscriptionKey" = "ed21210fc5d84f638d7f4cab8bb52e5c"
        }
    }
}

$modelContext = $environments[ $Environment ][ $Model ];

$body = @{ "sentence" = $Question } | ConvertTo-Json;

$headers = @{
    "Ocp-Apim-Subscription-Key" = $modelContext.SubscriptionKey
}

Invoke-RestMethod -Method Post -Headers $headers -ContentType "application/json" -Uri $modelContext.Uri -Body $body | ConvertTo-Json;

Write-Host "Completed";
