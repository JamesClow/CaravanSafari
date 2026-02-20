using UnityEngine;

/// <summary>
/// Coin that is attracted to the hero when in magnet radius, then adds to economy on collect.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    [Header("Value")]
    [Tooltip("Coins added to economy when collected.")]
    public int value = 1;

    [Header("Magnet (uses HeroController.magnetRadius)")]
    [Tooltip("Acceleration towards hero when in magnet range.")]
    public float magnetAcceleration = 25f;
    [Tooltip("Max speed when being pulled.")]
    public float magnetMaxSpeed = 15f;

    const float CollectDistanceSq = 0.64f; // 0.8f * 0.8f for collect distance

    HeroController _hero;
    float _magnetSpeed;
    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        FindHero();
    }

    void FindHero()
    {
        if (_hero != null) return;
        _hero = FindAnyObjectByType<HeroController>();
    }

    void FixedUpdate()
    {
        FindHero();
        if (_hero == null) return;

        Vector3 toHero = _hero.transform.position - transform.position;
        toHero.y = 0f; // keep movement horizontal so coins don't fly up/down
        float distSq = toHero.sqrMagnitude;
        float magnetRadius = _hero.GetMagnetRadius();
        float magnetRadiusSq = magnetRadius * magnetRadius;

        // Collect when very close to hero
        if (distSq < CollectDistanceSq)
        {
            if (Economy.Instance != null)
                Economy.Instance.AddCoins(value);
            Destroy(gameObject);
            return;
        }

        // In magnet range: accelerate towards hero
        if (distSq < magnetRadiusSq && distSq > 0.0001f)
        {
            _magnetSpeed += magnetAcceleration * Time.fixedDeltaTime;
            _magnetSpeed = Mathf.Min(_magnetSpeed, magnetMaxSpeed);

            Vector3 direction = toHero.normalized;
            Vector3 desiredVelocity = direction * _magnetSpeed;

            if (_rb != null && !_rb.isKinematic)
            {
                _rb.linearVelocity = new Vector3(desiredVelocity.x, _rb.linearVelocity.y, desiredVelocity.z);
            }
            else
            {
                transform.position += desiredVelocity * Time.fixedDeltaTime;
            }
        }
        else
        {
            _magnetSpeed = 0f;
        }
    }
}
