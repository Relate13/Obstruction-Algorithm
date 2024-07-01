public class IntervalTree
{
    private Node _root;

    public void Insert(Interval interval)
    {
        _root = _insert(_root, interval);
    }

    public bool IsOverlapping(float p)
    {
        return _isOverlapping(_root, p);
    }

    private static Node _insert(Node node, Interval interval)
    {
        if (node == null) return new Node { Interval = interval, Max = interval.High };

        if (interval.Low < node.Interval.Low)
            node.Left = _insert(node.Left, interval);
        else
            node.Right = _insert(node.Right, interval);

        if (node.Max < interval.High)
            node.Max = interval.High;

        return node;
    }

    private static bool _isOverlapping(Node node, float p)
    {
        if (node == null)
            return false;
        if (p >= node.Interval.Low && p <= node.Interval.High)
            return true;
        if (p < node.Interval.Low)
            return _isOverlapping(node.Left, p);
        if (node.Left != null && node.Left.Max >= p)
            return true;
        return _isOverlapping(node.Right, p);
    }

    public struct Interval
    {
        public float Low;
        public float High;

        public override string ToString()
        {
            return "[" + Low + "," + High + "]";
        }
    }

    private class Node
    {
        public Interval Interval;
        public Node Left;
        public float Max;
        public Node Right;
    }
}