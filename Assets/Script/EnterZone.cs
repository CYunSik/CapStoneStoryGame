using UnityEngine;

public class EnterZone : MonoBehaviour
{
    public GameObject enterUIText; // UI 텍스트 오브젝트

    void Start()
    {
        if (enterUIText != null)
        {
            enterUIText.SetActive(false); // 처음엔 UI 비활성화
        }
    }

    // 캐릭터가 트리거 안에 들어오면 UI를 활성화
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && enterUIText != null) // 캐릭터에 "Player" 태그가 있다고 가정
        {
            enterUIText.SetActive(true);
        }
    }

    // 캐릭터가 트리거를 벗어나면 UI를 비활성화
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && enterUIText != null) // enterUIText가 파괴되지 않았는지 확인
        {
            enterUIText.SetActive(false);
        }
    }
}
