using UnityEngine;

namespace CamelSociety
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        public float initialHeight = 50f;
        public float initialAngle = 45f;
        
        [Header("Movement Settings")]
        public float moveSpeed = 20f;
        public float zoomSpeed = 40f;
        public float rotateSpeed = 100f;
        
        [Header("Boundaries")]
        public float minHeight = 5f;
        public float maxHeight = 80f;
        public float minAngle = 20f;
        public float maxAngle = 80f;
        
        private void Start()
        {
            InitializeCamera();
        }

        private void InitializeCamera()
        {
            // 设置初始位置和角度
            transform.position = new Vector3(0, initialHeight, -initialHeight);
            transform.rotation = Quaternion.Euler(initialAngle, 0, 0);
        }
        
        private void Update()
        {
            HandleMovement();
            HandleZoom();
        }
        
        private void HandleMovement()
        {
            // WASD移动
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
            
            Vector3 movement = (forward * vertical + right * horizontal) * moveSpeed * Time.deltaTime;
            transform.position += movement;
        }
        
        private void HandleZoom()
        {
            // 滚轮缩放
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                Vector3 pos = transform.position;
                float targetHeight = pos.y - scroll * zoomSpeed;
                targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
                
                // 计算新的位置，保持视角
                float angle = transform.eulerAngles.x * Mathf.Deg2Rad;
                float zOffset = -targetHeight / Mathf.Tan(angle);
                
                transform.position = new Vector3(pos.x, targetHeight, pos.z);
                // 调整Z轴位置以保持视角
                Vector3 newPos = transform.position;
                newPos.z = zOffset;
                transform.position = newPos;
            }
        }
    }
}