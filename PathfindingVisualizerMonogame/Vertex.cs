using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizerMonogame
{
    internal class Vertex<T>
    {
        public T Value { get; set; }

        public bool wasVisited = false;
        public bool wasQueued = false;

        public double cumalativeDistance;
        public double finalDistance;
        public Dictionary<Vertex<T>, double> Edges { get; }
        public Vertex<T> Parent;
        public float X { get; private set; }
        public float Y { get; private set; }
        public Vertex(T value, float x, float y)
        {
            Value = value;
            Edges = new Dictionary<Vertex<T>, double>();
            X = x;
            Y = y;
        }
    }

    //internal class Vertex<T>where T : IComparable<T>
    //{
    //    public T Value { get; set; }
    //    public List<Edge<T>> Neighbors { get; set; }

    //    public int NeighborCount => Neighbors.Count;

    //    public Vertex(T value) 
    //    {
    //        Value = value;
    //    }

}
