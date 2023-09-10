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

    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or null to use the default EqualityComparer{T} for the type of the key.</param>
    /// <param name="options">The options with  minimum number of points required to create a cluster or to add additional points to the cluster.</param>
    public DbscanTrainer(IEqualityComparer<T>? comparer, DbscanOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

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
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        if (getDirectlyReachablePoints is null)
        {
            throw new ArgumentNullException(nameof(getDirectlyReachablePoints));
        }

        var clusters = new List<IReadOnlyCollection<T>>();
        var visited = new Dictionary<T, Label>(comparer);

        foreach (var p in points)
        {
            if (visited.ContainsKey(p))
            {
                continue;
            }

            var directlyReachablePoints = getDirectlyReachablePoints(p);
            if (directlyReachablePoints.Count >= minimumPointsPerCluster)
            {
                visited.Add(p, Label.Clustered);
                clusters.Add(BuildCluster(
                        visited,
                        getDirectlyReachablePoints,
                        p,
                        directlyReachablePoints));
            }
            else
            {
                visited.Add(p, Label.Noise);
            }
        }

        var noise = visited.Where(x => x.Value != Label.Clustered).Select(x => x.Key).ToArray();
        return new ClusterModel<T>(clusters, noise);
    }

    private IReadOnlyCollection<T> BuildCluster(
        Dictionary<T, Label> visited,
        Func<T, IReadOnlyCollection<T>> getDirectlyReachablePoints,
        T point,
        IReadOnlyCollection<T> directlyReachablePoints)
    {
        var clusterPoints = new List<T>() { point };
        var queue = new Queue<T>(directlyReachablePoints);
        var reachablePointsSet = new HashSet<T>(directlyReachablePoints) { point };
#if NET6_0_OR_GREATER
        while (queue.TryDequeue(out var newPoint))
        {
#else
        while (queue.Count > 0)
        {
            var newPoint = queue.Dequeue();
#endif
            if (!visited.TryGetValue(newPoint, out var label))
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
            
            if (label != Label.Clustered)
            {
                visited[newPoint] = Label.Clustered;
                clusterPoints.Add(newPoint);
            }
        }

        return clusterPoints;
    }
}
