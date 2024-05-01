using System;
using System.Collections;
using System.Collections.Generic;
using Zene.Structs;

namespace Zene.GUI
{
    public class ElementList : IList<IElement>
    {
        public ElementList(IElement source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        private readonly IElement _source;
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

            item.Properties.handle = handle;
            if (!item.HasChildren) { return; }

            foreach (IElement e in item.Children)
            {
                SetHandle(e, handle);
            }
        }
        public virtual void Add(IElement item)
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

            _source.Properties.handle?.LayoutElement(_source);
        }
        public virtual void Clear()
        {
            _source.Properties.handle.Window.GraphicsContext.Actions.Push(() =>
            {
                lock (_lockRef)
                {
                    foreach (IElement e in _elements)
                    {
                        if (e.Properties.focus && _source.Properties.handle != null)
                        {
                            _source.Properties.handle.Focus = null;
                        }

                        e.Properties.focus = false;
                        e.Properties.parent = null;
                        e.Properties.elementIndex = -1;
                        e.Properties.hover = false;
                        e.Properties.selected = false;

                        SetHandle(e, null);
                    }

                    _elements.Clear();
                }

                if (_source.LayoutManager != null &&
                    (_source.LayoutManager.ChildDependent ||
                    _source.LayoutManager.SizeDependent))
                {
                    _source.Properties.handle?.LayoutElement(_source);
                }
                else
                {
                    UIManager.RecalculateScrollBounds(_source.Properties);
                }
            });
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

        public bool Swap(IElement a, IElement b) => Swap(_elements.IndexOf(a), _elements.IndexOf(b));
        public virtual bool Swap(int indexA, int indexB)
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

            _source.Properties.handle?.LayoutElement(_source);

            return true;
        }

        void ICollection<IElement>.CopyTo(IElement[] array, int arrayIndex) => _elements.CopyTo(array, arrayIndex);
        void IList<IElement>.Insert(int index, IElement item) => throw new NotSupportedException();

        public IEnumerator<IElement> GetEnumerator() => new Enumerator(_elements);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_elements);

        public int IndexOf(IElement item) => _elements.IndexOf(item);

        public bool Remove(IElement item)
        {
            int index = _elements.IndexOf(item);

            if (index < 0) { return false; }

            RemoveAt(index);
            return true;
        }
        public virtual void RemoveAt(int index)
        {
            IElement item = _elements[index];
            SetHandle(item, null);
            item.Properties.parent = null;
            item.Properties.elementIndex = -1;
            item.Properties.hover = false;
            item.Properties.selected = false;

            if (item.Properties.focus && _source.Properties.handle != null)
            {
                _source.Properties.handle.Focus = null;
            }
            item.Properties.focus = false;

            lock (_lockRef)
            {
                _elements.RemoveAt(index);

                for (int i = index; i < _elements.Count; i++)
                {
                    _elements[i].Properties.elementIndex = i;
                }
            }

            if (item.GetRenderBounds().ShareBound(_source.Properties.scrollBounds))
            {
                UIManager.RecalculateScrollBounds(_source.Properties);
            }
            
            _source.Properties.handle?.LayoutElement(_source);
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
