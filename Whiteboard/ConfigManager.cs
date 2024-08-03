using BepInEx.Configuration;
using com.github.zehsteam.Whiteboard.MonoBehaviours;
using System.Collections.Generic;
using System.Reflection;

namespace com.github.zehsteam.Whiteboard;

internal class ConfigManager
{
    // General Settings
    public ConfigEntry<bool> ExtendedLogging { get; private set; }

    // Whiteboard Settings
    public ConfigEntry<int> Price { get; private set; }
    public ConfigEntry<bool> HostOnly { get; private set; }
    public ConfigEntry<string> DisplayText { get; private set; }

    public ConfigManager()
    {
        BindConfigs();
        SetupChangedEvents();
        ClearUnusedEntries();
    }

    private void BindConfigs()
    {
        ConfigFile configFile = Plugin.Instance.Config;

        // General Settings
        ExtendedLogging = configFile.Bind("General Settings", "ExtendedLogging", defaultValue: false, "Enable extended logging.");

        // Whiteboard Settings
        Price = configFile.Bind("Whiteboard Settings", "Price", defaultValue: 200, new ConfigDescription("The price of the whiteboard in the store.", new AcceptableValueRange<int>(0, 1000)));
        HostOnly = configFile.Bind("Whiteboard Settings", "HostOnly", defaultValue: false, "If enabled, only the host can edit the whiteboard.");
        DisplayText = configFile.Bind("Whiteboard Settings", "DisplayText", defaultValue: "- Dont leave the ship as last survivor\n- All in as long as more than 1 are alive\n- Teleport dead bodies", "The text that shows on the whiteboard. Supports rich text tags.");
    }

    private void SetupChangedEvents()
    {
        Price.SettingChanged += Price_SettingChanged;
        DisplayText.SettingChanged += DisplayText_SettingChanged;
    }

    private void Price_SettingChanged(object sender, System.EventArgs e)
    {
        if (!Plugin.IsHostOrServer) return;

        if (PluginNetworkBehaviour.Instance != null)
        {
            PluginNetworkBehaviour.Instance.SetWhiteboardUnlockablePriceClientRpc(Price.Value);
        }
    }

    private void DisplayText_SettingChanged(object sender, System.EventArgs e)
    {
        if (WhiteboardBehaviour.Instance == null) return;

        WhiteboardBehaviour.Instance.SetDisplayTextToConfigValue();
    }

    private void ClearUnusedEntries()
    {
        ConfigFile configFile = Plugin.Instance.Config;

        // Normally, old unused config entries don't get removed, so we do it with this piece of code. Credit to Kittenji.
        PropertyInfo orphanedEntriesProp = configFile.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
        var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(configFile, null);
        orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
        configFile.Save(); // Save the config file to save these changes
    }
}
