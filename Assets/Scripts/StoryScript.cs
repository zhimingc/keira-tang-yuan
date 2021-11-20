using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum STORY_STATE
{
	THOUGHT,
	SPEECH,
	CHOICE,
	NUM
}

public class StoryScript : MonoBehaviour
{
	public TextAsset inkAsset;
	public CharacterPortraits cpController;
	public CameraPan panningScript;

	private Story _inkStory;
	private bool storyNeeded;
	private bool advance;
	private bool refreshStory;

	public Canvas canvas;
	public float elementPadding;

	/* UI Prefabs */
	public Text text;
	public Image choiceAsk;
	public Button button;
	private Text storyText;

	/* Instanced gameobjects */
	// to be removed every Continue
	private List<GameObject> tempObjs = new List<GameObject>();	

	/* UI scripts */
	private StoryText storyTextScript;

	/* State Machine */
	public STORY_STATE StoryState;

	[SerializeField]
	private AK.Wwise.Event buttonAudio;

	void Awake()
	{
		_inkStory = new Story(inkAsset.text);
		storyNeeded = true;
		advance = true;
	}

	void Update_Debug()
    {

    }

	// Update is called once per frame
	void Update()
	{
		
		if (storyNeeded == true)
		{
			RemoveChildren();

			if (advance)
			{
				string storyString = "";
				// HACK: to fix weird behaviour from Ink where lines after choices will be blank.
				while (_inkStory.canContinue || storyString == "\n")
				{
					storyString = _inkStory.Continue();
				}
				advance = false;
				refreshStory = true;
			}

			if (refreshStory)
            {
				string storyString = _inkStory.currentText;
				storyText = Instantiate(text);
				storyTextScript = storyText.GetComponent<StoryText>();
				storyText.text = storyString;
				ProcessTags();
				storyTextScript.SetBacking((int)StoryState);
				storyText.transform.SetParent(canvas.transform, false);
				tempObjs.Add(storyText.gameObject);
				refreshStory = false;
			}

			if (!_inkStory.canContinue)
			{
				if (_inkStory.currentChoices.Count > 0)
				{
					bool createAsk = _inkStory.currentChoices.Count > 0;
					float offset = -75;
					for (int i = 0; i < _inkStory.currentChoices.Count; ++i)
					{
						Button choice = Instantiate(button);
						choice.transform.SetParent(canvas.transform, false);
						choice.transform.Translate(new Vector2(0, offset));

						Text choiceText = choice.GetComponentInChildren<Text>();
						choiceText.text = _inkStory.currentChoices[i].text;

						HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();

						int choiceId = i;
						choice.onClick.AddListener(delegate { ChoiceSelected(choiceId); });

						offset += (layoutGroup.padding.top + layoutGroup.padding.bottom + elementPadding);
						tempObjs.Add(choice.gameObject);
					}

					if (createAsk)
					{
						Image askImage = Instantiate(choiceAsk);
						askImage.transform.SetParent(canvas.transform, false);
						askImage.transform.Translate(new Vector2(0, offset));
						tempObjs.Add(askImage.gameObject);
					}
				}
			}

			storyNeeded = false;
		}
	}

    void ProcessTags()
    {
		for (int j = 0; j < _inkStory.currentTags.Count; ++j)
		{
			string currentTag = _inkStory.currentTags[j];
			string[] splitTag = currentTag.Split('_');
			string prefix = splitTag[0];
			// Check for story state tags
			for (int i = 0; i < (int)STORY_STATE.NUM; ++i)
			{
				string storyStateString = ((STORY_STATE)i).ToString().ToLower();
				if (currentTag.Contains(storyStateString))
				{
					StoryState = (STORY_STATE)i;
					if (i == 0)
                    {
						cpController.RemoveCharacter();
					}
				}
			}

			// Check for prefix tags
			switch (prefix)
            {
				// Character Portrait
				case "CP":
					string name = splitTag[1];
					storyTextScript.SetCharacterName(name);
					cpController.AddCharacter();
					break;
            }
		}
	}

    void RemoveChildren()
	{
		for (int i = 0; i < tempObjs.Count; ++i)
        {
			Destroy(tempObjs[i]);
        }
	}

	public void Advance()
	{
		storyNeeded = true;
		advance = true;
	}

	public void ChoiceSelected(int id)
	{
		_inkStory.ChooseChoiceIndex(id);
		buttonAudio.Post(gameObject);
		Advance();
	}

	public void SetStory(Story newStory)
    {
		_inkStory = newStory;
		refreshStory = true;
		storyNeeded = true;
	}
}
