using UnityEngine;
using System;
using strange.extensions.command.impl;

public class ChangeVolumeCommand : EventCommand
{
    [Inject]
    public AudioService audioService { get; set; }

    public override void Execute()
    {
        var volume = (float)evt.data;
        audioService.SetVolume(volume);
        Debug.LogFormat("ChangeVolumeCommand - volume: {0}", volume);
    }
}