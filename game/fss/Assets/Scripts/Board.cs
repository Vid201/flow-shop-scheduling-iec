using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    public Text noticeText, resultText, makespanText, gameoverText;
    public int allJobs;

    public static bool gameOver = false;

    void Start()
    {
        activeJobs[machineId] = new Dictionary<int, Location>();
        Validate();
    }

    public int GetJobIndex(int machineId, int jobId) {
        var myList = activeJobs[machineId].ToList();
        myList.Sort(
            delegate (KeyValuePair<int, Location> pair1,
            KeyValuePair<int, Location> pair2)
            {
                return pair1.Value.x1.CompareTo(pair2.Value.x1);
            }
        );
        for (int i = 0; i < myList.Count; ++i)
        {
            if (myList[i].Key == jobId) {
                return i;
            }
        }

        return -1;
    }

    public void Validate()
    {
        List<List<KeyValuePair<int, Location>>> jobsOrder = new List<List<KeyValuePair<int, Location>>>();
        bool correct = true;
        int counter = 0;
        float makespanMin = 100000.0f, makespanMax = -100000.0f;

        for (int i = 0; i < activeJobs.Count; i++)
        {
            var myList = activeJobs[i].ToList();
            myList.Sort(
                delegate (KeyValuePair<int, Location> pair1,
                KeyValuePair<int, Location> pair2)
                {
                    return pair1.Value.x1.CompareTo(pair2.Value.x1);
                }
            );
            jobsOrder.Add(myList);
            counter += myList.Count;

            if (myList.Count > 0 && myList[0].Value.x1 < makespanMin) {
                makespanMin = myList[0].Value.x1;
            }

            if (myList.Count > 0 && myList[myList.Count - 1].Value.x2 > makespanMax) {
                makespanMax = myList[myList.Count - 1].Value.x2;
            }
        }

        for (int j = 1; j < jobsOrder.Count; j++)
        {
            if (jobsOrder[j].Count != jobsOrder[0].Count)
            {
                correct = false;
                break;
            }

            for (int i = 0; i < jobsOrder[0].Count; i++)
            {
                if (jobsOrder[0][i].Key != jobsOrder[j][i].Key)
                {
                    correct = false;
                    break;
                }
            }

            if (!correct)
            {
                break;
            }
        }

        float diff = makespanMax - makespanMin;

        noticeText.color = correct ? Color.white : Color.red;
        resultText.text = "Result: " + counter.ToString() + "/" + allJobs.ToString();
        makespanText.text = "Makespan: " + (counter > 0 ? diff.ToString() : "0.0");

        if (counter == allJobs) {
            finishGame();
        }
    }

    void finishGame() {
        gameoverText.gameObject.SetActive(true);
        gameOver = true;
    }

    public bool AddJob(int jobId, float x1, float x2) {
        bool free = true;

        if (x1 < GameHandler.BoardMinX || x2 > GameHandler.BoardMaxX) {
            return false;
        }

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

        Validate();

        return true;
    }

    public bool RemoveJob(int jobId) {
        if (activeJobs[machineId].ContainsKey(jobId))
        {
            activeJobs[machineId].Remove(jobId);
            return true;
            Validate();
        }

        return false;
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

    public static List<float> GetSuggestions(int machineId, int jobId, float width) {
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

        foreach (KeyValuePair<int, Location> entry in activeJobs[machineId])
        {
            if (entry.Key != jobId) {
                values.Add(entry.Value.x1);
                values.Add(entry.Value.x2);
            }
        }

        values.Sort();

        List<float> suggestions = new List<float>();

        for (int i = 0; i < values.Count; i += 2) {
            if (width <= values[i + 1] - values[i]) {
                suggestions.Add(values[i]);
                suggestions.Add(values[i + 1]);
            }
        }

        return suggestions;
    }
}
