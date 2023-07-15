using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHintMovement : MonoBehaviour
{
    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        
        if (Input.mousePosition.x <= Screen.width / 2)
            localPoint.x += 5;
        else localPoint.x -= GetComponent<RectTransform>().sizeDelta.x + 5;

        if (Input.mousePosition.y >= Screen.height / 2)
            localPoint.y -= 5;
        else localPoint.y += GetComponent<RectTransform>().sizeDelta.y + 5;

        transform.localPosition = localPoint;
    }
}
