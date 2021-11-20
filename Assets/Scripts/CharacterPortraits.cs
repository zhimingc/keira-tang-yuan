using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraits : MonoBehaviour
{
    public Image RHSCharacterPortrait;

    // UI animations
    public Vector3 rhsCP_endPos;
    public Vector3 rhsCP_startPos;

    void Start()
    {
        rhsCP_endPos = RHSCharacterPortrait.rectTransform.anchoredPosition;
        rhsCP_startPos = rhsCP_endPos + new Vector3(1000.0f, 0, 0);
    }

    public void RemoveCharacter()
    {
        LeanTween.move(RHSCharacterPortrait.rectTransform, rhsCP_startPos, 1.0f);
    }

    public void AddCharacter()
    {
        RHSCharacterPortrait.gameObject.SetActive(true);
        RHSCharacterPortrait.rectTransform.anchoredPosition = rhsCP_startPos;
        LeanTween.move(RHSCharacterPortrait.rectTransform, rhsCP_endPos, 1.0f);
    }
}
