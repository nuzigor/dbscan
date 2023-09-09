# dbscan

Density-based spatial clustering of applications with noise (DBSCAN) algorithm implementation in .net

## Design principles

- A function which returns directly reachable points accepts only the point itself, thus the distance function/caching/indexing can be defined externally
- Uses Dictionary<TKey, TValue> to keep processed points, thus point type must satisfy same requirements as Dictionary key

## Typical usage

```c#
using Nuzigor.Dbscan;

var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(points, DistanceFunction, Epsilon);
var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 4 });
var clusters = trainer.Fit(points, searcher);

Console.WriteLine(clusters.Clusters.Count);
Console.WriteLine(clusters.Noise.Count);
```
