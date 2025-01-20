using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
    public Vector2 initialStartPosition; // Inspector에서 설정 가능한 게임 시작 위치
    private static bool isFirstStart = true;

    void Start()
    {
        // 게임을 처음 시작할 때 위치 설정
        if (isFirstStart && !PlayerPrefs.HasKey("GameInitialized"))
        {
            // 게임을 처음 시작한 경우에만 초기 위치를 설정하고 저장
            PlayerPrefs.SetFloat("PlayerStartX", initialStartPosition.x);
            PlayerPrefs.SetFloat("PlayerStartY", initialStartPosition.y);
            PlayerPrefs.SetInt("GameInitialized", 1); // 게임 초기화 플래그 설정
            PlayerPrefs.Save(); // 변경 사항을 즉시 저장

            isFirstStart = false; // 이후에는 시작 위치를 설정하지 않도록 플래그를 변경
        }

        // 저장된 위치로 플레이어 이동
        float startX = PlayerPrefs.GetFloat("PlayerStartX", initialStartPosition.x);
        float startY = PlayerPrefs.GetFloat("PlayerStartY", initialStartPosition.y);
        transform.position = new Vector2(startX, startY);
    }

    // 게임을 다시 시작하거나 새로운 게임을 시작할 때 초기화 함수 (필요시 호출)
    public static void ResetStartPosition()
    {
        PlayerPrefs.DeleteKey("GameInitialized");
        isFirstStart = true;
    }
}
