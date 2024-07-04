using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Hollow.Controls.Acrylic;

public class Acrylic: ContentControl
{
    private static readonly ImmutableExperimentalAcrylicMaterial DefaultAcrylicMaterial = (ImmutableExperimentalAcrylicMaterial) new ExperimentalAcrylicMaterial
    {
        MaterialOpacity = 0.1,
        TintColor = new Color(255, 5, 5, 5),
        TintOpacity = 1,
        PlatformTransparencyCompensationLevel = 0
    }.ToImmutable();

    public static readonly StyledProperty<ExperimentalAcrylicMaterial?> MaterialProperty =
        AvaloniaProperty.Register<Acrylic, ExperimentalAcrylicMaterial?>(nameof(Material));

    public ExperimentalAcrylicMaterial? Material
    {
        get => GetValue(MaterialProperty);
        set => SetValue(MaterialProperty, value);
    }

    public static readonly StyledProperty<int> BlurProperty = AvaloniaProperty.Register<Acrylic, int>(nameof(Blur));

    public int Blur
    {
        get => GetValue(BlurProperty);
        set => SetValue(BlurProperty, value);
    }

    static Acrylic()
    {
        AffectsRender<Acrylic>(MaterialProperty);
        AffectsRender<Acrylic>(BlurProperty);
    }

    public override void Render(DrawingContext context)
    {
        var material = Material != null ? (ImmutableExperimentalAcrylicMaterial) Material.ToImmutable() : DefaultAcrylicMaterial;
        context.Custom(new AcrylicRenderOperation(this, material, Blur, new Rect(default, Bounds.Size)));
    }
}