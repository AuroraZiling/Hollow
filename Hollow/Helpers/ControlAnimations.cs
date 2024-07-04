using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace Hollow.Helpers;

public static class ControlAnimationHelper
{
    public static void Animate<T>(this Animatable control, AvaloniaProperty property, T from, T to, TimeSpan duration, ulong count = 1)
    {
        new Animation
        {
            Duration = duration,
            FillMode = FillMode.Forward,
            Easing = new CubicEaseInOut(),
            IterationCount = new IterationCount(count),
            PlaybackDirection = PlaybackDirection.Normal,
            Children =
            {
                new KeyFrame
                {
                    Setters = { new Setter { Property = property, Value = from } },
                    KeyTime = TimeSpan.FromSeconds(0)
                },
                new KeyFrame
                {
                    Setters = { new Setter { Property = property, Value = to } },
                    KeyTime = duration
                }
            }
        }.RunAsync(control);
    }
    
}