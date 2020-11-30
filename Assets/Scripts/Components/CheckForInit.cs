using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckForInit : MonoBehaviour
{
    void Awake()
    {
        // Convenience script. Add to any scene and when hitting "Play"
        // the game will be launched via the Init scene.
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == SceneName.INIT)
            {
                return;
            }
        }
        SceneManager.LoadScene(SceneName.INIT, LoadSceneMode.Single);
    }
}