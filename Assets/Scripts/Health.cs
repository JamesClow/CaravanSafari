using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float healthMax = 10f;
    [HideInInspector]
    public float healthCurrent; // Always initialized to healthMax - hidden from inspector to prevent manual override

    [Header("Health Bar Display")]
    [Tooltip("Show health bar above this entity")]
    public bool showHealthBar = true;

    [Tooltip("Offset above the entity (in world units)")]
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    [Tooltip("Size of the health bar (width x height) - shared by all health bars")]
    public static Vector2 healthBarSize = new Vector2(1f, 0.15f);

    [Tooltip("Show health bar even when at full health")]
    public bool alwaysVisible = true;

    [Tooltip("Hide health bar when entity is at full health")]
    public bool hideAtFullHealth = false;

    [Header("Health Bar Colors")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    [Tooltip("Health percentage at which to show low health color (0-1)")]
    public float lowHealthThreshold = 0.3f;

    /// <summary>
    /// Fired when this entity dies. Passes this GameObject so listeners can identify who died.
    /// </summary>
    public event Action<GameObject> OnDeath;

    // Health bar UI elements
    private Canvas canvas;
    private GameObject healthBarContainer;
    private Image backgroundImage;  // Dark outline/border
    private Image redBackgroundImage; // Red background showing missing health
    private Image fillImage;        // Green fill showing current health
    private Camera mainCamera;

    void Awake()
    {
        // Always ensure health starts at max
        healthCurrent = healthMax;
    }

    void Start()
    {
        // Double-check in case healthMax was changed in inspector after Awake
        healthCurrent = healthMax;

        if (showHealthBar)
        {
            InitializeHealthBar();
        }
    }

    void OnValidate()
    {
        // In editor, ensure healthCurrent matches healthMax when healthMax changes
        if (Application.isPlaying == false)
        {
            healthCurrent = healthMax;
        }
    }

    void LateUpdate()
    {
        if (showHealthBar && healthBarContainer != null)
        {
            UpdateHealthBarPosition();
            UpdateHealthBarVisuals();
        }
    }

    public void TakeDamage(float dmg)
    {
        if (healthCurrent <= 0f) return; // Already dead

        healthCurrent -= dmg;

        if (healthCurrent <= 0f)
        {
            healthCurrent = 0f;
            Die();
        }
    }

    public void Heal(float amount)
    {
        healthCurrent = Mathf.Min(healthCurrent + amount, healthMax);
    }

    public float GetHealthPercent()
    {
        return healthMax > 0 ? healthCurrent / healthMax : 0f;
    }

    public bool IsDead()
    {
        return healthCurrent <= 0f;
    }

    private void Die()
    {
        // Fire death event so WaveManager, score systems, etc. can react
        OnDeath?.Invoke(gameObject);

        // Play death sound (optional — won't crash if AudioManager doesn't exist)
        AudioManager audioManager = AudioManager.getInstance();
        if (audioManager != null)
        {
            audioManager.Play("Chime");
        }

        Destroy(gameObject);
    }

    // --- Health Bar Methods ---

    private void InitializeHealthBar()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindAnyObjectByType<Camera>();
        }

        if (mainCamera == null)
        {
            Debug.LogWarning($"[Health] No camera found for health bar on '{gameObject.name}'. Health bar will not be created.");
            return;
        }

        // Create world-space canvas
        GameObject canvasObj = new GameObject("HealthBarCanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = mainCamera;

        // Set canvas size and position
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            canvasRect = canvasObj.AddComponent<RectTransform>();
        }
        canvasRect.sizeDelta = new Vector2(100, 100); // Canvas size in world units
        canvasRect.localScale = Vector3.one;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 100;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create container for the health bar
        healthBarContainer = new GameObject("HealthBar");
        healthBarContainer.transform.SetParent(canvas.transform, false);
        RectTransform containerRect = healthBarContainer.AddComponent<RectTransform>();
        containerRect.sizeDelta = healthBarSize;
        containerRect.localScale = Vector3.one;
        containerRect.localPosition = Vector3.zero;

        // Create dark background (outline/border)
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(healthBarContainer.transform, false);
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Dark gray with transparency
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;

        // Create red background (shows missing health - always full size)
        GameObject redBgObj = new GameObject("RedBackground");
        redBgObj.transform.SetParent(healthBarContainer.transform, false);
        redBackgroundImage = redBgObj.AddComponent<Image>();
        redBackgroundImage.color = lowHealthColor; // Red
        RectTransform redBgRect = redBgObj.GetComponent<RectTransform>();
        redBgRect.anchorMin = Vector2.zero;
        redBgRect.anchorMax = Vector2.one;
        redBgRect.sizeDelta = Vector2.zero;
        redBgRect.anchoredPosition = Vector2.zero;

        // Create green fill (shows current health - shrinks horizontally as health decreases)
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(healthBarContainer.transform, false);
        fillImage = fillObj.AddComponent<Image>();
        fillImage.color = fullHealthColor; // Green
        fillImage.type = Image.Type.Simple;
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
    }

    private void UpdateHealthBarPosition()
    {
        if (mainCamera == null || canvas == null) return;

        // Position canvas above entity
        canvas.transform.position = transform.position + healthBarOffset;

        // Face the camera
        canvas.transform.LookAt(mainCamera.transform);
        canvas.transform.Rotate(0, 180, 0); // Flip to face camera correctly
    }

    private void UpdateHealthBarVisuals()
    {
        if (fillImage == null || healthBarContainer == null) return;

        float healthPercent = GetHealthPercent();
        bool shouldShow = alwaysVisible || !hideAtFullHealth || healthPercent < 1f;

        // Show/hide based on settings
        if (healthBarContainer.activeSelf != shouldShow)
        {
            healthBarContainer.SetActive(shouldShow);
        }

        if (!shouldShow) return;

        // Shrink the green fill horizontally by moving its right anchor (0 = empty, 1 = full)
        RectTransform fillRect = fillImage.rectTransform;
        float clamped = Mathf.Clamp01(healthPercent);
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(clamped, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        fillImage.color = fullHealthColor;
    }

    void OnDestroy()
    {
        // Clean up UI elements
        if (healthBarContainer != null)
        {
            Destroy(healthBarContainer);
        }
    }
}
