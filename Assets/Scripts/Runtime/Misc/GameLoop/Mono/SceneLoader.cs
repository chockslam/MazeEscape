using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] SceneMapping.Scene scene;

    public void LoadScene()
    {
        SceneManager.LoadScene(SceneMapping.GetSceneName(scene));
    }
}