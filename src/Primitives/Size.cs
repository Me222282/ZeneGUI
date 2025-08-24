namespace Zene.GUI
{
    public struct Size
    {
        private enum SizeType
        {
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
        
        private SizeType _type;
        private floatv _value;
        
        // public floatv Calculate()
        // {
            
        // }
        
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