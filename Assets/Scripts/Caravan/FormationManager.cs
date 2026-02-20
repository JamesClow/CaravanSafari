using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assigns each CaravanFollower a unique slot (distance + lateral offset) so units form a line.
/// Slots are held when a unit enters Engage (paused follower); freed only on Unregister (death or leave).
/// </summary>
public class FormationManager : MonoBehaviour
{
    [System.Serializable]
    public struct Slot
    {
        [Tooltip("Distance along spline from caravan core. Negative = behind, positive = ahead.")]
        public float distanceOffset;
        [Tooltip("Perpendicular offset from path centerline. Positive = right, negative = left.")]
        public float lateralOffset;

        public Slot(float distance, float lateral)
        {
            distanceOffset = distance;
            lateralOffset = lateral;
        }
    }

    [Header("Formation")]
    [Tooltip("Pre-defined slots. If empty, slots are generated dynamically (staggered column behind core).")]
    public Slot[] predefinedSlots = new Slot[0];

    [Header("Dynamic generation (used when predefinedSlots is empty)")]
    [Tooltip("Distance between rows (along path).")]
    public float rowSpacing = 3f;
    [Tooltip("Lateral offset per slot (stagger left/right).")]
    public float lateralStagger = 0.5f;

    private readonly List<Slot> _availableSlots = new List<Slot>();
    private readonly Dictionary<CaravanFollower, Slot> _assignments = new Dictionary<CaravanFollower, Slot>();
    private int _dynamicSlotIndex;

    void Awake()
    {
        if (predefinedSlots != null && predefinedSlots.Length > 0)
        {
            foreach (Slot s in predefinedSlots)
                _availableSlots.Add(s);
        }
    }

    /// <summary>Register a follower and assign the next available slot. Returns the assigned slot.</summary>
    public Slot Register(CaravanFollower follower)
    {
        if (follower == null) return new Slot(0f, 0f);
        if (_assignments.TryGetValue(follower, out Slot existing))
            return existing;

        Slot slot = GetNextSlot();
        _assignments[follower] = slot;
        return slot;
    }

    /// <summary>Unregister a follower (e.g. on death). Frees the slot for reuse.</summary>
    public void Unregister(CaravanFollower follower)
    {
        if (follower == null) return;
        if (_assignments.TryGetValue(follower, out Slot slot))
        {
            _assignments.Remove(follower);
            _availableSlots.Add(slot);
        }
    }

    /// <summary>Get the slot currently assigned to this follower, if any.</summary>
    public bool TryGetSlot(CaravanFollower follower, out Slot slot)
    {
        return _assignments.TryGetValue(follower, out slot);
    }

    private Slot GetNextSlot()
    {
        if (_availableSlots.Count > 0)
        {
            Slot s = _availableSlots[0];
            _availableSlots.RemoveAt(0);
            return s;
        }
        return GenerateDynamicSlot();
    }

    private Slot GenerateDynamicSlot()
    {
        int row = _dynamicSlotIndex / 2;
        float lateral = (_dynamicSlotIndex % 2 == 0) ? lateralStagger : -lateralStagger;
        _dynamicSlotIndex++;
        return new Slot(-rowSpacing * (row + 1), lateral);
    }
}
