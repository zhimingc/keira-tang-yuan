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
    public Image RHSCharacterPortrait;
    public Image backing;
    public Text characterNameText;
    public GameObject LHSNameLabel;
    public GameObject RHSNameLabel;
    public Vector2 backingBuffer;
 
    private Text text;
    private StoryScript storyParent;
    private TextSpawner textSpawner;

    public string fullText;

    // Start is called before the first frame update
    void Start()
    { 
        storyParent = GetComponentInParent<StoryScript>();
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

            if (backingIndex == 0)
            {
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (textSpawner != null && !textSpawner.IsCompleted())
        {
            textSpawner.Skip();
            return;
        }

        if (storyParent)
        {
            storyParent.Advance();
        }
    }

    public void SetCharacterName(string cName)
    {
        characterNameText.text = cName;
    }

    public void ToggleCharacterName(bool lhs)
    {
        if (lhs)
        {
            LHSNameLabel.SetActive(true);
        }
        else
        {
            RHSNameLabel.SetActive(true);
        }
    }

    public void SetText(string textToSet) 
    {
        textSpawner = GetComponent<TextSpawner>();
        if (textSpawner == null)
        {
            GetComponent<Text>().text = textToSet;
            return;
        }
        textSpawner.PopulateText(textToSet);
    }

    public bool IsDonePopulating() 
    {
        textSpawner = GetComponent<TextSpawner>();
        return textSpawner != null && textSpawner.IsCompleted();
    }
}
