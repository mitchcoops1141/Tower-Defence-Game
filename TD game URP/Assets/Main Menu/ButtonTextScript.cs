using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTextScript : MonoBehaviour
{
    public float offsetX = 0, offsetY = 4.2f;
    public RectTransform textRect;
    Vector3 pos;

    void Start()
    {
        textRect = gameObject.GetComponent<RectTransform>();
        pos = textRect.localPosition;
    }

    public void Down()
    {
        textRect.localPosition = new Vector3(pos.x + offsetX, pos.y - offsetY, pos.z);
    }

    public void Up()
    {
        textRect.localPosition = pos;
    }
}
