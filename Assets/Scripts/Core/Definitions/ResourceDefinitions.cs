using UnityEngine;
using System.Collections.Generic;

namespace CamelSociety.Core
{
    public class ResourceData
    {
        public string name;                 // 资源名称
        public string description;          // 资源描述
        public ResourceType type;          // 资源类型
        public float baseValue;            // 基础价值
        public float weight;               // 重量
        public float deteriorationRate;    // 损耗率
        public bool isStorable;            // 是否可存储
        public bool isTransferable;        // 是否可转移
        public float minPrice;             // 最低价格
        public float maxPrice;             // 最高价格
        public Color displayColor;         // 显示颜色
        public List<ResourceType> components; // 组成成分
        public Dictionary<string, float> productionSkills; // 生产所需技能
        public List<BuildingType> productionBuildings; // 生产建筑
        public float productionTime;       // 生产时间
        public int techLevelRequired;      // 所需科技等级

        public ResourceData()
        {
            components = new List<ResourceType>();
            productionSkills = new Dictionary<string, float>();
            productionBuildings = new List<BuildingType>();
        }
    }

    public static class ResourceDatabase
    {
        private static Dictionary<ResourceType, ResourceData> resourceData;

        public static void Initialize()
        {
            resourceData = new Dictionary<ResourceType, ResourceData>();

            // 食物
            var food = new ResourceData
            {
                name = "食物",
                description = "基础食材",
                type = ResourceType.Food,
                baseValue = 10f,
                weight = 1f,
                deteriorationRate = 0.1f,
                isStorable = true,
                isTransferable = true,
                minPrice = 5f,
                maxPrice = 20f,
                displayColor = new Color(0.2f, 0.8f, 0.2f),
                productionTime = 5f,
                techLevelRequired = 0
            };
            food.productionSkills["farming"] = 0.1f;
            food.productionBuildings.Add(BuildingType.Farm);
            resourceData[ResourceType.Food] = food;

            // 加工食品
            var processedFood = new ResourceData
            {
                name = "加工食品",
                description = "经过加工的食品",
                type = ResourceType.ProcessedFood,
                baseValue = 20f,
                weight = 1f,
                deteriorationRate = 0.05f,
                isStorable = true,
                isTransferable = true,
                minPrice = 15f,
                maxPrice = 40f,
                displayColor = new Color(0.8f, 0.6f, 0.2f),
                productionTime = 2f,
                techLevelRequired = 1
            };
            processedFood.components.Add(ResourceType.Food);
            processedFood.productionSkills["cooking"] = 0.3f;
            processedFood.productionBuildings.Add(BuildingType.Restaurant);
            resourceData[ResourceType.ProcessedFood] = processedFood;

            // 原材料
            var rawMaterial = new ResourceData
            {
                name = "原材料",
                description = "未加工的原材料",
                type = ResourceType.RawMaterial,
                baseValue = 15f,
                weight = 2f,
                deteriorationRate = 0.01f,
                isStorable = true,
                isTransferable = true,
                minPrice = 10f,
                maxPrice = 30f,
                displayColor = new Color(0.6f, 0.4f, 0.2f),
                productionTime = 3f,
                techLevelRequired = 0
            };
            rawMaterial.productionSkills["gathering"] = 0.2f;
            rawMaterial.productionBuildings.Add(BuildingType.Farm);
            rawMaterial.productionBuildings.Add(BuildingType.Mine);
            resourceData[ResourceType.RawMaterial] = rawMaterial;

            // 加工材料
            var processedMaterial = new ResourceData
            {
                name = "加工材料",
                description = "经过加工的材料",
                type = ResourceType.ProcessedMaterial,
                baseValue = 30f,
                weight = 1.5f,
                deteriorationRate = 0.005f,
                isStorable = true,
                isTransferable = true,
                minPrice = 25f,
                maxPrice = 60f,
                displayColor = new Color(0.4f, 0.6f, 0.8f),
                productionTime = 4f,
                techLevelRequired = 2
            };
            processedMaterial.components.Add(ResourceType.RawMaterial);
            processedMaterial.productionSkills["manufacturing"] = 0.4f;
            processedMaterial.productionBuildings.Add(BuildingType.Factory);
            resourceData[ResourceType.ProcessedMaterial] = processedMaterial;

            // 继续添加其他资源...
        }

        public static ResourceData GetResourceData(ResourceType type)
        {
            if (resourceData == null)
            {
                Initialize();
            }
            return resourceData.ContainsKey(type) ? resourceData[type] : null;
        }

        public static float GetResourceValue(ResourceType type, float quality = 1f)
        {
            var data = GetResourceData(type);
            if (data != null)
            {
                return data.baseValue * quality;
            }
            return 0f;
        }

        public static List<ResourceType> GetProducibleResources(BuildingType buildingType, Dictionary<string, float> skills)
        {
            if (resourceData == null)
            {
                Initialize();
            }

            List<ResourceType> producibleResources = new List<ResourceType>();
            foreach (var resource in resourceData)
            {
                if (resource.Value.productionBuildings.Contains(buildingType))
                {
                    bool hasRequiredSkills = true;
                    foreach (var requiredSkill in resource.Value.productionSkills)
                    {
                        if (!skills.ContainsKey(requiredSkill.Key) || 
                            skills[requiredSkill.Key] < requiredSkill.Value)
                        {
                            hasRequiredSkills = false;
                            break;
                        }
                    }
                    if (hasRequiredSkills)
                    {
                        producibleResources.Add(resource.Key);
                    }
                }
            }
            return producibleResources;
        }
    }
} 