using System;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class GameMediator : EventMediator
{
    const int POINTS_TO_WIN = 3;

    // Lerp between these shot force values up to max time
    const float SHOT_CHARGE_MIN_FORCE = 0.1f;
    const float SHOT_CHARGE_MAX_FOCE = 15f;
    const float SHOT_CHARGE_MAX_TIME = 2f;

    // Inverse lerp between these collision magnitudes to get our
    // volume scale, but never go below the min volume scale
    const float HIT_VOLUME_MIN_MAGNITUDE = 1f;
    const float HIT_VOLUME_MAX_MAGNITUDE = 6f;
    const float HIT_VOLUME_MIN_SCALE = 0.2f;

    [Inject]
    public GameView view { get; set; }

    float shotChargeT = 0f;
    bool isGameActive = false;
    bool isChargingShot = false;
    bool isWaitingForBallsToStop = false;
    bool isWatchingReplay = false;

    public override void OnRegister()
    {
        base.OnRegister();

        // Our view is ready.
        view.dispatcher.AddListener(GameView.WATCH_REPLAY_CLICKED, OnWatchReplayClicked);
        view.dispatcher.AddListener(GameView.PLAY_AGAIN_CLICKED, OnPlayAgainClicked);

        // Subscribe to application events.
        dispatcher.AddListener(AppEvent.SHOT_NUMBER_UPDATED, OnShotNumberUpdated);
        dispatcher.AddListener(AppEvent.TOTAL_POINTS_UPDATED, OnTotalPointsUpdated);
        dispatcher.AddListener(AppEvent.GAME_TIME_UPDATED, OnGameTimeUpdated);
        dispatcher.AddListener(AppEvent.FULFILL_REPLAY_DATA, OnReplayData);
        dispatcher.AddListener(AppEvent.GAME_OVER_RESULTS, OnGameOverResults);

        StartNewGame();
    }

    public override void OnRemove()
    {
        base.OnRemove();

        // Our view was deregistered.
        view.dispatcher.RemoveListener(GameView.WATCH_REPLAY_CLICKED, OnWatchReplayClicked);
        view.dispatcher.RemoveListener(GameView.PLAY_AGAIN_CLICKED, OnPlayAgainClicked);

        // Remove our application event listeners.
        dispatcher.RemoveListener(AppEvent.SHOT_NUMBER_UPDATED, OnShotNumberUpdated);
        dispatcher.RemoveListener(AppEvent.TOTAL_POINTS_UPDATED, OnTotalPointsUpdated);
        dispatcher.RemoveListener(AppEvent.GAME_TIME_UPDATED, OnGameTimeUpdated);
        dispatcher.RemoveListener(AppEvent.FULFILL_REPLAY_DATA, OnReplayData);
        dispatcher.RemoveListener(AppEvent.GAME_OVER_RESULTS, OnGameOverResults);
    }

    private void StartNewGame()
    {
        // Prepare the view for a new game
        isGameActive = false;
        // Reset the balls
        view.InitGameBalls();
        var index = 0;
        foreach (var ball in view.balls)
        {
            // Subscribe to collision events
            ball.OnCollision += OnBallCollision;
            // Add a unique identifier
            ball.uid = index;
            index += 1;
        }
        // Hide necessary UI elements
        view.SetWatchReplayEnabled(false);
        view.HideGameOverScreen();
        // Focus the camera on our cue ball after a slight delay
        view.TransitionCameraToCueBall(1f, () =>
        {
            // Start the game
            dispatcher.Dispatch(AppEvent.START_NEW_GAME, view.balls);
            isGameActive = true;
        });
    }

    void OnBallCollision(BallCollisionVO collision)
    {
        // Two balls collided, both will notify us about the collision
        // We need to ignore one, so choose the one with the lower uid
        if (collision.srcId < collision.dstId)
        {
            return;
        }
        // Calculate how loud to play the sound
        var volume = Mathf.InverseLerp(HIT_VOLUME_MIN_MAGNITUDE, HIT_VOLUME_MAX_MAGNITUDE, collision.magnitude);
        // Never play it quieter than the min scale
        var volumeScale = Mathf.Max(volume, HIT_VOLUME_MIN_SCALE);
        // Play the sound at the specified volume
        dispatcher.Dispatch(AudioEvent.PLAY_SOUND, (SoundName.BALL_HIT, volumeScale));
        // If this is a replay, stop
        if (isWatchingReplay)
        {
            return;
        }
        // If one of the balls involved in this collision was the cue ball, notify our app.
        if (collision.srcType == BallType.cue)
        {
            dispatcher.Dispatch(AppEvent.CUE_BALL_HIT_BALL, collision.dstType);
        }
        else if (collision.dstType == BallType.cue)
        {
            dispatcher.Dispatch(AppEvent.CUE_BALL_HIT_BALL, collision.srcType);
        }
    }

    public bool CanTakeShot()
    {
        return isGameActive && !isChargingShot && !isWaitingForBallsToStop && !isWatchingReplay;
    }

    public bool CanWatchReplay()
    {
        return CanTakeShot();
    }

    /*
     * Player Input
     */

    void OnShotCharge()
    {
        if (CanTakeShot() == false)
        {
            return;  // not allowed to shoot
        }
        isChargingShot = true;
        // start our charge routine
        StartCoroutine(ShotChargeRoutine());
        // disable the watch replay button
        view.SetWatchReplayEnabled(false);
    }

    IEnumerator ShotChargeRoutine()
    {
        // reset the current charge
        shotChargeT = 0f;
        var shotChargeTime = 0f;
        while (isChargingShot)
        {
            // charge is time-based, up to max time
            yield return new WaitForEndOfFrame();
            if (shotChargeTime <= SHOT_CHARGE_MAX_TIME)
            {
                // increase t value each frame
                shotChargeTime += Time.deltaTime;
                shotChargeT = Mathf.Clamp01(shotChargeTime / SHOT_CHARGE_MAX_TIME);
            }
            // get the current direction between the camera and the cue ball
            var aimDirection = view.GetAimDirection();
            // update the charge amount and the current direction we're aiming
            view.cueBall.SetCharge(shotChargeT, aimDirection);
        }
        // stopped charging, reset charge bar
        shotChargeT = 0f;
        view.cueBall.SetCharge(shotChargeT, Vector3.zero);
    }

    void OnShotRelease()
    {
        if (isChargingShot == false)
        {
            return;
        }
        // stop our charging routine
        isChargingShot = false;
        // calculate the force and direction of the shot
        var aimDirection = view.GetAimDirection();
        var shotForce = Mathf.Lerp(SHOT_CHARGE_MIN_FORCE, SHOT_CHARGE_MAX_FOCE, shotChargeT);
        var shotVector = aimDirection * shotForce;
        // hit the cue ball
        view.HitCueBall(shotVector, () =>
        {
            // shot is finished and all balls have stopped
            isWaitingForBallsToStop = false;
            // start the next turn
            dispatcher.Dispatch(AppEvent.START_NEXT_TURN, view.balls);
            // enable the watch replay button
            view.SetWatchReplayEnabled(true);
        });
        // wait for all balls to stop
        isWaitingForBallsToStop = true;
        // notify the application
        dispatcher.Dispatch(AppEvent.TAKE_SHOT, shotVector);
    }

    void OnWatchReplayClicked()
    {
        if (CanWatchReplay() == false)
        {
            return;
        }
        dispatcher.Dispatch(AppEvent.REQUEST_REPLAY_DATA);
    }

    void OnPlayAgainClicked()
    {
        // Reset our state flags
        isGameActive = false;
        isChargingShot = false;
        isWaitingForBallsToStop = false;
        isWatchingReplay = false;
        // Reset the balls
        view.InitGameBalls();
        // Hide necessary UI elements
        view.SetWatchReplayEnabled(false);
        view.HideGameOverScreen();
        // Focus the camera on the cue ball
        view.TransitionCameraToCueBall(0f, () =>
        {
            // Start the game
            isGameActive = true;
            dispatcher.Dispatch(AppEvent.START_NEW_GAME, view.balls);
        });
    }

    /*
     * App Event Handlers
     */

    void OnReplayData(IEvent evt)
    {
        if (CanWatchReplay() == false)
        {
            return;
        }
        // Disable replays
        isWatchingReplay = true;
        view.SetWatchReplayEnabled(false);
        // This is turn we will replay
        var replayTurn = (GameTurnVO)evt.data;
        // Reset each ball back to its previous position
        foreach (var ballPosition in replayTurn.ballStartPositions)
        {
            var ballId = ballPosition.Item1;
            var ballStartPosition = ballPosition.Item2;
            var ball = view.balls.First(b => b.uid == ballId);
            ball.transform.position = ballStartPosition;
        }
        // Reset the camera
        view.TransitionCameraToCueBall(0f, () =>
        {
            // Strike the cue ball again with the same shot vector
            view.HitCueBall(replayTurn.shotVector, () =>
            {
                // Enable replays
                isWatchingReplay = false;
                view.SetWatchReplayEnabled(true);
            });
        });
    }

    void OnGameTimeUpdated(IEvent evt)
    {
        var gameTime = (int)evt.data;
        view.UpdateGameTime(gameTime);
    }

    void OnShotNumberUpdated(IEvent evt)
    {
        var shotNumber = (int)evt.data;
        view.UpdateShotNumber(shotNumber);
    }

    void OnTotalPointsUpdated(IEvent evt)
    {
        var totalPoints = (int)evt.data;
        view.UpdateTotalPoints(totalPoints);
        if (totalPoints >= POINTS_TO_WIN)
        {
            isGameActive = false;
            dispatcher.Dispatch(AppEvent.GAME_OVER);
        }
    }

    void OnGameOverResults(IEvent evt)
    {
        var gameResults = (GameResultsVO)evt.data;
        view.ShowGameOverScreen(gameResults.points, gameResults.shots, gameResults.timeInSeconds);
    }
}