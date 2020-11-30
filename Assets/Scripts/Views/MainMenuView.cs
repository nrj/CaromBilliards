using System;
using System.Collections;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using TMPro;
using UnityEngine.UI;

public class MainMenuView : View
{
    public static string START_BUTTON_CLICKED = "StartButtonClicked";
    public static string VOLUME_SLIDER_CHANGED = "VolumeSliderChanged";

    [Inject]
    public IEventDispatcher dispatcher { get; set; }

    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider volumeSlider;

    public void OnClickStart()
    {
        dispatcher.Dispatch(START_BUTTON_CLICKED);
    }

    public void OnChangeVolume()
    {
        dispatcher.Dispatch(VOLUME_SLIDER_CHANGED, volumeSlider.value);
    }

    public void ShowLoadingText()
    {
        loadingText.enabled = true;
    }

    public void SetVolumeSliderValue(float value)
    {
        volumeSlider.value = value;
    }

    ~MainMenuView()
    {
        Debug.Log("MainMenuView dealloc");
    }
}
