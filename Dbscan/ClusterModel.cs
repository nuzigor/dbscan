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
/// The result of running the DBSCAN algorithm on a set of data.
/// </summary>
/// <typeparam name="T">The type of elements in the cluster.</typeparam>
public class ClusterModel<T>
{
    public ClusterModel(IReadOnlyCollection<IReadOnlyCollection<T>> clusters, IReadOnlyCollection<T> noise)
    {
        Clusters = clusters;
        Noise = noise;
    }

    /// <summary>
    /// A list of the clusters that have been identified.
    /// </summary>
    public IReadOnlyCollection<IReadOnlyCollection<T>> Clusters { get; }

    /// <summary>
    /// A list of the items that were not identified as being part of a cluster.
    /// </summary>
    public IReadOnlyCollection<T> Noise { get; }
}
