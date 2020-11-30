using UnityEngine;
using System;
using System.Collections.Generic;
using strange.extensions.command.impl;

public class TakeShotCommand : EventCommand
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
        // Save the shot vector for the current turn
        var shotVector = (Vector3)evt.data;
        var currentTurn = game.turns[game.turns.Count - 1];
        currentTurn.shotVector = shotVector;
        Debug.LogFormat("TakeShotCommand - shotVector: {0}", shotVector);
    }
}