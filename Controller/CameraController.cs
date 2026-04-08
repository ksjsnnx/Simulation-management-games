using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("相机移动速度")]
    public float MoveSpeed = 5f;

    [Header("水平移动最大距离")]
    public float MX = 10f;

    [Header("垂直移动最大距离")]
    public float MY = 10f;

    [Header("最小高度（最接近地面）")]
    public float MinHeight = 5f;

    [Header("最大高度（最远离地面）")]
    public float MaxHeight = 20f;

    [Header("滚轮缩放速度")]
    public float ZoomSpeed = 10f;

    private Vector3 OriginPos;
    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        OriginPos = transform.position;
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // 获取输入
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 currentPos = transform.position;

        // 计算目标位置（XZ 平面移动）
        float targetX = Mathf.Clamp(currentPos.x + moveX, OriginPos.x - MX, OriginPos.x + MX);
        float targetZ = Mathf.Clamp(currentPos.z + moveZ, OriginPos.z - MY, OriginPos.z + MY);

        // 缩放：修改摄像机高度 Y
        float targetY = Mathf.Clamp(currentPos.y - scroll * ZoomSpeed, MinHeight, MaxHeight);

        // 平滑插值移动
        Vector3 targetPos = new Vector3(targetX, targetY, targetZ);
        transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref velocity,0.01f);
    }
}
