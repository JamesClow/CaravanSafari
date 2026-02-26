using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The caravan core: singleton that drives the train along the spline and positions cars.
/// This transform is the engine (front of the train). Tag this GameObject "HomeBase" so enemies advance toward it.
/// Units (CaravanFollower) are assigned to CaravanCars and pathfind toward each car's nav point.
/// </summary>
public class CaravanCore : SingletonMonoBehaviour<CaravanCore>
{
    [Header("Route")]
    [Tooltip("Spline path. Assign a GameObject with SplineRoute component.")]
    public SplineRoute splineRoute;

    [Header("Movement")]
    [Tooltip("Speed along the spline in world units per second.")]
    public float speed = 5f;
    [Tooltip("When false, the train stops (e.g. under heavy attack or at a waypoint).")]
    public bool isMoving = true;

    [Header("Cars")]
    [Tooltip("World-space distance between consecutive cars (and engine to first car). Cars snap to the spline at this spacing by slot index.")]
    public float carSpacing = 3f;
    [Tooltip("Assign caravan cars here. If empty, cars are auto-discovered from child GameObjects with CaravanCar.")]
    public List<CaravanCar> cars = new List<CaravanCar>();

    [SerializeField]
    [Range(0f, 1f)]
    private float currentT = 0f;

    private readonly List<CaravanCar> _cars = new List<CaravanCar>();

    /// <summary>Current normalized position (0..1) along the spline.</summary>
    public float CurrentT => currentT;
    /// <summary>World position of the engine (front of the train).</summary>
    public Vector3 EnginePosition => splineRoute != null ? splineRoute.GetPosition(currentT) : transform.position;
    /// <summary>Read-only list of caravan cars (discovered from child CaravanCar components in the scene).</summary>
    public IReadOnlyList<CaravanCar> Cars => _cars;
    /// <summary>Current movement speed in world units per second.</summary>
    public float Speed => speed;
    /// <summary>Whether the train is advancing along the spline.</summary>
    public bool IsMoving => isMoving;

    void Start()
    {
        if (splineRoute != null)
            currentT = splineRoute.GetNearestT(transform.position);

        _cars.Clear();
        if (cars != null && cars.Count > 0)
        {
            foreach (CaravanCar car in cars)
            {
                if (car != null)
                    _cars.Add(car);
            }
        }
        else
        {
            var existing = GetComponentsInChildren<CaravanCar>(true);
            foreach (CaravanCar car in existing)
            {
                if (car != null && car.transform != transform)
                    _cars.Add(car);
            }
        }
    }

    void Update()
    {
        if (splineRoute == null) return;

        float length = splineRoute.GetSplineLength();
        if (length <= 0f) return;

        if (isMoving)
        {
            float tDelta = splineRoute.DistanceToT(speed * Time.deltaTime);
            currentT = Mathf.Clamp01(currentT + tDelta);
        }

        // Engine (this transform) at currentT; cars snap to spline by slot index (negative = ahead, positive = behind).
        transform.position = splineRoute.GetPosition(currentT);
        Vector3 forward = splineRoute.GetDirection(currentT);
        if (forward.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        for (int i = 0; i < _cars.Count; i++)
        {
            CaravanCar car = _cars[i];
            if (car == null) continue;
            float offset = -car.SlotIndex * carSpacing;
            float carT = splineRoute.GetTAtDistanceOffset(currentT, offset);
            car.transform.position = splineRoute.GetPosition(carT);
            Vector3 carForward = splineRoute.GetDirection(carT);
            if (carForward.sqrMagnitude > 0.0001f)
                car.transform.rotation = Quaternion.LookRotation(carForward, Vector3.up);
        }
    }

    /// <summary>Add a new caravan car behind the engine. Uses the next positive slot index. Returns the new car.</summary>
    public CaravanCar AddCar()
    {
        int nextSlot = GetNextBehindSlotIndex();
        return AddCar(nextSlot);
    }

    /// <summary>Add a caravan car at a specific slot index (negative = ahead of engine, positive = behind). Returns the new car.</summary>
    public CaravanCar AddCar(int slotIndex)
    {
        var go = new GameObject("CaravanCar");
        go.transform.SetParent(transform);
        var car = go.AddComponent<CaravanCar>();
        car.SlotIndex = slotIndex;
        _cars.Add(car);
        return car;
    }

    private int GetNextBehindSlotIndex()
    {
        int max = 0;
        for (int i = 0; i < _cars.Count; i++)
        {
            if (_cars[i] != null && _cars[i].SlotIndex > max)
                max = _cars[i].SlotIndex;
        }
        return max + 1;
    }

    public void PauseMovement() => isMoving = false;
    public void ResumeMovement() => isMoving = true;
}
