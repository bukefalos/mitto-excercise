FROM mysql:5.7.15

MAINTAINER mitto

ENV MYSQL_DATABASE=mitto \
    MYSQL_ROOT_PASSWORD=root

ADD init.sql /docker-entrypoint-initdb.d

EXPOSE 3306