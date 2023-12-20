pipeline {
	agent {
		node {
			label: 'devops-agent'
		}
	}

	stages {
		stage('Build Apps'){
			steps {
				echo "ini step build apps"
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
				echo "ini step deploy apps"
			}
		}

	}
}