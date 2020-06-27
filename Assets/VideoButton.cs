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
    public MediaPlayer mediaPlayer;

    private void Start()
    {
        mediaPlayer.Events.AddListener(OnMediaPlayerEvent);
    }

    public override void OnClick(Vector3 hitPoint)
    {
        base.OnClick(hitPoint);
        mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, Application.streamingAssetsPath + videoName, true);
        mediaPlayer.m_Loop = false;
        canvas.enabled = false;
        mediaPlayer.Play();
    }

    public void OnMediaPlayerEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.Started:                
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                canvas.enabled = true;
                break;
        }

    }
}
