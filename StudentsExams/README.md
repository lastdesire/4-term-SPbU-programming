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

![Text](./Boxplots/Add100-150.png)
![Text](./Boxplots/Contains100-150.png)
![Text](./Boxplots/Remove100-150.png)
