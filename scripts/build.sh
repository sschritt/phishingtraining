#!/bin/bash
cd /srv/phishingtraining/git
git checkout master
# Replace user:password with credentials from: Dev.kdbx 518E51021BA96E4CBF14E20AE20C0983
git pull https://user:password@code.sba-research.org/Developers/phishing-training.git
cd /srv/phishingtraining/git/src
docker-compose --file docker-compose.yml --file docker-compose.production.yml --project-name phishing-training-prod build --pull --no-cache --parallel --force-rm --build-arg sbaNugetPassword="gLDgVFNyWnJjJoGHZVXE3c3uBc4mXFXbidUx9HqK"
docker image prune -f
