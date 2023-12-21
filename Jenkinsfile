pipeline {
	agent any

	stages {
		stage('Build Solution Apps'){
			steps {
				sh '''
				sudo dotnet build Solution.sln
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