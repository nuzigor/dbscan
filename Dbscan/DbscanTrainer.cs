// Copyright 2023 Igor Nuzhnov
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Nuzigor.Dbscan;

/// <summary>
/// Contains methods to run the DBSCAN algorithm.
/// </summary>
/// <typeparam name="T">The type of elements to cluster.</typeparam>
public class DbscanTrainer<T>
    where T: notnull
{
    enum Label
    {
        None,
        Noise,
        Clustered
    };

    readonly int minimumPointsPerCluster;
    readonly IEqualityComparer<T>? comparer;

    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default EqualityComparer<T> for the type of the key.</param>
    /// <param name="options">The options with  minimum number of points required to create a cluster or to add additional points to the cluster.</param>
    public DbscanTrainer(IEqualityComparer<T>? comparer, DbscanOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.comparer = comparer;
        minimumPointsPerCluster = options.MinimumPointsPerCluster;
    }

    /// <param name="options">The options with  minimum number of points required to create a cluster or to add additional points to the cluster.</param>
    public DbscanTrainer(DbscanOptions options): this(null, options)
    {
    }

    /// <summary>
    /// Run the DBSCAN algorithm on a collection of points.
    /// </summary>
    /// <param name="points">The collection of elements to cluster.</param>
    /// <param name="getDirectlyReachablePoints">The function which returns neighbors within epsilon parameter radius.</param>
    /// <returns>A <see cref="ClusterModel{T}"/> containing the list of clusters and a list of unclustered points.</returns>
    public ClusterModel<T> Fit(IEnumerable<T> points, Func<T, IReadOnlyCollection<T>> getDirectlyReachablePoints)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(getDirectlyReachablePoints);

        var clusters = new List<IReadOnlyCollection<T>>();
        var visited = new Dictionary<T, bool>(comparer);

        foreach (var p in points)
        {
            if (visited.ContainsKey(p))
            {
                continue;
            }

            var directlyReachablePoints = getDirectlyReachablePoints(p);
            if (directlyReachablePoints.Count >= minimumPointsPerCluster)
            {
                visited.Add(p, true);
                clusters.Add(BuildCluster(
                        visited,
                        getDirectlyReachablePoints,
                        p,
                        directlyReachablePoints));
            }
            else
            {
                visited.Add(p, false);
            }
        }

        var noise = visited.Where(x => !x.Value).Select(x => x.Key).ToArray();
        return new ClusterModel<T>(clusters, noise);
    }

    /// <summary>
    /// Run the DBSCAN algorithm on an indexed collection of points.
    /// </summary>
    /// <param name="points">The indexed collection of elements to cluster.</param>
    /// <param name="getDirectlyReachablePoints">The function which returns neighbor indexes within epsilon parameter radius.</param>
    /// <returns>A <see cref="ClusterModel{T}"/> containing the list of clusters and a list of unclustered points.</returns>
    public ClusterModel<int> FitIndexed(IReadOnlyList<T> points, Func<int, IReadOnlyCollection<int>> getDirectlyReachablePoints)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(getDirectlyReachablePoints);

        var clusters = new List<IReadOnlyCollection<int>>();
        var labels = new Label[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            if (labels[i] != Label.None)
            {
                continue;
            }

            var directlyReachablePoints = getDirectlyReachablePoints(i);
            if (directlyReachablePoints.Count >= minimumPointsPerCluster)
            {
                labels[i] = Label.Clustered;
                clusters.Add(BuildCluster(
                        getDirectlyReachablePoints,
                        labels,
                        i,
                        directlyReachablePoints));
            }
            else
            {
                labels[i] = Label.Noise;
            }
        }

        var noiseCount = labels.Count(x => x == Label.Noise);
        var noise = new int[noiseCount];
        var noiseIndex = 0;
        for (var i = 0; i < points.Count; i++)
        {
            if (labels[i] == Label.Noise)
            {
                noise[noiseIndex++] = i;
            }
        }

        return new ClusterModel<int>(clusters, noise);
    }

    private IReadOnlyCollection<T> BuildCluster(
        Dictionary<T, bool> visited,
        Func<T, IReadOnlyCollection<T>> getDirectlyReachablePoints,
        T point,
        IReadOnlyCollection<T> directlyReachablePoints)
    {
        var clusterPoints = new List<T>() { point };
        var queue = new Queue<T>(directlyReachablePoints);
        var reachablePointsSet = new HashSet<T>(directlyReachablePoints);
        reachablePointsSet.Add(point);
        while (queue.TryDequeue(out var newPoint))
        {
            if (!visited.TryGetValue(newPoint, out var clustered))
            {
                var newDirectlyReachablePoints = getDirectlyReachablePoints(newPoint);
                if (newDirectlyReachablePoints.Count >= minimumPointsPerCluster)
                {
                    foreach (var p in newDirectlyReachablePoints)
                    {
                        if (reachablePointsSet.Add(p))
                        {
                            queue.Enqueue(p);
                        }
                    }
                }
            }
            
            if (!clustered)
            {
                visited[newPoint] = true;
                clusterPoints.Add(newPoint);
            }
        }

        return clusterPoints;
    }

    private IReadOnlyCollection<int> BuildCluster(
        Func<int, IReadOnlyCollection<int>> getDirectlyReachablePoints,
        Label[] labels,
        int pointIndex,
        IReadOnlyCollection<int> directlyReachablePoints)
    {
        var clusterPoints = new List<int>() { pointIndex };
        var queue = new Queue<int>(directlyReachablePoints);
        var reachablePointsSet = new HashSet<int>(directlyReachablePoints);
        reachablePointsSet.Add(pointIndex);
        while (queue.TryDequeue(out var newPointIndex))
        {
            var newPointLabel = labels[newPointIndex];
            if (newPointLabel == Label.None)
            {
                var newDirectlyReachablePoints = getDirectlyReachablePoints(newPointIndex);
                if (newDirectlyReachablePoints.Count >= minimumPointsPerCluster)
                {
                    foreach (var p in newDirectlyReachablePoints)
                    {
                        if (reachablePointsSet.Add(p))
                        {
                            queue.Enqueue(p);
                        }
                    }
                }
            }

            if (newPointLabel != Label.Clustered)
            {
                labels[newPointIndex] = Label.Clustered;
                clusterPoints.Add(newPointIndex);
            }
        }

        return clusterPoints;
    }
}
