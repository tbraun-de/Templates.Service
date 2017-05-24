Param ([Parameter(Mandatory=$True)][string]$Version, [string]$Registry = "docker.axoom.cloud")
$ErrorActionPreference = "Stop"
$SolutionName = "MySolutionName"
$ImageName = "MyImageName"

pushd "$(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)/src"

dotnet msbuild /t:Restore /t:Build /t:Publish /p:PublishDir=./obj/Docker/publish /p:Configuration=Release /p:Version=$Version "$SolutionName.sln"
dotnet test --configuration Release --no-build "$SolutionName.UnitTests/$SolutionName.UnitTests.csproj"

docker build -t "$Registry/$MyImageName:$Version" .

popd