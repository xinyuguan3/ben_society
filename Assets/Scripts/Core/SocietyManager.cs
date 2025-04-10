using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
namespace CamelSociety.Core
{
    public class SocietyManager : MonoBehaviour
    {
        private static SocietyManager instance;
        public static SocietyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SocietyManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("SocietyManager");
                        instance = go.AddComponent<SocietyManager>();
                    }
                }
                return instance;
            }
        }

        [Header("Prefabs")]
        public GameObject agentPrefab;

        [Header("Society Settings")]
        public int initialPopulation = 10;
        public float simulationSpeed = 0.1f;

        [Header("Resource System")]
        public float resourceSpawnRange = 50f;
        public Material resourceMat;
        public List<ResourceType> availableResources;
        private Dictionary<ResourceType, List<GameObject>> resourceObjects = new Dictionary<ResourceType, List<GameObject>>();
        private Dictionary<BuildingType, List<GameObject>> buildings = new Dictionary<BuildingType, List<GameObject>>();

        [Header("Building Settings")]
        public GameObject restaurantPrefab;
        public GameObject factoryPrefab;
        public GameObject farmPrefab;
        public float buildingSpacing = 10f;

        [Header("Debug")]
        public bool showDebugInfo = true;

        [Header("Society Network Settings")]
        public float interactionRadius = 5f;           // Agent之间的互动范围
        public float interactionInterval = 1f;         // Agent互动的时间间隔
        public float ideologySpreadRate = 0.1f;        // 思想传播速率
        public float relationshipDecayRate = 0.01f;    // 关系衰减速率

        [Header("Society Statistics")]
        public float averageMorality = 50f;           // 社会平均道德水平
        public float culturalDiversity = 0f;          // 文化多样性
        public float socialStability = 100f;          // 社会稳定度
        public float innovationIndex = 0f;            // 创新指数
        public float giniCoefficient = 0f;            // 基尼系数

        // 使用Dictionary来存储agents，便于通过ID查找
        private Dictionary<string, Agent> agents = new Dictionary<string, Agent>();
        private float simulationTime = 0f;
        private float nextResourceSpawnTime = 0f;

        private Dictionary<IdeologyType, float> societyIdeologies = new Dictionary<IdeologyType, float>();
        private List<SocialEvent> socialEvents = new List<SocialEvent>();
        private float nextInteractionTime = 0f;

        public int CurrentPopulation => agents.Count;
        public float SimulationTime => simulationTime;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Debug.Log("SocietyManager Awake");
            if (agentPrefab == null)
            {
                Debug.LogError("Agent prefab is not assigned! Please assign it in the inspector.");
                return;
            }
        }

        private void Start()
        {
            Debug.Log("SocietyManager Start");
            InitializeSociety();
            InitializeResources();
            // InitializeSocietyStatistics();
        }

        private void Update()
        {
            simulationTime += Time.deltaTime * simulationSpeed;
            UpdateSociety();
            UpdateResources();
            
            // 更新社会互动
            if (Time.time >= nextInteractionTime)
            {
                ProcessSocialInteractions();
                nextInteractionTime = Time.time + interactionInterval;
            }

            // 定期更新社会统计数据
            if (Time.frameCount % 100 == 0)
            {
                UpdateSocietyStatistics();
            }
        }

        private void InitializeSociety()
        {
            Debug.Log("Initializing Society");
            agents.Clear();

            Debug.Log($"Attempting to create {initialPopulation} agents...");
            int successfulCreations = 0;

            for (int i = 0; i < initialPopulation; i++)
            {
                Agent newAgent = CreateAgent();
                if (newAgent != null)
                {
                    successfulCreations++;
                    Debug.Log($"Successfully created agent {successfulCreations}/{initialPopulation}");
                }
                else
                {
                    Debug.LogError($"Failed to create agent {i + 1}");
                }
            }

            Debug.Log($"Society initialization complete. Created {successfulCreations}/{initialPopulation} agents");
        }

        private void InitializeResources()
        {
            // 初始化资源对象字典
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                resourceObjects[type] = new List<GameObject>();
            }

            // 初始化建筑字典
            foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
            {
                buildings[type] = new List<GameObject>();
            }

            

            // 生成初始资源
            SpawnInitialResources();
            
            // 建造初始建筑
            ConstructInitialBuildings();
        }

        private void SpawnInitialResources()
        {
            
            availableResources = new List<ResourceType>(){
                ResourceType.Food,
                ResourceType.Wood,
                ResourceType.Stone,
                ResourceType.Metal,
                ResourceType.Gold
            };
            // 为每种资源类型生成多个资源点
            foreach (ResourceType resourceType in availableResources)
            {
                int resourceCount = GetInitialResourceCount(resourceType);
                for (int i = 0; i < resourceCount; i++)
                {
                    SpawnResourceOfType(resourceType, GetResourceSpawnPosition(resourceType));
                }
            }
        }

        private int GetInitialResourceCount(ResourceType type)
        {
            // 根据资源类型返回初始数量
            return type switch
            {
                ResourceType.Food => 15,    // 食物资源较多
                ResourceType.Wood => 10,    // 木材资源适中
                ResourceType.Stone => 8,    // 石材资源较少
                ResourceType.Metal => 5,     // 金属资源较少
                ResourceType.Gold => 3,     // 金矿很少
                _ => 5
            };
        }

        private Vector3 GetResourceSpawnPosition(ResourceType type)
        {
            // 根据资源类型决定生成位置（可以实现特定资源在特定区域生成）
            float angle = Random.Range(0f, 360f);
            float radius = Random.Range(0f, resourceSpawnRange);
            
            return new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                1f,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
        }

        private void SpawnResourceOfType(ResourceType type, Vector3 position)
        {
            GameObject resourceObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            resourceObj.name = $"Resource_{type}";
            resourceObj.transform.SetParent(transform);
            resourceObj.transform.position = position;
            resourceObj.transform.localScale = Vector3.one * 0.5f;
            // resourceObj.AddComponent<NavMeshObstacle>();

            // 设置材质和颜色
            Renderer renderer = resourceObj.GetComponent<Renderer>();
            if (renderer)
            {
                Material material = new Material(resourceMat);
                material.color = GetResourceColor(type);
                renderer.material = material;
            }

            // 添加资源组件
            ResourceObject resourceComponent = resourceObj.AddComponent<ResourceObject>();
            resourceComponent.type = type;
            resourceComponent.amount = GetInitialResourceAmount(type);
            resourceComponent.regenerationRate = GetResourceRegenerationRate(type);
            resourceComponent.maxAmount = GetResourceMaxAmount(type);

            // 添加到资源列表
            resourceObjects[type].Add(resourceObj);
        }

        private Color GetResourceColor(ResourceType type)
        {
            return type switch
            {
                ResourceType.Food => Color.green,
                ResourceType.Wood => new Color(0.5f, 0.3f, 0.2f),
                ResourceType.Stone => new Color(168/255f, 164/255f, 153/255f),
                ResourceType.Metal => Color.blue,
                ResourceType.Gold => Color.yellow,
                ResourceType.ProcessedFood => new Color(1f, 0.8f, 0.2f),
                ResourceType.Furniture => new Color(0.8f, 0.6f, 0.4f),
                ResourceType.Tools => Color.cyan,
                _ => Color.white
            };
        }

        private float GetInitialResourceAmount(ResourceType type)
        {
            return type switch
            {
                ResourceType.Food => Random.Range(50f, 100f),
                ResourceType.Wood => Random.Range(80f, 150f),
                ResourceType.Stone => Random.Range(100f, 200f),
                ResourceType.Metal => Random.Range(30f, 60f),
                ResourceType.Gold => Random.Range(10f, 20f),
                _ => Random.Range(20f, 50f)
            };
        }

        private float GetResourceRegenerationRate(ResourceType type)
        {
            return type switch
            {
                ResourceType.Food => 0.5f,    // 食物较快恢复
                ResourceType.Wood => 0.3f,    // 木材中等恢复
                ResourceType.Stone => 0.1f,   // 石材慢恢复
                ResourceType.Metal => 0.05f,   // 金属很慢恢复
                ResourceType.Gold => 0.02f,   // 金矿极慢恢复
                _ => 0.1f
            };
        }

        private float GetResourceMaxAmount(ResourceType type)
        {
            return type switch
            {
                ResourceType.Food => 200f,
                ResourceType.Wood => 300f,
                ResourceType.Stone => 400f,
                ResourceType.Metal => 100f,
                ResourceType.Gold => 50f,
                _ => 100f
            };
        }

        private void ConstructInitialBuildings()
        {
            // 建造初始建筑
            ConstructBuilding(BuildingType.Restaurant, new Vector3(-20f, 0f, -20f));
            ConstructBuilding(BuildingType.Factory, new Vector3(20f, 0f, -20f));
            ConstructBuilding(BuildingType.Farm, new Vector3(0f, 0f, 20f));
        }

        private void ConstructBuilding(BuildingType type, Vector3 position)
        {
            GameObject buildingPrefab = GetBuildingPrefab(type);
            if (buildingPrefab == null) return;

            GameObject building = Instantiate(buildingPrefab, position, Quaternion.identity, transform);
            building.name = $"Building_{type}";

            Building buildingComponent = building.AddComponent<Building>();
            buildingComponent.type = type;
            buildingComponent.Initialize();

            buildings[type].Add(building);
        }

        private GameObject GetBuildingPrefab(BuildingType type)
        {
            return type switch
            {
                BuildingType.Restaurant => restaurantPrefab,
                BuildingType.Factory => factoryPrefab,
                BuildingType.Farm => farmPrefab,
                _ => null
            };
        }

        public Agent CreateAgent()
        {
            if (agentPrefab == null)
            {
                Debug.LogError("Agent prefab is not assigned!");
                return null;
            }

            try
            {
                Vector3 randomPosition = new Vector3(
                    Random.Range(-resourceSpawnRange * 0.5f, resourceSpawnRange * 0.5f),
                    0,
                    Random.Range(-resourceSpawnRange * 0.5f, resourceSpawnRange * 0.5f)
                );

                GameObject agentObj = Instantiate(agentPrefab, randomPosition, Quaternion.identity, transform);
                if (agentObj == null)
                {
                    Debug.LogError("Failed to instantiate agent prefab!");
                    return null;
                }

                Agent agent = agentObj.GetComponent<Agent>();
                if (agent == null)
                {
                    Debug.LogError($"Agent component not found on prefab! GameObject name: {agentObj.name}");
                    Destroy(agentObj);
                    return null;
                }

                // 确保Agent有一个唯一的ID
                if (string.IsNullOrEmpty(agent.agentId))
                {
                    agent.agentId = System.Guid.NewGuid().ToString();
                }

                // 检查ID是否已存在
                if (agents.ContainsKey(agent.agentId))
                {
                    Debug.LogWarning($"Agent with ID {agent.agentId} already exists. Generating new ID.");
                    agent.agentId = System.Guid.NewGuid().ToString();
                }

                agents[agent.agentId] = agent;
                agentObj.name = $"Agent_{agent.agentId}";

                Debug.Log($"Created agent: {agentObj.name} at position {randomPosition}");
                return agent;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating agent: {e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public void RemoveAgent(string agentId)
        {
            if (agents.ContainsKey(agentId))
            {
                Agent agent = agents[agentId];
                agents.Remove(agentId);
                Destroy(agent.gameObject);
            }
        }

        public Agent GetAgent(string agentId)
        {
            return agents.ContainsKey(agentId) ? agents[agentId] : null;
        }

        public List<Agent> GetAllAgents()
        {
            return new List<Agent>(agents.Values);
        }

        private void UpdateSociety()
        {
            if (!showDebugInfo) return;

            if (Time.frameCount % 10000 == 0)
            {
                Debug.Log($"Society Status - Time: {simulationTime:F2}, Population: {agents.Count}");
            }

            // 更新所有Agent的社会关系
            foreach (var agent in agents.Values)
            {
                // 可以添加额外的社会层面的更新逻辑
            }
        }

        private void UpdateResources()
        {
            // 更新所有资源点
            foreach (var resourceList in resourceObjects.Values)
            {
                foreach (var resourceObj in resourceList)
                {
                    if (resourceObj == null) continue;
                    
                    ResourceObject resource = resourceObj.GetComponent<ResourceObject>();
                    if (resource != null)
                    {
                        // 资源再生
                        if (resource.amount < resource.maxAmount)
                        {
                            resource.amount += resource.regenerationRate * Time.deltaTime;
                            resource.amount = Mathf.Min(resource.amount, resource.maxAmount);
                        }
                    }
                }
            }

            // 更新所有建筑
            foreach (var buildingList in buildings.Values)
            {
                foreach (var building in buildingList)
                {
                    if (building == null) continue;

                    Building buildingComponent = building.GetComponent<Building>();
                    if (buildingComponent != null)
                    {
                        buildingComponent.UpdateProduction();
                    }
                }
            }
        }

        private void ProcessSocialInteractions()
        {
            var allAgents = new List<Agent>(agents.Values);
            
            foreach (var agent in allAgents)
            {
                // 查找附近的其他Agent
                var nearbyAgents = allAgents
                    .Where(other => other != agent && 
                           Vector3.Distance(agent.transform.position, other.transform.position) <= interactionRadius)
                    .ToList();

                foreach (var other in nearbyAgents)
                {
                    // 根据社会影响力和兼容性决定是否互动
                    float interactionChance = CalculateInteractionChance(agent, other);
                    if (Random.value < interactionChance)
                    {
                        agent.InteractWith(other);
                        
                        // 记录重要社会事件
                        if (agent.socialInfluence + other.socialInfluence > 150f)
                        {
                            RecordSocialEvent(new SocialEvent
                            {
                                type = "重要社交",
                                description = $"有影响力的个体 {agent.agentName} 与 {other.agentName} 进行了互动",
                                time = simulationTime
                            });
                        }
                    }
                }
            }
        }

        private float CalculateInteractionChance(Agent a, Agent b)
        {
            // 基础互动概率
            float baseChance = 0.3f;
            
            // 社会地位差异影响
            float statusDiff = Mathf.Abs(a.socialInfluence - b.socialInfluence) / 100f;
            float statusFactor = 1f - statusDiff * 0.5f;

            // 已有关系影响
            float relationFactor = 1f;
            if (a.socialNetwork.ContainsKey(b.agentId))
            {
                var relation = a.socialNetwork[b.agentId];
                relationFactor += relation.intimacy * 0.5f;
            }

            // 意识形态相似度影响
            float ideologyAlignment = 0f;
            foreach (var ideology in a.ideologies)
            {
                float diff = Mathf.Abs(ideology.Value - b.ideologies[ideology.Key]);
                ideologyAlignment += 1f - (diff / 100f);
            }
            float ideologyFactor = ideologyAlignment / a.ideologies.Count;

            return Mathf.Clamp01(baseChance * statusFactor * relationFactor * ideologyFactor);
        }

        private void UpdateSocietyStatistics()
        {
            if (agents.Count == 0) return;

            // 计算平均道德水平
            averageMorality = agents.Values.Average(a => a.moralityValue);

            // 计算文化多样性
            var ideologyVariance = new Dictionary<IdeologyType, float>();
            foreach (IdeologyType ideology in System.Enum.GetValues(typeof(IdeologyType)))
            {
                float mean = agents.Values.Average(a => a.ideologies[ideology]);
                float variance = agents.Values.Average(a => Mathf.Pow(a.ideologies[ideology] - mean, 2));
                ideologyVariance[ideology] = variance;
            }
            culturalDiversity = ideologyVariance.Values.Average();

            // 计算社会稳定度
            float conflictLevel = CalculateConflictLevel();
            socialStability = Mathf.Max(0, 100f - conflictLevel);

            // 计算创新指数
            innovationIndex = agents.Values.Average(a => a.innovationDrive);

            // 计算基尼系数
            CalculateGiniCoefficient();

            // 更新社会意识形态分布
            foreach (IdeologyType ideology in System.Enum.GetValues(typeof(IdeologyType)))
            {
                societyIdeologies[ideology] = agents.Values.Average(a => a.ideologies[ideology]);
            }

            if (showDebugInfo)
            {
                Debug.Log($"社会状态 - 时间: {simulationTime:F2}\n" +
                         $"人口: {agents.Count}\n" +
                         $"道德水平: {averageMorality:F2}\n" +
                         $"文化多样性: {culturalDiversity:F2}\n" +
                         $"社会稳定度: {socialStability:F2}\n" +
                         $"创新指数: {innovationIndex:F2}\n" +
                         $"基尼系数: {giniCoefficient:F2}");
            }
        }

        private float CalculateConflictLevel()
        {
            float conflictLevel = 0f;
            
            // 计算意识形态对立程度
            foreach (IdeologyType ideology in System.Enum.GetValues(typeof(IdeologyType)))
            {
                float ideologyMean = agents.Values.Average(a => a.ideologies[ideology]);
                float ideologyVariance = agents.Values.Average(a => Mathf.Pow(a.ideologies[ideology] - ideologyMean, 2));
                conflictLevel += ideologyVariance * 0.1f;
            }

            // 计算财富差距带来的社会矛盾
            float wealthMean = agents.Values.Average(a => a.wealth);
            float wealthVariance = agents.Values.Average(a => Mathf.Pow(a.wealth - wealthMean, 2));
            conflictLevel += wealthVariance * 0.0001f;

            return conflictLevel;
        }

        private void CalculateGiniCoefficient()
        {
            if (agents.Count < 2) return;

            var wealths = agents.Values.Select(a => a.wealth).OrderBy(w => w).ToList();
            float n = wealths.Count;
            float sumOfDifferences = 0f;
            float sumOfWealth = wealths.Sum();

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    sumOfDifferences += Mathf.Abs(wealths[i] - wealths[j]);
                }
            }

            giniCoefficient = sumOfDifferences / (2 * n * n * (sumOfWealth / n));
        }

        private void RecordSocialEvent(SocialEvent evt)
        {
            socialEvents.Add(evt);
            if (socialEvents.Count > 100)
            {
                socialEvents.RemoveAt(0);
            }

            if (showDebugInfo)
            {
                Debug.Log($"社会事件 - {evt.type}: {evt.description} (时间: {evt.time:F2})");
            }
        }

        private void OnDestroy()
        {
            agents.Clear();
        }
    }

    public class SocialEvent
    {
        public string type;
        public string description;
        public float time;
    }
} 