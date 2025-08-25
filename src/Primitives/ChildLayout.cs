namespace Zene.GUI
{
    public enum LayoutDirection : byte
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }
    public enum Alignment : byte
    {
        NearSide,
        Centre,
        FarSide,
        SpacedEvenly
    }
    
    public struct ChildLayout
    {
        public ChildLayout()
        {
            
        }
        
        internal Size dirGap = Size.Zero;
        internal Size wrapGap = Size.Zero;
        internal LayoutDirection direction = LayoutDirection.LeftToRight;
        internal Alignment dirAlign = Alignment.NearSide;
        internal Alignment wrapAlign = Alignment.NearSide;
        internal bool wrapping = false;
        internal bool scrolling = false;
    }
}