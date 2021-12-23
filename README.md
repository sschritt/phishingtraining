# Phishingtraining

## Prerequisites

* Installed docker as this projects relies heavily on docker and docker compose for development and deployment.
* Configure [user secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets):

```json
{
  "Secrets": {
    "ConnectionStringDefaultConnectionPassword": "dockerPhishingTrainingAdminDevelopment_1010"
  }
}
```

You can do this by executing the following command inside the Developer PowerShell inside the project \src\Web: 

```dotnet user-secrets set "Secrets:ConnectionStringDefaultConnectionPassword" "dockerPhishingTrainingAdminDevelopment_1010"```

## Development settings

**Do not forget to change all passwords in production settings**

The Webapp settings can be found in \src\Web\appsettings.json and \src\Web\appsettings.Development.json.

Dockerfiles:
*  \src\Web\Dockerfile
*  \src\Infrastructure\..

The default user accounts and passwords are created inside the \src\Web\Jobs\DatabaseSeedingJob.cs:

* "admin", Roles.Admin, "VwE3AdadTta6L1Jodgfj"
* "manager", Roles.Manager, "3Rnk0hq5kAqoZ19eQdI6"
* "participant", Roles.Participant, "lGAVsNavtmIDOKyME0hP"

For sending SMS we used https://www.cm.com/. If you want to use it, you need provide an API key in appsettings.Development.json

```
"SmsProvider": {
    "ApiKey": "YOUR_KEY"
  },
  ```
## Run the project

Set the docker-compose project as startup project. Then by running everything (db, mailserver, proxy, backend) will be setup automatically.
Open https://localhost:41443

## Endpoint connections

* Database via SSMS connect to `localhost,41433`, docker internally `db:1433`
  * System Administrator user: sa pwd: dockerSqlExpressDevelopment_1040
  * phishingtraining user: phishingtraining_admin pwd: dockerPhishingTrainingAdminDevelopment_1010
* Mailserver via SMTP `localhost:41025`, docker internally `mail:25` and `mail:587`
* Backend is only docker internally exposed on `backend:5000`
* Reverse proxy via http and https on http://localhost:41080 https://localhost:41443

### Hangfire Dashboard

Here you can find the scheduled tasks for sending emails, sms, etc.
Add /hangfire to the base url (e.g., https://URL/hangfire). You need the administrator Role to open the Dashboard.

### Debugging steps

* By running `sudo docker ps` you can se the state of the running containers.
* By running `sudo docker logs phishing-training-prod_backend_1 | more` you can check the logs of the backend container. You can also check the logs of the other containers by using the other containernames.
* To execute commands on the database do something similar to this `sudo docker exec -it phishing-training-prod_db_1 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "thesapwdfrom/srv/phishingtraining/secrets/db_sa_password" -Q "SELECT 1"`
* Log files on the prod. server can be found at `/var/phishingtraining/`  (see docker-compose.production.yml) (Development: &AppData% = `...\AppData\Roaming\PhishingTraining\`)
* Log levels can be set in /srv/phishingtraining/secrets/backend_appsettings.Production.json (/run/secrets in the backend image)

## Production deployment

1. To build new containers from the latest commit to the master branch execute `sudo /srv/phishingtraining/build.sh`.
1. To replace the running containers with the new ones run `sudo docker-compose --file /srv/phishingtraining/git/src/docker-compose.yml --file /srv/phishingtraining/git/src/docker-compose.production.yml --project-name phishing-training-prod up --detach --no-build --remove-orphans`.

### Setting up the database

Prepare the database by connecting interactivly to the db with sqlcmd. Use the scripts from `src/Infrastructure/db-init`:

```bash
sudo docker exec -it phishing-training-prod_db_1 /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U SA -P "justforSetup_1"
```

Ensure the password for the phishingtraining_admin is the same as in file defined in `/srv/phishingtraining/secrets/backend_appsettings.Production.json`.

Change the SA users password with the following command:

```bash
sudo docker exec -it phishing-training-prod_db_1 /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U SA -P "justforSetup_1" \
   -Q 'ALTER LOGIN SA WITH PASSWORD="PWDfrom/srv/phishingtraining/secrets/db_sa_password"'
```

### Setting up letsencrypt

Use this [guide](https://medium.com/@pentacent/nginx-and-lets-encrypt-with-docker-in-less-than-5-minutes-b4b8a60d3a71).
