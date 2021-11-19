using UnityEngine;
using System.Collections;
using Ink.Runtime;
using UnityEngine.EventSystems;

public enum STORY_STATE
{
	THOUGHT,
	SPEECH,
	CHOICE,
	NUM
}

public class StoryScript : MonoBehaviour
{
	[SerializeField]
	private TextAsset inkAsset;
	private Story _inkStory;
	private bool storyNeeded;
	private bool advance;

	[SerializeField]
	private Canvas canvas;
	[SerializeField]
	private float elementPadding;

	/* UI Prefabs */
	[SerializeField]
	private UnityEngine.UI.Text text;
	[SerializeField]
	private UnityEngine.UI.Image choiceAsk;
	[SerializeField]
	private UnityEngine.UI.Button button;
	private UnityEngine.UI.Text storyText;

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

	// Update is called once per frame
	void Update()
	{
		if (storyNeeded == true && _inkStory.canContinue)
		{
			RemoveChildren();

			if (_inkStory.canContinue)
			{
				if (advance)
				{
					storyText = Instantiate(text) as UnityEngine.UI.Text;
					string storyString;
					// HACK: to fix weird behaviour from Ink where lines after choices will be blank.
					do
					{
						storyString = _inkStory.Continue();
					}
					while (_inkStory.canContinue && storyString == "\n");
					storyText.text = storyString;
					ProcessTags(_inkStory);
					storyText.GetComponent<StoryText>().SetBacking((int)StoryState);
					storyText.transform.SetParent(canvas.transform, false);
					advance = false;
				}
			}

			if (!_inkStory.canContinue)
			{
				if (_inkStory.currentChoices.Count > 0)
				{
					bool createAsk = _inkStory.currentChoices.Count > 0;
					float offset = -75;
					for (int i = 0; i < _inkStory.currentChoices.Count; ++i)
					{
						UnityEngine.UI.Button choice = Instantiate(button) as UnityEngine.UI.Button;
						choice.transform.SetParent(canvas.transform, false);
						choice.transform.Translate(new Vector2(0, offset));

						UnityEngine.UI.Text choiceText = choice.GetComponentInChildren<UnityEngine.UI.Text>();
						choiceText.text = _inkStory.currentChoices[i].text;

						UnityEngine.UI.HorizontalLayoutGroup layoutGroup = choice.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();

						int choiceId = i;
						choice.onClick.AddListener(delegate { ChoiceSelected(choiceId); });

						offset += (layoutGroup.padding.top + layoutGroup.padding.bottom + elementPadding);
					}

					if (createAsk)
					{
						UnityEngine.UI.Image askImage = Instantiate<UnityEngine.UI.Image>(choiceAsk);
						askImage.transform.SetParent(canvas.transform, false);
						askImage.transform.Translate(new Vector2(0, offset));
					}
				}
			}

			storyNeeded = false;
		}
	}

    void ProcessTags(Story inkStory)
    {
		for (int i = 0; i < (int)STORY_STATE.NUM; ++i)
        {
			string storyStateString = ((STORY_STATE)i).ToString().ToLower();
			for (int j = 0; j < inkStory.currentTags.Count; ++j)
			{
				if (_inkStory.currentTags[j].Contains(storyStateString))
                {
					StoryState = (STORY_STATE)i;
                }
			}
		}
    }

    void RemoveChildren()
	{
		int childCount = canvas.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i)
		{
			GameObject.Destroy(canvas.transform.GetChild(i).gameObject);
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
}
