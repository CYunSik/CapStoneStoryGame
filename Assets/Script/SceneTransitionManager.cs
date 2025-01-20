using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public string targetSceneName; // �̵��� �� �̸�
    public Vector3 startPositionInTargetScene; // �̵��� �������� ���� ��ġ
    private bool isTransitioning = false; // �� �̵� �ߺ� ������ ����

    private void Update()
    {
        // F Ű�� ������ �̵� ���� �ƴ� �� �� �̵�
        if (Input.GetKeyDown(KeyCode.F) && !isTransitioning)
        {
            TransitionToTargetScene();
        }
    }

    private void TransitionToTargetScene()
    {
        // �̵� ������ ǥ���Ͽ� �ߺ� �̵� ����
        isTransitioning = true;

        // �̵��� ���� ���� ��ġ�� ����
        PlayerPrefs.SetFloat("StartX", startPositionInTargetScene.x);
        PlayerPrefs.SetFloat("StartY", startPositionInTargetScene.y);
        PlayerPrefs.SetFloat("StartZ", startPositionInTargetScene.z);

        // ������ ������ �̵�
        SceneManager.LoadScene(targetSceneName);
    }
}
