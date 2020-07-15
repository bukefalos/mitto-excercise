# mitto-excercise

Sms messaging program excercise which is able to:

 - process new SMS request by HTTP request
 - get processed SMS paginated data
 - get grouped SMS statistics by day and mobile calling code filtered by dates and mobile calling codes
 - show supported countries


## Requirements

 - docker
 - docker-compose

## Build/Run
There are two options:

### Run with docker-compose with MySQL docker image
To test solution navigate into root folder and run `docker-compose up` which runs program together with MySql docker image. DB gets populated by 3 required countries and API will become available on port 8080. Visit your application in a browser at `localhost:8080/[sms|countries|statistics]`

### Run with host DB
You are also able to run application on its own without running MySql DB inside docker. In that case you need to specify custom connection URL by providing `Mitto:Db:Connection` environment variable. Navigate into project root and run `docker build --file MittoSms/Dockerfile --tag mitto:1.0 .` 
 
 After run has successfully ended you can run application with:
    
    docker run 
         --publish 8000:80 
         --name mitto mitto:1.0 
         --env Mitto:Db:Connection="server=host.docker.internal;database=mitto;uid=mitto;pwd=mitto;"

and visit your application in a browser at `localhost:8000/[sms|countries|statistics]` 

## Usage
Use API by provided excercise assignment
