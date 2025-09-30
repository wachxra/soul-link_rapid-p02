using UnityEngine;
using UnityEngine.Video;

public class CutsceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public SceneTransitionManager transitionManager;
    public string nextSceneName;

    private UIManager uiManager;

    private void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        uiManager?.SetUIActive(false);

        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        transitionManager.FadeToScene(nextSceneName);
    }
}