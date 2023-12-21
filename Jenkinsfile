pipeline {
	agent any

	stages {
		stage('Get Tools Sonar Scanner Apps'){
			steps {
				sh '''
				sudo dotnet tool install --global dotnet-sonarscanner
				'''
			}
		}
		stage('Begin Sonar Scanner Apps'){
			steps {
				sh '''
				sudo dotnet sonarscanner begin /k:"HRIS" /d:sonar.host.url="http://103.171.164.79:9000"  /d:sonar.login="sqp_d82cdfad9c90665937de20522890364e8a5523a9"
				'''
			}
		}
		stage('Build Solution Apps'){
			steps {
				sh '''
				sudo dotnet build Solution.sln
				'''
			}
		}
		stage('End Sonar Scanner Apps'){
			steps {
				sh '''
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