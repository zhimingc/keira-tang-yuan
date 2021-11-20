using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSpawner : MonoBehaviour
{
    [SerializeField]
    [Range(1, 20)]
    public float spawnSpeed = 5;
    public string fullText;
    public AK.Wwise.Event typeWriterSound;
    
    private Text myText;

    Coroutine textCoroutine;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PopulateText(string text) 
    {
        myText = GetComponent<Text>();
        myText.text = "";
        fullText = text;
        textCoroutine = StartCoroutine("TextAppend");
    }

    public bool IsCompleted()
    {
        return fullText == myText.text;
    }

    public void Skip()
    {
        myText.text = fullText;
        StopCoroutine(textCoroutine);
    }

    IEnumerator TextAppend()
    {
        foreach (char c in fullText) 
        {
            myText.text += c;
            yield return new WaitForSeconds(0.1f / spawnSpeed);
        }
    }
}
