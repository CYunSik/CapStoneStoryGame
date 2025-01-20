using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 대상(캐릭터)
    public Vector3 offset;   // 카메라와 캐릭터 사이의 거리
    public float smoothSpeed = 0.125f; // 카메라 이동 부드럽게 하는 속도

    // 카메라가 이동할 수 있는 범위 설정 (배경의 경계값)
    public float minX, maxX, minY, maxY;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // 카메라가 지정된 경계를 벗어나지 않도록 위치 제한
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);

            transform.position = smoothedPosition;
        }
    }
}
