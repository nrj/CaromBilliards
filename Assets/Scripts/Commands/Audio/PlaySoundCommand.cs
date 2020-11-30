using UnityEngine;
using System;
using strange.extensions.command.impl;

public class PlaySoundCommand : EventCommand
{
    [Inject]
    public AudioService audioService { get; set; }

    public override void Execute()
    {
        var sound = ((SoundName, float))evt.data;
        var soundName = sound.Item1;
        var volumeScale = sound.Item2;
        audioService.PlaySound(soundName, volumeScale);
    }
}