FROM nginx

ADD nginx.conf /etc/nginx/nginx.conf

HEALTHCHECK --interval=1m --timeout=5s --start-period=10s --retries=3 CMD curl --fail http://localhost:80 || exit 1
