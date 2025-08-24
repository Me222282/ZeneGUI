using Zene.Structs;

namespace Zene.GUI
{
    public class Element
    {
        // used for layout calculations
        internal Rectangle bounds;
        internal Vector2 minSize;
        internal Vector2 maxSize;
        
        internal Position position;
        internal Size xOffset;
        internal Size yOffset;
        internal Length width;
        internal Length height;
        internal Size minWidth;
        internal Size minHeight;
        internal Size maxWidth;
        internal Size maxHeight;
        
        internal SizeBounds padding;
        internal SizeBounds margin;
        
        internal ChildLayout childLayout;
        
        // Final bounds of element relative to parent
        public Rectangle Bounds => bounds;
    }
}