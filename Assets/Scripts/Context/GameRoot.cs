using UnityEngine;
using strange.extensions.context.impl;

public class GameRoot : ContextView
{
    void Awake()
    {
        Application.targetFrameRate = 60;

        context = new GameContext(this, true);
        context.Start();
    }
}
