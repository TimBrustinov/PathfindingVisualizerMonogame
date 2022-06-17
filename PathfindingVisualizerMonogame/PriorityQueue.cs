using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizerMonogame
{
    class PriorityQueue<T, TPriority> where TPriority : IComparable<TPriority>
    {
        class Element
        {
            public T Value;
            public TPriority Priority;

            public Element(T value, TPriority priority)
            {
                Value = value;
                Priority = priority;
            }
        }

        private Heap<Element> heap;

        public int Count => heap.Count;

        int MaxPriority(Element a, Element b)
        {
            return b.Priority.CompareTo(a.Priority);
        }

        int MinPriority(Element a, Element b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        public PriorityQueue(bool max)
        {
            //the root of the heap has the greatest Priority
            if (max)
            {
                heap = new Heap<Element>(Comparer<Element>.Create(MaxPriority));
            }
            else
            {
                heap = new Heap<Element>(Comparer<Element>.Create(MinPriority));
            }
        }

        public void Enqueue(T value, TPriority priority)
        {
            heap.Insert(new Element(value, priority));
        }

        public T Dequeue()
        {
            return heap.Pop().Value;
        }
    }
}
