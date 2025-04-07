using UnityEngine;
using System.Collections.Generic;

namespace CamelSociety.Core
{
    public enum BuildingState
    {
        UnderConstruction,  // 建设中
        Operating,         // 运营中
        Upgrading,        // 升级中
        Damaged,          // 受损
        Abandoned         // 废弃
    }

    public enum BuildingType
    {
        // 基础设施
        House,              // 住宅
        Warehouse,          // 仓库
        Market,            // 市场
        
        // 资源生产
        Farm,              // 农场
        Mine,              // 矿场
        Quarry,            // 采石场
        WoodCutter,        // 伐木场
        Well,              // 水井
        
        // 工业生产
        Factory,           // 工厂
        Workshop,          // 工作坊
        Forge,             // 锻造铺
        TextileMill,       // 纺织厂
        
        // 服务业
        Restaurant,        // 餐厅
        Hospital,          // 医院
        School,            // 学校
        University,        // 大学
        Bank,              // 银行
        Inn,              // 旅馆
        
        // 文化娱乐
        Theater,           // 剧院
        Library,           // 图书馆
        Temple,            // 寺庙
        Park,             // 公园
        Entertainment,     // 娱乐中心
        CulturalCenter,   // 文化中心
        SocialCenter,     // 社交中心
        
        // 特殊建筑
        TownHall,         // 市政厅
        Laboratory,       // 实验室
        Observatory,      // 天文台
        ResearchCenter    // 研究中心
    }

    public class BuildingData
    {
        public string name;                 // 建筑名称
        public string description;          // 建筑描述
        public BuildingType type;          // 建筑类型
        public Vector2Int size;            // 建筑占地大小
        public int maxWorkers;             // 最大工人数
        public float baseEfficiency;       // 基础效率
        public Dictionary<ResourceType, float> constructionCosts;  // 建造成本
        public Dictionary<string, float> requiredSkills;          // 所需技能
        public float constructionTime;     // 建造时间
        public float productionTime;       // 生产时间
        public float maintenanceCost;      // 维护成本
        public float storageCapacity;      // 存储容量
        public List<ResourceType> acceptedResources;    // 可接收的资源类型
        public List<ResourceType> producedResources;    // 可生产的资源类型
        public List<CareerType> suitableCareers;       // 适合的职业类型
        public int unlockTechLevel;        // 解锁所需科技等级
        public float influenceRadius;      // 影响半径
        public float happinessBonus;       // 幸福度加成
        public float educationBonus;       // 教育加成
        public float healthBonus;          // 健康加成
        public float cultureBonus;         // 文化加成

        public BuildingData()
        {
            constructionCosts = new Dictionary<ResourceType, float>();
            requiredSkills = new Dictionary<string, float>();
            acceptedResources = new List<ResourceType>();
            producedResources = new List<ResourceType>();
            suitableCareers = new List<CareerType>();
        }
    }

    public static class BuildingDatabase
    {
        private static Dictionary<BuildingType, BuildingData> buildingData;

        public static void Initialize()
        {
            buildingData = new Dictionary<BuildingType, BuildingData>();
            
            // 住宅
            var house = new BuildingData
            {
                name = "住宅",
                description = "为居民提供居住场所",
                type = BuildingType.House,
                size = new Vector2Int(2, 2),
                maxWorkers = 0,
                baseEfficiency = 1f,
                constructionTime = 30f,
                maintenanceCost = 5f,
                storageCapacity = 200f,
                unlockTechLevel = 0,
                influenceRadius = 10f,
                happinessBonus = 5f
            };
            house.constructionCosts[ResourceType.Wood] = 100f;
            house.constructionCosts[ResourceType.Stone] = 50f;
            buildingData[BuildingType.House] = house;

            // 农场
            var farm = new BuildingData
            {
                name = "农场",
                description = "生产食物和原材料",
                type = BuildingType.Farm,
                size = new Vector2Int(4, 4),
                maxWorkers = 5,
                baseEfficiency = 1f,
                constructionTime = 40f,
                maintenanceCost = 10f,
                storageCapacity = 500f,
                unlockTechLevel = 0,
                influenceRadius = 15f
            };
            farm.constructionCosts[ResourceType.Wood] = 150f;
            farm.constructionCosts[ResourceType.Stone] = 100f;
            farm.requiredSkills["farming"] = 0.3f;
            farm.acceptedResources.Add(ResourceType.Water);
            farm.producedResources.Add(ResourceType.Food);
            farm.producedResources.Add(ResourceType.RawMaterial);
            farm.suitableCareers.Add(CareerType.Farmer);
            buildingData[BuildingType.Farm] = farm;

            // 工厂
            var factory = new BuildingData
            {
                name = "工厂",
                description = "将原材料加工成产品",
                type = BuildingType.Factory,
                size = new Vector2Int(4, 6),
                maxWorkers = 10,
                baseEfficiency = 1f,
                constructionTime = 60f,
                maintenanceCost = 20f,
                storageCapacity = 1000f,
                unlockTechLevel = 2,
                influenceRadius = 20f
            };
            factory.constructionCosts[ResourceType.Metal] = 200f;
            factory.constructionCosts[ResourceType.Stone] = 150f;
            factory.requiredSkills["manufacturing"] = 0.5f;
            factory.acceptedResources.Add(ResourceType.RawMaterial);
            factory.producedResources.Add(ResourceType.ProcessedMaterial);
            factory.suitableCareers.Add(CareerType.FactoryWorker);
            factory.suitableCareers.Add(CareerType.Engineer);
            buildingData[BuildingType.Factory] = factory;

            // 餐厅
            var restaurant = new BuildingData
            {
                name = "餐厅",
                description = "提供食物服务",
                type = BuildingType.Restaurant,
                size = new Vector2Int(3, 3),
                maxWorkers = 4,
                baseEfficiency = 1f,
                constructionTime = 35f,
                maintenanceCost = 15f,
                storageCapacity = 300f,
                unlockTechLevel = 1,
                influenceRadius = 12f,
                happinessBonus = 3f
            };
            restaurant.constructionCosts[ResourceType.Wood] = 120f;
            restaurant.constructionCosts[ResourceType.Stone] = 80f;
            restaurant.requiredSkills["cooking"] = 0.4f;
            restaurant.acceptedResources.Add(ResourceType.Food);
            restaurant.producedResources.Add(ResourceType.ProcessedFood);
            restaurant.suitableCareers.Add(CareerType.Chef);
            restaurant.suitableCareers.Add(CareerType.Waiter);
            buildingData[BuildingType.Restaurant] = restaurant;

            // 继续添加其他建筑类型...
        }

        public static BuildingData GetBuildingData(BuildingType type)
        {
            if (buildingData == null)
            {
                Initialize();
            }
            return buildingData.ContainsKey(type) ? buildingData[type] : null;
        }
    }
} 