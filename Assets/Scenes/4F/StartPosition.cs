using UnityEngine;

public class StartPosition : MonoBehaviour
{
    public GameObject player; // 캐릭터 오브젝트

    private void Start()
    {
        // 특정 조건이 활성화된 경우에만 위치를 설정
        if (PlayerPrefs.GetInt("UseCustomPosition", 0) == 1) // 기본값 0 (사용 안 함)
        {
            float playerX = PlayerPrefs.GetFloat("PlayerX", player.transform.position.x);
            float playerY = PlayerPrefs.GetFloat("PlayerY", player.transform.position.y);

            player.transform.position = new Vector2(playerX, playerY);

            // 위치를 한 번 설정한 후에는 조건을 초기화
            PlayerPrefs.SetInt("UseCustomPosition", 0);
        }
    }
}
