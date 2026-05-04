using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "SampleScene";
    public ShopManager shopManager;

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenShop()
    {
        if (shopManager != null)
            shopManager.OpenShop();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

