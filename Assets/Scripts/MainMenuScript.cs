using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    static float sfxVolume = 0.5f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    static float dialogueVolume = 0.5f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    static float bgmVolume = 0.5f;

    [SerializeField]
    public List<GameObject> menuPopUps;

    [SerializeField]
    public AK.Wwise.Event buttonAudio;



    // Start is called before the first frame update
    void Start()
    {
        
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
    public void AboutButton() 
    {
        buttonAudio.Post(gameObject);
        menuPopUps[0].SetActive(true);
    }
    public void OptionsButton() 
    {
        buttonAudio.Post(gameObject);
        menuPopUps[1].SetActive(true);
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
