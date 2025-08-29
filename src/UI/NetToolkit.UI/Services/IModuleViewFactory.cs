using System.Windows.Controls;

namespace NetToolkit.UI.Services;

public interface IModuleViewFactory
{
    UserControl CreateEducationView();
    UserControl CreateNetworkScannerView();
    UserControl CreateSecurityScanView();
    UserControl CreateSshTerminalView();
    UserControl CreatePowerShellView();
    UserControl CreateAiOrbView();
    UserControl CreateMicrosoftAdminView();
    object GetThreeJsService();
}