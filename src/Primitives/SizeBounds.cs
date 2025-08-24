using Zene.Structs;

namespace Zene.GUI
{
    public struct SizeBounds
    {
        public SizeBounds(Size left, Size right, Size top, Size bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
        
        public Size Left { get; set; }
        public Size Right { get; set; }
        public Size Top { get; set; }
        public Size Bottom { get; set; }
        
        // public Bounds Calculate()
        // {
            
        // }
        
        public static SizeBounds Pixels(Vector4 value)
            => new SizeBounds(Size.Pixels(value.X), Size.Pixels(value.Y),
                Size.Pixels(value.Z), Size.Pixels(value.W));
        public static SizeBounds Char(Vector4 value)
            => new SizeBounds(Size.Char(value.X), Size.Char(value.Y),
                Size.Char(value.Z), Size.Char(value.W));
        public static SizeBounds Font(Vector4 value)
            => new SizeBounds(Size.Font(value.X), Size.Font(value.Y),
                Size.Font(value.Z), Size.Font(value.W));
        public static SizeBounds ViewWidth(Vector4 value)
            => new SizeBounds(Size.ViewWidth(value.X), Size.ViewWidth(value.Y),
                Size.ViewWidth(value.Z), Size.ViewWidth(value.W));
        public static SizeBounds ViewHeight(Vector4 value)
            => new SizeBounds(Size.ViewHeight(value.X), Size.ViewHeight(value.Y),
                Size.ViewHeight(value.Z), Size.ViewHeight(value.W));
        public static SizeBounds ViewSmall(Vector4 value)
            => new SizeBounds(Size.ViewSmall(value.X), Size.ViewSmall(value.Y),
                Size.ViewSmall(value.Z), Size.ViewSmall(value.W));
        public static SizeBounds ViewLarge(Vector4 value)
            => new SizeBounds(Size.ViewLarge(value.X), Size.ViewLarge(value.Y),
                Size.ViewLarge(value.Z), Size.ViewLarge(value.W));
    }
}