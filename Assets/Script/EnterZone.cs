using UnityEngine;

public class EnterZone : MonoBehaviour
{
    public GameObject enterUIText; // UI �ؽ�Ʈ ������Ʈ

    void Start()
    {
        if (enterUIText != null)
        {
            enterUIText.SetActive(false); // ó���� UI ��Ȱ��ȭ
        }
    }

    // ĳ���Ͱ� Ʈ���� �ȿ� ������ UI�� Ȱ��ȭ
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && enterUIText != null) // ĳ���Ϳ� "Player" �±װ� �ִٰ� ����
        {
            enterUIText.SetActive(true);
        }
    }

    // ĳ���Ͱ� Ʈ���Ÿ� ����� UI�� ��Ȱ��ȭ
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && enterUIText != null) // enterUIText�� �ı����� �ʾҴ��� Ȯ��
        {
            enterUIText.SetActive(false);
        }
    }
}
