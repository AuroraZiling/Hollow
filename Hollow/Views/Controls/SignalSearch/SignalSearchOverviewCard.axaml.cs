using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Hollow.Models.SignalSearch;

namespace Hollow.Views.Controls.SignalSearch;

public class SignalSearchOverviewCard : UserControl
{
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, string>(nameof(Header));

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    
    public static readonly StyledProperty<string> TimeRangeProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, string>(nameof(TimeRange));

    public string TimeRange
    {
        get => GetValue(TimeRangeProperty);
        set => SetValue(TimeRangeProperty, value);
    }
    
    public static readonly StyledProperty<string> TotalProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, string>(nameof(Total));

    public string Total
    {
        get => GetValue(TotalProperty);
        set => SetValue(TotalProperty, value);
    }
    
    public static readonly StyledProperty<string> SAverageProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, string>(nameof(SAverage));

    public string SAverage
    {
        get => GetValue(SAverageProperty);
        set => SetValue(SAverageProperty, value);
    }
    
    public static readonly StyledProperty<int> SSingalsProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, int>(nameof(SSingals));

    public int SSingals
    {
        get => GetValue(SSingalsProperty);
        set => SetValue(SSingalsProperty, value);
    }
    
    public static readonly StyledProperty<double> SSingalsPercentageProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, double>(nameof(SSingalsPercentage));

    public double SSingalsPercentage
    {
        get => GetValue(SSingalsPercentageProperty);
        set => SetValue(SSingalsPercentageProperty, value);
    }
    
    public static readonly StyledProperty<int> ASingalsProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, int>(nameof(ASingals));

    public int ASingals
    {
        get => GetValue(ASingalsProperty);
        set => SetValue(ASingalsProperty, value);
    }
    
    public static readonly StyledProperty<double> ASingalsPercentageProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, double>(nameof(ASingalsPercentage));

    public double ASingalsPercentage
    {
        get => GetValue(ASingalsPercentageProperty);
        set => SetValue(ASingalsPercentageProperty, value);
    }
    
    public static readonly StyledProperty<int> BSingalsProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, int>(nameof(BSingals));

    public int BSingals
    {
        get => GetValue(BSingalsProperty);
        set => SetValue(BSingalsProperty, value);
    }
    
    public static readonly StyledProperty<double> BSingalsPercentageProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, double>(nameof(BSingalsPercentage));

    public double BSingalsPercentage
    {
        get => GetValue(BSingalsPercentageProperty);
        set => SetValue(BSingalsPercentageProperty, value);
    }
    
    public static readonly StyledProperty<int> LuckiestPullsProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, int>(nameof(LuckiestPulls));

    public int LuckiestPulls
    {
        get => GetValue(LuckiestPullsProperty);
        set => SetValue(LuckiestPullsProperty, value);
    }
    
    public static readonly StyledProperty<int> UnluckiestPullsProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, int>(nameof(UnluckiestPulls));

    public int UnluckiestPulls
    {
        get => GetValue(UnluckiestPullsProperty);
        set => SetValue(UnluckiestPullsProperty, value);
    }
    
    public static readonly StyledProperty<ObservableCollection<OverviewCardGachaItem>> ProgressBarsProperty =
        AvaloniaProperty.Register<SignalSearchOverviewCard, ObservableCollection<OverviewCardGachaItem>>(nameof(ProgressBars));

    public ObservableCollection<OverviewCardGachaItem> ProgressBars
    {
        get => GetValue(ProgressBarsProperty);
        set => SetValue(ProgressBarsProperty, value);
    }
}