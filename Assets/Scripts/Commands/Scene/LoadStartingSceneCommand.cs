using strange.extensions.command.impl;
using UnityEngine.SceneManagement;

public class LoadStartingSceneCommand : EventCommand
{
    public override void Execute()
    {
        // Load the starting scene synchronously
        SceneManager.LoadScene(SceneName.MAIN_MENU, LoadSceneMode.Additive);
    }
}