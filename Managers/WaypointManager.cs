using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoint Transforms

    // Get all waypoint names as strings
    public string[] GetWaypointNames()
    {
        List<string> names = new List<string>();
        foreach (var waypoint in waypoints)
        {
            names.Add(waypoint.name);
        }
        return names.ToArray();
    }

    // Retrieve a waypoint Transform by its name
    public Transform GetWaypointTransformByName(string name)
    {
        foreach (var waypoint in waypoints)
        {
            if (waypoint.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                return waypoint;
            }
        }
        return null;
    }
}
