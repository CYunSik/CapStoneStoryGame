using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // 싱글톤 인스턴스
    public AudioSource bgmSource;        // BGM용 AudioSource
    public AudioClip defaultBGM;         // 게임 시작 시 기본 BGM
    public AudioClip ending2BGM;         // Ending2 씬의 BGM
    public float fadeDuration = 1f;      // 페이드 효과 지속 시간

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 등록

            // AudioSource 설정
            if (bgmSource != null)
            {
                bgmSource.playOnAwake = false; // Play On Awake를 비활성화 (코드에서 제어)
                bgmSource.loop = true;        // BGM은 루프되도록 설정
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
            SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 로드 이벤트 해제
        }
    }

    private void Start()
    {
        // 게임 시작 시 기본 BGM 재생
        if (defaultBGM != null)
        {
            PlayBGM(defaultBGM);
        }
    }

    /// <summary>
    /// 씬이 로드될 때 호출되는 메서드
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ending1 씬에서는 BGM을 멈춥니다.
        if (scene.name == "Ending1")
        {
            StopBGM();
        }
        // Ending2 씬에서는 새로운 BGM 재생
        else if (scene.name == "Ending2")
        {
            PlayBGM(ending2BGM);
        }
    }

    /// <summary>
    /// 새로운 BGM을 재생합니다.
    /// 동일한 클립이 재생 중이면 무시합니다.
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return; // 같은 BGM이면 무시
        StartCoroutine(FadeOutAndPlayNewClip(clip));
    }

    /// <summary>
    /// 현재 BGM을 멈춥니다.
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
    /// 볼륨을 점점 줄인 후 새로운 BGM을 재생합니다.
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
            bgmSource.volume = startVolume; // 볼륨 복구
        }

        // 새로운 클립 재생
        bgmSource.clip = newClip;
        bgmSource.Play();
    }
}
