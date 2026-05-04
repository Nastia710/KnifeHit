using UnityEngine;

public class KnifeSpawner : MonoBehaviour
{
    public static KnifeSpawner Instance { get; private set; }

    [Header("Knife Settings")]
    public GameObject knifePrefab;        
    public Transform spawnPoint;          

    [Header("Knife Skins")]
    public Sprite[] knifeSprites;         

    private int knivesRemaining;
    private GameObject currentKnife;

    void Awake()
    {
        Instance = this;
    }

    public void StartLevel(int knifeCount)
    {
        knivesRemaining = knifeCount;

        if (UIManager.Instance != null)
            UIManager.Instance.SetupKnifeIcons(knivesRemaining);

        SpawnKnife();
    }

    void SpawnKnife()
    {
        if (knivesRemaining <= 0) return;

        currentKnife = Instantiate(knifePrefab, spawnPoint.position, Quaternion.identity);

        int selectedKnife = PlayerPrefs.GetInt("SelectedKnife", 0);
        if (knifeSprites != null && selectedKnife < knifeSprites.Length && knifeSprites[selectedKnife] != null)
        {
            SpriteRenderer sr = currentKnife.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = knifeSprites[selectedKnife];
        }
    }

    public void OnKnifeStuck()
    {
        knivesRemaining--;

        if (UIManager.Instance != null)
            UIManager.Instance.UseKnifeIcon();

        if (knivesRemaining > 0)
        {
            SpawnKnife();
        }
        else
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.LevelComplete();
        }
    }
}
