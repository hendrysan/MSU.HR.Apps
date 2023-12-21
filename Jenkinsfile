pipeline {
	agent any

	stages {
		stage('Build Solution Apps'){
			steps {
				sh '''
				dotnet build Solution.sln
				'''
			}
		}
		stage('Scanning Sonar'){
			steps {
				sh '''
				cd Solution
				sonar-scanner  
					-Dsonar.projectKey=HRIS  
					-Dsonar.source=. 
					-Dsonar.host.url=http://103.171.164.79:9000 
					-Dsonar.login=sqp_d82cdfad9c90665937de20522890364e8a5523a9
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