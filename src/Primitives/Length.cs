namespace Zene.GUI
{
    public enum LengthType : byte
    {
        Fixed,
        Portion,
        // Fill,
        Shrink,
        Fit
    }
    
    public struct Length
    {   
        private Length(LengthType t, Size v)
        {
            _type = t;
            _value = v;
        }
        
        private Size _value;
        private LengthType _type;
        
        public static Length Fixed(Size size) => new Length(LengthType.Fixed, size);
        public static Length Portion(uint portion) => new Length(LengthType.Portion, Size.Pixels(portion));
        public static Length Fill() => Portion(1);
        public static Length Shrink() => new Length(LengthType.Shrink, Size.Zero);
        public static Length Fit() => new Length(LengthType.Fit, Size.Zero);
    }
}