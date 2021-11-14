using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoryText : MonoBehaviour, IPointerDownHandler
{
    public Image backing;
    public Vector2 backingBuffer;

    private Text text;
    private StoryScript storyParent;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        storyParent = GetComponentInParent<StoryScript>();
        backing.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight) + backingBuffer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (storyParent)
        {
            storyParent.Advance();
        }
    }
}
