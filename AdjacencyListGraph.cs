using System.Collections.Generic;

public class AdjacencyListGraph<T>
{
    public Dictionary<T, HashSet<T>> Graph = new Dictionary<T, HashSet<T>>();

    public void AddEdge(T start, T end)
    {
        if (HasEdge(start, end)) return;

        if (!Graph.ContainsKey(start)) Graph[start] = new HashSet<T>();
        if (!Graph.ContainsKey(end)) Graph[end] = new HashSet<T>();

        Graph[start].Add(end);
        Graph[end].Add(start);
    }

    public bool HasEdge(T start, T end)
    {
        return Graph.ContainsKey(start) && Graph[start].Contains(end);
    }

    public List<T> GetVertexes()
    {
        return new List<T>(Graph.Keys);
    }

    public HashSet<T> GetNeighbors(T vertex)
    {
        if (Graph.TryGetValue(vertex, out var neighbors))
            return neighbors;
        return new HashSet<T>();
    }
}