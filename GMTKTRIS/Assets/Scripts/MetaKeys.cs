using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaKeys : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.Return)) SceneManager.LoadScene(0);
    }
}