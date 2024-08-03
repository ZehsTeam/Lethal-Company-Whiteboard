using com.github.zehsteam.Whiteboard.MonoBehaviours;
using LethalLib.Extras;
using UnityEngine;

namespace com.github.zehsteam.Whiteboard;

internal class Content
{
    // Network Handler
    public static GameObject NetworkHandlerPrefab;

    // Unlockable Items
    public static UnlockableItemDef WhiteboardUnlockableItemDef;

    // Terminal Nodes
    public static TerminalNode WhiteboardBuyTerminalNode;

    public static void Load()
    {
        LoadAssetsFromAssetBundle();
    }

    private static void LoadAssetsFromAssetBundle()
    {
        try
        {
            var dllFolderPath = System.IO.Path.GetDirectoryName(Plugin.Instance.Info.Location);
            var assetBundleFilePath = System.IO.Path.Combine(dllFolderPath, "whiteboard_assets");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundleFilePath);

            // Network Handler
            NetworkHandlerPrefab = assetBundle.LoadAsset<GameObject>("NetworkHandler");
            NetworkHandlerPrefab.AddComponent<PluginNetworkBehaviour>();

            // Unlockable Items
            WhiteboardUnlockableItemDef = assetBundle.LoadAsset<UnlockableItemDef>("Whiteboard");

            // Terminal Nodes
            WhiteboardBuyTerminalNode = assetBundle.LoadAsset<TerminalNode>("WhiteboardBuy");

            Plugin.logger.LogInfo("Successfully loaded assets from AssetBundle!");
        }
        catch (System.Exception e)
        {
            Plugin.logger.LogError($"Failed to load assets from AssetBundle.\n\n{e}");
        }
    }
}
