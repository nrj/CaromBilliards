using UnityEngine;
using UnityEngine.SceneManagement;
using strange.extensions.command.impl;


public class LoadSceneCommand : EventCommand
{
    public override void Execute()
    {
        // Super basic scene loading
        var args = ((string, string))evt.data;
        var sceneToLoad = args.Item1;
        var sceneToUnload = args.Item2;
        var op = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        op.allowSceneActivation = true;
        SceneManager.UnloadSceneAsync(sceneToUnload);
    }
}