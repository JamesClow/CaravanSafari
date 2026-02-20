using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Optional: time-slices updates across all registered UnitAI instances (enemy and friendly)
/// so that only a subset run evaluation and repath each frame. Reduces CPU spikes with many units.
/// UnitAI can register/unregister itself, or register automatically if this manager exists in the scene.
/// </summary>
public class UnitAIManager : MonoBehaviour
{
    [Tooltip("Max number of UnitAI instances to update (evaluation + repath) per frame. Rest still move via NavMeshAgent.")]
    public int updatesPerFrame = 30;

    private readonly List<UnitAI> _units = new List<UnitAI>();
    private int _sliceStart;

    public static UnitAIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (_units.Count > 0)
            _sliceStart = (_sliceStart + updatesPerFrame) % _units.Count;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
        _units.Clear();
    }

    public void Register(UnitAI unit)
    {
        if (unit != null && !_units.Contains(unit))
            _units.Add(unit);
    }

    public void Unregister(UnitAI unit)
    {
        _units.Remove(unit);
    }

    /// <summary>
    /// Returns true if this unit should run evaluation and repath this frame. Called by UnitAI each Update.
    /// </summary>
    public bool ShouldUpdateThisFrame(UnitAI unit)
    {
        if (_units.Count == 0) return true;
        int idx = _units.IndexOf(unit);
        if (idx < 0) return true;
        int n = _units.Count;
        int step = (idx - _sliceStart + n) % n;
        return step < updatesPerFrame;
    }
}
