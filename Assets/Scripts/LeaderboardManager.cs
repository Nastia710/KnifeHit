using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject leaderboardPanel; 
    public GameObject menuPanel;        
    public GameObject titleImage;      
    public GameObject leaderboardButton; 

    [Header("UI Text")]
    public TextMeshProUGUI[] scoreTexts; 

    void Start()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    public void OpenLeaderboard()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (titleImage != null) titleImage.SetActive(false);
        if (leaderboardButton != null) leaderboardButton.SetActive(false);

        if (leaderboardPanel != null) leaderboardPanel.SetActive(true);

        UpdateLeaderboardUI();
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);

        if (menuPanel != null) menuPanel.SetActive(true);
        if (titleImage != null) titleImage.SetActive(true);
        if (leaderboardButton != null) leaderboardButton.SetActive(true);
    }

    private void UpdateLeaderboardUI()
    {
        for (int i = 0; i < 5; i++)
        {
            int score = PlayerPrefs.GetInt("AppleRecord_" + i, 0);
            if (scoreTexts != null && i < scoreTexts.Length && scoreTexts[i] != null)
            {
                scoreTexts[i].text = (i + 1) + ". " + score + " APPLES";
            }
        }
    }
}
