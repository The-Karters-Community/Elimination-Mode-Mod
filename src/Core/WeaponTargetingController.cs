using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TheKartersModdingAssistant;

namespace EliminationMode.Core;

[HarmonyPatch(typeof(WeaponTargetingController), nameof(WeaponTargetingController.LookForTargetPlayers))]
public class WeaponTargetingController__LookForTargetPlayers {
    public static void Postfix(ref List<Ant_Player> __result) {
        List<Ant_Player> newFoundPlayers = new();
        
        foreach (Ant_Player antPlayer in __result) {
            Player player = Player.FindByAntPlayer(antPlayer);

            if (!(bool)player.Get("isEliminated", false)) {
                newFoundPlayers.Add(antPlayer);
            }
        }

        __result = newFoundPlayers;
    }
}