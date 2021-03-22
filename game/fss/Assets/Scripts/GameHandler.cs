using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject blockPrefab;

    [SerializeField]
    private GameObject boardPrefab;

    private float height, width;

    public static float BoardMinX, BoardMaxX;
    public static Dictionary<int, float> BoardPositions = new Dictionary<int, float>();

    struct Job {
        public int jobId, machineId;
        public Vector3 scale;
        public Color color;

        public Job(int jobId, int machineId, Vector3 scale, Color color) {
            this.jobId = jobId;
            this.machineId = machineId;
            this.scale = scale;
            this.color = color;
        }
    };

    private int numberOfMachines, numberOfJobs;
    private List<List<Job>> jobs = new List<List<Job>>();

    void SpawnBoard() {
        for (int i = 0; i < numberOfMachines; ++i)
        {
            var positionY = height / 2 - (height / 2) / (numberOfMachines + 1) * (i + 1);
            var position = new Vector3(0, positionY, 0);
            var gameObject = GameObject.Instantiate(boardPrefab, position, Quaternion.identity);
            gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            gameObject.GetComponent<Board>().machineId = i;

            if (i == 0)
            {
                BoardMinX = gameObject.GetComponent<SpriteRenderer>().bounds.min.x;
                BoardMaxX = gameObject.GetComponent<SpriteRenderer>().bounds.max.x;
            }

            BoardPositions.Add(i, positionY);
        }
    }

    void SpawnBlocks() {
        for (int i = 0; i < numberOfJobs; ++i) {
            for (int j = 0; j < numberOfMachines; ++j) {
                var position = new Vector3(-width / 2 + width / (numberOfJobs + 1) * (i + 1), 0 - (height / 2) / (numberOfMachines + 1) * (j + 1), 0);
                var gameObject = GameObject.Instantiate(blockPrefab, position, Quaternion.identity);
                gameObject.GetComponent<SpriteRenderer>().color = jobs[i][j].color;
                gameObject.GetComponent<Transform>().localScale = Vector3.Scale(jobs[i][j].scale, gameObject.GetComponent<Transform>().localScale);
                gameObject.GetComponent<Block>().machineId = jobs[i][j].machineId;
                gameObject.GetComponent<Block>().jobId = jobs[i][j].jobId;
            }
        }
    }

    void Start()
    {
        Camera camera = Camera.main;
        height = 2f * camera.orthographicSize;
        width = height * camera.aspect;

        // TODO: generate new random game
        numberOfMachines = 3;
        numberOfJobs = 5;

        for (int i = 0; i < numberOfJobs; ++i)
        {
            var currentJobs = new List<Job>();
            var currentColor = GenerateColor();

            for (int j = 0; j < numberOfMachines; ++j) {
                currentJobs.Add(new Job(i, j, GenerateScale(), currentColor));
            }

            jobs.Add(currentJobs);
        }

        SpawnBoard();
        SpawnBlocks();
    }

    Vector3 GenerateScale() {
        return new Vector3(
            Random.Range(0.2f, 1.0f),
            1.0f,
            1.0f
        );
    }

    Color GenerateColor() {
        return new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );
    }
}
