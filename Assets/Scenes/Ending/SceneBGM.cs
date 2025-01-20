using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    public AudioClip bgmClip;   // �� ������ ����� BGM
    public bool stopBGMOnStart; // �� ���� �� BGM ���߱� ����

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
