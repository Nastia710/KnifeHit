using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Knife Icons")]
    public Transform knifeIconPanel;    
    public GameObject knifeIconPrefab;  

    [Header("Score")]
    public TextMeshProUGUI scoreText;  

    [Header("Icon Colors")]
    public Color activeColor = Color.white;        
    public Color usedColor = new Color(1f, 1f, 1f, 0.25f); 

    private List<Image> knifeIcons = new List<Image>();
    private int currentIconIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPointsUpdated += UpdateScoreUI;
            UpdateScoreUI(GameManager.Instance.GlobalPoints);
        }
    }

    public void SetupKnifeIcons(int count)
    {
        foreach (Transform child in knifeIconPanel)
        {
            Destroy(child.gameObject);
        }
        knifeIcons.Clear();
        currentIconIndex = 0;

        for (int i = 0; i < count; i++)
        {
            GameObject icon = Instantiate(knifeIconPrefab, knifeIconPanel);
            Image img = icon.GetComponent<Image>();
            img.color = activeColor;
            knifeIcons.Add(img);
        }
    }

    public void UseKnifeIcon()
    {
        if (currentIconIndex < knifeIcons.Count)
        {
            knifeIcons[currentIconIndex].color = usedColor;
            currentIconIndex++;
        }
    }

    void UpdateScoreUI(int points)
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + points.ToString();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPointsUpdated -= UpdateScoreUI;
    }
}
