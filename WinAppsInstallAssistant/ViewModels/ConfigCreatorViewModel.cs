using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WinAppsInstallAssistant.ViewModels
{
    public partial class ConfigCreatorViewModel : ViewModelBase
    {
        private readonly string _configDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".config", "winapps");
        private string ComposeFile => Path.Combine(_configDir, "compose.yaml");
        private string WinAppsConfFile => Path.Combine(_configDir, "winapps.conf");
        private readonly string _assetDefaultsPath = "Assets/default_winapps.conf"; // Adjust if needed
        
        // Notification about existing config files
        [ObservableProperty]
        private string? _existingConfigWarning;

        // Config fields, example few from your list
        [ObservableProperty]
        private string _rdpUser = "";
        [ObservableProperty]
        private string _rdpPass = "";
        [ObservableProperty]
        private string _rdpDomain = "";
        [ObservableProperty]
        private string _rdpIp = "127.0.0.1";
        [ObservableProperty]
        private string _vmName = "RDPWindows";
        [ObservableProperty]
        private string _waflavor = "docker";
        [ObservableProperty]
        private string _rdpScale = "100";
        [ObservableProperty]
        private string _removableMedia = "/run/media";
        [ObservableProperty]
        private string _rdpFlags = "/grab-keyboard /cert:tofu /sound /microphone +home-drive";
        [ObservableProperty]
        private string _debug = "true";
        [ObservableProperty]
        private string _autopause = "off";
        [ObservableProperty]
        private string _autopauseTime = "300";
        [ObservableProperty]
        private string _freerdpCommand = "";
        [ObservableProperty]
        private string _portTimeout = "5";
        [ObservableProperty]
        private string _rdpTimeout = "30";
        [ObservableProperty]
        private string _appScanTimeout = "60";

        // Dictionary to hold explanations for hover/tooltips
        public Dictionary<string, string> HelpTexts { get; } = new()
        {
            ["RdpUser"] = "Windows username for RDP login.",
            ["RdpPass"] = "Windows password; required for FreeRDP v3.9.0+.",
            ["RdpDomain"] = "Windows domain (leave blank if none).",
            ["RdpIp"] = "Windows IP address; default '127.0.0.1' for docker/podman.",
            ["VmName"] = "Libvirt VM name; must match for VM management.",
            ["Waflavor"] = "Backend for WinApps: docker, podman, libvirt, manual.",
            ["RdpScale"] = "Display scaling factor: 100, 140, 180.",
            ["RemovableMedia"] = "Mount point for removable devices (default /run/media).",
            ["RdpFlags"] = "Additional FreeRDP flags and arguments.",
            ["Debug"] = "Enable or disable debug logging (true/false).",
            ["Autopause"] = "Auto-pause Windows ('on' or 'off').",
            ["AutopauseTime"] = "Seconds of inactivity before pausing (>=20).",
            ["FreerdpCommand"] = "Command to run FreeRDP client (e.g., xfreerdp).",
            ["PortTimeout"] = "Timeout in seconds when checking RDP port.",
            ["RdpTimeout"] = "Timeout in seconds for initial RDP connection.",
            ["AppScanTimeout"] = "Timeout for scanning installed Windows applications."
        };

        public ConfigCreatorViewModel()
        {
            CheckExistingConfigFiles();
            LoadDefaults();
        }

        private void CheckExistingConfigFiles()
        {
            if (File.Exists(ComposeFile) || File.Exists(WinAppsConfFile))
            {
                ExistingConfigWarning = "Existing configuration files detected. Please remove the previous Windows installation before installing a new one.";
            }
            else
            {
                ExistingConfigWarning = null;
            }
        }

        private void LoadDefaults()
        {
            if (!File.Exists(_assetDefaultsPath))
                return;

            var lines = File.ReadAllLines(_assetDefaultsPath);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                // Parsing format: KEY="value"
                var parts = trimmed.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim().Trim('"');

                // Map keys to properties
                switch (key)
                {
                    case "RDP_USER": RdpUser = value; break;
                    case "RDP_PASS": RdpPass = value; break;
                    case "RDP_DOMAIN": RdpDomain = value; break;
                    case "RDP_IP": RdpIp = value; break;
                    case "VM_NAME": VmName = value; break;
                    case "WAFLAVOR": Waflavor = value; break;
                    case "RDP_SCALE": RdpScale = value; break;
                    case "REMOVABLE_MEDIA": RemovableMedia = value; break;
                    case "RDP_FLAGS": RdpFlags = value; break;
                    case "DEBUG": Debug = value; break;
                    case "AUTOPAUSE": Autopause = value; break;
                    case "AUTOPAUSE_TIME": AutopauseTime = value; break;
                    case "FREERDP_COMMAND": FreerdpCommand = value; break;
                    case "PORT_TIMEOUT": PortTimeout = value; break;
                    case "RDP_TIMEOUT": RdpTimeout = value; break;
                    case "APP_SCAN_TIMEOUT": AppScanTimeout = value; break;
                }
            }
        }

        [RelayCommand]
        public async Task SaveConfigAsync()
        {
            // Create config dir if needed
            Directory.CreateDirectory(_configDir);

            // Compose YAML content (simple example)
            var composeYaml = 
$@"version: '3'
services:
  winapps:
    environment:
      - RDP_USER={RdpUser}
      - RDP_PASS={RdpPass}
      - RDP_DOMAIN={RdpDomain}
      - RDP_IP={RdpIp}
      - VM_NAME={VmName}
      - WAFLAVOR={Waflavor}
      - RDP_SCALE={RdpScale}
      - REMOVABLE_MEDIA={RemovableMedia}
      - RDP_FLAGS={RdpFlags}
      - DEBUG={Debug}
      - AUTOPAUSE={Autopause}
      - AUTOPAUSE_TIME={AutopauseTime}
      - FREERDP_COMMAND={FreerdpCommand}
      - PORT_TIMEOUT={PortTimeout}
      - RDP_TIMEOUT={RdpTimeout}
      - APP_SCAN_TIMEOUT={AppScanTimeout}
";

            // winapps.conf content
            var winappsConf =
$@"##################################
#   WINAPPS CONFIGURATION FILE   #
##################################

RDP_USER=""{RdpUser}""
RDP_PASS=""{RdpPass}""
RDP_DOMAIN=""{RdpDomain}""
RDP_IP=""{RdpIp}""
VM_NAME=""{VmName}""
WAFLAVOR=""{Waflavor}""
RDP_SCALE=""{RdpScale}""
REMOVABLE_MEDIA=""{RemovableMedia}""
RDP_FLAGS=""{RdpFlags}""
DEBUG=""{Debug}""
AUTOPAUSE=""{Autopause}""
AUTOPAUSE_TIME=""{AutopauseTime}""
FREERDP_COMMAND=""{FreerdpCommand}""
PORT_TIMEOUT=""{PortTimeout}""
RDP_TIMEOUT=""{RdpTimeout}""
APP_SCAN_TIMEOUT=""{AppScanTimeout}""
";

            await File.WriteAllTextAsync(ComposeFile, composeYaml);
            await File.WriteAllTextAsync(WinAppsConfFile, winappsConf);
        }
    }
}
