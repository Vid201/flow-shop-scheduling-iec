using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool moving, onBoard;
    private Vector3 startPosition;
    public int machineId, jobId;
    private float machinePositionY;
    private Board board;

    void Start() {
        startPosition = transform.position;
        onBoard = false;
    }

    public void OnMouseDown() {
        moving = true;
    }

    public void OnMouseUp() {
        moving = false;
        if (onBoard == false)
        {
            transform.position = startPosition;
            board.RemoveJob(jobId);
        } else
        {
            Vector3 tmp = transform.position;
            tmp.y = machinePositionY;
            transform.position = tmp;
            board.AddJob(jobId, transform.position.x, transform.position.x + transform.localScale.x);
        }
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
            Vector3 tmp = collider.gameObject.GetComponent<Transform>().position;
            machinePositionY = tmp.y;
            board = collider.gameObject.GetComponent<Board>();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        onBoard = false;
    }
}
