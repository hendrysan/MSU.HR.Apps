dotnet tool update dotnet-sonarscanner --global
export PATH="$PATH:/root/.dotnet/tools"
dotnet sonarscanner begin /k:"HRIS" /d:sonar.host.url="http://103.171.164.79:9000"  /d:sonar.login="sqp_d82cdfad9c90665937de20522890364e8a5523a9" 
dotnet build ./Solution.sln 
dotnet sonarscanner end /d:sonar.login="sqp_d82cdfad9c90665937de20522890364e8a5523a9" 