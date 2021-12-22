FROM freinet/postfix-relay

ENV MAILNAME=smtp.phishingtraining.at
ENV LOGOUTPUT=/var/log/maillog
ENV USE_TLS=no

#RUN apt-get update && apt-get install telnet -y && rm -rf /var/lib/apt/lists/*
#HEALTHCHECK --interval=10s --timeout=5s --start-period=15s --retries=3 CMD echo -e '\x1dclose\x0d' | telnet localhost 25 || exit 1