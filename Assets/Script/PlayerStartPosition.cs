using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
    public Vector2 initialStartPosition; // Inspector���� ���� ������ ���� ���� ��ġ
    private static bool isFirstStart = true;

    void Start()
    {
        // ������ ó�� ������ �� ��ġ ����
        if (isFirstStart && !PlayerPrefs.HasKey("GameInitialized"))
        {
            // ������ ó�� ������ ��쿡�� �ʱ� ��ġ�� �����ϰ� ����
            PlayerPrefs.SetFloat("PlayerStartX", initialStartPosition.x);
            PlayerPrefs.SetFloat("PlayerStartY", initialStartPosition.y);
            PlayerPrefs.SetInt("GameInitialized", 1); // ���� �ʱ�ȭ �÷��� ����
            PlayerPrefs.Save(); // ���� ������ ��� ����

            isFirstStart = false; // ���Ŀ��� ���� ��ġ�� �������� �ʵ��� �÷��׸� ����
        }

        // ����� ��ġ�� �÷��̾� �̵�
        float startX = PlayerPrefs.GetFloat("PlayerStartX", initialStartPosition.x);
        float startY = PlayerPrefs.GetFloat("PlayerStartY", initialStartPosition.y);
        transform.position = new Vector2(startX, startY);
    }

    // ������ �ٽ� �����ϰų� ���ο� ������ ������ �� �ʱ�ȭ �Լ� (�ʿ�� ȣ��)
    public static void ResetStartPosition()
    {
        PlayerPrefs.DeleteKey("GameInitialized");
        isFirstStart = true;
    }
}
