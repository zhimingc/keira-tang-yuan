using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            Vector3 screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(screenPoint.x, screenPoint.y, 0);
            
        }
    }
}
