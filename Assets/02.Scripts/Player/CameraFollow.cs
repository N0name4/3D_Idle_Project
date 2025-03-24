using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // 따라갈 대상 (플레이어)
    public Vector3 offset = new Vector3(0, 5, -7); // 카메라 위치 오프셋
    public float followSpeed = 5f;
    public float rotateSpeed = 5f;
    void LateUpdate()
    {
        if (target == null) return;

        // 부드러운 위치 따라가기
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // 이동 방향이 있을 때만 카메라 회전
        Vector3 lookDir = target.forward;
        if (lookDir.sqrMagnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
