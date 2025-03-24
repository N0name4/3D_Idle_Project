using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;               // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 1, -4); // 플레이어 기준 뒤쪽 오프셋
    public float positionSmoothTime = 0.1f;
    public float rotationSmoothTime = 0.1f;

    private Vector3 currentVelocity;
    private Quaternion currentRotation;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. 위치 따라가기 (플레이어 회전에 맞춘 offset)
        Vector3 desiredPosition = target.position + target.rotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, positionSmoothTime);

        // 2. 플레이어가 바라보는 방향으로 회전
        Quaternion desiredRotation = Quaternion.LookRotation(target.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothTime);
    }
}
