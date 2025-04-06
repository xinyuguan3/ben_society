using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        [Header("Resource Settings")]
        public float resourceSpawnInterval = 5f;
        public float resourceSpawnRange = 10f;
        public List<ResourceType> availableResources;
        public Material resourceMat;

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
            InitializeSocietyStatistics();
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
            Debug.Log("Initializing Resources");
            if (availableResources == null)
                availableResources = new List<ResourceType>();

            availableResources.Clear();

            foreach (ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
            {
                if (!availableResources.Contains(resource))
                    availableResources.Add(resource);
            }

            for (int i = 0; i < 10; i++)
            {
                SpawnResource();
            }

            Debug.Log($"Resources initialized with {availableResources.Count} types");
        }

        private void InitializeSocietyStatistics()
        {
            // 初始化社会意识形态分布
            foreach (IdeologyType ideology in System.Enum.GetValues(typeof(IdeologyType)))
            {
                societyIdeologies[ideology] = 50f; // 初始均衡分布
            }
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
            if (Time.time >= nextResourceSpawnTime)
            {
                SpawnResource();
                nextResourceSpawnTime = Time.time + resourceSpawnInterval;
            }
        }

        private void SpawnResource()
        {
            if (availableResources.Count == 0) return;

            ResourceType resourceType = availableResources[Random.Range(0, availableResources.Count)];

            GameObject resourceObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            resourceObj.name = $"Resource_{resourceType}";
            resourceObj.transform.SetParent(transform);

            Vector3 randomPosition = new Vector3(
                Random.Range(-resourceSpawnRange, resourceSpawnRange),
                0.5f,
                Random.Range(-resourceSpawnRange, resourceSpawnRange)
            );
            resourceObj.transform.position = randomPosition;

            resourceObj.transform.localScale = Vector3.one * 0.5f;

            Renderer renderer = resourceObj.GetComponent<Renderer>();
            if (renderer)
            {
                Material material = resourceMat;
                Color color = resourceType switch
                {
                    ResourceType.Food => Color.yellow,
                    ResourceType.Wood => new Color(0.5f, 0.3f, 0.2f),
                    ResourceType.Stone => Color.gray,
                    ResourceType.Gold => Color.yellow,
                    ResourceType.Knowledge => Color.cyan,
                    _ => Color.white
                };
                material.color = color;
                renderer.material = material;
            }

            ResourceObject resourceComponent = resourceObj.AddComponent<ResourceObject>();
            resourceComponent.type = resourceType;
            resourceComponent.amount = Random.Range(10f, 30f);
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