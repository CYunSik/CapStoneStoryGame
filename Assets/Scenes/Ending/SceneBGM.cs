using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    public AudioClip bgmClip;   // 이 씬에서 재생할 BGM
    public bool stopBGMOnStart; // 씬 시작 시 BGM 멈추기 여부

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            if (stopBGMOnStart)
            {
                AudioManager.Instance.StopBGM();
            }
            else if (bgmClip != null)
            {
                AudioManager.Instance.PlayBGM(bgmClip);
            }
        }
    }
}
