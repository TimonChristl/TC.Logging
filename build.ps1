#  _____ ____   _                      _
# |_   _/ ___| | |    ___   __ _  __ _(_)_ __   __ _
#   | || |     | |   / _ \ / _` |/ _` | | '_ \ / _` |
#   | || |___ _| |__| (_) | (_| | (_| | | | | | (_| |
#   |_| \____(_)_____\___/ \__, |\__, |_|_| |_|\__, |
#                          |___/ |___/         |___/
# Build file

$ErrorActionPreference="Stop"

#         ↓↓↓↓↓
$VERSION="2.0.1"

$BUILD_NUMBER = [System.Environment]::GetEnvironmentVariable('BUILD_NUMBER')
if([String]::IsNullOrEmpty($BUILD_NUMBER)) { $BUILD_NUMBER=999 }
$FULL_VERSION = "$VERSION.$BUILD_NUMBER"

$NUGET_SOURCE="gitea NuGet"
$GITEA_API_KEY = [System.Environment]::GetEnvironmentVariable('GITEA_API_KEY')
if([String]::IsNullOrEmpty($GITEA_API_KEY)) { Write-Output "Environment variable GITEA_API_KEY not set or empty"; exit 1 }

Push-Location src
try {
    dotnet clean
    if(!$?) { exit 1 }

    dotnet restore
    if(!$?) { exit 1 }

    dotnet build /p:Configuration=Release /p:Version=$FULL_VERSION
    if(!$?) { exit 1 }

    dotnet pack /p:Configuration=Release /p:Version=$FULL_VERSION /p:PackageVersion=$VERSION
    if(!$?) { exit 1 }

    dotnet nuget push --source $NUGET_SOURCE --api-key=$GITEA_API_KEY "TC.Logging\bin\Release\TC.Logging.$VERSION.nupkg"
    if(!$?) { exit 1 }
}
finally
{
    Pop-Location
}
