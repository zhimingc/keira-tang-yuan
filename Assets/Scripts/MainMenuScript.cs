using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public static float sfxVolume = 0.5f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public static float dialogueVolume = 0.5f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    public static float bgmVolume = 0.5f;

    [SerializeField]
    public List<GameObject> menuPopUps;

    [SerializeField]
    public AK.Wwise.Event buttonAudio;

    [SerializeField]
    public List<Slider> sliders;



    // Start is called before the first frame update
    void Start()
    {
        sliders[0].value = dialogueVolume;
        sliders[1].value = sfxVolume;
        sliders[2].value = bgmVolume;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton() 
    {
        buttonAudio.Post(gameObject);
        SceneManager.LoadScene("story-sandbox-zm");
    }
    public void CreditsButton()
    {
        buttonAudio.Post(gameObject);
        SceneManager.LoadScene("2-credits");
    }

    public void AboutButton() 
    {
        buttonAudio.Post(gameObject);
        menuPopUps[1].SetActive(true);
    }
    public void OptionsButton() 
    {
        buttonAudio.Post(gameObject);
        menuPopUps[0].SetActive(true);
    }
    public void QuitButton() 
    {
        buttonAudio.Post(gameObject);
        menuPopUps[2].SetActive(true);
    }
    public void ReallyQuit()
    {
        buttonAudio.Post(gameObject);
        Application.Quit();
    }

    public void ExitPopUp() 
    {
        foreach (var go in menuPopUps)
        {
            go.SetActive(false);
        }
        buttonAudio.Post(gameObject);
    }

    public void UpdateOptionSliderValues()
    {
        dialogueVolume = sliders[0].value;
        sfxVolume = sliders[1].value;
        bgmVolume = sliders[2].value;
    }

    bool IsPopUpActive() 
    {
        foreach (var go in menuPopUps) 
        {
            if (go.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }
}
