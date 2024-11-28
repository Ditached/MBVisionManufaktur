using UnityEngine;
using UnityEngine.Video;

public class VideoSwitcher : MonoBehaviour
{
    public VideoClip sandstoneVideo;
    public VideoClip crystalVideo;
    public VideoClip jungleVideo;

    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void OnEnable()
    {
        FindObjectOfType<LackSwitcher>().OnWorldChanged += HandleWorldChanged;
    }

    private void OnDisable()
    {
        FindObjectOfType<LackSwitcher>().OnWorldChanged -= HandleWorldChanged;
    }

    private void HandleWorldChanged(LackWorld world)
    {
        switch (world)
        {
            case LackWorld.Sandstone:
                videoPlayer.clip = sandstoneVideo;
                break;
            case LackWorld.Crystal:
                videoPlayer.clip = crystalVideo;
                break;
            case LackWorld.Jungle:
                videoPlayer.clip = jungleVideo;
                break;
        }

        videoPlayer.Play();
    }
}