using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tag-based utility for team rules: which GameObjects are hostile to which.
/// Uses only GameObject.tag; no component or behavior. "Enemy" = Enemy team; Hero, Tower, HomeBase, etc. = Friendly.
/// Use anywhere you need to check hostility or same-team (targeting, damage, etc.).
/// </summary>
public static class Team
{
    public enum TeamType { Enemy, Friendly }

    /// <summary>
    /// Tags considered part of the Friendly team. Used by GetGameObjectsForTeam(Friendly).
    /// </summary>
    public static readonly string[] FriendlyTags = { "Hero", "Tower", "HomeBase" };

    /// <summary>
    /// Team for a given tag. "Enemy" -> Enemy; anything else -> Friendly.
    /// </summary>
    public static TeamType GetTeamFromTag(string tag)
    {
        if (tag == "Enemy") return TeamType.Enemy;
        return TeamType.Friendly;
    }

    /// <summary>
    /// True if the two tags are considered the same team (no friendly fire).
    /// </summary>
    public static bool IsSameTeam(string tagA, string tagB)
    {
        return GetTeamFromTag(tagA) == GetTeamFromTag(tagB);
    }

    /// <summary>
    /// True if the two tags are hostile (valid to target / damage).
    /// </summary>
    public static bool AreHostile(string tagA, string tagB)
    {
        return GetTeamFromTag(tagA) != GetTeamFromTag(tagB);
    }

    /// <summary>
    /// Convenience: same-team check using two GameObjects' tags.
    /// </summary>
    public static bool IsSameTeam(GameObject a, GameObject b)
    {
        return a != null && b != null && IsSameTeam(a.tag, b.tag);
    }

    /// <summary>
    /// Convenience: hostile check using two GameObjects' tags.
    /// </summary>
    public static bool AreHostile(GameObject a, GameObject b)
    {
        return a != null && b != null && AreHostile(a.tag, b.tag);
    }

    /// <summary>
    /// Returns the team that is hostile to the given team.
    /// </summary>
    public static TeamType GetHostileTeam(TeamType team)
    {
        return team == TeamType.Enemy ? TeamType.Friendly : TeamType.Enemy;
    }

    /// <summary>
    /// Fetches all active GameObjects belonging to the given team.
    /// Enemy = objects with tag "Enemy"; Friendly = objects with any tag in FriendlyTags.
    /// </summary>
    public static List<GameObject> GetGameObjectsForTeam(TeamType team)
    {
        var list = new List<GameObject>();
        if (team == TeamType.Enemy)
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject go in found)
                if (go != null && go.activeInHierarchy)
                    list.Add(go);
            return list;
        }
        foreach (string tag in FriendlyTags)
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject go in found)
                if (go != null && go.activeInHierarchy && !list.Contains(go))
                    list.Add(go);
        }
        return list;
    }
}
