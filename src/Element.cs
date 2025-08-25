using System.Collections.Generic;
using Zene.Graphics;
using Zene.Structs;

namespace Zene.GUI
{
    public class Element : IRenderable
    {
        public Element()
        {
            
        }
        public Element(Length width, Length height)
        {
            this.width = width;
            this.height = height;
        }
        
        // used for layout calculations
        internal Rectangle bounds;
        internal Vector2 minSize;
        internal Vector2 maxSize;
        
        internal Position position = Position.Normal;
        internal Size xOffset = Size.Zero;
        internal Size yOffset = Size.Zero;
        internal Length width = Length.Fit();
        internal Length height = Length.Fit();
        // Size should not exceed parents available space
        internal bool clampWidth = true;
        internal bool clampHeight = true;
        
        internal SizeBounds padding = SizeBounds.Pixels(0);
        internal SizeBounds margin = SizeBounds.Pixels(0);
        
        internal ChildLayout childLayout = new ChildLayout();
        
        // Final bounds of element relative to parent
        public Rectangle Bounds => bounds;
        
        // TEMP
        public ColourF Colour { get; set; }
        public Element Parent { get; set; }
        public List<Element> Children { get; } = new List<Element>();
        public void OnRender(IDrawingContext context)
        {
            context.DrawBox(bounds, Colour);
        }
    }
}