using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ���� ���(ĳ����)
    public Vector3 offset;   // ī�޶�� ĳ���� ������ �Ÿ�
    public float smoothSpeed = 0.125f; // ī�޶� �̵� �ε巴�� �ϴ� �ӵ�

    // ī�޶� �̵��� �� �ִ� ���� ���� (����� ��谪)
    public float minX, maxX, minY, maxY;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // ī�޶� ������ ��踦 ����� �ʵ��� ��ġ ����
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);

            transform.position = smoothedPosition;
        }
    }
}
