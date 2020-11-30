using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using Cinemachine;
using System.Collections;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Linq;
using TMPro;

public class GameView : View
{
    public static string WATCH_REPLAY_CLICKED = "WatchReplayClicked";
    public static string PLAY_AGAIN_CLICKED = "PlayAgainClicked";

    [Inject]
    public IEventDispatcher dispatcher { get; set; }

    // scene object connections
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera tableCamera;
    [SerializeField] private CinemachineFreeLook ballCamera;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject ballParent;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject gameOverScreen;

    // UI connections
    [SerializeField] private TMP_Text shotNumberLabel;
    [SerializeField] private TMP_Text totalPointsLabel;
    [SerializeField] private TMP_Text gameTimeLabel;
    [SerializeField] private TMP_Text gameOverPointsLabel;
    [SerializeField] private TMP_Text gameOverShotsLabel;
    [SerializeField] private TMP_Text gameOverTimeLabel;
    [SerializeField] private Button watchReplayButton;
    [SerializeField] private Button playAgainButton;

    // view state flags for async routines
    private bool isFocusingCameraOnBall = false;
    private bool isHittingBall = false;

    public Ball cueBall;
    public List<Ball> balls = new List<Ball>();

    public void InitGameBalls()
    {
        var spawnPoints = ballParent.GetComponentsInChildren<BallSpawnPoint>();
        foreach (var spawnPoint in spawnPoints)
        {
            // Check if we already have a ball of this type
            var ball = balls.FirstOrDefault(b => b.ballType == spawnPoint.ballType);
            if (ball == null)
            {
                // We do not, create a new one from the prefab
                var ballObject = Instantiate(ballPrefab) as GameObject;
                ballObject.transform.parent = ballParent.transform;
                ball = ballObject.GetComponent<Ball>();
                ball.ballType = spawnPoint.ballType;
                balls.Add(ball);
            }
            // Stop the balls if it's moving
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // Store a reference to the cue ball
            if (ball.ballType == BallType.cue)
            {
                cueBall = ball;
            }
            // Reset the balls position and rotation
            ball.gameObject.transform.position = spawnPoint.transform.position;
            ball.gameObject.transform.rotation = spawnPoint.transform.rotation;
        }
        // Stop any previous hit ball routine
        isHittingBall = false;
    }

    public void TransitionCameraToCueBall(float delay, Action onFinished)
    {
        if (isFocusingCameraOnBall)
        {
            return;
        }
        isFocusingCameraOnBall = true;
        StartCoroutine(TransitionCameraToBallRoutine(cueBall, delay, onFinished));
    }

    private IEnumerator TransitionCameraToBallRoutine(Ball ball, float delay, Action onFinished)
    {
        // Optional delay
        yield return new WaitForSeconds(delay);
        var transitionDuration = 0.3f;
        if (tableCamera.enabled)
        {
            // Longer duration if we're transitioning from the table camera
            transitionDuration = mainCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time;
        }
        // Enable the ball camera if needed
        tableCamera.enabled = false;
        ballCamera.enabled = true;
        // Update the target/follow object of the ball camera
        ballCamera.m_Follow = ball.gameObject.transform;
        ballCamera.m_LookAt = ball.gameObject.transform;
        // Wait for the transition to finish and invoke the callback
        yield return new WaitForSeconds(transitionDuration);
        if (onFinished != null)
        {
            onFinished();
        }
        isFocusingCameraOnBall = false;
    }

    public void HitCueBall(Vector3 force, Action onFinished)
    {
        if (isHittingBall)
        {
            return;
        }
        isHittingBall = true;
        StartCoroutine(HitBallRoutine(cueBall, force, onFinished));
    }

    IEnumerator HitBallRoutine(Ball ball, Vector3 force, Action onFinished)
    {
        // Appy the force to the specified ball
        ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        // Get a list of balls currently in the scene
        var ballsInScene = ballParent.GetComponentsInChildren<Ball>();
        var ballsActive = ballsInScene.Length > 0;
        while (ballsActive)
        {
            if (isHittingBall == false)
            {
                yield break; // Leave this routine if the balls were re-initialized
            }
            // Check each frame if any balls are still moving
            yield return new WaitForEndOfFrame();
            ballsActive = ballsInScene.FirstOrDefault(b => b.GetComponent<Rigidbody>().IsSleeping() == false) != null;
        }
        // All balls have stopped, invoke the callback
        if (onFinished != null)
        {
            onFinished();
        }
        isHittingBall = false;
    }

    public Vector3 GetAimDirection()
    {
        var delta = (cueBall.transform.position - ballCamera.transform.position);
        delta.y = 0f;
        return delta.normalized;
    }

    public void UpdateTotalPoints(int points)
    {
        totalPointsLabel.text = points.ToString();
    }

    public void UpdateShotNumber(int shotNumber)
    {
        shotNumberLabel.text = shotNumber.ToString();
    }

    public void UpdateGameTime(int timeInSeconds)
    {
        gameTimeLabel.text = GetTimeFormattedString(timeInSeconds);
    }

    public void SetWatchReplayEnabled(bool enabled)
    {
        watchReplayButton.gameObject.SetActive(enabled);
    }

    public void OnWatchReplayButtonClicked()
    {
        dispatcher.Dispatch(WATCH_REPLAY_CLICKED);
    }

    public void OnPlayAgainClicked()
    {
        dispatcher.Dispatch(PLAY_AGAIN_CLICKED);
    }

    public void ShowGameOverScreen(int points, int shots, int timeInSeconds)
    {
        inGameUI.SetActive(false);
        gameOverScreen.SetActive(true);
        gameOverPointsLabel.text = points.ToString();
        gameOverShotsLabel.text = shots.ToString();
        gameOverTimeLabel.text = GetTimeFormattedString(timeInSeconds);
    }

    public void HideGameOverScreen()
    {
        inGameUI.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    private string GetTimeFormattedString(int timeInSeconds)
    {
        var seconds = (timeInSeconds % 60).ToString("D2");
        var minutes = (timeInSeconds / 60).ToString("D2");
        return String.Format("{0}:{1}", minutes, seconds);
    }
}