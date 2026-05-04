using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Knife : MonoBehaviour
{
    [Header("Movement")]
    public float throwForce = 25f;
    public float bounceForce = 10f;
    public float bounceTorque = 50f;

    [Header("Collision")]
    public float minAngleBetweenKnives = 10f;

    [Header("Hit Effects")]
    public ParticleSystem hitParticlePrefab;
    public Color woodHitColor = new Color(0.8f, 0.6f, 0.4f);

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip failSound;

    private Rigidbody2D rb;
    private Collider2D bladeCollider;
    private AudioSource audioSource;
    private bool isThrown = false;
    private bool isStuck = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bladeCollider = GetComponent<Collider2D>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        bool isPressed = Pointer.current != null && Pointer.current.press.wasPressedThisFrame;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (isPressed && !isThrown && !isStuck)
        {
            Throw();
        }
    }

    void Throw()
    {
        isThrown = true;
        rb.linearVelocity = new Vector2(0, throwForce);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStuck) return;
        if (!isThrown) return;

        if (collision.CompareTag("Target"))
        {
            Transform targetTransform = collision.transform;

            Vector2 localKnifePos = targetTransform.InverseTransformPoint(transform.position);
            float knifeAngle = Mathf.Atan2(localKnifePos.y, localKnifePos.x) * Mathf.Rad2Deg;

            foreach (Transform child in targetTransform)
            {
                if (child == transform) continue;
                if (child.GetComponent<Knife>() == null) continue;

                Vector2 childLocalPos = child.localPosition;
                float childAngle = Mathf.Atan2(childLocalPos.y, childLocalPos.x) * Mathf.Rad2Deg;

                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(knifeAngle, childAngle));

                if (angleDiff < minAngleBetweenKnives)
                {
                    isStuck = true;
                    BounceOff();
                    GameManager.Instance.GameOver();
                    return;
                }
            }

            isStuck = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.SetParent(targetTransform);

            bladeCollider.enabled = false;

            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            if (hitParticlePrefab != null)
            {
                ParticleSystem part = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
                var main = part.main;
                main.startColor = woodHitColor;
            }

            GameManager.Instance.AddPoints(1);

            KnifeSpawner.Instance.OnKnifeStuck();

            Target target = collision.GetComponent<Target>();
            if (target != null)
                target.OnHit();
        }

        else if (collision.CompareTag("Apple"))
        {
            GameManager.Instance.AddPoints(3);
            collision.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
        }
    }

    void BounceOff()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = new Vector2(Random.Range(-3f, 3f), -bounceForce);
        rb.AddTorque(Random.Range(-bounceTorque, bounceTorque));
        rb.gravityScale = 2f;

        bladeCollider.enabled = false;
        
        if (failSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(failSound);
        }

        this.enabled = false;
    }
}
