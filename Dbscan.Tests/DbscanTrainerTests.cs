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

using System.Linq;

namespace Nuzigor.Dbscan.Tests;

public class DbscanTrainerTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void BorderTest1()
    {
        var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(DbscanTestData.Borders, DbscanTestData.EuclideanSquareDistance, 1.0);
        var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 4 });
        var clusters = trainer.Fit(DbscanTestData.Borders, searcher);

        Assert.That(clusters.Clusters.Count, Is.EqualTo(2));
        Assert.That(clusters.Noise.Count, Is.EqualTo(1));

        Assert.That(clusters.Noise.First(), Is.EqualTo(DbscanTestData.Borders[0]));
        Assert.That(clusters.Clusters.ElementAt(0).Count, Is.EqualTo(4));
        Assert.That(clusters.Clusters.ElementAt(1).Count, Is.EqualTo(4));
    }

    [Test]
    public void BorderTest2()
    {
        var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(DbscanTestData.Borders, DbscanTestData.EuclideanSquareDistance, 2.0);
        var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 3 });
        var clusters = trainer.Fit(DbscanTestData.Borders, searcher);

        Assert.That(clusters.Clusters.Count, Is.EqualTo(1));
        Assert.That(clusters.Noise.Count, Is.EqualTo(0));

        Assert.That(clusters.Clusters.ElementAt(0).Count, Is.EqualTo(9));
    }

    [Test]
    public void BorderTest3()
    {
        var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(DbscanTestData.Borders, DbscanTestData.EuclideanSquareDistance, 2.0);
        var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 4 });
        var clusters = trainer.Fit(DbscanTestData.Borders, searcher);

        Assert.That(clusters.Clusters.Count, Is.EqualTo(2));
        Assert.That(clusters.Noise.Count, Is.EqualTo(0));

        Assert.That(clusters.Clusters.ElementAt(0).Count, Is.EqualTo(5));
        Assert.That(clusters.Clusters.ElementAt(1).Count, Is.EqualTo(4));
    }

    [Test]
    public void BorderTest4()
    {
        var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(DbscanTestData.Borders, DbscanTestData.EuclideanSquareDistance, 2.0);
        var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 5 });
        var clusters = trainer.Fit(DbscanTestData.Borders, searcher);

        Assert.That(clusters.Clusters.Count, Is.EqualTo(2));
        Assert.That(clusters.Noise.Count, Is.EqualTo(0));

        Assert.That(clusters.Clusters.ElementAt(0).Count, Is.EqualTo(5));
        Assert.That(clusters.Clusters.ElementAt(1).Count, Is.EqualTo(4));
    }

    [Test]
    public void RingTest1()
    {
        var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(DbscanTestData.RingDataset, DbscanTestData.EuclideanSquareDistance, 1.0);
        var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 4 });
        var clusters = trainer.Fit(DbscanTestData.RingDataset, searcher);

        Assert.That(clusters.Clusters.Count, Is.EqualTo(9));
        Assert.That(clusters.Noise.Count, Is.EqualTo(60));
    }

    [Test]
    public void RingTest2()
    {
        var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(DbscanTestData.RingDataset, DbscanTestData.EuclideanSquareDistance, 1.01);
        var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 4 });
        var clusters = trainer.Fit(DbscanTestData.RingDataset, searcher);

        Assert.That(clusters.Clusters.Count, Is.EqualTo(12));
        Assert.That(clusters.Noise.Count, Is.EqualTo(45));
    }

    [Test]
    public void RingTest3()
    {
        var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(DbscanTestData.RingDataset, DbscanTestData.EuclideanSquareDistance, 1.3);
        var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 4 });
        var clusters = trainer.Fit(DbscanTestData.RingDataset, searcher);

        Assert.That(clusters.Clusters.Count, Is.EqualTo(15));
        Assert.That(clusters.Noise.Count, Is.EqualTo(30));
    }

    [Test]
    public void RingTest4()
    {
        var searcher = Utils.CreateFullScanNeighborsSearcher<Point2D>(DbscanTestData.RingDataset, DbscanTestData.EuclideanSquareDistance, 0.99);
        var trainer = new DbscanTrainer<Point2D>(new DbscanOptions { MinimumPointsPerCluster = 2 });
        var clusters = trainer.Fit(DbscanTestData.RingDataset, searcher);

        Assert.That(clusters.Clusters.Count, Is.EqualTo(15));
        Assert.That(clusters.Noise.Count, Is.EqualTo(45));
    }
}
