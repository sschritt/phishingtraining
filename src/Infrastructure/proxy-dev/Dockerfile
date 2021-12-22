FROM nginx

ADD nginx.conf /etc/nginx/nginx.conf
ADD localhost.crt /etc/ssl/certs/localhost.crt
ADD localhost.key /etc/ssl/private/localhost.key

HEALTHCHECK --interval=1m --timeout=5s --start-period=10s --retries=3 CMD curl --fail http://localhost:80 || exit 1
