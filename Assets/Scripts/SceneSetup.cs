using UnityEngine;
using Unity.AI.Navigation;
namespace CamelSociety
{
    public class SceneSetup : MonoBehaviour
    {
        [Header("Ground Settings")]
        public float groundSize = 500f;
        public Material groundMaterial;
        public GameObject groundPrefab;
        
        private void Awake()
        {
            CreateGround();
            SetupLight();
        }
        
        private void CreateGround()
        {
            GameObject ground = Instantiate(groundPrefab);
            
            if (groundMaterial != null)
            {
                ground.GetComponent<Renderer>().material = groundMaterial;
            }
            
            // 添加边界墙
            // CreateBoundaryWalls();
        }
        
        private void CreateBoundaryWalls()
        {
            float wallHeight = 2f;
            float halfSize = groundSize / 2f;
            
            // 创建四面墙
            for (int i = 0; i < 4; i++)
            {
                GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = $"Wall_{i}";
                
                float angle = i * 90f;
                float rad = angle * Mathf.Deg2Rad;
                
                Vector3 position = new Vector3(
                    Mathf.Sin(rad) * halfSize,
                    wallHeight / 2f,
                    Mathf.Cos(rad) * halfSize
                );
                
                wall.transform.position = position;
                wall.transform.rotation = Quaternion.Euler(0, angle, 0);
                wall.transform.localScale = new Vector3(groundSize, wallHeight, 1);
                
                // 设置为透明材质
                Renderer renderer = wall.GetComponent<Renderer>();
                Color transparentColor = new Color(1, 1, 1, 0.1f);
                renderer.material.color = transparentColor;
                renderer.material.SetFloat("_Mode", 3); // 设置为透明模式
            }
        }
        
        private void SetupLight()
        {
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            light.shadows = LightShadows.Soft;
            
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
    }
}