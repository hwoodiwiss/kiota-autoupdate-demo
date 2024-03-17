dotnet run --project .\src\AutoupdateDemoApi\AutoupdateDemoApi.csproj --launch-profile "https" &
kiota generate -d http://localhost:50000/swagger/v1/swagger.json -l CSharp -o .\src\AutoupdateDemoApi.Client\generated -n AutoupdateDemoApi.Client -c AutoupdateDemoApiClient --clean-output
git --no-pager diff --exit-code -- . :^**/kiota-lock.json
echo $LASTEXITCODE
