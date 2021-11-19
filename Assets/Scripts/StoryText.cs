using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoryText : MonoBehaviour, IPointerDownHandler
{
    //public List<Sprite> backingTypes;
    public List<Image> backings;
    public List<Color> fontColorTypes;
    public Image backing;
    public Vector2 backingBuffer;
 
    private Text text;
    private StoryScript storyParent;

    // Start is called before the first frame update
    void Start()
    {
        storyParent = GetComponentInParent<StoryScript>();
        //backing.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight) + backingBuffer * 2.0f;
        //backing.rectTransform.anchoredPosition = new Vector3(-backingBuffer.x, backingBuffer.y, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBacking(int backingIndex)
    {
        text = GetComponent<Text>();
        if (backingIndex < backings.Count)
        {
            //backing.sprite = backingTypes[backingIndex];
            backings[backingIndex].gameObject.SetActive(true);
            text.color = fontColorTypes[backingIndex];
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (storyParent)
        {
            storyParent.Advance();
        }
    }
}
