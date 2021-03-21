using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    struct Location {
        float x1, x2;

        public Location(float x1, float x2) {
            this.x1 = x1;
            this.x2 = x2;
        }
    };

    public int machineId;
    private Dictionary<int, Location> activeJobs = new Dictionary<int, Location>();

    public void AddJob(int jobId, float x1, float x2) {
        if (activeJobs.ContainsKey(jobId))
        {
            RemoveJob(jobId);
        }
        activeJobs.Add(jobId, new Location(x1, x2));
    }

    public void RemoveJob(int jobId) {
        if (activeJobs.ContainsKey(jobId))
        {
            activeJobs.Remove(jobId);
        }
    }
}
