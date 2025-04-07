using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CamelSociety.Core
{
    #region 资源枚举
    public enum ResourceTier
    {
        Basic,          // 基础资源
        Processed,      // 加工品
        Advanced,       // 高级产品
        Spiritual       // 精神产品
    }
    #endregion

    public class ResourceDefinition
    {
        public ResourceType type;
        public ResourceTier tier;
        public float baseValue;
        public float deteriorationRate;  // 资源损耗率
        public bool isStorable;          // 是否可存储
        public float maxStorage;         // 最大存储量
        public string description;

        public ResourceDefinition(ResourceType type, ResourceTier tier, float baseValue, 
            float deteriorationRate, bool isStorable, float maxStorage, string description)
        {
            this.type = type;
            this.tier = tier;
            this.baseValue = baseValue;
            this.deteriorationRate = deteriorationRate;
            this.isStorable = isStorable;
            this.maxStorage = maxStorage;
            this.description = description;
        }
    }

    public class ResourceConversionRule
    {
        public Dictionary<ResourceType, float> inputs;  // 输入资源及数量
        public Dictionary<ResourceType, float> outputs; // 输出资源及数量
        public float conversionTime;                    // 转换所需时间
        public float energyCost;                        // 能源消耗
        public float skillRequirement;                  // 所需技能等级
        public string requiredOccupation;              // 所需职业

        public ResourceConversionRule(Dictionary<ResourceType, float> inputs, 
            Dictionary<ResourceType, float> outputs, float conversionTime, 
            float energyCost, float skillRequirement, string requiredOccupation)
        {
            this.inputs = inputs;
            this.outputs = outputs;
            this.conversionTime = conversionTime;
            this.energyCost = energyCost;
            this.skillRequirement = skillRequirement;
            this.requiredOccupation = requiredOccupation;
        }
    }

    public class MarketData
    {
        public ResourceType resourceType;
        public float currentPrice;
        public float supply;
        public float demand;
        public float priceVolatility;
        public List<float> priceHistory = new List<float>();

        public void UpdatePrice(float newSupply, float newDemand)
        {
            float supplyDemandRatio = newSupply / (newDemand + 0.01f);
            float priceChange = (1f - supplyDemandRatio) * priceVolatility;
            currentPrice = Mathf.Max(currentPrice + priceChange, 0.01f);
            priceHistory.Add(currentPrice);
            if (priceHistory.Count > 100) priceHistory.RemoveAt(0);
        }
    }

    public class ResourceSystem : MonoBehaviour
    {
        private static ResourceSystem instance;
        public static ResourceSystem Instance => instance;

        public Dictionary<ResourceType, ResourceDefinition> resourceDefinitions = new Dictionary<ResourceType, ResourceDefinition>();
        public List<ResourceConversionRule> conversionRules = new List<ResourceConversionRule>();
        public Dictionary<ResourceType, MarketData> market = new Dictionary<ResourceType, MarketData>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeResourceSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeResourceSystem()
        {
            InitializeResourceDefinitions();
            InitializeConversionRules();
            InitializeMarket();
        }

        private void InitializeResourceDefinitions()
        {
            // 基础资源
            AddResourceDefinition(ResourceType.Food, ResourceTier.Basic, 10f, 0.1f, true, 1000f, "基础食物");
            AddResourceDefinition(ResourceType.Water, ResourceTier.Basic, 5f, 0.05f, true, 2000f, "清洁水源");
            AddResourceDefinition(ResourceType.Wood, ResourceTier.Basic, 15f, 0.01f, true, 5000f, "木材");
            AddResourceDefinition(ResourceType.Stone, ResourceTier.Basic, 20f, 0f, true, 10000f, "石材");
            AddResourceDefinition(ResourceType.Metal, ResourceTier.Basic, 30f, 0f, true, 5000f, "金属原料");
            AddResourceDefinition(ResourceType.Energy, ResourceTier.Basic, 25f, 0.2f, false, 1000f, "能源");

            // 加工品
            AddResourceDefinition(ResourceType.Medicine, ResourceTier.Processed, 50f, 0.05f, true, 500f, "医疗用品");
            AddResourceDefinition(ResourceType.Tools, ResourceTier.Processed, 40f, 0.02f, true, 1000f, "工具");
            AddResourceDefinition(ResourceType.Clothes, ResourceTier.Processed, 35f, 0.03f, true, 1000f, "服装");
            AddResourceDefinition(ResourceType.Electronics, ResourceTier.Processed, 100f, 0.05f, true, 500f, "电子产品");
            AddResourceDefinition(ResourceType.Vehicle, ResourceTier.Processed, 200f, 0.02f, true, 100f, "交通工具");

            // 高级产品
            AddResourceDefinition(ResourceType.Computer, ResourceTier.Advanced, 300f, 0.1f, true, 200f, "计算机");
            AddResourceDefinition(ResourceType.Robot, ResourceTier.Advanced, 500f, 0.05f, true, 50f, "机器人");

            // 精神产品
            AddResourceDefinition(ResourceType.Artwork, ResourceTier.Spiritual, 150f, 0f, true, 1000f, "艺术品");
            AddResourceDefinition(ResourceType.Knowledge, ResourceTier.Spiritual, 200f, 0f, true, float.MaxValue, "知识");
            AddResourceDefinition(ResourceType.Entertainment, ResourceTier.Spiritual, 80f, 0.1f, false, 1000f, "娱乐产品");
            AddResourceDefinition(ResourceType.Culture, ResourceTier.Spiritual, 300f, 0f, true, float.MaxValue, "文化产品");
            AddResourceDefinition(ResourceType.Stock, ResourceTier.Spiritual, 500f, 0.2f, true, float.MaxValue, "股票");
            AddResourceDefinition(ResourceType.Bond, ResourceTier.Spiritual, 500f, 0.2f, true, float.MaxValue, "债券");
        }

        private void InitializeConversionRules()
        {
            // 食物加工
            AddConversionRule(
                new Dictionary<ResourceType, float> { 
                    { ResourceType.Food, 10f }, 
                    { ResourceType.Energy, 2f } 
                },
                new Dictionary<ResourceType, float> { 
                    { ResourceType.Medicine, 1f } 
                },
                2f, 2f, 5f, "医生"
            );

            // 工具制造
            AddConversionRule(
                new Dictionary<ResourceType, float> { 
                    { ResourceType.Metal, 5f }, 
                    { ResourceType.Energy, 3f } 
                },
                new Dictionary<ResourceType, float> { 
                    { ResourceType.Tools, 1f } 
                },
                3f, 3f, 3f, "工匠"
            );

            // 更多转换规则...
        }

        private void InitializeMarket()
        {
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                var definition = resourceDefinitions[type];
                market[type] = new MarketData
                {
                    resourceType = type,
                    currentPrice = definition.baseValue,
                    supply = 100f,
                    demand = 100f,
                    priceVolatility = 0.1f
                };
            }
        }

        private void AddResourceDefinition(ResourceType type, ResourceTier tier, float baseValue, 
            float deteriorationRate, bool isStorable, float maxStorage, string description)
        {
            resourceDefinitions[type] = new ResourceDefinition(type, tier, baseValue, 
                deteriorationRate, isStorable, maxStorage, description);
        }

        private void AddConversionRule(Dictionary<ResourceType, float> inputs, 
            Dictionary<ResourceType, float> outputs, float conversionTime, 
            float energyCost, float skillRequirement, string requiredOccupation)
        {
            conversionRules.Add(new ResourceConversionRule(inputs, outputs, 
                conversionTime, energyCost, skillRequirement, requiredOccupation));
        }

        public bool CanConvert(Agent agent, ResourceConversionRule rule)
        {
            // 检查技能要求
            if (agent.GetSkillLevel(rule.requiredOccupation) < rule.skillRequirement)
                return false;

            // 检查资源是否足够
            foreach (var input in rule.inputs)
            {
                if (!agent.HasResource(input.Key, input.Value))
                    return false;
            }

            return true;
        }

        public void ConvertResources(Agent agent, ResourceConversionRule rule)
        {
            if (!CanConvert(agent, rule))
                return;

            // 消耗输入资源
            foreach (var input in rule.inputs)
            {
                agent.ConsumeResource(input.Key, input.Value);
            }

            // 生产输出资源
            foreach (var output in rule.outputs)
            {
                agent.AddResource(output.Key, output.Value);
            }

            // 消耗能源
            agent.ConsumeResource(ResourceType.Energy, rule.energyCost);

            // 增加经验
            agent.AddExperience(rule.requiredOccupation, Random.Range(1f, 3f));
        }

        public void UpdateMarket()
        {
            foreach (var marketData in market.Values)
            {
                // 更新供需关系
                float supplyChange = Random.Range(-10f, 10f);
                float demandChange = Random.Range(-10f, 10f);
                
                marketData.supply = Mathf.Max(0, marketData.supply + supplyChange);
                marketData.demand = Mathf.Max(0, marketData.demand + demandChange);
                
                // 更新价格
                marketData.UpdatePrice(marketData.supply, marketData.demand);
            }
        }

        public float GetResourcePrice(ResourceType type)
        {
            return market[type].currentPrice;
        }

        public void Trade(Agent seller, Agent buyer, ResourceType type, float amount)
        {
            if (!market.ContainsKey(type))
                return;

            float price = GetResourcePrice(type) * amount;
            
            if (seller.HasResource(type, amount) && buyer.HasResource(ResourceType.Money, price))
            {
                seller.ConsumeResource(type, amount);
                seller.AddResource(ResourceType.Money, price);
                
                buyer.AddResource(type, amount);
                buyer.ConsumeResource(ResourceType.Money, price);

                // 更新市场数据
                market[type].supply += amount;
                market[type].demand -= amount;
                market[type].UpdatePrice(market[type].supply, market[type].demand);
            }
        }

        private void Update()
        {
            // 定期更新市场
            if (Time.frameCount % 100 == 0)
            {
                UpdateMarket();
            }
        }
    }
} 