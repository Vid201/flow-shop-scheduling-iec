using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool moving;
    private Vector3 startPosition;
    public int machineId, jobId;

    void Start() {
        startPosition = transform.position;
    }

    public void OnMouseDown() {
        moving = true;
    }

    public void OnMouseUp() {
        moving = false;
        transform.position = startPosition;
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
            Debug.Log("correct");
        }
    }
}
