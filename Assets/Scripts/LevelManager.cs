using UnityEngine;
using System.Collections;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Target Prefabs")]
    public GameObject normalTargetPrefab;    
    public GameObject[] bossPrefabs;         
    public Vector3 targetSpawnPosition;      

    [Header("Level Settings")]
    public int bossEveryNLevels = 5;         
    public int normalKnives = 5;             
    public int bossKnives = 8;               
    public float delayBetweenLevels = 1.5f;  

    [Header("UI")]
    public TextMeshProUGUI levelText;        

    [Header("Audio")]
    public AudioClip levelStartSound;        

    private int currentLevel = 1;
    private GameObject currentTarget;
    private AudioSource audioSource;
    private const string LEVEL_KEY = "SavedLevel";

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        currentLevel = PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }

    void Start()
    {
        SpawnLevel();
    }

    void SpawnLevel()
    {
        bool isBoss = (currentLevel % bossEveryNLevels == 0) && bossPrefabs != null && bossPrefabs.Length > 0;

        GameObject prefab;
        int knives;

        if (isBoss)
        {
            prefab = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
            knives = bossKnives;
        }
        else
        {
            prefab = normalTargetPrefab;
            knives = normalKnives;
        }

        currentTarget = Instantiate(prefab, targetSpawnPosition, Quaternion.identity);

        UpdateLevelUI();

        if (levelStartSound != null && audioSource != null)
            audioSource.PlayOneShot(levelStartSound);

        if (KnifeSpawner.Instance != null)
            KnifeSpawner.Instance.StartLevel(knives);
    }

    public void LevelComplete()
    {
        StartCoroutine(LevelTransition());
    }

    private IEnumerator LevelTransition()
    {
        yield return new WaitForSeconds(delayBetweenLevels);

        if (currentTarget != null)
            Destroy(currentTarget);

        currentLevel++;
        
        PlayerPrefs.SetInt(LEVEL_KEY, currentLevel);
        PlayerPrefs.Save();

        SpawnLevel();
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            bool isBoss = (currentLevel % bossEveryNLevels == 0) && bossPrefabs != null && bossPrefabs.Length > 0;
            if (isBoss)
                levelText.text = "BOSS!";
            else
                levelText.text = "LEVEL " + currentLevel;
        }
    }
}
