using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraits : MonoBehaviour
{
    public Image RHSCharacterPortrait;
    public Sprite placeholderSprite;
    public Text placeholderText;

    private string currentCharacterName;
    private bool isCharacterIn;

    // UI animations
    public Vector3 rhsCP_endPos;
    public Vector3 rhsCP_startPos;

    void Start()
    {
        rhsCP_endPos = RHSCharacterPortrait.rectTransform.anchoredPosition;
        rhsCP_startPos = rhsCP_endPos + new Vector3(1000.0f, 0, 0);
    }

    public void MoveCharacterOut()
    {
        currentCharacterName = "";
        LeanTween.move(RHSCharacterPortrait.rectTransform, rhsCP_startPos, 1.0f);
        isCharacterIn = false;
    }

    public void MoveCharacterIn(string cName)
    {
        currentCharacterName = cName;
        RHSCharacterPortrait.rectTransform.anchoredPosition = rhsCP_startPos;
        LeanTween.move(RHSCharacterPortrait.rectTransform, rhsCP_endPos, 1.0f);
        isCharacterIn = true;
    }

    public void SwapCharacters()
    {
       // TODO: For CP transitions when there's already a diff character out
    }

    public void AddCharacter(string cName)
    {
        if (currentCharacterName == cName) return;

        placeholderText.text = "";
        // Load sprite
        // Look for sprite with official folder structure
        Sprite cpSprite = Resources.Load<Sprite>("Art/Character/" + cName + "/" + cName + "_Default");
        // else look for sprite in placeholder folder
        if (cpSprite == null)
        {
            cpSprite = Resources.Load<Sprite>("Art/Character/Placeholder/PH_" + cName);
        }
        // else use a white square as placeholder
        if (cpSprite == null)
        {
            print("WARNING: Tried to load [" + cName + "] character portrait, but sprite could not be found!");
            cpSprite = placeholderSprite;
            placeholderText.text = cName;
        }

        RHSCharacterPortrait.gameObject.SetActive(true);
        RHSCharacterPortrait.sprite = cpSprite;
        MoveCharacterIn(cName);
    }
}