FROM mcr.microsoft.com/mssql-tools

RUN apt-get update && apt-get install netcat -y && rm -rf /var/lib/apt/lists/*

WORKDIR /tmp

ADD wait-for.sh wait-for.sh
# remove windows line endings from file as this triggers very strange behaviour when executing the sh file
RUN sed -i 's/\r//g' wait-for.sh

ADD init_phishingtraining.sql init_phishingtraining.sql
ADD init_hangfire.sql init_hangfire.sql

CMD ["sh","-c","/tmp/wait-for.sh db:1433 -t 90 -- sleep 10s && /opt/mssql-tools/bin/sqlcmd -S db -U sa -P $SA_PASSWORD -i /tmp/init_phishingtraining.sql && /opt/mssql-tools/bin/sqlcmd -S db -U sa -P $SA_PASSWORD -i /tmp/init_hangfire.sql"]