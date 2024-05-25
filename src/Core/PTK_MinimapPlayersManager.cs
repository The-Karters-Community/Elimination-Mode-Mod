using HarmonyLib;
using TheKartersModdingAssistant;

namespace EliminationMode.Core;

[HarmonyPatch(typeof(PTK_MinimapPlayersManager), nameof(PTK_MinimapPlayersManager.Update))]
public class PTK_MinimapPlayersManager__Update {
    public static void Postfix(PTK_MinimapPlayersManager __instance) {
        Ant_Player uAntPlayer = __instance.parentHUD.localPlayer;

        if (uAntPlayer is null) {
            EliminationMode.Get().logger.Error("(PTK_MinimapPlayersManager__Update::Postfix) Ant_Player not found from PlayerHUDManager.");
            return;
        }

        Player player = Player.FindByAntPlayer(uAntPlayer);

        // Useless to continue if the local player is not a human,
        // because it does not have a HUD.
        if (!player.IsHuman()) {
            return;
        }

        // Disable minimap elements of all players that have been eliminated.
        foreach (PTK_MinimapPlayer uPtkMinimapPlayer in __instance.otherPlayers) {
            Player otherPlayer = Player.FindByIndex(uPtkMinimapPlayer.GetCurrentMinimapAntPlayerNr());
            bool isEliminated = (bool)otherPlayer.Get("isEliminated", false);

            uPtkMinimapPlayer.gameObject.SetActive(true);

            if (isEliminated) {
                uPtkMinimapPlayer.gameObject.SetActive(false);
            }
        }
    }
}