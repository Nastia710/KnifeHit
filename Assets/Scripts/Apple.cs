using UnityEngine;

public class Apple : MonoBehaviour
{
    [Header("Effects")]
    public GameObject applePart1Prefab;
    public GameObject applePart2Prefab;
    public ParticleSystem hitParticlePrefab;
    public Color appleHitColor = new Color(0.8f, 0.1f, 0.1f);

    public void Hit()
    {
        if (applePart1Prefab != null && applePart2Prefab != null)
        {
            GameObject part1 = Instantiate(applePart1Prefab, transform.position, transform.rotation);
            GameObject part2 = Instantiate(applePart2Prefab, transform.position, transform.rotation);

            Rigidbody2D rb1 = part1.GetComponent<Rigidbody2D>();
            Rigidbody2D rb2 = part2.GetComponent<Rigidbody2D>();

            if (rb1)
            {
                rb1.AddForce(new Vector2(-3f, 5f), ForceMode2D.Impulse);
                rb1.AddTorque(20f);
            }
            if (rb2)
            {
                rb2.AddForce(new Vector2(3f, 5f), ForceMode2D.Impulse);
                rb2.AddTorque(-20f);
            }
        }

        if (hitParticlePrefab != null)
        {
            ParticleSystem part = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
            var main = part.main;
            main.startColor = appleHitColor;
        }

        Destroy(gameObject);
    }
}
