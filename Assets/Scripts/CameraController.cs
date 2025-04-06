using UnityEngine;

namespace CamelSociety
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 20f;
        public float zoomSpeed = 40f;
        public float rotateSpeed = 100f;
        
        [Header("Boundaries")]
        public float minHeight = 5f;
        public float maxHeight = 30f;
        public float minAngle = 20f;
        public float maxAngle = 80f;
        
        private float currentRotation = 0f;
        
        private void Update()
        {
            HandleMovement();
            HandleZoom();
            HandleRotation();
        }
        
        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
            
            Vector3 movement = (forward * vertical + right * horizontal) * moveSpeed * Time.deltaTime;
            transform.position += movement;
        }
        
        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Vector3 pos = transform.position;
            
            pos.y -= scroll * zoomSpeed * Time.deltaTime * 100f;
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
            
            // 调整z位置以保持视角
            float angle = transform.eulerAngles.x * Mathf.Deg2Rad;
            pos.z = -pos.y / Mathf.Tan(angle);
            
            transform.position = pos;
        }
        
        private void HandleRotation()
        {
            // 按住右键旋转
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                currentRotation += mouseX * rotateSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, currentRotation, 0);
            }
            
            // 按住中键调整俯仰角
            if (Input.GetMouseButton(2))
            {
                float mouseY = Input.GetAxis("Mouse Y");
                Vector3 angles = transform.eulerAngles;
                angles.x = Mathf.Clamp(angles.x - mouseY * rotateSpeed * Time.deltaTime, minAngle, maxAngle);
                transform.rotation = Quaternion.Euler(angles);
            }
        }
    }
}