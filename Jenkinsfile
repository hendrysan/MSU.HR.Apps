pipeline {
	agent any

	stages {
		stage('Build All Projects'){
			steps {
				sh '''cd Solution
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