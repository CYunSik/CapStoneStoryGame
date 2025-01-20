using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterUG : MonoBehaviour
{
    public string sceneName; // 이동할 씬의 이름을 Inspector에서 설정 가능
    public Vector2 playerStartPosition; // 다음 씬에서 캐릭터가 시작할 위치를 Inspector에서 설정 가능

    private bool isPlayerInRange = false; // 플레이어가 범위 안에 있는지 확인

    void Update()
    {
        // 플레이어가 범위 안에 있고 "F" 키를 눌렀으며, GameManager의 enterUG가 1일 때 씬 전환
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && GameManager.instance != null && GameManager.instance.PlayCat_1F == 1)
        {
            LoadSceneByName();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어와 충돌 감지
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName)) // 씬 이름이 비어있지 않은지 확인
        {
            SavePlayerStartPosition(); // 상호작용으로 씬 이동이 발생했을 때만 위치 저장
            FadeManager.Instance.LoadSceneWithFade(sceneName); // 씬 이름으로 씬 로드
        }
    }

    private void SavePlayerStartPosition()
    {
        // 캐릭터의 시작 위치를 PlayerPrefs에 저장
        PlayerPrefs.SetFloat("PlayerStartX", playerStartPosition.x);
        PlayerPrefs.SetFloat("PlayerStartY", playerStartPosition.y);

        // GameManager에 다음 씬 이름과 위치 정보 업데이트
        if (GameManager.instance != null)
        {
            GameManager.instance.UpdatePosition(sceneName, playerStartPosition); // 다음 씬 이름을 직접 전달
        }
    }
}