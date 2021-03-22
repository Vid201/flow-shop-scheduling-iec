using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    struct Location {
        public float x1, x2;

        public Location(float x1, float x2) {
            this.x1 = x1;
            this.x2 = x2;
        }
    };

    public int machineId;
    private static Dictionary<int, Dictionary<int, Location>> activeJobs = new Dictionary<int, Dictionary<int, Location>>();

    void Start()
    {
        activeJobs[machineId] = new Dictionary<int, Location>();
    }

    public bool AddJob(int jobId, float x1, float x2) {
        bool free = true;

        foreach (KeyValuePair<int, Location> entry in activeJobs[machineId])
        {
            if (entry.Key == jobId)
            {
                continue;
            }

            if ((x2 > entry.Value.x1 && x2 < entry.Value.x2) || (x1 < entry.Value.x2 && x1 > entry.Value.x1))
            {
                free = false;
                break;
            }
        }

        if (!free || CheckOtherMachines(jobId, x1, x2))
        {
            return false;
        }


        if (activeJobs[machineId].ContainsKey(jobId))
        {
            RemoveJob(jobId);
        }
        activeJobs[machineId].Add(jobId, new Location(x1, x2));

        return true;
    }

    public void RemoveJob(int jobId) {
        if (activeJobs[machineId].ContainsKey(jobId))
        {
            activeJobs[machineId].Remove(jobId);
        }
    }

    bool CheckOtherMachines(int jobId, float x1, float x2) {
        foreach (KeyValuePair<int, Dictionary<int, Location>> entry in activeJobs) {
            if (entry.Key == machineId)
            {
                continue;
            }

            foreach (KeyValuePair<int, Location> entry2 in entry.Value)
            {
                if (entry2.Key == jobId)
                {
                    if ((entry.Key < machineId && entry2.Value.x2 > x1) || (entry.Key > machineId && entry2.Value.x1 < x2)) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static List<float> GetSuggestions(int machineId, int jobId) {
        List<Location> suggestions = new List<Location>();
        float lowerBound = GameHandler.BoardMinX, upperBound = GameHandler.BoardMaxX;

        for (int i = machineId - 1; i >= 0; --i)
        {
            if (activeJobs[i].ContainsKey(jobId)) {
                lowerBound = activeJobs[i][jobId].x2;
                break;
            }
        }

        for (int i = machineId + 1; i < activeJobs.Count; ++i)
        {
            if (activeJobs[i].ContainsKey(jobId))
            {
                upperBound = activeJobs[i][jobId].x1;
                break;
            }
        }

        List<float> values = new List<float>();
        values.Add(lowerBound);
        values.Add(upperBound);

        foreach (KeyValuePair<int, Location> entry in activeJobs[machineId]) {
            values.Add(entry.Value.x1);
            values.Add(entry.Value.x2);
        }

        values.Sort();

        for (int i = 0; i < values.Count; i += 2) {
            suggestions.Add(new Location(values[i], values[i+1]));
        }

        return values;
    }
}
