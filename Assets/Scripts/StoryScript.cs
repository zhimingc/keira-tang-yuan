using UnityEngine;
using System.Collections;
using Ink.Runtime;
using UnityEngine.EventSystems;

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
	private UnityEngine.UI.Button button;

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

			float offset = 0;
			if (_inkStory.canContinue)
			{
				if (advance)
                {
					UnityEngine.UI.Text storyText = Instantiate(text) as UnityEngine.UI.Text;
					storyText.text = _inkStory.Continue();
					storyText.transform.SetParent(canvas.transform, false);
					storyText.transform.Translate(new Vector2(0, offset));
					offset -= (storyText.preferredHeight + elementPadding);
					advance = false;
				}
			}
			
			if (!_inkStory.canContinue)
            {
				if (_inkStory.currentChoices.Count > 0)
				{
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

						offset -= (choiceText.preferredHeight + layoutGroup.padding.top + layoutGroup.padding.bottom + elementPadding);
					}
				}
            }


			storyNeeded = false;
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
		Advance();
	}
}