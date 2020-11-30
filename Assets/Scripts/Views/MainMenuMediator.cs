using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

public class MainMenuMediator : EventMediator
{
    [Inject]
    public MainMenuView view { get; set; }

    private bool isLoadingGame = false;

    public override void OnRegister()
    {
        base.OnRegister();

        view.dispatcher.AddListener(MainMenuView.START_BUTTON_CLICKED, OnClickStart);
        view.dispatcher.AddListener(MainMenuView.VOLUME_SLIDER_CHANGED, OnChangeVolume);
    }

    public override void OnRemove()
    {
        base.OnRemove();

        view.dispatcher.RemoveListener(MainMenuView.START_BUTTON_CLICKED, OnClickStart);
        view.dispatcher.RemoveListener(MainMenuView.VOLUME_SLIDER_CHANGED, OnChangeVolume);
    }

    void OnClickStart()
    {
        if (isLoadingGame)
        {
            return;
        }
        isLoadingGame = true;
        view.ShowLoadingText();
        // Load the game asynchronously, then unload the menu
        dispatcher.Dispatch(SceneEvent.LOAD_SCENE, (SceneName.GAME, SceneName.MAIN_MENU));
    }

    void OnChangeVolume(IEvent evt)
    {
        var sliderValue = (float)evt.data;
        dispatcher.Dispatch(AudioEvent.CHANGE_VOLUME, sliderValue);
    }
}