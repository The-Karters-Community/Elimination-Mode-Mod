using System.Collections.Generic;
using HarmonyLib;
using TheKartersModdingAssistant;

namespace EliminationMode.Core;

[HarmonyPatch(typeof(PTK_HudWeaponTargetsPreview), nameof(PTK_HudWeaponTargetsPreview.Update))]
public class PTK_HudWeaponTargetsPreview__Update {
    public static void Postfix(PTK_HudWeaponTargetsPreview __instance) {
        if (__instance?.playerHudManager?.localPlayer is null) {
            return;
        }

        Player player = Player.FindByAntPlayer(__instance.playerHudManager.localPlayer);

        if (!player.IsHuman()) {
            return;
        }

        // Hide the "back target" icon when detected players behind are all eliminated.
        WeaponsController weaponsController = __instance.playerHudManager.localPlayer.weaponsController;
        List<Ant_Player> antPlayers = new(weaponsController.GetCurrentPlayerTargets());
        bool areAllTargetsEliminated = true;

        foreach (Ant_Player antPlayer in antPlayers) {
            Player p = Player.FindByAntPlayer(antPlayer);
            bool isEliminated = (bool)p.Get("isEliminated", false);

            if (!isEliminated) {
                areAllTargetsEliminated = false;
                break;
            }
        }

        if (areAllTargetsEliminated) {
            __instance.backTargetImageIcon.SetActive(false);
        }
    }
}