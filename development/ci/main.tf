terraform {
  required_version = "~> 1.5"

  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "~> 5.12"
    }

    tls = {
      source  = "hashicorp/tls"
      version = "~> 4.0"
    }
  }
}

variable "runner_token" {
  type      = string
  sensitive = true
}

locals {
  google_project = "android-gitlab-runner-2"
  google_region  = "europe-west1"
  google_zone    = "europe-west1-b"
}

provider "google" {
  project = local.google_project
  region  = local.google_region
  zone    = local.google_zone
}


module "runner_deployment" {
  source = "git::https://gitlab.com/gitlab-org/ci-cd/runner-tools/grit.git//scenarios/google/linux/docker-autoscaler-default"

  google_project = local.google_project
  google_region  = local.google_region
  google_zone    = local.google_zone

  name = "ubss-android-2"

  runner_machine_type = "n2d-highmem-2"

  gitlab_url = "https://code.fki.htw-berlin.de"

  runner_token = var.runner_token
  runner_version = "18.0.2"

  autoscaling_policies  = [
    {
        periods            = ["* * * * *"]
        timezone           = ""
        scale_min          = 1
        idle_time          = "2m30s"
        scale_factor       = 0.2
        scale_factor_limit = 8
    }
  ]

 ephemeral_runner = {
   disk_size    = 50
 }


 runners_docker_section = <<EOS
   pull_policy = [
     "if-not-present"
   ]
 EOS


# uncomment for reuseable builds. they are much faster but have other issues (need lots of storage or a cleanup step)
# max_use_count = 100

}

output "runner_manager_external_ip" {
  value = module.runner_deployment.runner_manager_external_ip
}

