using System;
using System.Collections.Generic;

namespace Zene.GUI
{
    public class ListActions : IDisposable
    {
        public ListActions(IElement source)
        {
            _source = source.Children;
            _source.InitGroupAction(this);
        }
        public ListActions(ElementList source)
        {
            _source = source;
            source.InitGroupAction(this);
        }
        
        private readonly ElementList _source;
        private List<ElementList.Action> _actions = new List<ElementList.Action>();
        private object _lock = new object();

        public IElement EndingFocus { get; set; } = null;

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed) { return; }
            
            _disposed = true;
            lock (_lock)
            {
                _source.ImplementActions(_actions, EndingFocus);
                // so that any further attempts to add throw exception
                _actions = null;
            }

            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Calls <see cref="Dispose"/>.
        /// </summary>
        public void Apply() => Dispose();
        
        public void Add(IElement item)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Add, item, null));
            }
        }
        public void Insert(int index, IElement item)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Insert, index, item));
            }
        }
        public void Clear()
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Clear, null, null));
            }
        }
        public void Remove(IElement item)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Remove, item, null));
            }
        }
        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.RemoveAt, index, 0));
            }
        }
        public void Sort(Comparison<IElement> comparison)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Sort, comparison));
            }
        }
        public void SortDepth(Comparison<IElement> comparison)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.SortDepth, comparison));
            }
        }
        public void Swap(IElement a, IElement b)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Swap, a, b));
            }
        }
        public void Swap(int indexA, int indexB)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.SwapAt, indexA, indexB));
            }
        }
        public void Replace(IElement item, IElement replacement)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Replace, item, replacement));
            }
        }
        public void ReplaceAt(int index, IElement replacement)
        {
            lock (_lock)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.ReplaceAt, index, replacement));
            }
        }
    }
}