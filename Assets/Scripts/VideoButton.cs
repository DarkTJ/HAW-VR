using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RenderHeads.Media.AVProVideo;

// Use https://www.programmersought.com/article/66161219799/;jsessionid=41296F10E975A2ED94BFAB41F590BA30
public class VideoButton : UIButton
{
    public string videoName;
    public Canvas canvas;
    Animator canvasAnim;
    public MediaPlayer mediaPlayer;
    public PlanetariumSoundManager pManager;
    public int buttonID;

    private void Start()
    {
        canvasAnim = canvas.GetComponent<Animator>();
        mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
    }

    public override void OnClick(Vector3 hitPoint)
    {
        base.OnClick(hitPoint);
        PlayVideo(buttonID);
    }

    public void PlayVideo(int videoID)
    {
        mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, Application.streamingAssetsPath + videoName, true);
        mediaPlayer.m_Loop = false;
        mediaPlayer.Play();
        pManager.PlaySound(videoID);
    }

    public void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.Started:
                canvasAnim.SetTrigger("fadeOut");
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                canvasAnim.SetTrigger("fadeIn");
                break;
        }

    }
}
