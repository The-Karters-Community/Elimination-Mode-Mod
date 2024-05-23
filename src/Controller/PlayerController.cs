using System.Collections.Generic;
using System.Linq;
using TheKartersModdingAssistant;
using TheKartersModdingAssistant.Event;

namespace EliminationMode.Controller;

public static class PlayerController {
    public static void Initialize() {
        PlayerEvent.onFixedUpdate += PlayerController.OnFixedUpdate;
        PlayerEvent.onNewLap += PlayerController.OnNewLap;
    }

    public static void OnFixedUpdate(Player player) {
        List<Player> players = Player.GetPlayersSortedByPosition();
        int amountOfPlayers = players.Count;
        Player firstPlayer = players.First();

        // Force all players to finish the race when the first has finished.
        if (firstPlayer.uVisualInstanceSyncedParams.bRaceFinished) {
            player.uVisualInstanceSyncedParams.bRaceFinished = true;
        }

        bool isEliminated = (bool)player.Get("isEliminated", false);

        if (isEliminated) {
            // Store and force at which position the player has been eliminated.
            if (player.Has("eliminatedAtPosition")) {
                int eliminatedAtPosition = (int)player.Get("eliminatedAtPosition", null);

                player.SetPosition(eliminatedAtPosition);
            }

            // Force spectator camera to last player from alive ones.
            PixelGameKartCamera camera = player.uAntPlayer.gameplayCamera;
            Player beforeLastPlayer = players[amountOfPlayers - GameController.amountOfEliminatedPlayers - 1];

            camera.SetCameraAsSpectatorCamera(true, PixelGameKartCamera.ESpectatorCamType.E_PLAYER_RACE_CAM);
            camera.ChangeTargetPlayer(beforeLastPlayer.uAntPlayer);
        }
    }

    public static void OnNewLap(Player player) {
        // Initialize the "isEliminated" flag on first lap.
        if (player.GetCurrentLapCount() == 0 || player.GetCurrentLapCount() == 1) {
            player.Set("isEliminated", false);

            return;
        }
        
        List<Player> players = Player.GetPlayersSortedByPosition();
        int amountOfPlayers = players.Count;
        bool isBeforeLastPlayerFromAliveOnes = player.GetPosition() == amountOfPlayers - (GameController.amountOfEliminatedPlayers + 1);

        // When the before last player passes the finish line,
        // eliminate the last one.
        if (isBeforeLastPlayerFromAliveOnes) {
            Player lastPlayer = players[amountOfPlayers - GameController.amountOfEliminatedPlayers - 1];

            lastPlayer.Set("isEliminated", true);
            lastPlayer.Set("eliminatedAtPosition", lastPlayer.GetPosition());

            GameController.amountOfEliminatedPlayers++;

            lastPlayer.uAntPlayer.visualInstanceParent.SetActive(false);
            lastPlayer.uAntPlayer.humanControllingParent.gameObject.SetActive(false);
            lastPlayer.uAntPlayer.aiControllingParent.gameObject.SetActive(false);
        }
    }
}