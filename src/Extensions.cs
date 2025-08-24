#pragma warning disable CS8981
global using floatv =
#if DOUBLE
    System.Double;
#else
    System.Single;
#endif

global using Maths =
#if DOUBLE
    System.Math;
#else
    System.MathF;
#endif
#pragma warning restore CS8981

namespace Zene.GUI
{
    internal static class Extensions
    {
        
    }
}