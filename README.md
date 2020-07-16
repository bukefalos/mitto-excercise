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
To test solution navigate into root folder and run: 

    docker-compose up 
which runs program together with MySql docker image. DB gets populated by 3 required countries and API will become available on port 8080. Visit your application in a browser at `localhost:8080/[sms|countries|statistics]`

### Run with host DB
You are also able to run application on its own without running MySql DB inside docker. In that case you need to specify custom connection URL by providing `Mitto:Db:Connection` environment variable. Navigate into project root and run: 

    docker build --file MittoSms/Dockerfile --tag mitto:1.0 .
 
 After run has successfully ended you can run application with:
    
    docker run 
         --publish 8000:80 
         --name mitto mitto:1.0 
         --env Mitto:Db:Connection="server=host.docker.internal;database=mitto;uid=mitto;pwd=mitto;"

and visit your application in a browser at `localhost:8000/[sms|countries|statistics]` 

## Usage
Use API by provided excercise assignment

## Other notes

### Difficulties:
- Visual Studio for Mac (it seems sometimes broken)
- ServiceStack was for me unknown technology. Moreover I had not been working with C# for almost a decade
- I had very busy week


### Possible optimalizations:
- Change SendSMS service to use POST. I would not pick GET as HTTP verb that is changing state. It must have some strong reason
- I assume usual usecase of Sending SMS service is often with multiple recipients. I would extend SendSMS service to accept more receivers in one request. I would put list of receivers into request body
- Matching country calling code - currently I have implemented very naive approach which is based on matching calling codes of each country to first N numbers of receiver mobile number in a loop starting with longest N down to 1. There is also no other number validation by country. 
- When grouping Statistics records I had to cast DATETIME to DATE with DATE function. This can be slow for big list of SMS records. We can possibly save one more culumn with DATE string or DATETIME to group Statstics records faster without using DATE function.
- There is very simple validation checking which currently checks only presence of parameters
- I have no integration tests, tests that should run whole application, try API out and assert if responses are in correct format. 
