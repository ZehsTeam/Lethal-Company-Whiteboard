using GameNetcodeStuff;
using System;
using UnityEngine.InputSystem;

namespace com.github.zehsteam.Whiteboard;

internal class PlayerUtils
{
    public static int GetLocalPlayerId()
    {
        PlayerControllerB playerScript = GetLocalPlayerScript();
        if (playerScript == null) return -1;

        return (int)playerScript.playerClientId;
    }

    public static bool IsLocalPlayerId(int playerId)
    {
        return playerId == GetLocalPlayerId();
    }

    public static PlayerControllerB GetPlayerScript(int playerId)
    {
        try
        {
            return StartOfRound.Instance.allPlayerScripts[playerId];
        }
        catch
        {
            return null;
        }
    }

    public static PlayerControllerB GetLocalPlayerScript()
    {
        return GameNetworkManager.Instance.localPlayerController;
    }

    public static bool IsLocalPlayerSpawned()
    {
        PlayerControllerB playerScript = GetLocalPlayerScript();
        if (playerScript == null) return false;

        return playerScript.IsSpawned;
    }

    public static void SetControlsEnabled(bool value)
    {
        if (value)
        {
            EnableControls();
        }
        else
        {
            DisableControls();
        }
    }

    private static void EnableControls()
    {
        PlayerControllerB playerScript = GetLocalPlayerScript();
        if (playerScript == null) return;

        playerScript.disableMoveInput = false;

        InputActionAsset actions = IngamePlayerSettings.Instance.playerInput.actions;

        try
        {
            // PlayerControllerB
            playerScript.playerActions.Movement.Look.performed += playerScript.Look_performed;
            actions.FindAction("Jump").performed += playerScript.Jump_performed;
            actions.FindAction("Crouch").performed += playerScript.Crouch_performed;
            actions.FindAction("Interact").performed += playerScript.Interact_performed;
            actions.FindAction("ItemSecondaryUse").performed += playerScript.ItemSecondaryUse_performed;
            actions.FindAction("ItemTertiaryUse").performed += playerScript.ItemTertiaryUse_performed;
            actions.FindAction("ActivateItem").performed += playerScript.ActivateItem_performed;
            actions.FindAction("ActivateItem").canceled += playerScript.ActivateItem_canceled;
            actions.FindAction("Discard").performed += playerScript.Discard_performed;
            actions.FindAction("SwitchItem").performed += playerScript.ScrollMouse_performed;
            //actions.FindAction("OpenMenu").performed += playerScript.OpenMenu_performed;
            actions.FindAction("InspectItem").performed += playerScript.InspectItem_performed;
            actions.FindAction("SpeedCheat").performed += playerScript.SpeedCheat_performed;
            actions.FindAction("Emote1").performed += playerScript.Emote1_performed;
            actions.FindAction("Emote2").performed += playerScript.Emote2_performed;

            playerScript.isTypingChat = false;

            // HUDManager
            actions.FindAction("EnableChat").performed += HUDManager.Instance.EnableChat_performed;
            //actions.FindAction("OpenMenu").performed += HUDManager.Instance.OpenMenu_performed;
            actions.FindAction("SubmitChat").performed += HUDManager.Instance.SubmitChat_performed;
            actions.FindAction("PingScan").performed += HUDManager.Instance.PingScan_performed;

            playerScript.playerActions.Movement.Enable();
        }
        catch (Exception e)
        {
            Plugin.logger.LogError($"Error while subscribing to input in PlayerController\n\n{e}");
        }

        playerScript.playerActions.Movement.Enable();
    }

    private static void DisableControls()
    {
        PlayerControllerB playerScript = GetLocalPlayerScript();
        if (playerScript == null) return;

        playerScript.disableMoveInput = true;

        InputActionAsset actions = IngamePlayerSettings.Instance.playerInput.actions;

        try
        {
            // PlayerControllerB
            playerScript.playerActions.Movement.Look.performed -= playerScript.Look_performed;
            actions.FindAction("Jump").performed -= playerScript.Jump_performed;
            actions.FindAction("Crouch").performed -= playerScript.Crouch_performed;
            actions.FindAction("Interact").performed -= playerScript.Interact_performed;
            actions.FindAction("ItemSecondaryUse").performed -= playerScript.ItemSecondaryUse_performed;
            actions.FindAction("ItemTertiaryUse").performed -= playerScript.ItemTertiaryUse_performed;
            actions.FindAction("ActivateItem").performed -= playerScript.ActivateItem_performed;
            actions.FindAction("ActivateItem").canceled -= playerScript.ActivateItem_canceled;
            actions.FindAction("Discard").performed -= playerScript.Discard_performed;
            actions.FindAction("SwitchItem").performed -= playerScript.ScrollMouse_performed;
            //actions.FindAction("OpenMenu").performed -= playerScript.OpenMenu_performed;
            actions.FindAction("InspectItem").performed -= playerScript.InspectItem_performed;
            actions.FindAction("SpeedCheat").performed -= playerScript.SpeedCheat_performed;
            actions.FindAction("Emote1").performed -= playerScript.Emote1_performed;
            actions.FindAction("Emote2").performed -= playerScript.Emote2_performed;

            playerScript.isTypingChat = true;

            // HUDManager
            actions.FindAction("EnableChat").performed -= HUDManager.Instance.EnableChat_performed;
            //actions.FindAction("OpenMenu").performed -= HUDManager.Instance.OpenMenu_performed;
            actions.FindAction("SubmitChat").performed -= HUDManager.Instance.SubmitChat_performed;
            actions.FindAction("PingScan").performed -= HUDManager.Instance.PingScan_performed;

            playerScript.playerActions.Movement.Disable();
        }
        catch (Exception e)
        {
            Plugin.logger.LogError($"Error while unsubscribing to input in PlayerController\n\n{e}");
        }

        playerScript.playerActions.Movement.Disable();
    }
}
