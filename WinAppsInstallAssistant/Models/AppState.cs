namespace WinAppsInstallAssistant.Models;

public class AppState
{
    private static AppState? _instance;
    public static AppState Instance => _instance ??= new AppState();

    private AppState() { }

    public string? IdLike { get; set; }
    
}
