using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Global Currency")]
    public int GlobalPoints;
    public int applesThisRun;

    public event Action<int> OnPointsUpdated;

    private const string POINTS_KEY = "GlobalPoints";
    private const string FIRST_LAUNCH_KEY = "FirstLaunch";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        LoadData();
        applesThisRun = 0;
    }

    private void LoadData()
    {
        if (PlayerPrefs.GetInt(FIRST_LAUNCH_KEY, 0) == 0)
        {
            GlobalPoints = 50;
            PlayerPrefs.SetInt(POINTS_KEY, GlobalPoints);
            PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 1);
            PlayerPrefs.Save();
        }
        else
        {
            GlobalPoints = PlayerPrefs.GetInt(POINTS_KEY, 0);
        }
    }

    public void AddPoints(int amount)
    {
        GlobalPoints += amount;
        SavePoints();
        
        if (amount == 3)
        {
            applesThisRun++;
        }

        OnPointsUpdated?.Invoke(GlobalPoints);
    }

    public bool SpendPoints(int amount)
    {
        if (GlobalPoints >= amount)
        {
            GlobalPoints -= amount;
            SavePoints();
            OnPointsUpdated?.Invoke(GlobalPoints);
            return true;
        }
        return false;
    }

    private void SavePoints()
    {
        PlayerPrefs.SetInt(POINTS_KEY, GlobalPoints);
        PlayerPrefs.Save();
    }

    public void GameOver()
    {
        int currentLevel = PlayerPrefs.GetInt("SavedLevel", 1);
        bool isBoss = (currentLevel % 5 == 0);

        if (isBoss)
        {
            int checkpoint = ((currentLevel - 1) / 5) * 5 + 1;
            PlayerPrefs.SetInt("SavedLevel", checkpoint);
            PlayerPrefs.Save();
        }
        
        CheckLeaderboardRecord();
        Invoke("ReloadScene", 1.5f);
    }

    public void BackToMenu()
    {
        int currentLevel = PlayerPrefs.GetInt("SavedLevel", 1);
        bool isBoss = (currentLevel % 5 == 0);

        if (isBoss)
        {
            int checkpoint = ((currentLevel - 1) / 5) * 5 + 1;
            PlayerPrefs.SetInt("SavedLevel", checkpoint);
            PlayerPrefs.Save();
        }
        
        CheckLeaderboardRecord();

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void CheckLeaderboardRecord()
    {
        int[] tops = new int[5];
        for (int i = 0; i < 5; i++)
        {
            tops[i] = PlayerPrefs.GetInt("AppleRecord_" + i, 0);
        }

        int newScore = applesThisRun;
        for (int i = 0; i < 5; i++)
        {
            if (newScore > tops[i])
            {
                for (int j = 4; j > i; j--)
                {
                    tops[j] = tops[j - 1];
                }

                tops[i] = newScore;
                break;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt("AppleRecord_" + i, tops[i]);
        }
        PlayerPrefs.Save();

        applesThisRun = 0;
    }

    private void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
