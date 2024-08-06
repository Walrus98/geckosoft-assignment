# Geckosoft Assignment

## Libraries

The application was written in DotNet 8 and includes the following libraries:

- **EntityFrameworkCore**
- **EntityFrameworkCore.InMemory**
- **Swashbuckle.AspNetCore**

## Requirement

In order to run the program you have to install ffmpeg on your device, used to generate thumbnails from the videos.

The following version has been used:
```
ffmpeg version 5.1.5-0+deb12u1 Copyright (c) 2000-2024 the FFmpeg developers
built with gcc 12 (Debian 12.2.0-14)
```

## Summary

### Database

A memory database provided by the Entity Framework was used to implement thumbnail and video persistence. The following entities were implemented:

Video:
- id: GUID ID of the thumbnail (primary key)
- filePath: GUID of the video

Thumbnail:
- id: GUID ID of the thumbnail (primary key)
- videoId: GUID of the video (foreign key)
- width: width of the thumbnail
- height: height of the thumbnail
- filePath: the path where the thumbnail has been uploaded
- status: the current process status of the thumbnail

the relation between videos and thumbnails is one to many.

### Endpoints

All endpoints conform with the OpenAPI specification and are accessible via SwaggerUI at this [link](http://localhost:5281/swagger/index.html).
To implement the required funcionalities, two groups of endpoints have been developed: video and thumbnails.

#### Videos

- **POST** ``/v0/videos/upload``
    used to upload a video to the application, the payload size for this endpoint has been increased up to 1GB.

- **GET** ``/v0/videos/``
    used to obtain the list of uploaded videos within range specified by the following query parameters: 
    - offset: numbers of element to skip from the start;
    - limit: numbers of element that has to be retrieved;
    each video is formatted as follows:
    - id: Guid ID
    - filePath: the path where the video has been uploaded

- **GET** ``/v0/videos/{id}``
    used to download the requested video 

#### Thumbnails

- **POST** ``/v0/thumbnails/generate``
    used to request the generation of the thumbnail for the video, by sending in the request body:
    - videoId: GUID of the video
    - width: width of the thumbnail
    - height: height of the thumbnail
    the policy of the thumbnail generation uses the first frame of the video

- **GET** ``/v0/thumbnails``
    used to obtain the list of created thumbnails within range specified by the following query parameters: 
    - offset: numbers of element to skip from the start;
    - limit: numbers of element that has to be retrieved;
    each thumbnail is formatted as follows:
    - id: GUID ID of the thumbnail
    - videoId: GUID of the video
    - width: width of the thumbnail
    - height: height of the thumbnail
    - filePath: the path where the thumbnail has been uploaded
    - status: the current process status of the thumbnail, since the status was developed as Enum, in order to get the state as a 'string', a thumbnailDto was also implemented

- **GET** ``/v0/thumbnails/{id}``
    used to download the requested thumbnail, only if it has reached the state of **COMPLETED**.
    the following parameters are required:
    - Id: GUID of the thumbnail's video
    - w: width of the thumbnail
    - h: height of the thumbnail
    
- **GET** ``/v0/thumbnails/status/{id}``
    used to query the status of the thumbnail, where id is the Guid of thumbnail's video 

- **SignalR** ``/v0/thumbnails/thumbnailHub``
    used to monitor in realtime the status of the thumbnail creation process.

### Thumbnail Generation

Thumbnail generation is done through a service, which is registered as a singleton. This service is responsible for generating thumbnails launching a process of ffmpeg. The service is also responsible for the real time update of the thumbnail's process status, implemented using signal R. Each generation Job is processed in a different Task.

The process status of the thumnbail can assume the following values:
- QUEUED: upon receiving the request of generation
- ANALYZING: while performing the extraction of the thumbnail of the video with ffmpeg (first frame)
- COMPLETED: upon successfull generation of the thumbnail 
- FAILED: if the generation process encounts an error 

#### Real time notification

In order to be able to monitor the change of job thumnbail status, it is necessary for the client to register to a group specified by thumbnail id to be notified.
Whenever a thumnbail status change occurr, Thumbnail Service notifies all clients registered to the group identified by thumbnail's id

#### Future upgrades

Currently, the application also supports a User Entity in Database and offers the possibility of making endpoints with authorization (using Json Web Token), although they have not been implemented. In fact, the project has also the following libraries. 

- **Authentication.JwtBearer**
- **Identity.EntityFrameworkCore**

A possible improvement would be to add registration and login endpoints, so that each users can upload his own videos and their respective thumbnails. The current implementation allows all clients to view
every video and generate their thumbnails.

The new database relation would be the following:
a user has many videos and a video has many thumbnails, by inserting a foreign key for the user in the video entity
