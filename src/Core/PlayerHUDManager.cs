using EliminationMode.Controller;
using HarmonyLib;
using TheKartersModdingAssistant;

namespace EliminationMode.Core;

[HarmonyPatch(typeof(PlayerHUDManager), nameof(PlayerHUDManager.Update))]
public class PlayerHUDManager__Update {
    public static void Postfix(PlayerHUDManager __instance) {
        if (__instance?.localPlayer is null) {
            return;
        }

        Player player = Player.FindByAntPlayer(__instance.localPlayer);

        if (!player.IsHuman()) {
            return;
        }

        int amountOfPlayersAlive = Player.GetPlayersSortedByPosition().Count - GameController.amountOfEliminatedPlayers;

        __instance.playerPosSuffix.text = $"<space=0.1em>/<space=0.1em>{amountOfPlayersAlive}";

        bool isEliminated = (bool)player.Get("isEliminated", false);

        if (isEliminated) {
            __instance.lapCountParent.gameObject.SetActive(false);
            __instance.timerParent.gameObject.SetActive(false);
            __instance.minimapParent.gameObject.SetActive(false);
            __instance.heartsPresentParent.gameObject.SetActive(false);
            __instance.playersScoreParent.gameObject.SetActive(false);
        }
    }
}