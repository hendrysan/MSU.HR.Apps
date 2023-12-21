pipeline {
	agent any

	stages {
	
		stage('Sonar Scanner Apps'){
			steps {
				sh '''
				dotnet tool install --global dotnet-sonarscanner
				dotnet sonarscanner begin /k:"HRIS" /d:sonar.host.url="http://103.171.164.79:9000"  /d:sonar.login="sqp_d82cdfad9c90665937de20522890364e8a5523a9"
				dotnet build Solution.sln
				dotnet sonarscanner end /d:sonar.login="sqp_d82cdfad9c90665937de20522890364e8a5523a9"
				'''
			}
		}
		stage('Compose Runner'){
			steps {
				sh '''
				sudo docker-compose -f "Solution/docker-compose.yml" up --build -d
				'''
			}
		}

	}
}