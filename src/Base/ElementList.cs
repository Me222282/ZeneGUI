using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zene.Structs;

namespace Zene.GUI
{
    public class ElementList : ElementListManage
    {
        public enum ActionType : byte
        {
            Add,
            Clear,
            Remove,
            RemoveAt,
            Sort,
            SortDepth,
            Swap,
            SwapAt
        }
        public struct Action
        {
            public Action(ActionType at, IElement ea, IElement eb)
            {
                AT = at;
                EA = ea;
                EB = eb;
                IA = 0;
                IB = 0;
                Comp = null;
            }
            public Action(ActionType at, int ia, int ib)
            {
                AT = at;
                EA = null;
                EB = null;
                IA = ia;
                IB = ib;
                Comp = null;
            }
            public Action(ActionType at, Comparison<IElement> comp)
            {
                AT = at;
                EA = null;
                EB = null;
                IA = 0;
                IB = 0;
                Comp = comp;
            }
            
            public ActionType AT;
            public IElement EA;
            public IElement EB;
            public int IA;
            public int IB;
            public Comparison<IElement> Comp;
        }
        
        public ElementList(IElement source)
            : base(source)
        {
            
        }
        
        private List<Action> _actions = new List<Action>();
        private bool _inGroupAction = false;
        
        internal List<Action> InitGroupAction()
        {
            _actions.Clear();
            _inGroupAction = true;
            return _actions;
        }
        internal void ImplementActions(IElement fbf = null)
        {
            _inGroupAction = false;
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                IA();
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() => 
            {
                if (fbf != null)
                {
                    _source.Properties.handle.FallBackFocus = fbf;
                }
                IA();
                _source.Properties.handle.LayoutElement(_source);
                // Don't know if this is needed
                UIManager.RecalculateScrollBounds(_source.Properties);
            });
        }
        private void IA()
        {
            Span<Action> span = CollectionsMarshal.AsSpan(_actions);
            for (int i = 0; i < span.Length; i++)
            {
                Action a = span[i];
                switch (a.AT)
                {
                    case ActionType.Add:
                        BaseAdd(a.EA);
                        continue;
                    case ActionType.Clear:
                        BaseClear();
                        continue;
                    case ActionType.Remove:
                        BaseRemove(a.EA);
                        continue;
                    case ActionType.RemoveAt:
                        BaseRemoveAt(a.IA);
                        continue;
                    case ActionType.Sort:
                        BaseSort(a.Comp);
                        continue;
                    case ActionType.SortDepth:
                        BaseSort(a.Comp);
                        continue;
                    case ActionType.Swap:
                        BaseSwap(a.EA, a.EB);
                        continue;
                    case ActionType.SwapAt:
                        BaseSwap(a.IA, a.IB);
                        continue;
                }
            }
        }
        
        public override void Add(IElement item)
        {
            if (_inGroupAction)
            {
                lock (_actions)
                {
                    _actions.Add(new Action(ActionType.Add, item, null));
                }
                return;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                BaseAdd(item);
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() => 
            {
                BaseAdd(item);
                _source.Properties.handle.LayoutElement(_source);
            });
        }

        public override void Clear()
        {
            if (_inGroupAction)
            {
                lock (_actions)
                {
                    _actions.Add(new Action(ActionType.Clear, null, null));
                }
                return;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                BaseClear();
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() =>
            {
                BaseClear();
                if (_source.LayoutManager != null &&
                (_source.LayoutManager.ChildDependent ||
                _source.LayoutManager.SizeDependent))
                {
                    _source.Properties.handle.LayoutElement(_source);
                }
                else
                {
                    UIManager.RecalculateScrollBounds(_source.Properties);
                }
            });
        }

        public override bool Remove(IElement item)
        {
            if (_inGroupAction)
            {
                lock (_actions)
                {
                    _actions.Add(new Action(ActionType.Remove, item, null));
                }
                return true;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                return BaseRemove(item);
            }
            
            h.Window.GraphicsContext.Actions.Push(() =>
            {
                BaseRemove(item);
                _source.Properties.handle.LayoutElement(_source);
                
                if (item.GetRenderBounds().ShareBound(_source.Properties.scrollBounds))
                {
                    UIManager.RecalculateScrollBounds(_source.Properties);
                }
            });
            // ?
            return true;
        }
        public override void RemoveAt(int index)
        {
            if (_inGroupAction)
            {
                lock (_actions)
                {
                    _actions.Add(new Action(ActionType.RemoveAt, index, 0));
                }
                return;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                BaseRemoveAt(index);
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() =>
            {
                IElement e = this[index];
                BaseRemoveAt(index);
                _source.Properties.handle.LayoutElement(_source);
                
                if (e.GetRenderBounds().ShareBound(_source.Properties.scrollBounds))
                {
                    UIManager.RecalculateScrollBounds(_source.Properties);
                }
            });
        }

        public override void Sort(Comparison<IElement> comparison)
        {
            if (_inGroupAction)
            {
                lock (_actions)
                {
                    _actions.Add(new Action(ActionType.Sort, comparison));
                }
                return;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                BaseSort(comparison);
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() =>
            {
                BaseSort(comparison);
                _source.Properties.handle.LayoutElement(_source);
            });
        }

        public override void SortDepth(Comparison<IElement> comparison)
        {
            if (_inGroupAction)
            {
                lock (_actions)
                {
                    _actions.Add(new Action(ActionType.SortDepth, comparison));
                }
                return;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                BaseSortDepth(comparison);
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() =>
            {
                BaseSortDepth(comparison);
                _source.Properties.handle.LayoutElement(_source);
            });
        }

        public override bool Swap(IElement a, IElement b)
        {
            if (_inGroupAction)
            {
                lock (_actions)
                {
                    _actions.Add(new Action(ActionType.Swap, a, b));
                }
                return true;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                return BaseSwap(a, b);
            }
            
            h.Window.GraphicsContext.Actions.Push(() => 
            {
                BaseSwap(a, b);
                _source.Properties.handle.LayoutElement(_source);
            });
            // ?
            return true;
        }

        public override bool Swap(int indexA, int indexB)
        {
            if (_inGroupAction)
            {
                lock (_actions)
                {
                    _actions.Add(new Action(ActionType.SwapAt, indexA, indexB));
                }
                return true;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                return BaseSwap(indexA, indexB);
            }
            
            h.Window.GraphicsContext.Actions.Push(() => 
            {
                BaseSwap(indexA, indexB);
                _source.Properties.handle.LayoutElement(_source);
            });
            // ?
            return true;
        }
    }
}
