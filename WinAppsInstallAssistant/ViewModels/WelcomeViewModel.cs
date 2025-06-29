using Avalonia;
using CommunityToolkit.Mvvm.Input;

namespace WinAppsInstallAssistant.ViewModels;

public partial class WelcomeViewModel : ViewModelBase
{
    [RelayCommand]
    private static void StartInstall()
    {
        if (Application.Current is App { MainWindow.DataContext: MainWindowViewModel main })
        {
            main.CurrentViewModel = new DistroSelectionViewModel();
        }
    }
}
