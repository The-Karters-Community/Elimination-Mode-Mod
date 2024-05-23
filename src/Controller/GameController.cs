using System;
using System.Collections.Generic;
using Cpp2IL.Core.Extensions;
using TheKartersModdingAssistant;
using TheKartersModdingAssistant.Event;

namespace EliminationMode.Controller;

public static class GameController {
    public static int amountOfEliminatedPlayers = 0;
    public static List<Player> playersAtRaceInitialization = new();

    public static void Initialize() {
        GameEvent.onRaceInitialize += GameController.OnRaceInitialize;
    }

    public static void OnRaceInitialize() {
        GameController.amountOfEliminatedPlayers = 0;

        Game.Get().SetAmountOfLaps(Player.GetActivePlayers().Count - 1);

        // Reset all players states.
        foreach (Player player in Player.GetActivePlayers()) {
            player.Unset("isEliminated");
            player.Unset("eliminatedAtPosition");

            player.uAntPlayer.visualInstanceParent.SetActive(true);

            if (player.IsHuman()) {
                player.uAntPlayer.humanControllingParent.gameObject.SetActive(true);
            } else if (player.IsAi()) {
                player.uAntPlayer.aiControllingParent.gameObject.SetActive(true);
            }
        }
    }
}