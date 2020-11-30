using UnityEngine;
using System;
using System.Collections.Generic;
using strange.extensions.command.impl;

public class GameOverCommand : EventCommand
{
    [Inject]
    public GameModel model { get; set; }

    public override void Execute()
    {
        var game = model.currentGame;
        if (game.isOver)
        {
            return;
        }
        game.isOver = true;
        var results = new GameResultsVO()
        {
            points = game.totalPoints,
            shots = game.turns.Count,
            timeInSeconds = game.timeInSeconds
        };
        Debug.LogFormat("GameOverCommand - points {0}, shots {1}, time {2}", results.points, results.shots, results.timeInSeconds);
        dispatcher.Dispatch(AppEvent.GAME_OVER_RESULTS, results);
    }
}