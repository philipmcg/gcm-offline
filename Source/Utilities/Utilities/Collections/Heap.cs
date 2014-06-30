using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    public class Heap<P, V> : ICollection<KeyValuePair<P, V>>
    {
        private List<KeyValuePair<P, V>> m_baseHeap;
        private IComparer<P> m_comparer;

        public Heap()
            : this(Comparer<P>.Default)
        {
        }

        public Heap(int capacity)
            : this(capacity, Comparer<P>.Default)
        {
        }

        public Heap(int capacity, IComparer<P> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException();

            m_baseHeap = new List<KeyValuePair<P, V>>(capacity);
            m_comparer = comparer;
        }

        public Heap(IComparer<P> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException();

            m_baseHeap = new List<KeyValuePair<P, V>>();
            m_comparer = comparer;
        }

        public Heap(IEnumerable<KeyValuePair<P, V>> data)
            : this(data, Comparer<P>.Default)
        {
        }

        public Heap(IEnumerable<KeyValuePair<P, V>> data, IComparer<P> comparer)
        {
            if (data == null || comparer == null)
                throw new ArgumentNullException();

            m_comparer = comparer;
            m_baseHeap = new List<KeyValuePair<P, V>>(data);
            for (int pos = m_baseHeap.Count / 2 - 1; pos >= 0; pos--)
                Heapify(pos);
        }

        public static Heap<P, V> MergeQueues(Heap<P, V> pq1, Heap<P, V> pq2)
        {
            if (pq1 == null || pq2 == null)
                throw new ArgumentNullException();
            if (pq1.m_comparer != pq2.m_comparer)
                throw new InvalidOperationException("Heaps to be merged must have equal comparers");
            return MergeQueues(pq1, pq2, pq1.m_comparer);
        }

        public static Heap<P, V> MergeQueues(Heap<P, V> pq1, Heap<P, V> pq2, IComparer<P> comparer)
        {
            if (pq1 == null || pq2 == null || comparer == null)
                throw new ArgumentNullException();
            Heap<P, V> result = new Heap<P, V>(pq1.Count + pq2.Count, pq1.m_comparer);
            result.m_baseHeap.AddRange(pq1.m_baseHeap);
            result.m_baseHeap.AddRange(pq2.m_baseHeap);
            for (int pos = result.m_baseHeap.Count / 2 - 1; pos >= 0; pos--)
                result.Heapify(pos);

            return result;
        }

        public void Enqueue(P priority, V value)
        {
            Insert(priority, value);
        }

        public KeyValuePair<P, V> Dequeue()
        {
            if (!IsEmpty)
            {
                KeyValuePair<P, V> result = m_baseHeap[0];
                DeleteRoot();
                return result;
            }
            else
                throw new InvalidOperationException("Heap is empty");
        }

        public V DequeueValue()
        {
            return Dequeue().Value;
        }

        public KeyValuePair<P, V> Peek()
        {
            if (!IsEmpty)
                return m_baseHeap[0];
            else
                throw new InvalidOperationException("Heap is empty");
        }

        public V PeekValue()
        {
            return Peek().Value;
        }

        public bool IsEmpty
        {
            get { return m_baseHeap.Count == 0; }
        }


        private void ExchangeElements(int pos1, int pos2)
        {
            KeyValuePair<P, V> val = m_baseHeap[pos1];
            m_baseHeap[pos1] = m_baseHeap[pos2];
            m_baseHeap[pos2] = val;
        }

        private void Insert(P priority, V value)
        {
            KeyValuePair<P, V> val = new KeyValuePair<P, V>(priority, value);
            m_baseHeap.Add(val);

            int pos = m_baseHeap.Count - 1;
            while (pos > 0)
            {
                int parentPos = (pos - 1) / 2;
                if (m_comparer.Compare(m_baseHeap[parentPos].Key, m_baseHeap[pos].Key) > 0)
                {
                    ExchangeElements(parentPos, pos);
                    pos = parentPos;
                }
                else break;
            }
        }

        private void DeleteRoot()
        {
            if (m_baseHeap.Count <= 1)
            {
                m_baseHeap.Clear();
                return;
            }

            m_baseHeap[0] = m_baseHeap[m_baseHeap.Count - 1];
            m_baseHeap.RemoveAt(m_baseHeap.Count - 1);

            Heapify(0);
        }

        private void Heapify(int pos)
        {
            if (pos >= m_baseHeap.Count) return;

            while (true)
            {
                int smallest = pos;
                int left = 2 * pos + 1;
                int right = 2 * pos + 2;
                if (left < m_baseHeap.Count && m_comparer.Compare(m_baseHeap[smallest].Key, m_baseHeap[left].Key) > 0)
                    smallest = left;
                if (right < m_baseHeap.Count && m_comparer.Compare(m_baseHeap[smallest].Key, m_baseHeap[right].Key) > 0)
                    smallest = right;

                if (smallest != pos)
                {
                    ExchangeElements(smallest, pos);
                    pos = smallest;
                }
                else break;
            }
        }

        public void Add(KeyValuePair<P, V> item)
        {
            Enqueue(item.Key, item.Value);
        }

        public void Clear()
        {
            m_baseHeap.Clear();
        }
        public bool Contains(KeyValuePair<P, V> item)
        {
            return m_baseHeap.Contains(item);
        }

        public int Count
        {
            get { return m_baseHeap.Count; }
        }

        public void CopyTo(KeyValuePair<P, V>[] array, int arrayIndex)
        {
            m_baseHeap.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<P, V> item)
        {
            int elementIdx = m_baseHeap.IndexOf(item);
            if (elementIdx < 0) return false;

            m_baseHeap[elementIdx] = m_baseHeap[m_baseHeap.Count - 1];
            m_baseHeap.RemoveAt(m_baseHeap.Count - 1);

            Heapify(elementIdx);

            return true;
        }

        public IEnumerator<KeyValuePair<P, V>> GetEnumerator()
        {
            return m_baseHeap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
