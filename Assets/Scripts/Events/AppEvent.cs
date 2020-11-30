using System;

public class AppEvent
{
    public static string START_NEW_GAME = "StartNewGameEvent";
    public static string START_NEXT_TURN = "StartNextTurnEvent";
    public static string TAKE_SHOT = "TakeShotEvent";
    public static string CUE_BALL_HIT_BALL = "CueBallHitBallEvent";
    public static string GAME_TIME_UPDATED = "GameTimeUpdatedEvent";
    public static string SHOT_NUMBER_UPDATED = "ShotNumberUpdatedEvent";
    public static string TOTAL_POINTS_UPDATED = "TotalPointsUpdatedEvent";
    public static string REQUEST_REPLAY_DATA = "RequestReplayDataEvent";
    public static string FULFILL_REPLAY_DATA = "FulfillReplayDataEvent";
    public static string GAME_OVER = "GameOverEvent";
    public static string GAME_OVER_RESULTS = "GameOverResultsEvent";
}