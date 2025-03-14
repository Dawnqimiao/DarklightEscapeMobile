using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public void LoadLevel(int index)
    {
        Debug.Log("Loading level: " + index);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        SceneManager.LoadScene(index);
    }
}

