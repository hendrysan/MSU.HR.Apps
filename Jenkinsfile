pipeline {
	agent any

	stages {
		
		stage('Scanning Sonar'){
			steps {
				sh '''
				Solution/sonnar-scanner.sh
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