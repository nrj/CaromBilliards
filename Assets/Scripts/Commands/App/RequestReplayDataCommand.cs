using UnityEngine;
using System.Collections.Generic;
using strange.extensions.command.impl;

public class RequestReplayDataCommand : EventCommand
{
    [Inject]
    public GameModel model { get; set; }

    public override void Execute()
    {
        // Get the last turn where a shot was taken
        var game = model.currentGame;
        var lastTurnIndex = game.turns.FindLastIndex(t => t.shotVector != Vector3.zero);
        Debug.LogFormat("RequestReplayDataCommand - lastTurnIndex: {0}", lastTurnIndex);
        if (lastTurnIndex != -1)
        {
            dispatcher.Dispatch(AppEvent.FULFILL_REPLAY_DATA, game.turns[lastTurnIndex]);
        }
    }
}