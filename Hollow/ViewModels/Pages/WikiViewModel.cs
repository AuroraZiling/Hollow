using CommunityToolkit.Mvvm.Input;
using Hollow.Enums;
using Hollow.Services.ConfigurationService;
using Hollow.Views.Controls;

namespace Hollow.ViewModels.Pages;

public partial class WikiViewModel(IConfigurationService configurationService): ViewModelBase, IViewModelBase
{
    public void Navigated()
    {
        
    }
}