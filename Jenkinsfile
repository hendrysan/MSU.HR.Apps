pipeline {
	agent any

	stages {
		stage('Build Solution Apps'){
			steps {
				sh '''
				dotnet build Solution/Solution.sln
				'''
			}
		}
		stage('Scanning Sonar'){
			steps {
				sh '''
				sudo cd Solution 
				sudo dotnet sonarscanner begin /k:"HRIS" /d:sonar.host.url="http://103.171.164.79:9000"  /d:sonar.login="sqp_d82cdfad9c90665937de20522890364e8a5523a9"
				sudo dotnet sonarscanner end /d:sonar.login="sqp_d82cdfad9c90665937de20522890364e8a5523a9"
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