namespace Zene.GUI
{
    public struct Size
    {
        private enum SizeType : byte
        {
            None,
            Pixels,
            Char,
            Font,
            ViewWidth,
            ViewHeight,
            ViewSmall,
            ViewLarge
        }
        
        private Size(SizeType t, floatv v)
        {
            _type = t;
            _value = v;
        }
        
        private floatv _value;
        private SizeType _type;
        
        public floatv Calculate(UIManager manager, Element context)
        {
            return _type switch
            {
                SizeType.None => 0,
                SizeType.Pixels => _value,
                SizeType.ViewWidth => manager.FrameSize.X * _value,
                SizeType.ViewHeight => manager.FrameSize.Y * _value,
                SizeType.ViewSmall => manager.FrameSizeSL.X * _value,
                SizeType.ViewLarge => manager.FrameSizeSL.Y * _value,
                _ => _value,
            };
        }
        
        public static Size None = new Size(SizeType.None, 0);
        public static Size Zero = Pixels(0);
        public static Size Pixels(floatv value) => new Size(SizeType.Pixels, value);
        public static Size Char(floatv value) => new Size(SizeType.Char, value);
        public static Size Font(floatv value) => new Size(SizeType.Font, value);
        public static Size ViewWidth(floatv value) => new Size(SizeType.ViewWidth, value);
        public static Size ViewHeight(floatv value) => new Size(SizeType.ViewHeight, value);
        public static Size ViewSmall(floatv value) => new Size(SizeType.ViewSmall, value);
        public static Size ViewLarge(floatv value) => new Size(SizeType.ViewLarge, value);
    }
}