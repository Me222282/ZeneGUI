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
        internal Size dirGap;
        internal Size wrapGap;
        internal LayoutDirection direction;
        internal Alignment dirAlign;
        internal Alignment wrapAlign;
        internal bool wrapping;
        internal bool scrolling;
    }
}