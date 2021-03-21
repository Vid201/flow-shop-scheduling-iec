using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject blockPrefab;

    struct Job {
        public int jobId, machineId;
        public float width;
        public Color color;

        public Job(int jobId, int machineId, float width, Color color) {
            this.jobId = jobId;
            this.machineId = machineId;
            this.width = width;
            this.color = color;
        }
    };

    private int numberOfMachines, numberOfJobs;
    private List<List<Job>> jobs = new List<List<Job>>();

    void SpawnBlocks() {
        Camera camera = Camera.main;
        float height = 2f * camera.orthographicSize;
        float width = height * camera.aspect;

        for (int i = 0; i < numberOfJobs; ++i) {
            for (int j = 0; j < numberOfMachines; ++j) {
                var position = new Vector3(width / numberOfJobs * i - width / numberOfJobs, (-height / 2 + (height / 2) / numberOfMachines) + (height / 2) / numberOfMachines * j, 0);
                var gameObject = GameObject.Instantiate(blockPrefab, position, Quaternion.identity);
                gameObject.GetComponent<SpriteRenderer>().color = jobs[i][j].color;
            }
        }
    }

    void Start()
    {
        // TODO: generate new random game
        numberOfMachines = 3;
        numberOfJobs = 3;

        for (int i = 0; i < numberOfJobs; ++i)
        {
            var currentJobs = new List<Job>();
            var currentColor = GenerateColor();

            for (int j = 0; j < numberOfMachines; ++j) {
                currentJobs.Add(new Job(i, j, GenerateWidth(), currentColor));
            }

            jobs.Add(currentJobs);
        }

        SpawnBlocks();
    }

    float GenerateWidth() {
        return Random.Range(3f, 6f);
    }

    Color GenerateColor() {
        return new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );
    }

    void Update()
    {
        
    }
}
