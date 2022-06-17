using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizerMonogame
{
    internal class Graph<T>
    {
        private List<Vertex<T>> vertices;

        public IReadOnlyList<Vertex<T>> Vertices => vertices;
        public int VertexCount => vertices.Count;

        public Graph()
        {
            vertices = new List<Vertex<T>>();
        }
        public void AddVertex(Vertex<T> vertex)
        {
            if (vertex == null || vertex.Edges.Count != 0 || vertices.Contains(vertex))
            {
                return;
            }
            vertices.Add(vertex);
        }
        public void ClearEdges()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].Edges.Clear();
            }
        }
        public bool AddEdge(Vertex<T> A, Vertex<T> B, double distance)
        {
            if (A == null || B == null)
            {
                return false;
            }
            else if (!vertices.Contains(A) || !vertices.Contains(B))
            {
                return false;
            }
            else if (A.Edges.ContainsKey(B))
            {
                return false;
            }
            else
            {
                A.Edges.Add(B, distance);
                return true;
            }
        }
        public bool RemoveVertex(Vertex<T> vertex)
        {
            if (vertices.Contains(vertex))
            {
                vertices.Remove(vertex);
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (vertices[i].Edges.ContainsKey(vertex))
                    {
                        vertices[i].Edges.Remove(vertex);
                    }
                }
                return true;
            }
            return false;
        }
        public bool RemoveEdge(Vertex<T> A, Vertex<T> B)
        {
            if (!vertices.Contains(A) || !vertices.Contains(B) || A.Edges.ContainsKey(B))
            {
                return false;
            }
            A.Edges.Remove(B);
            return true;
        }
        public bool search(Vertex<T> vertex)
        {
            if (vertices.Contains(vertex))
            {
                return true;
            }
            return false;
        }
        public (List<T> path, List<T> visitedList) BFS(Vertex<T> start, Vertex<T> end)
        {
            //TODO: this function is broken!!

            for (int i = 0; i < vertices.Count - 1; i++)
            {
                vertices[i].wasVisited = false;
            }

            Queue<Vertex<T>> queue = new Queue<Vertex<T>>();
            List<T> path = new List<T>();
            List<T> visitedList = new List<T>();
            queue.Enqueue(start);
            
            while(queue.Count > 0)
            {
                var curr = queue.Dequeue();
                curr.wasVisited = true;
                visitedList.Add(curr.Value);
                if (curr == end) break;

                foreach(var child in curr.Edges)
                {
                    if (!child.Key.wasVisited && !queue.Contains(child.Key))
                    {
                        queue.Enqueue(child.Key);
                        child.Key.Parent = curr;
                    }
                }
            }
            if (queue.Count == 0)
            {
                return (null, visitedList);
            }
            var temp = end;
            while(temp != null)
            {
                path.Add(temp.Value);
                temp = temp.Parent;
            }
            path.Reverse();
            return (path, visitedList);
        }
        public (List<T> Path, List<T> visitedList) DijkstraAlgorithm(Vertex<T> start, Vertex<T> end)
        {
            PriorityQueue<Vertex<T>, double> priorityQueue = new PriorityQueue<Vertex<T>, double>(false);
            List<T> visitedList = new List<T>();
            Vertex<T> current = start;
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].cumalativeDistance = double.MaxValue;
                vertices[i].wasVisited = false;
                vertices[i].wasQueued = false;
            }
            start.cumalativeDistance = 0;
            priorityQueue.Enqueue(start, 0);
            while (current != end)
            {
                current = priorityQueue.Dequeue();
                visitedList.Add(current.Value);
                if (current == end) break;
                foreach (var kvp in current.Edges)
                {
                    double tenativeDistance = current.cumalativeDistance + kvp.Value;
                    if (tenativeDistance < kvp.Key.cumalativeDistance)
                    {
                        kvp.Key.cumalativeDistance = tenativeDistance;
                        kvp.Key.Parent = current;
                        if (!kvp.Key.wasVisited && !kvp.Key.wasQueued)
                        {
                            priorityQueue.Enqueue(kvp.Key, kvp.Key.cumalativeDistance);
                            kvp.Key.wasQueued = true;
                            kvp.Key.wasVisited = true;
                        }
                    }
                }
                current.wasVisited = true;
            }
            if (priorityQueue.Count == 0)
            {
                return (null, visitedList);
            }
            current = end;
            List<T> path = new List<T>();
            while (current != null)
            {
                path.Add(current.Value);
                current = current.Parent;
            }
            path.Reverse();
            return (path, visitedList);
        }
        public (List<T> path, List<T> visitedList) AStarSearchAlgorithm(Vertex<T> start, Vertex<T> end)
        {
            PriorityQueue<Vertex<T>, double> priorityQueue = new PriorityQueue<Vertex<T>, double>(false);
            List<T> visitedList = new List<T>();
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].cumalativeDistance = double.MaxValue;
                vertices[i].finalDistance = double.MaxValue;
                vertices[i].Parent = null;
                vertices[i].wasVisited = false;
                vertices[i].wasQueued = false;
            }
            start.cumalativeDistance = 0;
            start.finalDistance = ManhattanHeuristic(start, end, 1);
            priorityQueue.Enqueue(start, start.finalDistance);
            Vertex<T> current = start;
            while (priorityQueue.Count > 0)
            {
                current = priorityQueue.Dequeue();
                visitedList.Add(current.Value);
                if (current == end) break;
                foreach (var kvp in current.Edges)
                {
                    double tenativeDistance = current.cumalativeDistance + kvp.Value;
                    if (tenativeDistance < kvp.Key.cumalativeDistance)
                    {
                        kvp.Key.cumalativeDistance = tenativeDistance;
                        kvp.Key.Parent = current;
                        kvp.Key.finalDistance = kvp.Key.cumalativeDistance + ManhattanHeuristic(kvp.Key, end, 1);
                        if (!kvp.Key.wasVisited && !kvp.Key.wasQueued)
                        {
                            priorityQueue.Enqueue(kvp.Key, kvp.Key.finalDistance);
                            kvp.Key.wasQueued = true;
                            kvp.Key.wasVisited = true;
                        }
                    }
                }
                current.wasVisited = true;
            }
            if(priorityQueue.Count == 0)
            {
                return (null, visitedList);
            }
            current = end;
            List<T> path = new List<T>();
            while (current != null)
            {
                path.Add(current.Value);
                current = current.Parent;
            }
            path.Reverse();
            return (path, visitedList);
        }
        private double ManhattanHeuristic(Vertex<T> node, Vertex<T> goal, double D)
        {
            double dx = Math.Abs(node.X - goal.X);
            double dy = Math.Abs(node.Y - goal.Y);
            return D * Math.Sqrt(dx * dx + dy * dy);
        }

        public void ClearVertices()
        {
            vertices.Clear();
        }
    }
}
