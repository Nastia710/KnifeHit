using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Target : MonoBehaviour
{
    [Header("Base Speed")]
    public float minSpeed = 100f;
    public float maxSpeed = 250f;

    [Header("Direction Change")]
    public float minDirectionTime = 3.0f;
    public float maxDirectionTime = 6.0f;

    [Header("Hit Effects")]
    public Color flashColor = Color.white;
    public float flashDuration = 1.0f;
    public float bumpUpDistance = 0.15f;
    public float bumpDuration = 0.08f;

    [Header("Apple Spawning")]
    public GameObject applePrefab;
    public float appleSpawnRadius = 1.8f;
    public int minApples = 0;
    public int maxApples = 5;
    public float minAngleBetweenItems = 30f;

    [Header("Pre-Stuck Knife Spawning (Bosses)")]
    public GameObject stuckKnifePrefab;     
    public float knifeSpawnRadius = 3.2f;    
    public int minPreStuckKnives = 0;        
    public int maxPreStuckKnives = 0;        

    private float currentSpeed;
    private float targetSpeed;
    private float direction = 1f;
    private float directionTimer;
    private float acceleration = 80f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalLocalPos;
    private List<float> occupiedAngles = new List<float>();

    void Start()
    {
        direction = Random.value > 0.5f ? 1f : -1f;
        currentSpeed = minSpeed;
        SetNewTargetSpeed();
        SetNewDirectionTimer();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        originalLocalPos = transform.localPosition;

        int appleCount = Random.Range(minApples, maxApples + 1);
        SpawnApples(appleCount);

        int knifeCount = Random.Range(minPreStuckKnives, maxPreStuckKnives + 1);
        SpawnPreStuckKnives(knifeCount);
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            direction *= -1f;
            SetNewTargetSpeed();
            SetNewDirectionTimer();
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        transform.Rotate(0f, 0f, currentSpeed * direction * Time.deltaTime);
    }

    private void SetNewTargetSpeed()
    {
        targetSpeed = Random.Range(minSpeed, maxSpeed);
    }

    private void SetNewDirectionTimer()
    {
        directionTimer = Random.Range(minDirectionTime, maxDirectionTime);
    }

    public void SpawnApples(int count)
    {
        if (applePrefab == null || count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            float angle = FindFreeAngle();
            if (angle < 0f) continue;

            occupiedAngles.Add(angle);

            float rad = angle * Mathf.Deg2Rad;
            Vector3 localPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * appleSpawnRadius;

            GameObject apple = Instantiate(applePrefab, transform);
            apple.transform.localPosition = localPos;
            apple.transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    public void SpawnPreStuckKnives(int count)
    {
        if (stuckKnifePrefab == null || count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            float angle = FindFreeAngle();
            if (angle < 0f) continue;

            occupiedAngles.Add(angle);

            float rad = angle * Mathf.Deg2Rad;
            Vector3 localPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * knifeSpawnRadius;

            GameObject knife = Instantiate(stuckKnifePrefab, transform);
            knife.transform.localPosition = localPos;
            
            knife.transform.localRotation = Quaternion.Euler(0, 0, angle + 90f);

            knife.transform.localScale = new Vector3(
                stuckKnifePrefab.transform.localScale.x / transform.localScale.x,
                stuckKnifePrefab.transform.localScale.y / transform.localScale.y,
                1f
            );

            Knife knifeScript = knife.GetComponent<Knife>();
            if (knifeScript != null)
                knifeScript.enabled = false;

            Rigidbody2D rb = knife.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.bodyType = RigidbodyType2D.Kinematic;

            Collider2D col = knife.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }

    private float FindFreeAngle()
    {
        for (int attempt = 0; attempt < 30; attempt++)
        {
            float candidateAngle = Random.Range(0f, 360f);
            if (IsAngleFree(candidateAngle))
                return candidateAngle;
        }
        return -1f;
    }

    private bool IsAngleFree(float candidateAngle)
    {
        foreach (float occupied in occupiedAngles)
        {
            if (Mathf.Abs(Mathf.DeltaAngle(candidateAngle, occupied)) < minAngleBetweenItems)
                return false;
        }
        return true;
    }

    public void AddOccupiedAngle(float angle)
    {
        occupiedAngles.Add(angle);
    }

    public void OnHit()
    {
        StopAllCoroutines();
        StartCoroutine(BumpCoroutine());
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator BumpCoroutine()
    {
        float elapsed = 0f;
        Vector3 targetPos = originalLocalPos + new Vector3(0, bumpUpDistance, 0);

        while (elapsed < bumpDuration / 2f)
        {
            transform.localPosition = Vector3.Lerp(originalLocalPos, targetPos, elapsed / (bumpDuration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < bumpDuration / 2f)
        {
            transform.localPosition = Vector3.Lerp(targetPos, originalLocalPos, elapsed / (bumpDuration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalLocalPos;
    }

    private IEnumerator FlashCoroutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    public void SetDifficulty(float newMinSpeed, float newMaxSpeed, float newMinTime, float newMaxTime)
    {
        minSpeed = newMinSpeed;
        maxSpeed = newMaxSpeed;
        minDirectionTime = newMinTime;
        maxDirectionTime = newMaxTime;
    }
}

