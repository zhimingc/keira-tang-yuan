using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoneBehaviour : MonoBehaviour
{
    public Vector2 index = new Vector2(0, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.parent.GetComponent<CameraPan>().OnZoneMouseOver(index);
    }
}
