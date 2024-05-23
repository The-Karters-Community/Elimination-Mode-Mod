using BepInEx;
using BepInEx.Configuration;
using EliminationMode.Controller;
using EliminationMode.Core;
using TheKarters2Mods;
using TheKartersModdingAssistant;

namespace EliminationMode;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(TheKartersModdingAssistant.MyPluginInfo.PLUGIN_GUID, "0.1.3")]
[BepInDependency(DisableLeaderboards_BepInExInfo.PLUGIN_GUID, "1.0.0")]
public class EliminationMode: AbstractPlugin {
    public static EliminationMode instance;

    /// <summary>
    /// Get the plugin instance.
    /// </summary>
    /// 
    /// <returns>EliminationMode</returns>
    public static EliminationMode Get() {
        return EliminationMode.instance;
    }

    public ConfigData data = new();

    /// <summary>
    /// EliminationMode constructor.
    /// </summary>
    public EliminationMode(): base() {
        this.pluginGuid = MyPluginInfo.PLUGIN_GUID;
        this.pluginName = MyPluginInfo.PLUGIN_NAME;
        this.pluginVersion = MyPluginInfo.PLUGIN_VERSION;

        this.harmony = new(this.pluginGuid);
        this.logger = new(this.Log);

        EliminationMode.instance = this;
    }

    /// <summary>
    /// Patch all the methods with Harmony.
    /// </summary>
    public override void ProcessPatching() {
        this.BindFromConfig();

        if (this.data.isModEnabled) {
            DisableLeaderboardsPlugin.Enable();

            // Put all methods that should patched by Harmony here.
            this.harmony.PatchAll(typeof(WeaponTargetingController__LookForTargetPlayers));
            this.harmony.PatchAll(typeof(PlayerHUDManager__Update));
            this.harmony.PatchAll(typeof(PTK_MinimapPlayersManager__Update));
            this.harmony.PatchAll(typeof(PTK_MainMenuInputManager__Player_Game_SpectatorSwitchPlayerClicked));

            // Then, add methods to the SDK actions.
            GameController.Initialize();
            PlayerController.Initialize();

            this.logger.Info($"{this.pluginName} has been enabled.", true);
        }
    }

    /// <summary>
    /// Bind configurations from the config file.
    /// </summary>
    public void BindFromConfig() {
        this.BindGeneralConfig();
        this.BindCustomizationConfig();
    }

    /// <summary>
    /// Bind general configurations from the config file.
    /// </summary>
    protected void BindGeneralConfig() {
        ConfigEntry<bool> isModEnabled = Config.Bind(
            ConfigCategory.General,
            nameof(isModEnabled),
            true,
            "Whether the mod is enabled."
        );

        this.data.isModEnabled = isModEnabled.Value;
    }

    /// <summary>
    /// Bind customization configurations from the config file.
    /// </summary>
    protected void BindCustomizationConfig() {
        
    }
}
