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
            Insert,
            Clear,
            Remove,
            RemoveAt,
            Sort,
            SortDepth,
            Swap,
            SwapAt,
            Replace,
            ReplaceAt
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
            public Action(ActionType at, int ia, IElement eb)
            {
                AT = at;
                EA = null;
                EB = eb;
                IA = ia;
                IB = 0;
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
        
        private ListActions _actions;
        private bool _inGroupAction = false;
        
        public bool InGroupAction => _inGroupAction;
        public ListActions StartGroupAction() => new ListActions(this);
        internal void InitGroupAction(ListActions current)
        {
            if (_inGroupAction)
            {
                throw new Exception("Group action already started.");
            }
            
            _inGroupAction = true;
            _actions = current;
        }
        internal void ImplementActions(List<Action> actions, IElement ef = null)
        {
            _inGroupAction = false;
            _actions = null;
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                IA(actions);
                if (ef != null)
                {
                    _source.Properties.handle.Focus = ef;
                }
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() => 
            {
                IA(actions);
                _source.Properties.handle.LayoutElement(_source);
                // Don't know if this is needed
                UIManager.RecalculateScrollBounds(_source.Properties);
                if (ef != null)
                {
                    _source.Properties.handle.Focus = ef;
                }
            });
        }
        private void IA(List<Action> actions)
        {
            Span<Action> span = CollectionsMarshal.AsSpan(actions);
            for (int i = 0; i < span.Length; i++)
            {
                Action a = span[i];
                switch (a.AT)
                {
                    case ActionType.Add:
                        BaseAdd(a.EA);
                        continue;
                    case ActionType.Insert:
                        BaseInsert(a.IA, a.EB);
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
                    case ActionType.Replace:
                        BaseReplace(a.EA, a.EB);
                        continue;
                    case ActionType.ReplaceAt:
                        BaseReplaceAt(a.IA, a.EB);
                        continue;
                }
            }
        }
        
        public override void Add(IElement item)
        {
            if (_inGroupAction)
            {
                _actions.Add(item);
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
        public override void Insert(int index, IElement item)
        {
            if (_inGroupAction)
            {
                _actions.Insert(index, item);
                return;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                BaseInsert(index, item);
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() => 
            {
                BaseInsert(index, item);
                _source.Properties.handle.LayoutElement(_source);
            });
        }

        public override void Clear()
        {
            if (_inGroupAction)
            {
                _actions.Clear();
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
                _actions.Remove(item);
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
                
                if (((Bounds)item.GetRenderBounds()).ShareBound(_source.Properties.scrollBounds))
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
                _actions.RemoveAt(index);
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
                
                if (((Bounds)e.GetRenderBounds()).ShareBound(_source.Properties.scrollBounds))
                {
                    UIManager.RecalculateScrollBounds(_source.Properties);
                }
            });
        }
        public override bool Replace(IElement item, IElement replacement)
        {
            if (_inGroupAction)
            {
                _actions.Replace(item, replacement);
                return true;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                return BaseReplace(item, replacement);
            }
            
            h.Window.GraphicsContext.Actions.Push(() =>
            {
                BaseReplace(item, replacement);
                _source.Properties.handle.LayoutElement(_source);
                
                if (((Bounds)item.GetRenderBounds()).ShareBound(_source.Properties.scrollBounds))
                {
                    UIManager.RecalculateScrollBounds(_source.Properties);
                }
            });
            // ?
            return true;
        }
        public override void ReplaceAt(int index, IElement replacement)
        {
            if (_inGroupAction)
            {
                _actions.ReplaceAt(index, replacement);
                return;
            }
            
            UIManager h = _source.Properties.handle;
            if (h == null)
            {
                BaseReplaceAt(index, replacement);
                return;
            }
            
            h.Window.GraphicsContext.Actions.Push(() =>
            {
                IElement e = this[index];
                BaseReplaceAt(index, replacement);
                _source.Properties.handle.LayoutElement(_source);
                
                if (((Bounds)e.GetRenderBounds()).ShareBound(_source.Properties.scrollBounds))
                {
                    UIManager.RecalculateScrollBounds(_source.Properties);
                }
            });
        }

        public override void Sort(Comparison<IElement> comparison)
        {
            if (_inGroupAction)
            {
                _actions.Sort(comparison);
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
                _actions.SortDepth(comparison);
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
                _actions.Swap(a, b);
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
                _actions.Swap(indexA, indexB);
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
