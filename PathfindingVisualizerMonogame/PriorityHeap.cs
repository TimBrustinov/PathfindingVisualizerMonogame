using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingVisualizerMonogame
{
    class Heap<T>
    {
        T[] array;
        public int Count { get; private set; }
        IComparer<T> Comparer;

        public Heap(IComparer<T> comparer, int capacity = 10)
        {
            Comparer = comparer;
            array = new T[capacity];
            Count = 0;
        }

        public Heap(int capacity = 10)
            : this(Comparer<T>.Default, capacity)
        {
        }

        int FindParentIndex(int childIndex)
        {
            int parentNodeIndex = (childIndex - 1) / 2;
            return parentNodeIndex;
        }
        int FindLeftChild(int parentIndex)
        {
            int leftChildIndex = (2 * parentIndex) + 1;
            return leftChildIndex;
        }
        int FindRightChild(int parentIndex)
        {
            int RightChildIndex = (2 * parentIndex) + 2;
            return RightChildIndex;
        }
        public void Insert(T value)
        {
            if (Count >= array.Length)
            {
                Resize();
            }
            array[Count] = value;
            Count++;
            HeapifyUp();
        }
        private void HeapifyUp()
        {
            if (Count == 0) return;
            int current = Count - 1;
            int parent = FindParentIndex(current);
            while (Comparer.Compare(array[parent], array[current]) > 0 && parent >= 0)
            {
                T temp = array[current];
                array[current] = array[parent];
                array[parent] = temp;
                current = parent;
                parent = FindParentIndex(current);
            }
        }
        public T Pop()
        {
            if(Count == 0)
            {
                throw new InvalidOperationException("Cannot pop from empty heap");
            }
            T temp = array[0];
            array[0] = array[Count - 1];
            array[Count - 1] = default;
            Count--;
            HeapifyDown();
            return temp;
        }
        private void HeapifyDown()
        {
            int currentIndex = 0;
            int RightChildIndex = FindRightChild(currentIndex);
            int LeftChildIndex = FindLeftChild(currentIndex);
            while (currentIndex < Count)
            {
                if (LeftChildIndex >= Count)
                {
                    break;
                }
                else if (RightChildIndex >= Count)
                {
                    //Compare(a, b) = a - b
                    //a - b > 0 is the same as a > b
                    if (Comparer.Compare(array[currentIndex], array[LeftChildIndex]) > 0)
                    {
                        Swap(LeftChildIndex, currentIndex);
                        currentIndex = LeftChildIndex;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (Comparer.Compare(array[LeftChildIndex], array[RightChildIndex]) < 0)
                {
                    Swap(LeftChildIndex, currentIndex);
                    currentIndex = LeftChildIndex;
                }
                else
                {
                    Swap(RightChildIndex, currentIndex);
                    currentIndex = RightChildIndex;
                }
                RightChildIndex = FindRightChild(currentIndex);
                LeftChildIndex = FindLeftChild(currentIndex);
            }
        }
        private void Swap(int childIndex, int currentIndex)
        {
            T temp = array[currentIndex];
            array[currentIndex] = array[childIndex];
            array[childIndex] = temp;
        }
        private void Resize()
        {
            T[] resizeArray = new T[array.Length * 2];
            for (int i = 0; i < array.Length; i++)
            {
                resizeArray[i] = array[i];
            }
            array = resizeArray;
        }
    }
}
