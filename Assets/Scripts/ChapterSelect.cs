using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class ChapterSelect : MonoBehaviour
{
    public StoryScript storyScript;
    public Dropdown dropDownObj;

    public int currentChapterSelect;
    
    private Story inkStory;

    public void SetChapterSelect()
    {
        currentChapterSelect = dropDownObj.value + 1;
    }

    public void GoToChapter()
    {
        inkStory = new Story(storyScript.inkAsset.text);
        string storyString;
        while (inkStory.canContinue || inkStory.currentChoices.Count > 0)
        {
            if (inkStory.canContinue)
            {
                storyString = inkStory.Continue();
            }
            else
            {
                inkStory.ChooseChoiceIndex(0);
                continue;
            }

            if (CheckTagsForChapterSelected())
            {
                storyScript.SetStory(inkStory);
                print("set success!");
                return;
            }
        }

        print("set failed");
    }

    public bool CheckTagsForChapterSelected()
    {
        for (int j = 0; j < inkStory.currentTags.Count; ++j)
        {
            string currentTag = inkStory.currentTags[j];
            string[] splitTag = currentTag.Split('_');
            string prefix = splitTag[0];

            if (prefix == "CHP")
            {
                if (int.Parse(splitTag[1]) == currentChapterSelect)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
