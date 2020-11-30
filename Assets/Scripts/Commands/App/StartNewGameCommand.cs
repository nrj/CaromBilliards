using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;

public class StartNewGameCommand : EventCommand
{
    [Inject]
    public GameModel model { get; set; }

    [Inject]
    public IRoutineRunner routineRunner { get; set; }


    public override void Execute()
    {
        // Stop the current game if any
        if (model.currentGame != null)
        {
            model.currentGame.isOver = true;
        }
        // Create a new game vo
        var newGame = new GameVO();
        // Create a new turn with the current ball states
        var gameBalls = (List<Ball>)evt.data;
        var turnOne = new GameTurnVO(gameBalls);
        // add the turn to our game
        newGame.turns.Add(turnOne);
        // set the new game as the current game
        model.currentGame = newGame;
        Debug.Log("StartNewGameCommand");
        // Notify model updates
        dispatcher.Dispatch(AppEvent.SHOT_NUMBER_UPDATED, newGame.turns.Count);
        dispatcher.Dispatch(AppEvent.TOTAL_POINTS_UPDATED, newGame.totalPoints);
        dispatcher.Dispatch(AppEvent.GAME_TIME_UPDATED, newGame.timeInSeconds);
        // Start a game timer
        Debug.Log("Started game timer");
        routineRunner.StartCoroutine(GameTimerRoutine(newGame));
    }

    IEnumerator GameTimerRoutine(GameVO game)
    {
        Retain();
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (game.isOver)
            {
                break;
            }
            game.timeInSeconds += 1;
            dispatcher.Dispatch(AppEvent.GAME_TIME_UPDATED, game.timeInSeconds);
        }
        Debug.Log("GameTimerRoutine - timer ended");
        Release();
    }
}