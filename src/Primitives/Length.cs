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
        private Length(LengthType t, Size v, Size v2)
        {
            _type = t;
            _value = v;
            _value2 = v2;
        }
        private Length(uint port, Size min, Size max)
        {
            _type = LengthType.Portion;
            _port = port;
            _value = min;
            _value2 = max;
        }
        
        private Size _value;
        private Size _value2;
        private uint _port;
        private LengthType _type;
        
        public static Length Fixed(Size size) => new Length(LengthType.Fixed, size, Size.None);
        
        public static Length Portion(uint portion) => new Length(portion, Size.None, Size.None);
        public static Length Fill() => Portion(1);
        public static Length Shrink() => new Length(LengthType.Shrink, Size.None, Size.None);
        public static Length Fit() => new Length(LengthType.Fit, Size.None, Size.None);
        
        public static Length Portion(uint portion, Size min, Size max) => new Length(portion, min, max);
        public static Length Fill(Size min, Size max) => Portion(1, min, max);
        public static Length Shrink(Size min, Size max) => new Length(LengthType.Shrink, min, max);
        public static Length Fit(Size min, Size max) => new Length(LengthType.Fit, min, max);
        
        public static Length PortionWithMin(uint portion, Size min) => new Length(portion, min, Size.None);
        public static Length FillWithMin(Size min) => PortionWithMin(1, min);
        public static Length ShrinkWithMin(Size min) => new Length(LengthType.Shrink, min, Size.None);
        public static Length FitWithMin(Size min) => new Length(LengthType.Fit, min, Size.None);
        
        public static Length PortionWithMax(uint portion, Size max) => new Length(portion, Size.None, max);
        public static Length FillWithMax(Size max) => PortionWithMax(1, max);
        public static Length ShrinkWithMax(Size max) => new Length(LengthType.Shrink, Size.None, max);
        public static Length FitWithMax(Size max) => new Length(LengthType.Fit, Size.None, max);
    }
}