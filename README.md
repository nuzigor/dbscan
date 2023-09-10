# dbscan

Density-based spatial clustering of applications with noise ([DBSCAN](https://en.wikipedia.org/wiki/DBSCAN)) algorithm implementation in .net

## Usage

A function which returns directly reachable points accepts only the point itself,
thus the 'distance function'/caching/indexing has to be implemented outside of Dbscan Trainer scope.

DbscanTrainer uses [Dictionary<TKey, TValue>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-7.0#remarks) to keep track of the processed points, thus point type must satisfy same requirements as Dictionary key.

'getDirectlyReachablePoints' function must also return the point itself, otherwise 'MinimumPointsPerCluster' condition would not work.

```c#
using Nuzigor.Dbscan;

var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(points, DistanceFunction, Epsilon);
var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 4 });
var model = trainer.Fit(points, searcher);

Console.WriteLine(model.Clusters.Count);
Console.WriteLine(model.Noise.Count);
```

## Credits

Part of the code is based on work done by [DBSCAN](https://github.com/viceroypenguin/DBSCAN)