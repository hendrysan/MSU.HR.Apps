pipeline {
	agent any

	stages {
		
		stage('Scanning Sonar'){
			steps {
				sh '''
				cd Solution
				chmod +x sonarscanner.sh
				sudo sonarscanner.sh
				cd ..
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