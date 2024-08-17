using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zene.Structs;

namespace Zene.GUI
{
    public abstract class ElementListManage : IList<IElement>
    {
        public ElementListManage(IElement source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        protected internal readonly IElement _source;
        public IElement Source => _source;

        private readonly List<IElement> _elements = new List<IElement>();
        private readonly object _lockRef = new object();

        public int Length => _elements.Count;

        public IElement this[int index] => _elements[index];
        public IElement this[Index index] => _elements[index];

        IElement IList<IElement>.this[int index]
        {
            get => _elements[index];
            set => throw new NotSupportedException();
        }
        int ICollection<IElement>.Count => _elements.Count;

        public bool IsReadOnly => false;

        private static void SetHandle(IElement item, UIManager handle)
        {
            if (item.Properties.handle == handle) { return; }

            if (item.Properties.handle != null &&
                item.Properties.handle.Focus == item &&
                item.Properties.handle != handle)
            {
                //item.Properties.handle.Focus = null;
                // Very specific bug fixed with this
                item.Properties.handle.ResetFocusNoEvent();
                item.Properties.focus = false;
            }

            item.Properties.handle = handle;
            if (!item.HasChildren) { return; }

            foreach (IElement e in item.Children)
            {
                SetHandle(e, handle);
            }
        }
        public abstract void Add(IElement item);
        protected void BaseAdd(IElement item)
        {
            if (item.Properties.elementIndex >= 0)
            {
                throw new ArgumentException("The given element is already the child of another element.", nameof(item));
            }

            SetHandle(item, _source.Properties.handle);
            item.Properties.parent = _source;
            item.Properties.elementIndex = _elements.Count;

            if (item.Properties.Depth < 0d)
            {
                item.Properties.Depth = item.Properties.elementIndex;
            }

            lock (_lockRef)
            {
                _elements.Add(item);
            }
        }
        public abstract void Clear();
        protected void BaseClear()
        {
            
            lock (_lockRef)
            {
                foreach (IElement e in _elements)
                {
                    // e.Properties.focus = false;
                    e.Properties.parent = null;
                    e.Properties.elementIndex = -1;
                    e.Properties.hover = false;
                    e.Properties.selected = false;

                    SetHandle(e, null);
                }

                _elements.Clear();
            }
        }

        public bool Contains(IElement item) => _elements.Contains(item);

        /// <summary>
        /// Finds the first element of a certain type. Searches child elements as well.
        /// </summary>
        /// <typeparam name="T">The type of element to search for.</typeparam>
        /// <returns></returns>
        public T RecursiveFind<T>()
            where T : class, IElement
        {
            lock (_lockRef)
            {
                foreach (IElement e in _elements)
                {
                    if (e is T t) { return t; }

                    if (e.HasChildren)
                    {
                        T test = e.Children.RecursiveFind<T>();

                        if (test == null) { continue; }

                        return test;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Finds the first element of a certain type.
        /// </summary>
        /// <typeparam name="T">The type of element to search for.</typeparam>
        /// <returns></returns>
        public T Find<T>()
            where T : class, IElement
        {
            lock (_lockRef)
            {
                foreach (IElement e in _elements)
                {
                    if (e is not T t) { continue; }

                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first element of a certain type with a given id. Searches child elements as well.
        /// </summary>
        /// <typeparam name="T">The type of element to search for.</typeparam>
        /// <returns></returns>
        public T RecursiveFind<T>(string id)
            where T : class, IElement
        {
            lock (_lockRef)
            {
                foreach (IElement e in _elements)
                {
                    if (e is T t && e.Id == id) { return t; }

                    if (e.HasChildren)
                    {
                        T test = e.Children.RecursiveFind<T>(id);

                        if (test == null) { continue; }

                        return test;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Finds the first element of a certain type with a given id.
        /// </summary>
        /// <typeparam name="T">The type of element to search for.</typeparam>
        /// <returns></returns>
        public T Find<T>(string id)
            where T : class, IElement
        {
            lock (_lockRef)
            {
                foreach (IElement e in _elements)
                {
                    if (e is not T t || e.Id != id) { continue; }

                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first element with a given id. Searches child elements as well.
        /// </summary>
        /// <returns></returns>
        public IElement RecursiveFind(string id)
        {
            lock (_lockRef)
            {
                foreach (IElement e in _elements)
                {
                    if (e.Id == id) { return e; }

                    if (e.HasChildren)
                    {
                        IElement test = e.Children.RecursiveFind(id);

                        if (test == null) { continue; }

                        return test;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Finds the first element with a given id.
        /// </summary>
        /// <returns></returns>
        public IElement Find(string id)
        {
            lock (_lockRef)
            {
                foreach (IElement e in _elements)
                {
                    if (e.Id != id) { continue; }

                    return e;
                }
            }

            return null;
        }
        
        public abstract bool Swap(IElement a, IElement b);
        public abstract bool Swap(int indexA, int indexB);
        protected bool BaseSwap(IElement a, IElement b) => BaseSwap(_elements.IndexOf(a), _elements.IndexOf(b));
        protected bool BaseSwap(int indexA, int indexB)
        {
            lock (_lockRef)
            {
                if (indexA < 0 || indexA >= _elements.Count)
                {
                    return false;
                }
                if (indexB < 0 || indexB >= _elements.Count)
                {
                    return false;
                }

                _elements[indexA].Properties.elementIndex = indexB;
                _elements[indexB].Properties.elementIndex = indexA;

                _elements.Swap(indexA, indexB);
            }

            return true;
        }

        void ICollection<IElement>.CopyTo(IElement[] array, int arrayIndex) => _elements.CopyTo(array, arrayIndex);
        void IList<IElement>.Insert(int index, IElement item) => throw new NotSupportedException();

        public IEnumerator<IElement> GetEnumerator() => new Enumerator(_elements);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_elements);

        public int IndexOf(IElement item) => _elements.IndexOf(item);
        
        public abstract bool Remove(IElement item);
        public abstract void RemoveAt(int index);
        protected bool BaseRemove(IElement item)
        {
            int index = _elements.IndexOf(item);

            if (index < 0) { return false; }

            BaseRemoveAt(index);
            return true;
        }
        protected void BaseRemoveAt(int index)
        {
            IElement item = _elements[index];
            SetHandle(item, null);
            item.Properties.parent = null;
            item.Properties.elementIndex = -1;
            item.Properties.hover = false;
            item.Properties.selected = false;

            lock (_lockRef)
            {
                _elements.RemoveAt(index);

                for (int i = index; i < _elements.Count; i++)
                {
                    _elements[i].Properties.elementIndex = i;
                }
            }
        }
        
        public abstract void Sort(Comparison<IElement> comparison);
        public abstract void SortDepth(Comparison<IElement> comparison);
        protected void BaseSort(Comparison<IElement> comparison)
        {
            lock (_lockRef)
            {
                _elements.Sort(comparison);
                
                // May be way of doing this at the same time as sorting?
                Span<IElement> span = CollectionsMarshal.AsSpan(_elements);
                for (int i = 0; i < span.Length; i++)
                {
                    span[i].Properties.elementIndex = i;
                }
            }
        }
        protected void BaseSortDepth(Comparison<IElement> comparison)
        {
            lock (_lockRef)
            {
                _elements.Sort(comparison);
                
                // May be way of doing this at the same time as sorting?
                Span<IElement> span = CollectionsMarshal.AsSpan(_elements);
                for (int i = 0; i < span.Length; i++)
                {
                    span[i].Properties.elementIndex = i;
                    span[i].Properties.Depth = i;
                }
            }
        }

        public struct Enumerator : IEnumerator<IElement>, IEnumerator
        {
            public Enumerator(List<IElement> source)
            {
                _currentIndex = 0;
                _source = source;
                _current = null;
            }

            private int _currentIndex;
            private readonly List<IElement> _source;
            private IElement _current;

            public IElement Current => _current;
            object IEnumerator.Current => _current;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (_currentIndex < _source.Count)
                {
                    _current = _source[_currentIndex];
                    _currentIndex++;
                    return true;
                }

                _current = null;
                return false;
            }

            void IEnumerator.Reset()
            {
                _currentIndex = 0;
                _current = null;
            }
        }
    }
}
