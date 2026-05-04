using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Knife Data")]
    public Sprite[] knifeSprites;       
    public int[] knifePrices;           

    [Header("UI Panels")]
    public GameObject shopPanel;        
    public GameObject menuPanel;        
    public GameObject titleImage;       
    public GameObject leaderboardButton; 

    [Header("UI Score")]
    public TextMeshProUGUI shopScoreText; 

    [Header("UI Knife Items")]
    public Button[] knifeButtons;       
    public Image[] knifeImages;         
    public TextMeshProUGUI[] priceTexts; 
    public Image[] knifeBackgrounds;    

    private int selectedKnife = 0;

    void Start()
    {
        selectedKnife = PlayerPrefs.GetInt("SelectedKnife", 0);

        if (PlayerPrefs.GetInt("Knife_0", 0) == 0)
        {
            PlayerPrefs.SetInt("Knife_0", 1);
            PlayerPrefs.Save();
        }

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    public void OpenShop()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (titleImage != null) titleImage.SetActive(false);
        if (leaderboardButton != null) leaderboardButton.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(true);
        UpdateShopUI();
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
        if (titleImage != null) titleImage.SetActive(true);
        if (leaderboardButton != null) leaderboardButton.SetActive(true);
    }

    public void OnKnifeClicked(int index)
    {
        bool isUnlocked = PlayerPrefs.GetInt("Knife_" + index, 0) == 1;

        if (isUnlocked)
        {
            selectedKnife = index;
            PlayerPrefs.SetInt("SelectedKnife", selectedKnife);
            PlayerPrefs.Save();
        }
        else
        {
            int price = knifePrices[index];
            int currentPoints = PlayerPrefs.GetInt("GlobalPoints", 0);

            if (currentPoints >= price)
            {
                currentPoints -= price;
                PlayerPrefs.SetInt("GlobalPoints", currentPoints);
                PlayerPrefs.SetInt("Knife_" + index, 1);
                selectedKnife = index;
                PlayerPrefs.SetInt("SelectedKnife", selectedKnife);
                PlayerPrefs.Save();
            }
        }

        UpdateShopUI();
    }

    void UpdateShopUI()
    {
        int currentPoints = PlayerPrefs.GetInt("GlobalPoints", 0);

        if (shopScoreText != null)
            shopScoreText.text = "SCORE: " + currentPoints;

        for (int i = 0; i < knifeSprites.Length; i++)
        {
            bool isUnlocked = PlayerPrefs.GetInt("Knife_" + i, 0) == 1;
            bool isSelected = (i == selectedKnife);

            if (knifeImages != null && i < knifeImages.Length)
                knifeImages[i].sprite = knifeSprites[i];

            if (priceTexts != null && i < priceTexts.Length)
            {
                if (isSelected)
                    priceTexts[i].text = "SELECTED";
                else if (isUnlocked)
                    priceTexts[i].text = "OWNED";
                else
                    priceTexts[i].text = knifePrices[i].ToString();
            }

            if (knifeBackgrounds != null && i < knifeBackgrounds.Length)
            {
                if (isSelected)
                {
                    knifeBackgrounds[i].color = new Color(0.3f, 0.85f, 0.4f); 
                    if (knifeButtons != null && i < knifeButtons.Length) knifeButtons[i].interactable = true;
                }
                else if (isUnlocked)
                {
                    knifeBackgrounds[i].color = new Color(0.9f, 0.9f, 0.9f); 
                    if (knifeButtons != null && i < knifeButtons.Length) knifeButtons[i].interactable = true;
                }
                else if (currentPoints >= knifePrices[i])
                {
                    knifeBackgrounds[i].color = new Color(1f, 0.85f, 0.3f); 
                    if (knifeButtons != null && i < knifeButtons.Length) knifeButtons[i].interactable = true;
                }
                else
                {
                    knifeBackgrounds[i].color = new Color(0.5f, 0.5f, 0.5f);
                    if (knifeButtons != null && i < knifeButtons.Length) knifeButtons[i].interactable = false;
                }
            }
        }
    }
}
