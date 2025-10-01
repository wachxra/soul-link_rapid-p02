using UnityEngine;
using UnityEngine.Video;

public class CutsceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public SceneTransitionManager transitionManager;

    [Header("Video Settings")]
    public string videoFileName;
    public string nextSceneName;

    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        uiManager?.SetUIActive(false);

        string videoURL = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

#if UNITY_WEBGL
        videoPlayer.url = videoURL;
#else
            videoPlayer.url = videoURL;
#endif

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        transitionManager.FadeToScene(nextSceneName);
    }
}