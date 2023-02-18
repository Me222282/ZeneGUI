using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public interface IElement
    {
        public UIProperties Properties { get; }
        public ElementList Children { get; }

        public Box Bounds => Properties.bounds;
        public Vector2 Size => Properties.bounds.Size;
        public Vector2 Location => Properties.bounds.Location;

        public bool HasChildren => Children != null && Children.Length > 0;
        public bool IsParent => Children != null;
        public bool HasParent => Properties.parent != null;

        /// <summary>
        /// Determines whether <paramref name="key"/> is currently being pressed.
        /// </summary>
        /// <param name="key">THe key to query</param>
        /// <returns></returns>
        public bool this[Keys key] => Properties.focus && Properties.Window[key];
        /// <summary>
        /// Determines whether <paramref name="mod"/> is currently active.
        /// </summary>
        /// <remarks>
        /// Throws <see cref="NotSupportedException"/> if <paramref name="mod"/> is <see cref="Mods.CapsLock"/> or <see cref="Mods.NumLock"/>.
        /// </remarks>
        /// <param name="mod">The modifier to query.</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public bool this[Mods mod] => Properties.Window[mod];
        /// <summary>
        /// Determines whether <paramref name="button"/> is currently pressed.
        /// </summary>
        /// <param name="button">The mouse button to query.</param>
        /// <returns></returns>
        public bool this[MouseButton button] => Properties.selected && Properties.Window[button];

        protected ActionManager Actions
        {
            get
            {
                Window w = Properties.Window;
                if (w == null)
                {
                    return ActionManager.Temporary;
                }

                return w.GraphicsContext.Actions;
            }
        }

        public string Id { get; }

        public ILayout Layout { get; }
        public ILayoutManager LayoutManager { get; }

        public UIEvents Events { get; }

        public GraphicsManager Graphics { get; }

        public bool IsMouseHover(Vector2 mousePos);
    }
}
