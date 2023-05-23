# Docker
To start working with the application, run following commands:
```
docker build -t students-exams:latest .
docker run -p 8080:80 students-exams:latest
```

## Boxplots
Box diagrams of the distribution of query execution of each type in the absence of another load at specified load levels:
* Number of Threads (users): 100/150
* Ramp-up period (seconds): 0
* Loop Count: 10

![CoarseHashSetAdd100-150](./Boxplots/CoarseHashSetAdd100-150.png)
![CoarseHashSetContains100-150](./Boxplots/CoarseHashSetContains100-150.png)
![CoarseHashSetRemove100-150](./Boxplots/CoarseHashSetRemove100-150.png)
