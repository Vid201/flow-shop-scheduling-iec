using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    private bool moving;

    public void OnMouseDown() {
        moving = true;
    }

    public void OnMouseUp() {
        moving = false;
    }

    void Update() {
        if (moving) {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);
        }
    }
}
