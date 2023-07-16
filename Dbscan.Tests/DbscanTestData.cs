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

namespace Nuzigor.Dbscan.Tests;

public record Point2D(double X, double Y);

public static class DbscanTestData
{
    public static  double EuclideanSquareDistance(Point2D p1, Point2D p2) 
    {
        ArgumentNullException.ThrowIfNull(p1);
        ArgumentNullException.ThrowIfNull(p2);

        var xDist = p2.X - p1.X;
        var yDist = p2.Y - p1.Y;
        return Math.Sqrt(xDist * xDist + yDist * yDist);    
    }

    internal static IReadOnlyList<Point2D> Borders = new List<Point2D>
    {
        new Point2D(0, 0),
        new Point2D(-1.8, 0),
        new Point2D(-2.3, 0),
        new Point2D(-2.3, 0.5),
        new Point2D(-2.3, -0.5),
        new Point2D(1.8, 0),
        new Point2D(2.3, 0),
        new Point2D(2.3, 0.5),
        new Point2D(2.3, -0.5),
    };

    private static List<Point2D> BuildRingDataset()
    {
        var points = new List<Point2D>();

        var rows = 6;
        var cols = 5;
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                var minPoints = row;
                var eps = 1.25 - col * 0.25;

                var x0 = -15 + (30 / (rows + 1)) * (row + 1);
                var y0 = -12 + (24 / (cols + 1)) * (col + 1);

                points.Add(new Point2D(x0, y0));
                for (var i = 0; i < minPoints; i++)
                {
                    var x = x0 + eps * Math.Sin(2 * Math.PI * i / minPoints);
                    var y = y0 + eps * Math.Cos(2 * Math.PI * i / minPoints);
                    points.Add(new Point2D(x, y));
                }
            }
        }

        return points;
    }

    internal static IReadOnlyList<Point2D> RingDataset = BuildRingDataset();
}
