using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScene : MonoBehaviour
{
    [SerializeField]
    public float fadeTime = 2.0f;
    public float fadeTimer = 0.0f;
    private bool fadingIn = true;

    [SerializeField]
    public float holdTime = 8.0f;
    private float holdTimer = 0.0f;

    [SerializeField]
    public Image fader;

    // Start is called before the first frame update
    void Start()
    {
        fadingIn = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (holdTimer > 0.0f)
        {
            holdTimer += Time.deltaTime;
            if (Input.anyKeyDown)
            {
                holdTimer = 8.0f;
            }
            if (holdTimer > holdTime)
            {
                holdTimer = 0;
            }
            return;
        }
        if (fadingIn)
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer > fadeTime)
            {
                fadingIn = false;
                fadeTimer = 0;
                holdTimer += Time.deltaTime;
                OnFadeIn();
                SetAlpha(0);
                return;
            }
            SetAlpha(Mathf.Lerp(1.0f, 0.0f, fadeTimer / fadeTime));
        }
        else 
        {
            fadeTimer += Time.deltaTime;
            if (fadeTimer > fadeTime)
            {
                OnFadeOut();
                fadeTimer = 0;
                holdTimer += Time.deltaTime;
                fadingIn = true;
                SetAlpha(1);
                return;
            }
            SetAlpha(Mathf.Lerp(0.0f, 1.0f, fadeTimer / fadeTime));
        }
    }

    void SetAlpha(float a)
    {
        var ogColor = fader.color;
        ogColor.a = a;
        fader.color = ogColor;
    }

    void OnFadeIn()
    {
    }

    void OnFadeOut() 
    {
        SceneManager.LoadScene("0-mainmenu");
    }
}
