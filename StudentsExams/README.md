# Docker
To start working with the application, run following commands:
```
docker build -t students-exams:latest .
docker run -p 8080:80 students-exams:latest
```

# Boxplots
Box diagrams of the distribution of query execution of each type in the absence of another load at specified load levels:
* Number of Threads (users): 100/150;
* Ramp-up period (seconds): 0;
* Loop Count: 10.

## CoarseHashSet
![CoarseHashSet-Add100-150](./Boxplots/CoarseHashSet-Add100-150.png)
![CoarseHashSet-Contains100-150](./Boxplots/CoarseHashSet-Contains100-150.png)
![CoarseHashSet-Remove100-150](./Boxplots/CoarseHashSet-Remove100-150.png)

## StripedCuckooHashSet
![StripedCuckooHashSet-Add100-150](./Boxplots/StripedCuckooHashSet-Add100-150.png)
![StripedCuckooHashSet-Contains100-150](./Boxplots/StripedCuckooHashSet-Contains100-150.png)
![StripedCuckooHashSet-Remove100-150](./Boxplots/StripedCuckooHashSet-Remove100-150.png)

## Add (90%), Contains (9%), Remove (1%)
The number of clients resulting in a 10-second timeout failure: 1500.

Approximate number of entries in the dictionary: 250.
