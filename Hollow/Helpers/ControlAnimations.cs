using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
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
    
    public static void MakeOpacityAnimate(CompositionVisual compositionVisual, double millis = 700)
    {
        if (compositionVisual == null)
            return;

        Compositor compositor = compositionVisual.Compositor;

        var animationGroup = compositor.CreateAnimationGroup();
      
        
        ScalarKeyFrameAnimation opacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.Target = "Opacity";
        opacityAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        opacityAnimation.Duration = TimeSpan.FromMilliseconds(millis);
        
    
       
        Vector3KeyFrameAnimation offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        offsetAnimation.Target = "Offset";
        offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
        offsetAnimation.Duration = TimeSpan.FromMilliseconds(millis);
        

        animationGroup.Add(offsetAnimation);
        animationGroup.Add(opacityAnimation);
      
        
        ImplicitAnimationCollection implicitAnimationCollection = compositor.CreateImplicitAnimationCollection();
        implicitAnimationCollection["Opacity"] = animationGroup;
        implicitAnimationCollection["Offset"] = animationGroup;

        
        compositionVisual.ImplicitAnimations = implicitAnimationCollection;

    }
}