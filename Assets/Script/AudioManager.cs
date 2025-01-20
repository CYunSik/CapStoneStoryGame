using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // �̱��� �ν��Ͻ�
    public AudioSource bgmSource;        // BGM�� AudioSource
    public AudioClip defaultBGM;         // ���� ���� �� �⺻ BGM
    public AudioClip ending2BGM;         // Ending2 ���� BGM
    public float fadeDuration = 1f;      // ���̵� ȿ�� ���� �ð�

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ���

            // AudioSource ����
            if (bgmSource != null)
            {
                bgmSource.playOnAwake = false; // Play On Awake�� ��Ȱ��ȭ (�ڵ忡�� ����)
                bgmSource.loop = true;        // BGM�� �����ǵ��� ����
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // �� �ε� �̺�Ʈ ����
        }
    }

    private void Start()
    {
        // ���� ���� �� �⺻ BGM ���
        if (defaultBGM != null)
        {
            PlayBGM(defaultBGM);
        }
    }

    /// <summary>
    /// ���� �ε�� �� ȣ��Ǵ� �޼���
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ending1 �������� BGM�� ����ϴ�.
        if (scene.name == "Ending1")
        {
            StopBGM();
        }
        // Ending2 �������� ���ο� BGM ���
        else if (scene.name == "Ending2")
        {
            PlayBGM(ending2BGM);
        }
    }

    /// <summary>
    /// ���ο� BGM�� ����մϴ�.
    /// ������ Ŭ���� ��� ���̸� �����մϴ�.
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return; // ���� BGM�̸� ����
        StartCoroutine(FadeOutAndPlayNewClip(clip));
    }

    /// <summary>
    /// ���� BGM�� ����ϴ�.
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }
    }

    /// <summary>
    /// ������ ���� ���� �� ���ο� BGM�� ����մϴ�.
    /// </summary>
    private IEnumerator FadeOutAndPlayNewClip(AudioClip newClip)
    {
        if (bgmSource.isPlaying)
        {
            float startVolume = bgmSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }

            bgmSource.Stop();
            bgmSource.volume = startVolume; // ���� ����
        }

        // ���ο� Ŭ�� ���
        bgmSource.clip = newClip;
        bgmSource.Play();
    }
}
