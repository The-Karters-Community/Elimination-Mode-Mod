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

        // Store the instance into the player.
        if (!player.Has("PlayerHUDManager")) {
            player.Set("PlayerHUDManager", __instance);
        }

        // Display the amount of players still alive after current player's position.
        int amountOfPlayersAlive = Player.GetPlayersSortedByPosition().Count - GameEventHandler.amountOfEliminatedPlayers;

        __instance.playerPosSuffix.text = $"<space=0.1em>/<space=0.1em>{amountOfPlayersAlive}";

        // Display correct amount of required laps.
        __instance.iLastVisibleLapCount = Game.Get().GetAmountOfLaps();
    }
}