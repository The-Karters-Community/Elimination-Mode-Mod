using HarmonyLib;
using TheKartersModdingAssistant;
using UnityEngine;

namespace EliminationMode.Core;

[HarmonyPatch(typeof(PTK_TopHudPlayerItem), nameof(PTK_TopHudPlayerItem.RefreshItemInfo))]
public class PTK_TopHudPlayerItem__RefreshItemInfo {
    public static void Postfix(PTK_TopHudPlayerItem __instance, string _strPlayerName, int _iPlayerRacePositionIndex) {
        Player player = null;

        foreach (Player p in Player.GetActivePlayers()) {
            if (p.GetName() == _strPlayerName) {
                player = p;
                break;
            }
        }

        if (player is null) {
            return;
        }

        // Update content and color of eliminated players position in the topbar.
        bool isEliminated = (bool)player.Get("isEliminated", false);

        if (isEliminated) {
            string position = PTK_TopHudPlayerItem.GetRacePosStringFromPos(_iPlayerRacePositionIndex);

            __instance.playerRacePositionText_playing.text = $"{position} (E)";

            __instance.playerRacePositionText_playing.color = Color.red;
        }
    }
}