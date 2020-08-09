using System;
using System.Collections.Generic;

namespace Utils
{
    public class Heap<T>
        {
            private static int Parent(int i) => i / 2;
            private static int Left(int i) => i * 2;
            private static int Right(int i) => i * 2 + 1;

            private readonly IComparer<T> _comparer;
            private readonly List<T> _content = new List<T> {default};

            public Heap(Comparison<T> comparison)
            {
                _comparer = Comparer<T>.Create(comparison);
            }

            public int Count => _content.Count - 1;

            public void Push(T element)
            {
                _content.Add(element);
                SiftUp(_content.Count - 1);
            }

            public T Pop()
            {
                var result = _content[1];
                _content[1] = _content[_content.Count - 1];
                _content.RemoveAt(_content.Count - 1);
                SiftDown(1);
                return result;
            }
            
            private void SiftUp(int i)
            {
                while (i > 1)
                {
                    var parent = Parent(i);
                    if (_comparer.Compare(_content[i], _content[parent]) > 0)
                        return;

                    (_content[parent], _content[i]) = (_content[i], _content[parent]);
                    i = parent;
                }
            }

            private void SiftDown(int i)
            {
                for (var left = Left(i); left < _content.Count; left = Left(i))
                {
                    var smallest = _comparer.Compare(_content[left], _content[i]) <= 0 ? left : i;
                    var right = Right(i);

                    if (right < _content.Count && _comparer.Compare(_content[right], _content[smallest]) <= 0)
                        smallest = right;

                    if (smallest == i)
                        return;

                    (_content[i], _content[smallest]) = (_content[smallest], _content[i]);
                    i = smallest;
                }
            }
        }
}