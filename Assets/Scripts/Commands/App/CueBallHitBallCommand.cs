using UnityEngine;
using System;
using strange.extensions.command.impl;

public class CueBallHitBallCommand : EventCommand
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
        var ballHit = (BallType)evt.data;
        Debug.LogFormat("CueBallHitBallCommand - {0}", ballHit);
        var currentTurn = game.turns[game.turns.Count - 1];
        var didAlreadyHitBall = currentTurn.ballsHit.Contains(ballHit);
        if (didAlreadyHitBall)
        {
            return;  // We don't care about hitting the same ball twice
        }
        // Add the ball
        currentTurn.ballsHit.Add(ballHit);

        // Check if we earned a point
        if (currentTurn.ballsHit.Contains(BallType.yellow) && currentTurn.ballsHit.Contains(BallType.red))
        {
            // We earned a point!
            game.totalPoints += 1;
            Debug.LogFormat("Point scored - totalPoints: {0}", game.totalPoints);
            dispatcher.Dispatch(AppEvent.TOTAL_POINTS_UPDATED, game.totalPoints);
        }
    }
}