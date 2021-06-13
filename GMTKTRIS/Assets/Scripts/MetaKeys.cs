using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaKeys : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.Return)) ReloadScene();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}