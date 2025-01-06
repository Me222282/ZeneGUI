using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public delegate floatv AnimationType(floatv input);

    public class Animator
    {
        private readonly List<BaseAnimation> _animations = new List<BaseAnimation>();

        public void Add<T>(AnimatorData<T> animation)
        {
            if (animation.SetValue == null)
            {
                throw new ArgumentNullException(nameof(animation.SetValue), "There must be a SetValue function.");
            }

            if (animation.Animating || animation.Duration <= 0 ||
                animation.StartValue.Equals(animation.EndValue)) { return; }

            if (animation.Type == null)
            {
                animation.Type = Linear;
            }

            animation.startTime = Core.Time;
            animation.Handle = this;
            lock (_animations)
            {
                _animations.Add(animation);
            }
        }
        public void Add(BaseAnimation animation)
        {
            if (animation.Animating || animation.Duration <= 0) { return; }

            if (animation.Type == null)
            {
                animation.Type = Linear;
            }

            animation.startTime = Core.Time;
            animation.Handle = this;
            lock (_animations)
            {
                _animations.Add(animation);
            }
        }
        public AnimatorData<T> Add<T>(Action<T> setValue, floatv duration, T start, T end, AnimationType type)
        {
            AnimatorData<T> ad = new AnimatorData<T>(setValue, duration, start, end, type);
            Add(ad);
            return ad;
        }
        public AnimatorData<T> Add<T>(Action<T> setValue, floatv duration, T start, T end)
            => Add(setValue, duration, start, end, Linear);

        public void Remove(BaseAnimation animation)
        {
            bool exisits;

            lock (_animations)
            {
                exisits = _animations.Remove(animation);
            }

            if (!exisits) { return; }

            floatv scale = 1;

            if (animation.Looping)
            {
                scale = (floatv)(Core.Time - animation.startTime) / animation.Duration;
            }

            // Set value to end
            animation.OnFrame(animation.Type(animation.Reversed ? 1 - scale : scale));

            animation.startTime = -1;
            animation.Handle = null;
            animation.InvokeFinish();
        }

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
                        ad.OnFrame(ad.Type(ad.Reversed ? 0 : 1));

                        if (ad.Looping)
                        {
                            ad.startTime = time;
                            continue;
                        }

                        // Remove animation
                        _animations.RemoveAt(i);
                        i--;

                        ad.startTime = -1;
                        ad.InvokeFinish();
                        continue;
                    }

                    floatv scale = ad.Type((floatv)t / ad.Duration);
                    ad.OnFrame(ad.Reversed ? 1 - scale : scale);
                }
            }
        }

        private static floatv LinearType(floatv input) => input;
        private static floatv LogarithmicType(floatv input) => Maths.Log((input * (Maths.E - 1)) + 1);
        private static floatv ExponentialType(floatv input) => (Maths.Exp(input) - 1) / (Maths.E - 1);

        public static AnimationType Linear => LinearType;
        public static AnimationType Logarithmic => LogarithmicType;
        public static AnimationType Exponential => ExponentialType;

        public static AnimationType CreateLogarithmic(floatv exp)
        {
            floatv v = Maths.Pow(2, exp);

            return (input) =>
            {
                return Maths.Log((input * (v - 1)) + 1, v);
            };
        }
        public static AnimationType CreateExponential(floatv exp)
        {
            floatv v = Maths.Pow(2, exp);

            return (input) =>
            {
                return (Maths.Pow(v, input) - 1) / (v - 1);
            };
        }
    }

    public abstract class BaseAnimation
    {
        public BaseAnimation(floatv duration, AnimationType type)
        {
            Duration = duration;
            _type = type ?? Animator.Linear;
        }

        internal double startTime = -1;
        public floatv Duration { get; set; }

        public bool Reversed { get; set; } = false;
        public bool Animating => startTime > 0d;
        public bool Looping { get; set; } = false;

        private AnimationType _type;
        public AnimationType Type
        {
            get => _type;
            set => _type = value ?? Animator.Linear;
        }

        public Animator Handle { get; internal set; }

        public event EventHandler Finish;
        internal void InvokeFinish() => Finish?.Invoke(this, EventArgs.Empty);

        public abstract void OnFrame(floatv scale);

        public void Stop()
        {
            if (Handle != null)
            {
                Handle.Remove(this);
            }
        }
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
        public AnimatorData(Action<T> setValue, floatv duration, T start, T end, AnimationType type = null)
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
                case AnimatorData<Radian> ad:
                    _onFrame = (s) => FrameFloatV(ad, s);
                    break;
                case AnimatorData<Degrees> ad:
                    _onFrame = (s) => FrameFloatV(ad, s);
                    break;
                default:
                    _onFrame = (_) => throw new NotSupportedException("Type not supported.");
                    break;
            }
        }

        public T StartValue { get; set; }
        public T EndValue { get; set; }

        public Action<T> SetValue { get; }
        private readonly Action<floatv> _onFrame;

        public override void OnFrame(floatv scale) => _onFrame(scale);

        public void Start(T start, T end, Animator handle)
        {
            StartValue = start;
            EndValue = end;

            handle.Add(this);
        }
        public void Start(Animator handle) => handle.Add(this);

        private static void FrameDouble(AnimatorData<double> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameFloatV(AnimatorData<Radian> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameFloatV(AnimatorData<Degrees> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameFloat(AnimatorData<float> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, (float)scale));
        }
        private static void FrameInt(AnimatorData<int> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameLong(AnimatorData<long> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec2(AnimatorData<Vector2> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec3(AnimatorData<Vector3> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec4(AnimatorData<Vector4> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec2(AnimatorData<Vector2I> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec3(AnimatorData<Vector3I> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameVec4(AnimatorData<Vector4I> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameColour(AnimatorData<Colour> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameColour(AnimatorData<Colour3> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
        }
        private static void FrameColour(AnimatorData<ColourF> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, (float)scale));
        }
        private static void FrameColour(AnimatorData<ColourF3> ad, floatv scale)
        {
            ad.SetValue(ad.StartValue.Lerp(ad.EndValue, (float)scale));
        }
    }
}
