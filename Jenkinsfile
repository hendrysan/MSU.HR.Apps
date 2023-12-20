pipeline {
	agent any

	stages {
		stage('Build Apps WebClient'){
			steps {
				sh '''cd Solution/WebClient
				 dotnet build
				'''
			}
		}

		stage('Test Apps'){
			steps {
				echo "ini step test apps"
			}
		}

		stage('Build Images'){
			steps {
				echo "ini step build images"
			}
		}

		stage('Deploy Apps'){
			steps {
				sh '''cd Solution
				'''
			}
		}

	}
}