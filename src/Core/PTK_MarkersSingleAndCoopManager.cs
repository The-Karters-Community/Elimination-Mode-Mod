using HarmonyLib;
using TheKartersModdingAssistant;

namespace EliminationMode.Core;

[HarmonyPatch(typeof(PTK_MarkersSingleAndCoopManager), nameof(PTK_MarkersSingleAndCoopManager.Update))]
public class PTK_MarkersSingleAndCoopManager__Update {
    public static void Postfix(PTK_MarkersSingleAndCoopManager __instance) {
        if (__instance is null) {
            return;
        }

        for (int i = 0; i < __instance.createdTargetMarkersPerPlayer.Count; ++i) {
            PTK_MarkersSingleAndCoopManager.CLocalPlayerMarkers localPlayerMarkers = __instance.createdTargetMarkersPerPlayer[i];
            Player localPlayer = Player.FindByAntPlayer(localPlayerMarkers.localPlayer);

            if (!localPlayer.IsHuman()) {
                continue;
            }

            for (int j = 0; j < localPlayerMarkers.sortedMarkersFromDist.Count; ++j) {
                PTK_MarkerIndicator marker = localPlayerMarkers.sortedMarkersFromDist[j];
                Player player = Player.FindByAntPlayer(marker.markerAboveParentPlayer);

                if (player is null) {
                    continue;
                }

                bool isEliminated = (bool)player.Get("isEliminated", false);
                bool isWatched = (bool)player.Get("isWatched", false) && (bool)localPlayer.Get("isEliminated", false);

                float currentAlpha = marker.markerIndicatorImpl.indicatorCanvasGroup.alpha;

                if (currentAlpha >= 0 && (isEliminated || isWatched)) {
                    currentAlpha = 0;
                }

                marker.markerIndicatorImpl.indicatorCanvasGroup.alpha = currentAlpha;
            }
        }
    }
}