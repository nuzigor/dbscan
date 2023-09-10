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
/// Various utility methods to create 'range query' functions.
/// </summary>
public static class Utils
{
    /// <summary>
    /// A delegate to return a distance between two points.
    /// </summary>
    public delegate double GetDistance<T>(T point1, T point2);

    /// <summary>
    /// Creates a function which pre-calculates neighbors of every point.
    /// </summary>
    public static Func<T, IReadOnlyCollection<T>> CreateFullScanCachedNeighborsSearcher<T>(
        IReadOnlyCollection<T> points,
        GetDistance<T> distanceCalculator,
        double epsilon)
        where T: notnull => CreateFullScanCachedNeighborsSearcher(points, null, distanceCalculator, epsilon);

    /// <summary>
    /// Creates a function which pre-calculates neighbors of every point.
    /// </summary>
    public static Func<T, IReadOnlyCollection<T>> CreateFullScanCachedNeighborsSearcher<T>(
        IReadOnlyCollection<T> points,
        IEqualityComparer<T>? comparer,
        GetDistance<T> distanceCalculator,
        double epsilon)
        where T: notnull
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        if (distanceCalculator is null)
        {
            throw new ArgumentNullException(nameof(distanceCalculator));
        }

        var neighborsSet = new Dictionary<T, IReadOnlyCollection<T>>(comparer);
        foreach (var point in points)
        {
            var list = new List<T>();
            neighborsSet.Add(point, list);
            foreach (var otherPoint in points)
            {
                var distance = distanceCalculator(point, otherPoint);
                if (distance < epsilon)
                {
                    list.Add(point);
                }
            }
        }

        return (T p) => neighborsSet[p];
    }

    /// <summary>
    /// Creates a function which returns neighbors of every point.
    /// </summary>
    public static Func<T, IReadOnlyCollection<T>> CreateFullScanNeighborsSearcher<T>(
        IReadOnlyCollection<T> points,
        GetDistance<T> distanceCalculator,
        double epsilon)
        where T : notnull
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        if (distanceCalculator is null)
        {
            throw new ArgumentNullException(nameof(distanceCalculator));
        }

        return (T point) => points.Where(x => distanceCalculator(point, x) < epsilon).ToArray();
    }

    /// <summary>
    /// Creates a function which pre-calculates distances between every two points.
    /// </summary>
    public static Func<T, double, IReadOnlyCollection<T>> CreateFullScanCachedNeighborsSearcher<T>(
        IReadOnlyCollection<T> points,
        GetDistance<T> distanceCalculator)
        where T : notnull => CreateFullScanCachedNeighborsSearcher(points, null, distanceCalculator);

    /// <summary>
    /// Creates a function which pre-calculates distances between every two points.
    /// </summary>
    public static Func<T, double, IReadOnlyCollection<T>> CreateFullScanCachedNeighborsSearcher<T>(
        IReadOnlyCollection<T> points,
        IEqualityComparer<T>? comparer,
        GetDistance<T> distanceCalculator)
        where T : notnull
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        if (distanceCalculator is null)
        {
            throw new ArgumentNullException(nameof(distanceCalculator));
        }

        var distancesSet = new Dictionary<T, Dictionary<T, double>>(comparer);
        foreach (var point in points)
        {
            var set = new Dictionary<T, double>(comparer);
            distancesSet.Add(point, set);
            foreach (var otherPoint in points)
            {
                var distance = distanceCalculator(point, otherPoint);
                set.Add(otherPoint, distance);
            }
        }

        return (T p, double epsilon) => distancesSet[p].Where(x => x.Value < epsilon).Select(x => x.Key).ToArray();
    }
}
