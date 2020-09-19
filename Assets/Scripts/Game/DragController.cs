using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {

                if (hit.collider != null)
                {
                    if (hit.collider.tag == "Road")
                    {
                        transform.position = hit.point;
                    }
                }
            }
        }
    }
}
