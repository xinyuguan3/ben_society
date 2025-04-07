using UnityEngine;
using System.Collections.Generic;

namespace CamelSociety.Core
{
    public class ResourceVisualizer : MonoBehaviour
    {
        [Header("Resource Prefabs")]
        public GameObject basicResourcePrefab;
        public GameObject processedResourcePrefab;
        public GameObject advancedResourcePrefab;
        public GameObject spiritualResourcePrefab;

        [Header("Resource Materials")]
        public Material[] basicResourceMaterials;
        public Material[] processedResourceMaterials;
        public Material[] advancedResourceMaterials;
        public Material[] spiritualResourceMaterials;

        [Header("Visualization Settings")]
        public float resourceScale = 0.5f;
        public float floatHeight = 0.5f;
        public float floatSpeed = 1f;
        public float rotateSpeed = 30f;

        private Dictionary<ResourceType, GameObject> resourcePrefabs = new Dictionary<ResourceType, GameObject>();
        private Dictionary<ResourceType, Material> resourceMaterials = new Dictionary<ResourceType, Material>();
        private Dictionary<GameObject, ResourceObject> activeResources = new Dictionary<GameObject, ResourceObject>();

        private void Start()
        {
            InitializeResourceVisuals();
        }

        private void InitializeResourceVisuals()
        {
            // 为每种资源类型分配预制体和材质
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                var definition = ResourceSystem.Instance.resourceDefinitions[type];
                GameObject prefab = null;
                Material material = null;

                switch (definition.tier)
                {
                    case ResourceTier.Basic:
                        prefab = basicResourcePrefab;
                        material = basicResourceMaterials[Random.Range(0, basicResourceMaterials.Length)];
                        break;
                    case ResourceTier.Processed:
                        prefab = processedResourcePrefab;
                        material = processedResourceMaterials[Random.Range(0, processedResourceMaterials.Length)];
                        break;
                    case ResourceTier.Advanced:
                        prefab = advancedResourcePrefab;
                        material = advancedResourceMaterials[Random.Range(0, advancedResourceMaterials.Length)];
                        break;
                    case ResourceTier.Spiritual:
                        prefab = spiritualResourcePrefab;
                        material = spiritualResourceMaterials[Random.Range(0, spiritualResourceMaterials.Length)];
                        break;
                }

                if (prefab != null)
                {
                    resourcePrefabs[type] = prefab;
                    resourceMaterials[type] = material;
                }
            }
        }

        public GameObject CreateResourceVisual(ResourceType type, Vector3 position)
        {
            if (!resourcePrefabs.ContainsKey(type))
                return null;

            GameObject resourceObj = Instantiate(resourcePrefabs[type], position, Quaternion.identity);
            resourceObj.name = $"Resource_{type}";
            resourceObj.transform.localScale = Vector3.one * resourceScale;

            // 设置材质
            Renderer renderer = resourceObj.GetComponent<Renderer>();
            if (renderer && resourceMaterials.ContainsKey(type))
            {
                renderer.material = resourceMaterials[type];
            }

            // 添加资源组件
            ResourceObject resourceComponent = resourceObj.AddComponent<ResourceObject>();
            resourceComponent.type = type;
            resourceComponent.amount = Random.Range(10f, 30f);

            // 添加视觉效果
            ResourceVisualEffect visualEffect = resourceObj.AddComponent<ResourceVisualEffect>();
            visualEffect.floatHeight = floatHeight;
            visualEffect.floatSpeed = floatSpeed;
            visualEffect.rotateSpeed = rotateSpeed;

            activeResources[resourceObj] = resourceComponent;
            return resourceObj;
        }

        public void RemoveResourceVisual(GameObject resourceObj)
        {
            if (activeResources.ContainsKey(resourceObj))
            {
                activeResources.Remove(resourceObj);
                Destroy(resourceObj);
            }
        }

        private void Update()
        {
            // 更新所有资源的视觉效果
            foreach (var resource in activeResources.Keys)
            {
                if (resource != null)
                {
                    ResourceVisualEffect effect = resource.GetComponent<ResourceVisualEffect>();
                    if (effect != null)
                    {
                        effect.UpdateVisualEffect();
                    }
                }
            }
        }
    }

    public class ResourceVisualEffect : MonoBehaviour
    {
        public float floatHeight = 0.5f;
        public float floatSpeed = 1f;
        public float rotateSpeed = 30f;

        private Vector3 startPosition;
        private float timeOffset;

        private void Start()
        {
            startPosition = transform.position;
            timeOffset = Random.Range(0f, Mathf.PI * 2);
        }

        public void UpdateVisualEffect()
        {
            // 上下浮动
            float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // 旋转
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
} 