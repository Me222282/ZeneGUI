using System;
using System.Collections;
using System.Collections.Generic;

namespace Zene.GUI
{
    public sealed class ElementList : IList<IElement>
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

            foreach (IElement e in item.Children._elements)
            {
                SetHandle(e, handle);
            }
        }
        public void Add(IElement item)
        {
            if (item.Properties.elementIndex >= 0)
            {
                throw new ArgumentException("The given element is already the child of another element.", nameof(item));
            }

            SetHandle(item, _source.Properties.handle);
            item.Properties.parent = _source;
            item.Properties.elementIndex = _elements.Count;

            lock (_lockRef)
            {
                _elements.Add(item);
            }

            _source.Properties.handle?.LayoutElement(_source);
        }
        public void Clear()
        {
            lock (_lockRef)
            {
                foreach (IElement e in _elements)
                {
                    if (e.Properties.focus)
                    {
                        _source.Properties.handle.Focus = null;
                    }

                    e.Properties.parent = null;
                    e.Properties.elementIndex = -1;
                    e.Properties.hover = false;
                    e.Properties.selected = false;

                    SetHandle(e, null);
                }

                _elements.Clear();
            }

            _source.Properties.handle?.LayoutElement(_source);
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

        public bool Swap(IElement a, IElement b) => Swap(_elements.IndexOf(a), _elements.IndexOf(b));
        public bool Swap(int indexA, int indexB)
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

        public IEnumerator<IElement> GetEnumerator() => _elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();

        public int IndexOf(IElement item) => _elements.IndexOf(item);

        public bool Remove(IElement item)
        {
            int index = _elements.IndexOf(item);

            if (index < 0) { return false; }

            RemoveAt(index);
            return true;
        }
        public void RemoveAt(int index)
        {
            IElement item = _elements[index];
            SetHandle(item, null);
            item.Properties.parent = null;
            item.Properties.elementIndex = -1;
            item.Properties.hover = false;
            item.Properties.selected = false;

            if (item.Properties.focus)
            {
                _source.Properties.handle.Focus = null;
            }

            lock (_lockRef)
            {
                _elements.RemoveAt(index);

                for (int i = index; i < _elements.Count; i++)
                {
                    _elements[i].Properties.elementIndex = i;
                }
            }

            _source.Properties.handle?.LayoutElement(_source);
        }
    }
}
