using UnityEngine;
using UnityEngine.Video;

public class VideoSwitcher : MonoBehaviour
{
    public VideoClip sandstoneVideo;
    public VideoClip crystalVideo;
    public VideoClip jungleVideo;

    private VideoPlayer videoPlayer;
    private MeshRenderer meshRenderer;


    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
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
        if (!meshRenderer.enabled)
        {
            meshRenderer.enabled = true;
        }
        
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
            default:
                videoPlayer.clip = null;
                break;
        }

        videoPlayer.Play();
    }
}