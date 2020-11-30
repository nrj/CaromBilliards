using UnityEngine;
using strange.extensions.context.impl;
using strange.extensions.context.api;

public class GameContext : MVCSContext
{
    public GameContext(MonoBehaviour view, bool autoStartup) : base(view, autoStartup)
    {
    }

    protected override void mapBindings()
    {
        // Bind services
        injectionBinder.Bind<IRoutineRunner>().To<RoutineRunner>().ToSingleton();
        injectionBinder.Bind<AudioService>().To<AudioService>().ToSingleton();

        // Bind our model
        injectionBinder.Bind<GameModel>().To<GameModel>().ToSingleton();

        // Bind views to their mediators
        mediationBinder.Bind<MainMenuView>().To<MainMenuMediator>();
        mediationBinder.Bind<GameView>().To<GameMediator>();

        // Bind events to their commands
        commandBinder.Bind(AppEvent.START_NEW_GAME).To<StartNewGameCommand>();
        commandBinder.Bind(AppEvent.START_NEXT_TURN).To<StartNextTurnCommand>();
        commandBinder.Bind(AppEvent.TAKE_SHOT).To<TakeShotCommand>();
        commandBinder.Bind(AppEvent.CUE_BALL_HIT_BALL).To<CueBallHitBallCommand>();
        commandBinder.Bind(AppEvent.REQUEST_REPLAY_DATA).To<RequestReplayDataCommand>();
        commandBinder.Bind(AppEvent.GAME_OVER).To<GameOverCommand>();
        commandBinder.Bind(AudioEvent.PLAY_SOUND).To<PlaySoundCommand>();
        commandBinder.Bind(AudioEvent.CHANGE_VOLUME).To<ChangeVolumeCommand>();
        commandBinder.Bind(SceneEvent.LOAD_SCENE).To<LoadSceneCommand>();

        // When strange is done doing its thing, this is event fired to kick off your app
        commandBinder.Bind(ContextEvent.START).To<LoadStartingSceneCommand>();
    }
}