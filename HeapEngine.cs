namespace HeapPriorityQueue
{
    
    public struct PriorityNode
    {
        public string Name;
        public int Priority; 
    }

    
    public class HeapEngine
    {
        private readonly List<PriorityNode> _heap = new();

        public int Count => _heap.Count;

        public IReadOnlyList<PriorityNode> Nodes => _heap.AsReadOnly();

        
        public void Insert(string name, int priority)
        {
            _heap.Add(new PriorityNode { Name = name, Priority = priority });
            HeapifyUp(_heap.Count - 1);
        }

        
        public PriorityNode ExtractMin()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Heap is empty.");

            var min = _heap[0];
            _heap[0] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);
            if (_heap.Count > 0) HeapifyDown(0);
            return min;
        }

        
        public PriorityNode Peek()
        {
            if (_heap.Count == 0)
                throw new InvalidOperationException("Heap is empty.");
            return _heap[0];
        }

        
        public void Clear() => _heap.Clear();

        
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {

                int parent = (index - 1) / 2;
                if (_heap[index].Priority >= _heap[parent].Priority) 
                    break;

                (_heap[index], _heap[parent]) = (_heap[parent], _heap[index]);

                index = parent;

            }
        }

        
        private void HeapifyDown(int index)
        {
            while (index < _heap.Count)
            {

                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int smallest = index;

                if (left < _heap.Count && _heap[left].Priority < _heap[smallest].Priority)
                    smallest = left;
                if (right < _heap.Count && _heap[right].Priority < _heap[smallest].Priority)
                    smallest = right;

                if (smallest == index)
                    break;

                (_heap[index], _heap[smallest]) = (_heap[smallest], _heap[index]);

                index = smallest;

            }
        }
    }
}
