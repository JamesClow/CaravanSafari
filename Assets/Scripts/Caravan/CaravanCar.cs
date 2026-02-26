using UnityEngine;

/// <summary>
/// One car on the caravan train. Position and rotation are set by CaravanCore each frame based on SlotIndex.
/// Can be placed anywhere in the scene (e.g. as a child of CaravanCore); at runtime it snaps to the spline with correct spacing.
/// Exposes a nav point that CaravanFollower units pathfind toward.
/// </summary>
public class CaravanCar : MonoBehaviour
{
    [Tooltip("Slot along the train: negative = ahead of engine, positive = behind. E.g. -1 = first car ahead, 1 = first behind. Spacing is set by CaravanCore.")]
    [SerializeField]
    private int slotIndex = 1;

    /// <summary>Slot along the train: negative = ahead of engine, positive = behind. Set in inspector or when added at runtime.</summary>
    public int SlotIndex
    {
        get => slotIndex;
        set => slotIndex = value;
    }

    /// <summary>World position that units should seek. Updated by CaravanCore each frame.</summary>
    public Vector3 NavPoint => transform.position;

    /// <summary>Optional flag for your own use (e.g. to show a car is taken). Not used by CaravanFollower.</summary>
    public bool IsOccupied { get; set; }
}
