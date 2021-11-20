using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoControl : MonoBehaviour
{
    public StoryScript storyScript;
    public GameObject backgroundCanvas;

    private VideoPlayer videoPlayer;
    private bool isPlaying = false;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += EndReached;
    }

    public void PlayVideo(string path)
    {
        VideoClip videoToPlay = Resources.Load<VideoClip>("Videos/" + path);
        if (videoToPlay)
        {
            videoPlayer.clip = videoToPlay;
            videoPlayer.Play();
            storyScript.gameObject.SetActive(false);
            backgroundCanvas.SetActive(false);
        }
        else
        {
            print("WARNING: Trying to play video [" + path + "] but file does not exist!");
        }
    }

    void EndReached(VideoPlayer vp)
    {
        print("done playing");
        storyScript.AdvanceAfterVideo();
        storyScript.gameObject.SetActive(true);
        backgroundCanvas.SetActive(true);
        videoPlayer.Stop();
    }
}
