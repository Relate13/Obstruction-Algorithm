using System.Collections.Generic;
using Godot;

public class Main : Node2D
{
    public readonly int PlanetDistance = 80;
    public List<Planet> PlanetList = new List<Planet>();
    public AdjacencyListGraph<Planet> RouteGraph = new AdjacencyListGraph<Planet>();
    public AdjacencyListGraph<Planet> BlockGraph = new AdjacencyListGraph<Planet>();
    public bool bShowBlockGraph = false;

    public void GeneratePlanets()
    {
        var coordinates = PoissonDiscSampling.GeneratePoints(PlanetDistance, GetViewportRect().Size, 10);
        foreach (var planet in coordinates)
            PlanetList.Add(new Planet
            {
                position = planet,
                radius = (float)GD.RandRange(1, (float)PlanetDistance / 2)
            });
    }

    public override void _Ready()
    {
        GD.Randomize();
        GeneratePlanets();
        GenerateGraph();
        Update();
    }

    public override void _Draw()
    {
        foreach (var planet in PlanetList) DrawCircle(planet.position, planet.radius, new Color(1, 1, 1));

        foreach (var planet in PlanetList)
        {
            var neighbors = RouteGraph.GetNeighbors(planet);
            foreach (var neighbor in neighbors) DrawLine(planet.position, neighbor.position, new Color(0, 1, 0, 0.8f));
            
            if (bShowBlockGraph)
            {
                neighbors = BlockGraph.GetNeighbors(planet);
                foreach (var neighbor in neighbors) DrawLine(planet.position, neighbor.position, new Color(1, 0, 0, 0.8f));
            }
        }
    }

    public void GenerateGraph()
    {
        foreach (var planet in PlanetList)
        {
            var collsionTree = new IntervalTree();
            var neighbors = SortByDistance(planet);
            foreach (var neighbor in neighbors)
            {
                var (interval, centerAngle) = GetAngleInterval(planet, neighbor.Value);

                if (!collsionTree.IsOverlapping(centerAngle))
                    RouteGraph.AddEdge(planet, neighbor.Value);
                else
                    BlockGraph.AddEdge(planet, neighbor.Value);

                if (interval.Low < interval.High)
                {
                    collsionTree.Insert(interval);
                }
                else
                {
                    collsionTree.Insert(new IntervalTree.Interval
                    {
                        Low = interval.Low,
                        High = 2 * Mathf.Pi
                    });
                    collsionTree.Insert(new IntervalTree.Interval
                    {
                        Low = 0,
                        High = interval.High
                    });
                }
            }
        }
    }

    public SortedList<float, Planet> SortByDistance(Planet origin)
    {
        var sortedPlanets = new SortedList<float, Planet>();
        foreach (var dest in PlanetList)
        {
            if (origin.Equals(dest))
                continue;
            var distance = origin.position.DistanceTo(dest.position) - dest.radius;
            sortedPlanets.Add(distance, dest);
        }

        return sortedPlanets;
    }

    public (IntervalTree.Interval, float) GetAngleInterval(Planet origin, Planet dest)
    {
        var d = origin.position.DistanceTo(dest.position);
        var r = dest.radius;
        var halfAngle = Mathf.Asin(r / d);

        var centerAngle = Vector2.Up.AngleTo(dest.position - origin.position);
        centerAngle = (centerAngle % (2 * Mathf.Pi) + 2 * Mathf.Pi) % (2 * Mathf.Pi);

        return (new IntervalTree.Interval
        {
            Low = ((centerAngle - halfAngle) % (2 * Mathf.Pi) + 2 * Mathf.Pi) % (2 * Mathf.Pi),
            High = ((centerAngle + halfAngle) % (2 * Mathf.Pi) + 2 * Mathf.Pi) % (2 * Mathf.Pi)
        }, centerAngle);
    }

    public struct Planet
    {
        public Vector2 position;
        public float radius;
    }
}