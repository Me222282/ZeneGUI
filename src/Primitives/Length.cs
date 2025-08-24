namespace Zene.GUI
{
    public struct Length
    {
        private enum LengthType
        {
            Fixed,
            Portion,
            // Fill,
            Shrink
        }
        
        private Length(LengthType t, Size v)
        {
            _type = t;
            _value = v;
        }
        
        private LengthType _type;
        private Size _value;
        
        public static Length Fixed(Size size) => new Length(LengthType.Fixed, size);
        public static Length Portion(uint portion) => new Length(LengthType.Portion, Size.Pixels(portion));
        public static Length Fill() => Portion(1);
        public static Length Shrink() => new Length(LengthType.Shrink, Size.Zero);
    }
}