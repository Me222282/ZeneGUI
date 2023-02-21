using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public delegate double AnimationType(double input);

    public class Animator
    {
        private readonly List<BaseAnimation> _animations = new List<BaseAnimation>();

        public void Add<T>(AnimatorData<T> animation)
        {
            if (animation.SetValue == null)
            {
                throw new ArgumentNullException(nameof(animation.SetValue), "There must be a SetValue function.");
            }

            if (animation.Animating)
            {
                animation.startTime = Core.Time;
                return;
            }

            if (animation.Animating || animation.Duration <= 0 ||
                animation.StartValue.Equals(animation.EndValue)) { return; }

            if (animation.Type == null)
            {
                animation.Type = Linear;
            }

            animation.startTime = Core.Time;
            lock (_animations)
            {
                _animations.Add(animation);
            }
        }
        public void Add(BaseAnimation animation)
        {
            if (animation.Animating)
            {
                animation.startTime = Core.Time;
                return;
            }

            if (animation.Duration <= 0) { return; }

            if (animation.Type == null)
            {
                animation.Type = Linear;
            }

            animation.startTime = Core.Time;
            lock (_animations)
            {
                _animations.Add(animation);
            }
        }
        public AnimatorData<T> Add<T>(Action<T> setValue, double duration, T start, T end, AnimationType type)
        {
            AnimatorData<T> ad = new AnimatorData<T>(setValue, duration, start, end, type)
            {
                startTime = Core.Time
            };
            Add(ad);
            return ad;
        }
        public AnimatorData<T> Add<T>(Action<T> setValue, double duration, T start, T end)
            => Add(setValue, duration, start, end, Linear);

        public void Invoke()
        {
            double time = Core.Time;

            lock (_animations)
            {
                ReadOnlySpan<BaseAnimation> span = CollectionsMarshal.AsSpan(_animations);

                for (int i = 0; i < _animations.Count; i++)
                {
                    BaseAnimation ad = span[i];
                    double t = time - ad.startTime;

                    // End of animation
                    if (t >= ad.Duration)
                    {
                        // Set value to end
                        ad.OnFrame(ad.Type(ad.Reversed ? 0d : 1d));

                        ad.startTime = -1;
                        ad.InvokeFinish();

                        // Remove animation
                        _animations.RemoveAt(i);
                        i--;
                        continue;
                    }

                    double scale = ad.Type(t / ad.Duration);
                    ad.OnFrame(ad.Reversed ? 1d - scale : scale);
                }
            }
        }

        private static double LinearType(double input) => input;
        private static double LogarithmicType(double input) => Math.Log((input * (Math.E - 1)) + 1);
        private static double ExponentialType(double input) => (Math.Exp(input) - 1) / (Math.E - 1);

        public static AnimationType Linear => LinearType;
        public static AnimationType Logarithmic => LogarithmicType;
        public static AnimationType Exponential => ExponentialType;

        public static AnimationType CreateLogarithmic(double exp)
        {
            double v = Math.Pow(2, exp);

            return (input) =>
            {
                return Math.Log((input * (v - 1)) + 1, v);
            };
        }
        public static AnimationType CreateExponential(double exp)
        {
            double v = Math.Pow(2, exp);

            return (input) =>
            {
                return (Math.Pow(v, input) - 1) / (v - 1);
            };
        }
    }

    public abstract class BaseAnimation
    {
        public BaseAnimation(double duration, AnimationType type)
        {
            Duration = duration;
            _type = type ?? Animator.Linear;
        }

        internal double startTime = -1;
        public double Duration { get; set; }

        public bool Reversed { get; set; } = false;
        public bool Animating => startTime > 0d;

        private AnimationType _type;
        public AnimationType Type
        {
            get => _type;
            set => _type = value ?? Animator.Linear;
        }

        public event EventHandler Finish;
        internal void InvokeFinish() => Finish?.Invoke(this, EventArgs.Empty);

        public abstract void OnFrame(double scale);

        public void Stop() => Duration = 0;
        public void Reset(Animator handle)
        {
            if (!Animating)
            {
                handle.Add(this);
                return;
            }

            startTime = Core.Time;
        }
    }

    public class AnimatorData<T> : BaseAnimation
    {
        public AnimatorData(Action<T> setValue, double duration, T start, T end, AnimationType type = null)
            : base(duration, type)
        {
            SetValue = setValue;
            StartValue = start;
            EndValue = end;

            switch (this)
            {
                case AnimatorData<double> ad:
                    _onFrame = (s) => FrameDouble(ad, s);
                    break;
                case AnimatorData<float> ad:
                    _onFrame = (s) => FrameFloat(ad, s);
                    break;
                case AnimatorData<int> ad:
                    _onFrame = (s) => FrameInt(ad, s);
                    break;
                case AnimatorData<long> ad:
                    _onFrame = (s) => FrameLong(ad, s);
                    break;
                case AnimatorData<Vector2> ad:
                    _onFrame = (s) => FrameVec2(ad, s);
                    break;
                case AnimatorData<Vector3> ad:
                    _onFrame = (s) => FrameVec3(ad, s);
                    break;
                case AnimatorData<Vector4> ad:
                    _onFrame = (s) => FrameVec4(ad, s);
                    break;
                case AnimatorData<Vector2I> ad:
                    _onFrame = (s) => FrameVec2(ad, s);
                    break;
                case AnimatorData<Vector3I> ad:
                    _onFrame = (s) => FrameVec3(ad, s);
                    break;
                case AnimatorData<Vector4I> ad:
                    _onFrame = (s) => FrameVec4(ad, s);
                    break;
                case AnimatorData<Colour> ad:
                    _onFrame = (s) => FrameColour(ad, s);
                    break;
                case AnimatorData<Colour3> ad:
                    _onFrame = (s) => FrameColour(ad, s);
                    break;
                case AnimatorData<ColourF> ad:
                    _onFrame = (s) => FrameColour(ad, s);
                    break;
                case AnimatorData<ColourF3> ad:
                    _onFrame = (s) => FrameColour(ad, s);
                    break;
                default:
                    _onFrame = (_) => throw new NotSupportedException("Type not supported.");
                    break;
            }
        }

        public T StartValue { get; set; }
        public T EndValue { get; set; }

        public Action<T> SetValue { get; }
        private readonly Action<double> _onFrame;

        public override void OnFrame(double scale) => _onFrame(scale);

        public void Start(T start, T end, Animator handle)
        {
            StartValue = start;
            EndValue = end;

            handle.Add(this);
        }

        private static void FrameDouble(AnimatorData<double> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameFloat(AnimatorData<float> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, (float)scale));
        }
        private static void FrameInt(AnimatorData<int> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameLong(AnimatorData<long> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec2(AnimatorData<Vector2> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec3(AnimatorData<Vector3> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec4(AnimatorData<Vector4> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec2(AnimatorData<Vector2I> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec3(AnimatorData<Vector3I> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec4(AnimatorData<Vector4I> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameColour(AnimatorData<Colour> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameColour(AnimatorData<Colour3> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameColour(AnimatorData<ColourF> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, (float)scale));
        }
        private static void FrameColour(AnimatorData<ColourF3> ad, double scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, (float)scale));
        }
    }
}
