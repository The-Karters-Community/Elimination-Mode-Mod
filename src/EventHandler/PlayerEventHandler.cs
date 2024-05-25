using System.Collections.Generic;
using System.Linq;
using TheKartersModdingAssistant;
using TheKartersModdingAssistant.Event;
using TMPro;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using Logger = TheKartersModdingAssistant.Logger;

namespace EliminationMode.Controller;

public static class PlayerEventHandler {
    public static Logger logger;

    public static void Initialize(Logger logger) {
        PlayerEventHandler.logger = logger;

        PlayerEvent.onFixedUpdate += PlayerEventHandler.OnFixedUpdate;
        PlayerEvent.onFixedUpdateAfter += PlayerEventHandler.OnFixedUpdateAfter;
        PlayerEvent.onNewLap += PlayerEventHandler.OnNewLap;
    }

    public static void OnFixedUpdate(Player player) {
        bool isEliminated = (bool)player.Get("isEliminated", false);

        if (isEliminated) {
            player.Set("isWatched", false);
        }
    }

    public static void OnFixedUpdateAfter(Player player) {
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
            Player beforeLastPlayer = players[amountOfPlayers - GameEventHandler.amountOfEliminatedPlayers - 1];

            camera.SetCameraAsSpectatorCamera(true, PixelGameKartCamera.ESpectatorCamType.E_PLAYER_RACE_CAM);
            camera.ChangeTargetPlayer(beforeLastPlayer.uAntPlayer);

            beforeLastPlayer.Set("isWatched", true);
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
        bool isBeforeLastPlayerFromAliveOnes = player.GetPosition() == amountOfPlayers - (GameEventHandler.amountOfEliminatedPlayers + 1);

        // When the before last player passes the finish line,
        // eliminate the last one.
        if (isBeforeLastPlayerFromAliveOnes) {
            Player lastPlayer = players[amountOfPlayers - GameEventHandler.amountOfEliminatedPlayers - 1];

            lastPlayer.Set("isEliminated", true);
            lastPlayer.Set("eliminatedAtPosition", lastPlayer.GetPosition());

            GameEventHandler.amountOfEliminatedPlayers++;

            lastPlayer.uAntPlayer.visualInstanceParent.SetActive(false);
            lastPlayer.uAntPlayer.humanControllingParent.gameObject.SetActive(false);
            lastPlayer.uAntPlayer.aiControllingParent.gameObject.SetActive(false);

            // Display who is eliminated on human players screen.
            PlayerEventHandler.DisplayEliminatedPlayerOnScreen(lastPlayer);
        }
    }

    /// <summary>
    /// Get a list of human players.
    /// </summary>
    /// 
    /// <returns>List<Player></returns>
    public static List<Player> GetHumanPlayers() {
        List<Player> humanPlayers = new();

        foreach (Player player in Player.GetActivePlayers()) {
            if (player.IsHuman()) {
                humanPlayers.Add(player);
            }
        }

        return humanPlayers;
    }

    /// <summary>
    /// Tell whether the race is with many human players.
    /// </summary>
    /// 
    /// <returns>bool</returns>
    public static bool IsLocalMultiplayerRace() {
        return PlayerEventHandler.GetHumanPlayers().Count > 1;
    }

    /// <summary>
    /// Display on screen the player which has been eliminated.
    /// </summary>
    /// 
    /// <param name="eliminatedPlayer">Player</param>
    public static void DisplayEliminatedPlayerOnScreen(Player eliminatedPlayer) {
        foreach (Player player in PlayerEventHandler.GetHumanPlayers()) {
            if (!player.Has("PlayerHUDManager")) {
                PlayerEventHandler.logger.Error($"(PlayerEventHandler::DisplayEliminatedPlayerOnScreen) PlayerHUDManager not found for {player.GetName()}.");
                continue;
            }

            PlayerHUDManager hud = (PlayerHUDManager)player.Get("PlayerHUDManager", null);
            RectTransform hudRect = hud.GetComponent<RectTransform>();

            TextMeshProUGUI eliminatedText = UnityObject.Instantiate(
                hud.playerPosText,
                hud.transform
            );

            RectTransform eliminatedTextRect = eliminatedText.GetComponent<RectTransform>();

            // Set the pivot of the rect on the middle.
            eliminatedTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            eliminatedTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            eliminatedTextRect.pivot = new Vector2(0.5f, 0.5f);
            eliminatedTextRect.sizeDelta = new Vector2(eliminatedTextRect.sizeDelta.x * 3, eliminatedTextRect.sizeDelta.y);

            // Update text content and position.
            string content;

            if (player.GetIndex() == eliminatedPlayer.GetIndex()) {
                content = "You have been eliminated.";
            } else if (PlayerEventHandler.IsLocalMultiplayerRace() && eliminatedPlayer.IsHuman()) {
                content = $"{eliminatedPlayer.GetName()} ({(int)eliminatedPlayer.uAntPlayer.eAntLocalPlayerNr + 1}) has been eliminated.";
            } else {
                content = $"{eliminatedPlayer.GetName()} has been eliminated.";
            }

            eliminatedText.text = content;
            eliminatedText.fontSize = 42;
            eliminatedText.alignment = TextAlignmentOptions.Center;

            float x = hudRect.sizeDelta.x / 2;
            float y = hudRect.sizeDelta.y / 2 + 200;
            float z = eliminatedText.transform.position.z;

            eliminatedText.transform.position = new Vector3(x, y, z);
        
            // Destroy the text object after 3 seconds.
            UnityObject.Destroy(eliminatedText, 3);
        }
    }
}