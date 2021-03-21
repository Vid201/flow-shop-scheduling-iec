using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool moving, onBoard;
    private Vector3 startPosition;
    public int machineId, jobId;
    private float width, machinePositionY;
    private Board board;

    void Start() {
        startPosition = transform.position;
        onBoard = false;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public void OnMouseDown() {
        moving = true;
    }

    public void OnMouseUp() {
        moving = false;
        if (onBoard == false)
        {
            transform.position = startPosition;

            if (board)
            {   
                board.RemoveJob(jobId);
            }
        } else
        {
            Vector3 tmp = transform.position;
            tmp.y = machinePositionY;

            if (board.AddJob(jobId, transform.position.x - width / 2, transform.position.x + width / 2))
            {
                transform.position = tmp;
            } else
            {
                transform.position = startPosition;
            }
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
