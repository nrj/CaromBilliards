using UnityEngine;
using System;
using System.Collections.Generic;
using strange.extensions.command.impl;

public class StartNextTurnCommand : EventCommand
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
        // Create a new turn and add it to the current game
        var balls = (List<Ball>)evt.data;
        var nextTurn = new GameTurnVO(balls);
        game.turns.Add(nextTurn);
        Debug.LogFormat("StartNextTurnCommand - shotNumber {0}, totalPoins {1}", game.turns.Count, game.totalPoints);
        // Notify the model changed
        dispatcher.Dispatch(AppEvent.SHOT_NUMBER_UPDATED, game.turns.Count);
    }
}