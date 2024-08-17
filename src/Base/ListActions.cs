using System;
using System.Collections.Generic;

namespace Zene.GUI
{
    public class ListActions : IDisposable
    {
        public ListActions(IElement source)
        {
            _source = source.Children;
            _actions = _source.InitGroupAction();
        }
        public ListActions(ElementList source)
        {
            _source = source;
            _actions = source.InitGroupAction();
        }
        
        private readonly ElementList _source;
        private List<ElementList.Action> _actions;

        public IElement FallBackFocus { get; set; } = null;

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed) { return; }
            
            _disposed = true;
            _actions = null;
            _source.ImplementActions(FallBackFocus);

            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Calls <see cref="Dispose"/>.
        /// </summary>
        public void Apply() => Dispose();
        
        public void Add(IElement item)
        {
            lock (_actions)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Add, item, null));
            }
        }
        public void Clear()
        {
            lock (_actions)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Clear, null, null));
            }
        }
        public void Remove(IElement item)
        {
            lock (_actions)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Remove, item, null));
            }
        }
        public void RemoveAt(int index)
        {
            lock (_actions)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.RemoveAt, index, 0));
            }
        }
        public void Sort(Comparison<IElement> comparison)
        {
            lock (_actions)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Sort, comparison));
            }
        }
        public void SortDepth(Comparison<IElement> comparison)
        {
            lock (_actions)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.SortDepth, comparison));
            }
        }
        public void Swap(IElement a, IElement b)
        {
            lock (_actions)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.Swap, a, b));
            }
        }
        public void Swap(int indexA, int indexB)
        {
            lock (_actions)
            {
                _actions.Add(new ElementList.Action(ElementList.ActionType.SwapAt, indexA, indexB));
            }
        }
    }
}