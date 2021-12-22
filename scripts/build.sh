#!/bin/bash
cd /srv/phishingtraining/git
git checkout master
# Replace user:password with your credentials and your git repo
git pull https://user:password@URL/phishing-training.git
cd /srv/phishingtraining/git/src
docker-compose --file docker-compose.yml --file docker-compose.production.yml --project-name phishing-training-prod build --pull --no-cache --parallel --force-rm
docker image prune -f