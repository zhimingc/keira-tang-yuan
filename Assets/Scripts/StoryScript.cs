using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Windows;
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
	public GameObject[] debugObjs;
	public TextAsset inkAsset;
	public string inkStringAsset;
	public bool inkLoaded = false;
	public CharacterPortraits cpController;
	public CameraPan panningScript;
	public VideoControl videoControl;

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

	private bool delaySpawn;

	[SerializeField]
	public AK.Wwise.Event buttonAudio;

	void Awake()
	{
		if (inkAsset)
        {
			_inkStory = new Story(inkAsset.text);
		}
		storyNeeded = true;
		advance = true;
	}

    private void Start()
    {
		LoadStory();
	}

	void LoadStory()
    {
		StartCoroutine(loadStreamingAsset("HDB_Housewives.ink"));
	}

	IEnumerator loadStreamingAsset(string fileName)
	{
		string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

		if (File.Exists(filePath))
        {
			if (filePath.Contains("://") || filePath.Contains(":///"))
			{
				WWW www = new WWW(filePath);
				yield return www;
				inkStringAsset = www.text;
			}
			else
			{
				inkStringAsset = System.IO.File.ReadAllText(filePath);
			}

			var compiler = new Ink.Compiler(inkStringAsset);
			_inkStory = compiler.Compile();
		}
	}

	void Update_Debug()
    {
		// toggle debug display
		if (Input.GetKeyDown(KeyCode.Escape))
        {
			foreach (GameObject obj in debugObjs)
            {
				obj.SetActive(!obj.activeSelf);
            }
        }
    }

	// Update is called once per frame
	void Update()
	{
		Update_Debug();

		if (storyNeeded == true && _inkStory != null)
		{
			RemoveChildren();
			storyNeeded = false;

			if (advance && _inkStory.canContinue)
			{
				// HACK: to fix weird behaviour from Ink where lines after choices will be blank.
				string storyString = "\n";
				while (storyString == "\n")
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
				storyTextScript.SetText(storyString);
				// storyText.text = storyString;
				ProcessTags();
				storyTextScript.SetBacking((int)StoryState);
				storyText.transform.SetParent(canvas.transform, false);
				tempObjs.Add(storyText.gameObject);
				refreshStory = false;
			}

			if (!storyTextScript.IsDonePopulating())
				delaySpawn = true;

			if (!_inkStory.canContinue)
			{
				if (_inkStory.currentChoices.Count > 0)
				{
					bool createAsk = _inkStory.currentChoices.Count > 0;
					float offset = -75;
					for (int i = 0; i < _inkStory.currentChoices.Count; ++i)
					{
						Button choice = Instantiate(button);
						StartCoroutine(SetVisibleWhenDonePopulating(choice.gameObject));
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
						StartCoroutine(SetVisibleWhenDonePopulating(askImage.gameObject));
						askImage.transform.SetParent(canvas.transform, false);
						askImage.transform.Translate(new Vector2(0, offset));
						tempObjs.Add(askImage.gameObject);
					}
				}
			}

		}
	}

	IEnumerator SetVisibleWhenDonePopulating(GameObject go) 
	{
		Debug.Log("Running cor");
		while (!storyTextScript.IsDonePopulating())
		{
			go.SetActive(false);
			yield return null;
		}
		go.SetActive(true);
	}

	public void AdvanceAfterVideo()
    {
		ChoiceSelected(0);
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
				}
			}

			string afterPrefix = currentTag.Substring(currentTag.IndexOf('_') + 1, currentTag.Length - currentTag.IndexOf('_') - 1);
			// Check for prefix tags
			switch (prefix)
            {
				// Character Portrait
				case "CP":
					string nameOnly = splitTag[1];
					storyTextScript.SetCharacterName(nameOnly);
					if (nameOnly == "MC")
                    {
						cpController.UpdateMC(afterPrefix);
					}
					else if (splitTag[splitTag.Length - 1] == "Off")
					{
						cpController.MoveCharacterOut();
					}
					else
					{
						cpController.AddCharacter(nameOnly, afterPrefix);
					}
					break;
				case "UI":
					if (currentTag.Contains("speech_right"))
                    {
						storyTextScript.ToggleCharacterName(false);
					}
					else if (currentTag.Contains("speech_left"))
                    {
						storyTextScript.ToggleCharacterName(true);

					}
					break;
				// Environment background images
				case "BG":
					Sprite bgSprite = Resources.Load<Sprite>("Art/Background/" + afterPrefix);
					if (bgSprite != null)
					{
						panningScript.SetBackgroundImage(bgSprite);
					}
					else
                    {
						print("WARNING: Trying to load background with invalid path! [" + afterPrefix + "]");
                    }
					break;
				// play video for stock footage
				case "VID":
					videoControl.PlayVideo(currentTag);
					gameObject.SetActive(false);
					break;
				// Sound stuff
				case "VO":
				case "sfx":
				case "amb":
					// Debug.Log("Play Sound " + currentTag);
					AkSoundEngine.PostEvent(currentTag, gameObject);
					break;
				case "BGM":
					// Debug.Log("Change state " + currentTag);
					AkSoundEngine.SetState("HH_States", currentTag);
					break;
				case "RTPC":
					//Debug.Log("RTPC RAW" + currentTag);
					string[] rtpcTag = currentTag.Split(' ');
					if (rtpcTag.Length < 2)
						break;
					//Debug.Log(rtpcTag.Length + ", RTPC VALUE " + rtpcTag[0] + " to " + rtpcTag[1]);
					AkSoundEngine.SetRTPCValue(rtpcTag[0], float.Parse(rtpcTag[1]));
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
		if (_inkStory.canContinue)
        {
			storyNeeded = true;
			advance = true;
        }
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
