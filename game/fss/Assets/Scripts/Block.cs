using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private GameObject suggestionPrefab;

    private bool moving, onBoard;
    private Vector3 startPosition;
    public int machineId, jobId;
    public float width;
    public float machinePositionY;
    private Board board;
    private List<GameObject> suggestions = new List<GameObject>();

    public GameServer gameServer;

    void Start() {
        startPosition = transform.position;
        onBoard = false;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void ShowSuggestions(List<float> suggestionPositions) {
        for (int i = 0; i < suggestionPositions.Count; i += 2) {
            var position = new Vector3((suggestionPositions[i] + suggestionPositions[i + 1]) / 2, GameHandler.BoardPositions[machineId], 0);
            float ratio = (suggestionPositions[i+1] - suggestionPositions[i]) / (GameHandler.BoardMaxX - GameHandler.BoardMinX);
            var gameObject = GameObject.Instantiate(suggestionPrefab, position, Quaternion.identity);
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            gameObject.GetComponent<Transform>().localScale = Vector3.Scale(new Vector3(ratio, 1.0f, 1.0f), gameObject.GetComponent<Transform>().localScale);
            suggestions.Add(gameObject);
        }
    }

    public void OnMouseDown() {
        if (Board.gameOver)
        {
            return;
        }

        moving = true;
        List<float> suggestionPositions = Board.GetSuggestions(machineId, jobId, width);
        ShowSuggestions(suggestionPositions);
    }

    public void resetPosition() {
        transform.position = startPosition;
    }

    public void OnMouseUp() {
        if (Board.gameOver)
        {
            return;
        }

        moving = false;

        foreach (GameObject suggestion in suggestions) {
            Destroy(suggestion);
        }
        suggestions.Clear();

        if (onBoard == false)
        {
            resetPosition();

            if (board)
            {
                int jobIndex = board.GetJobIndex(machineId, jobId);
                if (board.RemoveJob(machineId, jobId)) {
                    gameServer.sendMove("remove", jobId, jobIndex);
                }
            }
        } else
        {
            Vector3 tmp = transform.position;
            tmp.y = machinePositionY;

            if (board.AddJob(jobId, transform.position.x - width / 2, transform.position.x + width / 2))
            {
                gameServer.sendMove("add", jobId, board.GetJobIndex(machineId, jobId));
            } else
            {
                transform.position = startPosition;
            }
        }
    }

    public void setPosition(float x) {
        Vector3 pos = new Vector3(x, machinePositionY, 0.0f);
        transform.position = pos;
    }

    void Update() {
        if (moving) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        int otherMachineId = collider.gameObject.GetComponent<Board>().machineId;
        if (otherMachineId == machineId)
        {
            onBoard = true;
            board = collider.gameObject.GetComponent<Board>();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        int otherMachineId = collider.gameObject.GetComponent<Board>().machineId;
        if (otherMachineId == machineId)
        {
            onBoard = false;
        }
    }
}
