using UnityEngine;

public class StartPosition : MonoBehaviour
{
    public GameObject player; // ĳ���� ������Ʈ

    private void Start()
    {
        // Ư�� ������ Ȱ��ȭ�� ��쿡�� ��ġ�� ����
        if (PlayerPrefs.GetInt("UseCustomPosition", 0) == 1) // �⺻�� 0 (��� �� ��)
        {
            float playerX = PlayerPrefs.GetFloat("PlayerX", player.transform.position.x);
            float playerY = PlayerPrefs.GetFloat("PlayerY", player.transform.position.y);

            player.transform.position = new Vector2(playerX, playerY);

            // ��ġ�� �� �� ������ �Ŀ��� ������ �ʱ�ȭ
            PlayerPrefs.SetInt("UseCustomPosition", 0);
        }
    }
}
