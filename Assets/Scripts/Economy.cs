using UnityEngine;

/// <summary>
/// Economy: tracks coins. Coins are added when the hero picks them up.
/// Add this component to a GameObject in your scene (e.g. a GameManager).
/// </summary>
public class Economy : MonoBehaviour
{
    public static Economy Instance { get; private set; }

    [SerializeField] int _coins;

    /// <summary>Current total coins.</summary>
    public int Coins => _coins;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>Add coins (e.g. when hero picks up a coin).</summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        _coins += amount;
    }

    /// <summary>Spend coins if enough are available. Returns true if spent.</summary>
    public bool TrySpend(int amount)
    {
        if (amount <= 0 || _coins < amount) return false;
        _coins -= amount;
        return true;
    }

    /// <summary>Reset coins (e.g. for new game).</summary>
    public void ResetCoins()
    {
        _coins = 0;
    }
}
