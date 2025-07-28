using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerCursor : MonoBehaviour
{
    [SerializeField]RectTransform cursor;
    Vector2 pos;
    [SerializeField] float height;
    private void Start()
    {
        pos.y = cursor.position.y;
    }
    private void Update()
    {
        pos.x = Input.mousePosition.x;
        cursor.position = pos;
        if (Input.GetMouseButtonDown(0))
        {
            cursor.localScale = new Vector2(0.9f, 0.9f);
            Invoke("ResetCursor", 0.15f);
        }
    }
    void ResetCursor()
    {
        cursor.localScale = new Vector2(1, 1);
    }
}
