using HarmonyLib;

namespace EliminationMode.Core;

[HarmonyPatch(typeof(PTK_MainMenuInputManager), nameof(PTK_MainMenuInputManager.Player_Game_SpectatorSwitchPlayerClicked))]
public class PTK_MainMenuInputManager__Player_Game_SpectatorSwitchPlayerClicked {
    public static void Postfix(ref bool __result) {
        __result = false;
    }
}