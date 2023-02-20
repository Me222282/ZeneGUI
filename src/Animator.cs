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
        private readonly List<AnimatorData> _animations = new List<AnimatorData>();

        public void Add(AnimatorData animation)
        {
            if (animation.SetValue == null)
            {
                throw new ArgumentNullException(nameof(animation.SetValue), "There must be a SetValue function.");
            }
            if (animation.Animating)
            {
                throw new ArgumentException(nameof(animation), "Already in animation");
            }
            // No animation
            if (animation.Duration <= 0 ||
                animation.StartValue == animation.EndValue) { return; }

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
        public AnimatorData Add(Action<double> setValue, double duration, double start, double end, AnimationType type)
        {
            AnimatorData ad = new AnimatorData(setValue, duration, start, end, type)
            {
                startTime = Core.Time
            };
            Add(ad);
            return ad;
        }
        public AnimatorData Add(Action<double> setValue, double duration, double start, double end)
            => Add(setValue, duration, start, end, Animator.Linear);

        public void Invoke()
        {
            double time = Core.Time;

            lock (_animations)
            {
                ReadOnlySpan<AnimatorData> span = CollectionsMarshal.AsSpan(_animations);

                for (int i = 0; i < _animations.Count; i++)
                {
                    AnimatorData ad = span[i];
                    double t = time - ad.startTime;

                    // End of animation
                    if (t >= ad.Duration)
                    {
                        // Set value to end
                        ad.SetValue(
                            ad.StartValue.Lerp(
                                ad.EndValue,
                                ad.Type(1d)));

                        ad.startTime = -1;
                        ad.InvokeFinish();

                        // Remove animation
                        _animations.RemoveAt(i);
                        i--;
                        continue;
                    }

                    double scale = ad.Type(t / ad.Duration);
                    ad.SetValue(ad.StartValue.Lerp(ad.EndValue, scale));
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

    public class AnimatorData
    {
        public AnimatorData(Action<double> setValue, double duration, double start, double end, AnimationType type)
        {
            SetValue = setValue;
            Duration = duration;
            StartValue = start;
            EndValue = end;
            _type = type;
        }
        public AnimatorData(Action<double> setValue, double duration, double start, double end)
        {
            SetValue = setValue;
            Duration = duration;
            StartValue = start;
            EndValue = end;
            _type = Animator.Linear;
        }

        internal double startTime = -1;
        public double Duration { get; set; }
        public double StartValue { get; set; }
        public double EndValue { get; set; }

        public bool Animating => startTime > 0d;

        public Action<double> SetValue { get; }
        private AnimationType _type;
        public AnimationType Type
        {
            get => _type;
            set => _type = value ?? Animator.Linear;
        }

        public event EventHandler Finish;
        internal void InvokeFinish() => Finish?.Invoke(this, EventArgs.Empty);

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
        public void Start(double start, double end, Animator handle)
        {
            StartValue = start;
            EndValue = end;

            if (Animating)
            {
                startTime = Core.Time;
                return;
            }

            handle.Add(this);
        }
    }
}
