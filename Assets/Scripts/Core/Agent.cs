using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

namespace CamelSociety.Core
{
    public enum State
    {
        Wandering,
        MovingToResource,
        CollectingResource
    }

    public enum IdeologyType
    {
        Conservative,    // 保守主义
        Liberal,        // 自由主义
        Socialist,      // 社会主义
        Capitalist,     // 资本主义
        Environmentalist, // 环保主义
        Technocrat,     // 技术官僚
        Humanitarian    // 人道主义
    }
    public class DNA
    {
        public Dictionary<string, float> personalityTraits = new Dictionary<string, float>();
        public Dictionary<string, float> values = new Dictionary<string, float>();
        public Dictionary<string, float> abilities = new Dictionary<string, float>();

        public DNA()
        {
            InitializeTraits();
            InitializeValues();
            InitializeAbilities();
        }

        private void InitializeTraits()
        {
            personalityTraits["openness"] = Random.Range(0f, 1f);      // 开放性
            personalityTraits["conscientiousness"] = Random.Range(0f, 1f); // 尽责性
            personalityTraits["extraversion"] = Random.Range(0f, 1f);   // 外向性
            personalityTraits["agreeableness"] = Random.Range(0f, 1f);  // 宜人性
            personalityTraits["neuroticism"] = Random.Range(0f, 1f);    // 神经质
        }

        private void InitializeValues()
        {
            values["tradition"] = Random.Range(0f, 1f);    // 传统价值观
            values["innovation"] = Random.Range(0f, 1f);   // 创新精神
            values["collectivism"] = Random.Range(0f, 1f); // 集体主义
            values["individualism"] = Random.Range(0f, 1f); // 个人主义
            values["materialism"] = Random.Range(0f, 1f);  // 物质主义
            values["spiritualism"] = Random.Range(0f, 1f); // 精神追求
        }

        private void InitializeAbilities()
        {
            abilities["learning"] = Random.Range(0f, 1f);    // 学习能力
            abilities["creativity"] = Random.Range(0f, 1f);  // 创造力
            abilities["leadership"] = Random.Range(0f, 1f);  // 领导力
            abilities["social"] = Random.Range(0f, 1f);      // 社交能力
            abilities["technical"] = Random.Range(0f, 1f);   // 技术能力
        }
    }

    public class SocialRelation
    {
        public string targetId;           // 关系对象ID
        public RelationType type;         // 关系类型
        public float intimacy;            // 亲密度 0-1
        public float trust;               // 信任度 0-1
        public float influence;           // 影响力 0-1
        public float duration;            // 关系持续时间
        public List<string> interactions; // 互动历史记录

        public SocialRelation(string targetId, RelationType type)
        {
            this.targetId = targetId;
            this.type = type;
            intimacy = Random.Range(0.1f, 0.3f);
            trust = Random.Range(0.1f, 0.3f);
            influence = Random.Range(0f, 0.1f);
            duration = 0;
            interactions = new List<string>();
        }

        public void UpdateRelation(float deltaTime)
        {
            duration += deltaTime;
            // 关系自然衰减
            intimacy = Mathf.Max(0, intimacy - deltaTime * 0.01f);
            trust = Mathf.Max(0, trust - deltaTime * 0.005f);
        }
    }

    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Agent : MonoBehaviour
    {
        #region 基础属性
        [Header("Basic Information")]
        public string agentId;
        public string agentName;
        public int age;
        public Gender gender;
        public float health = 100f;
        public float education;
        public List<Skill> skills = new List<Skill>();
        public Personality personality;
        public Material agentMat;
        #endregion

        #region 社会属性
        [Header("Social Attributes")]
        public Occupation occupation;
        public SocialClass socialClass;
        public float wealth;
        public float prestige;
        public float influence;
        public Dictionary<string, Relationship> relationships = new Dictionary<string, Relationship>();
        #endregion

        #region 需求系统
        [Header("Needs")]
        public Dictionary<NeedType, Need> needs = new Dictionary<NeedType, Need>();
        public float hunger;
        public float energy;
        public float happiness;
        public float needUpdateInterval = 1f;
        private float lastNeedUpdateTime;
        private Building targetConsumptionBuilding;
        private NeedType currentNeed;
        private float consumptionCooldown = 2f;
        private float lastConsumptionTime;
        #endregion

        #region 资源系统
        [Header("Resources")]
        public Dictionary<ResourceType, Resource> resources = new Dictionary<ResourceType, Resource>();
        public Dictionary<ResourceType, float> inventory = new Dictionary<ResourceType, float>();
        public float resourceCapacity = 1000f;
        private ResourceConversionRule currentConversion;
        private float conversionProgress = 0f;
        private float lastTradeTime = 0f;
        private float tradeInterval = 1f;
        #endregion

        #region 行为系统
        [Header("Behavior")]
        public float moveSpeed = 2f;
        public float rotateSpeed = 10f;
        public float resourceDetectionRange = 5f;
        public float interactionRange = 1.5f;
        public float wanderRadius = 10f;
        private State currentState = State.Wandering;
        private Vector3 targetPosition;
        private float minDistanceToTarget = 0.5f;
        #endregion

        #region 组件引用
        [Header("Components")]
        private CapsuleCollider agentCollider;
        private NavMeshAgent navMeshAgent;
        private Transform visualTransform;
        private GameObject visualParent;
        #endregion

        #region 思想体系
        [Header("Ideology System")]
        public DNA dna;                           // DNA属性
        public Dictionary<IdeologyType, float> ideologies = new Dictionary<IdeologyType, float>();  // 意识形态倾向
        public float moralityValue = 50f;         // 道德值 0-100
        public float culturalIdentity = 50f;      // 文化认同度 0-100
        public float innovationDrive = 50f;       // 创新动力 0-100
        public float socialInfluence = 0f;        // 社会影响力
        private List<string> beliefs = new List<string>();  // 信念系统
        private List<string> memories = new List<string>(); // 重要记忆
        #endregion

        #region 社会关系网络
        [Header("Social Network")]
        public Dictionary<string, SocialRelation> socialNetwork = new Dictionary<string, SocialRelation>();
        public float networkInfluence = 0f;       // 关系网络影响力
        public int maxRelations = 50;             // 最大关系数量
        private List<string> recentInteractions = new List<string>(); // 最近互动记录
        #endregion

        [Header("Career System")]
        public string currentCareerId;
        public float careerExperience;
        public float jobSatisfaction;
        public Dictionary<string, float> skillLevels = new Dictionary<string, float>();

        private float careerUpdateInterval = 7f; // 每7天检查一次职业发展
        private float lastCareerUpdateTime;

        [Header("Resource Collection")]
        public float resourceCollectionSpeed = 1f;
        public float carryCapacity = 50f;
        private ResourceObject targetResource;
        public Building assignedBuilding;
        private float lastResourceCheckTime;
        private float resourceCheckInterval = 1f;

        private void Awake()
        {
            Debug.Log($"Agent {gameObject.name} Awake");
            InitializeComponents();
        }

        private void Start()
        {
            Debug.Log($"Agent {gameObject.name} Start");
            InitializeAgent();
            SetRandomTarget();
        }

        private void InitializeComponents()
        {
            agentCollider = GetComponent<CapsuleCollider>();
            if (agentCollider == null)
            {
                agentCollider = gameObject.AddComponent<CapsuleCollider>();
            }

            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            }

            // 设置NavMeshAgent参数
            navMeshAgent.radius = 0.3f;
            navMeshAgent.height = 2f;
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.angularSpeed = rotateSpeed;
            navMeshAgent.acceleration = 8f;
            navMeshAgent.stoppingDistance = 0.1f;
            navMeshAgent.autoBraking = true;

            agentCollider.height = 2f;
            agentCollider.radius = 0.3f;
            agentCollider.center = Vector3.up;

            if (visualParent == null)
            {
                visualParent = new GameObject("Visuals");
                visualParent.transform.SetParent(transform);
                visualParent.transform.localPosition = Vector3.zero;
                visualParent.transform.localRotation = Quaternion.identity;
                visualTransform = visualParent.transform;
            }
        }

        private void InitializeAgent()
        {
            Debug.Log($"Agent {gameObject.name} InitializeAgent");
            
            // 初始化基础属性
            agentId = System.Guid.NewGuid().ToString();
            agentName = GenerateRandomName();
            age = Random.Range(18, 65);
            gender = Random.value > 0.5f ? Gender.Male : Gender.Female;
            health = Random.Range(70f, 100f);
            education = Random.Range(0f, 100f);

            // 初始化社会属性
            socialClass = DetermineSocialClass();
            wealth = Random.Range(1000f, 10000f);
            prestige = Random.Range(0f, 100f);
            influence = Random.Range(0f, 100f);

            // 初始化各个系统
            InitializeNeeds();
            InitializePersonality();
            InitializeSkills();
            InitializeResources();
            
            // 设置外观
            CreateVisuals();

            // 初始化DNA和思想体系
            InitializeIdeologySystem();

            // 初始化社会关系网络
            InitializeSocialNetwork();

            // 初始化职业系统
            InitializeCareer();
        }

        private void InitializeInventory()
        {
            inventory.Clear();
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                inventory[type] = 0f;
            }

            // 初始资源
            switch (socialClass)
            {
                case SocialClass.WorkingClass:
                    AddResource(ResourceType.Food, Random.Range(10f, 30f));
                    AddResource(ResourceType.Money, Random.Range(100f, 300f));
                    break;
                case SocialClass.MiddleClass:
                    AddResource(ResourceType.Food, Random.Range(30f, 60f));
                    AddResource(ResourceType.Money, Random.Range(500f, 1000f));
                    AddResource(ResourceType.Electronics, Random.Range(1f, 3f));
                    break;
                case SocialClass.UpperClass:
                    AddResource(ResourceType.Food, Random.Range(50f, 100f));
                    AddResource(ResourceType.Money, Random.Range(2000f, 5000f));
                    AddResource(ResourceType.Electronics, Random.Range(2f, 5f));
                    AddResource(ResourceType.Vehicle, 1f);
                    break;
                case SocialClass.RulingClass:
                    AddResource(ResourceType.Food, Random.Range(100f, 200f));
                    AddResource(ResourceType.Money, Random.Range(10000f, 20000f));
                    AddResource(ResourceType.Electronics, Random.Range(5f, 10f));
                    AddResource(ResourceType.Artwork, Random.Range(1f, 3f));
                    break;
            }
        }

        private string GenerateRandomName()
        {
            string[] firstNames = { "张", "王", "李", "赵", "刘", "陈", "杨", "黄", "周", "吴" };
            string[] secondNames = { "伟", "芳", "娜", "秀英", "敏", "静", "丽", "强", "磊", "军" };
            return firstNames[Random.Range(0, firstNames.Length)] + secondNames[Random.Range(0, secondNames.Length)];
        }

        private SocialClass DetermineSocialClass()
        {
            float rand = Random.value;
            if (rand < 0.6f) return SocialClass.WorkingClass;
            if (rand < 0.9f) return SocialClass.MiddleClass;
            if (rand < 0.98f) return SocialClass.UpperClass;
            return SocialClass.RulingClass;
        }

        private void CreateVisuals()
        {
            Debug.Log($"Agent {gameObject.name} CreateVisuals");

            if (visualParent == null)
            {
                InitializeComponents();
            }

            foreach (Transform child in visualParent.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(visualParent.transform);
            body.transform.localPosition = Vector3.up;
            body.transform.localRotation = Quaternion.identity;

            Destroy(body.GetComponent<Collider>());

            Renderer renderer = body.GetComponent<Renderer>();
            if (renderer)
            {
                Material material = agentMat;
                Color color = socialClass switch
                {
                    SocialClass.WorkingClass => new Color(181/255f,51/255f,36/255f),
                    SocialClass.MiddleClass => new Color(223/255f,188/255f,148/255f),
                    SocialClass.UpperClass => new Color(229/255f,166/255f,87/255f),
                    SocialClass.RulingClass => new Color(38/255f,34/255f,36/255f),
                    _ => Color.white
                };
                material.color = color;
                renderer.material = material;
            }

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(visualParent.transform);
            head.transform.localPosition = Vector3.up * 1.6f;
            head.transform.localScale = Vector3.one * 0.5f;
            head.transform.localRotation = Quaternion.identity;

            Destroy(head.GetComponent<Collider>());

            if (renderer != null)
            {
                Material headMaterial = new Material(renderer.material);
                head.GetComponent<Renderer>().material = headMaterial;
            }

            Debug.Log($"Agent {gameObject.name} visuals created successfully");
        }

        private void Update()
        {
            UpdateNeeds();
            UpdateBehavior();
            UpdateRelations(Time.deltaTime);
            UpdateIdeology();
            UpdateCareer();
            UpdateResourceCollection();

            // 如果到达了目标建筑，存放资源
            if (assignedBuilding != null && Vector3.Distance(transform.position, assignedBuilding.transform.position) <= 1f)
            {
                DepositResources();
            }

            // 如果到达了目标建筑，进行消费
            if (targetConsumptionBuilding != null && 
                Vector3.Distance(transform.position, targetConsumptionBuilding.transform.position) <= 1f)
            {
                ConsumeAtBuilding();
            }
        }

        private void UpdateNeeds()
        {
            if (Time.time - lastNeedUpdateTime < needUpdateInterval)
                return;

            lastNeedUpdateTime = Time.time;

            // 更新所有需求值
            needs[NeedType.Food].value = Mathf.Min(needs[NeedType.Food].value + 2f * needUpdateInterval, 100f);
            needs[NeedType.Sleep].value = Mathf.Max(needs[NeedType.Sleep].value - 1f * needUpdateInterval, 0f);
            needs[NeedType.Entertainment].value = Mathf.Max(needs[NeedType.Entertainment].value - 1.5f * needUpdateInterval, 0f);
            needs[NeedType.Social].value = Mathf.Max(needs[NeedType.Social].value - 1f * needUpdateInterval, 0f);
            needs[NeedType.Health].value = Mathf.Max(needs[NeedType.Health].value - 0.5f * needUpdateInterval, 0f);
            needs[NeedType.Culture].value = Mathf.Max(needs[NeedType.Culture].value - 0.8f * needUpdateInterval, 0f);

            // 检查是否需要消费
            if (Time.time - lastConsumptionTime > consumptionCooldown)
            {
                CheckConsumptionNeeds();
            }
        }

        private void CheckConsumptionNeeds()
        {
            if (targetConsumptionBuilding != null)
                return;

            // 获取最迫切的需求
            var mostUrgentNeed = GetMostUrgentNeed();
            if (mostUrgentNeed.Value.value > GetNeedThreshold(mostUrgentNeed.Key))
            {
                currentNeed = mostUrgentNeed.Key;
                FindConsumptionBuilding(currentNeed);
            }
        }

        private KeyValuePair<NeedType, Need> GetMostUrgentNeed()
        {
            return needs.OrderByDescending(n => n.Value.value).First();
        }

        private float GetNeedThreshold(NeedType type)
        {
            return type switch
            {
                NeedType.Food => 70f,
                NeedType.Sleep => 30f,
                NeedType.Entertainment => 30f,
                NeedType.Social => 40f,
                NeedType.Health => 40f,
                NeedType.Culture => 50f,
                _ => 50f
            };
        }

        private void FindConsumptionBuilding(NeedType need)
        {
            Building[] buildings = FindObjectsOfType<Building>();
            float nearestDistance = float.MaxValue;
            Building nearestBuilding = null;

            foreach (Building building in buildings)
            {
                if (IsBuildingSuitableForNeed(building, need))
                {
                    float distance = Vector3.Distance(transform.position, building.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestBuilding = building;
                    }
                }
            }

            if (nearestBuilding != null)
            {
                targetConsumptionBuilding = nearestBuilding;
                Move(nearestBuilding.transform.position);
            }
        }

        private bool IsBuildingSuitableForNeed(Building building, NeedType need)
        {
            return (building.type, need) switch
            {
                (BuildingType.Restaurant, NeedType.Food) => building.HasResource(ResourceType.ProcessedFood,1f),
                (BuildingType.Hospital, NeedType.Health) => true,
                (BuildingType.Entertainment, NeedType.Entertainment) => true,
                (BuildingType.CulturalCenter, NeedType.Culture) => true,
                (BuildingType.SocialCenter, NeedType.Social) => true,
                _ => false
            };
        }

        private void ConsumeAtBuilding()
        {
            if (targetConsumptionBuilding == null || currentNeed == NeedType.None)
                return;

            // 根据需求类型消费资源
            switch (currentNeed)
            {
                case NeedType.Food:
                    if (targetConsumptionBuilding.type == BuildingType.Restaurant)
                    {
                        float foodAmount = 20f;
                        if (targetConsumptionBuilding.RequestResource(ResourceType.ProcessedFood, foodAmount) > 0)
                        {
                            needs[NeedType.Food].value = Mathf.Max(0, needs[NeedType.Food].value - 40f);
                            happiness += 10f;
                        }
                    }
                    break;

                case NeedType.Health:
                    if (targetConsumptionBuilding.type == BuildingType.Hospital)
                    {
                        needs[NeedType.Health].value = Mathf.Min(needs[NeedType.Health].value + 30f, 100f);
                        happiness += 5f;
                    }
                    break;

                case NeedType.Entertainment:
                    if (targetConsumptionBuilding.type == BuildingType.Entertainment)
                    {
                        needs[NeedType.Entertainment].value = Mathf.Min(needs[NeedType.Entertainment].value + 40f, 100f);
                        happiness += 15f;
                    }
                    break;

                case NeedType.Culture:
                    if (targetConsumptionBuilding.type == BuildingType.CulturalCenter)
                    {
                        needs[NeedType.Culture].value = Mathf.Min(needs[NeedType.Culture].value + 35f, 100f);
                        happiness += 12f;
                        education += 2f;
                    }
                    break;

                case NeedType.Social:
                    if (targetConsumptionBuilding.type == BuildingType.SocialCenter)
                    {
                        needs[NeedType.Social].value = Mathf.Min(needs[NeedType.Social].value + 30f, 100f);
                        happiness += 8f;
                    }
                    break;
            }

            // 重置消费相关变量
            lastConsumptionTime = Time.time;
            targetConsumptionBuilding = null;
            currentNeed = NeedType.None;
        }

        private void UpdateBehavior()
        {
            switch (currentState)
            {
                case State.Wandering:
                    Wander();
                    LookForResources();
                    
                    break;

                case State.MovingToResource:
                    if (targetResource == null)
                    {
                        currentState = State.Wandering;
                        break;
                    }

                    MoveToTarget(targetResource.transform.position);

                    if (Vector3.Distance(transform.position, targetResource.transform.position) < interactionRange)
                    {
                        currentState = State.CollectingResource;
                    }
                    break;

                case State.CollectingResource:
                    if (targetResource == null)
                    {
                        currentState = State.Wandering;
                        break;
                    }

                    currentState = State.Wandering;
                    break;
            }
        }

        private void Wander()
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                Debug.Log($"Agent {gameObject.name} SetRandomTarget");
                SetRandomTarget();
                Debug.Log($"Agent {gameObject.name} Wander to {targetPosition}");
            }

            Move();
        }

        private void LookForResources()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, resourceDetectionRange);
            float closestDistance = float.MaxValue;
            ResourceObject closestResource = null;

            foreach (Collider collider in colliders)
            {
                ResourceObject resource = collider.GetComponent<ResourceObject>();
                if (resource != null)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestResource = resource;
                    }
                }
            }

            if (closestResource != null)
            {
                targetResource = closestResource;
                currentState = State.MovingToResource;
            }
        }

        private void MoveToTarget(Vector3 target)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, 5f, NavMesh.AllAreas))
            {
                targetPosition = hit.position;
                Move();
            }
            else
            {
                Debug.LogWarning($"Agent {gameObject.name} 无法找到到达目标的有效路径");
                currentState = State.Wandering;
            }
        }

        private void Move()
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            moveDirection.y = 0;

            // 检查是否超出活动范围
            Vector3 nextPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            if (Mathf.Abs(nextPosition.x) > wanderRadius || Mathf.Abs(nextPosition.z) > wanderRadius)
            {
                Vector3 toCenter = Vector3.zero;
                SetRandomTarget();
                return;
            }

            // 使用NavMeshAgent设置目标位置
            navMeshAgent.SetDestination(targetPosition);

            // 更新视觉朝向
            if (navMeshAgent.velocity.sqrMagnitude > 0.1f)
            {
                visualTransform.rotation = Quaternion.Lerp(
                    visualTransform.rotation,
                    Quaternion.LookRotation(navMeshAgent.velocity.normalized),
                    Time.deltaTime * rotateSpeed
                );
            }
        }

        private void Move(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        private void SetRandomTarget()
        {
            float range = wanderRadius * 0.8f;
            int maxAttempts = 5;
            Vector3 newTarget = Vector3.zero;
            bool validTarget = false;

            for (int i = 0; i < maxAttempts; i++)
            {
                float randomAngle = Random.Range(0f, 360f);
                float randomRadius = Random.Range(2f, range);
                newTarget = new Vector3(
                    Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomRadius,
                    0,
                    Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomRadius
                );

                // 使用NavMesh.SamplePosition找到最近的可行走位置
                NavMeshHit hit;
                if (NavMesh.SamplePosition(newTarget, out hit, 5f, NavMesh.AllAreas))
                {
                    newTarget = hit.position;
                    validTarget = true;
                    break;
                }
            }

            // 如果找不到有效位置，使用当前位置附近的可行走点
            if (!validTarget)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * 5f, out hit, 5f, NavMesh.AllAreas))
                {
                    newTarget = hit.position;
                }

            }

            targetPosition = newTarget;
            
            // 随机返回中心点
            if (Random.value < 0.3f)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(Vector3.zero, out hit, 5f, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                }
            }
        }

        #region 资源管理

        public Dictionary<ResourceType, float> GetInventory()
        {
            return inventory;
        }

        public bool HasResource(ResourceType type, float amount)
        {
            return inventory.ContainsKey(type) && inventory[type] >= amount;
        }

        public void AddResource(ResourceType type, float amount)
        {
            if (!inventory.ContainsKey(type))
            {
                inventory[type] = 0;
            }

            var resourceDef = ResourceSystem.Instance.resourceDefinitions[type];
            float availableSpace = resourceDef.maxStorage - inventory[type];
            float actualAmount = Mathf.Min(amount, availableSpace);

            inventory[type] += actualAmount;

            // 更新市场数据
            if (ResourceSystem.Instance.market.ContainsKey(type))
            {
                ResourceSystem.Instance.market[type].supply += actualAmount;
            }
        }

        public bool ConsumeResource(ResourceType type, float amount)
        {
            if (HasResource(type, amount))
            {
                inventory[type] -= amount;
                
                // 更新市场数据
                if (ResourceSystem.Instance.market.ContainsKey(type))
                {
                    ResourceSystem.Instance.market[type].demand += amount;
                }
                
                return true;
            }
            return false;
        }

        private void UpdateResources()
        {
            // 资源自然损耗
            foreach (var resource in inventory.Keys.ToList())
            {
                var definition = ResourceSystem.Instance.resourceDefinitions[resource];
                float deterioration = definition.deteriorationRate * Time.deltaTime;
                inventory[resource] = Mathf.Max(0, inventory[resource] - deterioration);
            }

            // 更新资源转换进度
            if (currentConversion != null)
            {
                conversionProgress += Time.deltaTime;
                if (conversionProgress >= currentConversion.conversionTime)
                {
                    CompleteResourceConversion();
                }
            }
            else
            {
                // 尝试开始新的资源转换
                TryStartResourceConversion();
            }

            // 尝试交易
            if (Time.time >= lastTradeTime + tradeInterval)
            {
                TryTrade();
                lastTradeTime = Time.time;
            }
        }

        private void TryStartResourceConversion()
        {
            foreach (var rule in ResourceSystem.Instance.conversionRules)
            {
                if (ResourceSystem.Instance.CanConvert(this, rule))
                {
                    currentConversion = rule;
                    conversionProgress = 0f;
                    break;
                }
            }
        }

        private void CompleteResourceConversion()
        {
            if (currentConversion != null)
            {
                ResourceSystem.Instance.ConvertResources(this, currentConversion);
                currentConversion = null;
                conversionProgress = 0f;
            }
        }

        private void TryTrade()
        {
            // 寻找附近的交易对象
            var nearbyAgents = Physics.OverlapSphere(transform.position, 5f)
                .Select(c => c.GetComponent<Agent>())
                .Where(a => a != null && a != this)
                .ToList();

            foreach (var other in nearbyAgents)
            {
                // 检查是否有可以交易的资源
                foreach (var resource in inventory)
                {
                    if (resource.Value > 0)
                    {
                        float price = ResourceSystem.Instance.GetResourcePrice(resource.Key);
                        float amount = Mathf.Min(resource.Value * 0.1f, 10f); // 交易10%的资源，最多10单位

                        if (other.HasResource(ResourceType.Money, price * amount))
                        {
                            ResourceSystem.Instance.Trade(this, other, resource.Key, amount);
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region 社交系统
        protected virtual void InitializeNeeds()
        {
            // 初始化基本需求
            needs[NeedType.Food].value = Random.Range(20f, 40f);      // 饥饿度
            needs[NeedType.Sleep].value = Random.Range(60f, 80f);      // 能量
            needs[NeedType.Entertainment].value = Random.Range(40f, 60f); // 娱乐需求
            needs[NeedType.Social].value = Random.Range(30f, 70f);      // 社交需求
            needs[NeedType.Health].value = Random.Range(70f, 90f);      // 健康
            needs[NeedType.Culture].value = Random.Range(40f, 60f);     // 文化需求
        }

        protected virtual void InitializePersonality()
        {
            personality = new Personality();
        }

        protected virtual void InitializeSkills()
        {
            skills.Add(new Skill("交际", "与他人交往的能力"));
            skills.Add(new Skill("工作", "完成工作任务的能力"));
            skills.Add(new Skill("学习", "获取新知识的能力"));
        }

        protected virtual void InitializeResources()
        {
            resources[ResourceType.Money] = new Resource(ResourceType.Money, wealth);
            resources[ResourceType.Food] = new Resource(ResourceType.Food, Random.Range(10f, 50f));
            resources[ResourceType.Water] = new Resource(ResourceType.Water, Random.Range(10f, 50f));
        }
        #endregion

        #region 思想体系
        private void InitializeIdeologySystem()
        {
            dna = new DNA();
            
            // 初始化意识形态倾向
            foreach (IdeologyType ideology in System.Enum.GetValues(typeof(IdeologyType)))
            {
                ideologies[ideology] = Random.Range(0f, 100f);
            }

            // 生成初始信念
            GenerateInitialBeliefs();
        }

        private void InitializeSocialNetwork()
        {
            socialNetwork.Clear();
            networkInfluence = CalculateNetworkInfluence();
        }

        private void GenerateInitialBeliefs()
        {
            // 基于DNA和意识形态生成初始信念
            if (dna.values["tradition"] > 0.7f)
            {
                beliefs.Add("传统价值观念应该得到保护");
            }
            if (dna.values["innovation"] > 0.7f)
            {
                beliefs.Add("创新是社会进步的动力");
            }
            // 添加更多信念生成逻辑...
        }

        private float CalculateNetworkInfluence()
        {
            float influence = 0f;
            foreach (var relation in socialNetwork.Values)
            {
                influence += relation.influence * relation.trust;
            }
            return Mathf.Min(influence, 100f);
        }

        public void AddSocialRelation(string targetId, RelationType type)
        {
            if (!socialNetwork.ContainsKey(targetId) && socialNetwork.Count < maxRelations)
            {
                var relation = new SocialRelation(targetId, type);
                socialNetwork[targetId] = relation;
                Debug.Log($"Agent {agentName} 建立了新的{type}关系");
            }
        }

        public void UpdateRelations(float deltaTime)
        {
            foreach (var relation in socialNetwork.Values)
            {
                relation.UpdateRelation(deltaTime);
            }
            networkInfluence = CalculateNetworkInfluence();
        }

        private void UpdateIdeology()
        {
            // 思想体系随时间和经历演变
            foreach (var ideology in ideologies.Keys.ToList())
            {
                float change = Random.Range(-0.1f, 0.1f);
                ideologies[ideology] = Mathf.Clamp(ideologies[ideology] + change, 0f, 100f);
            }

            // 更新社会影响力
            socialInfluence = CalculateSocialInfluence();
        }

        private float CalculateSocialInfluence()
        {
            float influence = 0f;
            influence += networkInfluence * 0.3f;
            influence += education * 0.2f;
            influence += wealth * 0.0001f;
            influence += prestige * 0.3f;
            return Mathf.Min(influence, 100f);
        }

        public void AddMemory(string memory)
        {
            memories.Add($"[{System.DateTime.Now}] {memory}");
            if (memories.Count > 100) // 限制记忆数量
            {
                memories.RemoveAt(0);
            }
        }

        public void InteractWith(Agent other)
        {
            if (other == null) return;

            // 检查或创建社会关系
            string otherId = other.agentId;
            if (!socialNetwork.ContainsKey(otherId))
            {
                AddSocialRelation(otherId, RelationType.Friend);
            }

            var relation = socialNetwork[otherId];

            // 计算互动效果
            float compatibilityScore = CalculateCompatibility(other);
            float ideologyAlignment = CalculateIdeologyAlignment(other);

            // 更新关系参数
            float intimacyChange = compatibilityScore * 0.1f;
            float trustChange = (compatibilityScore + ideologyAlignment) * 0.05f;
            
            relation.intimacy = Mathf.Clamp01(relation.intimacy + intimacyChange);
            relation.trust = Mathf.Clamp01(relation.trust + trustChange);

            // 记录互动
            string interaction = $"与{other.agentName}进行了社交互动";
            relation.interactions.Add(interaction);
            AddMemory(interaction);

            // 思想交流
            ExchangeIdeologies(other);
        }

        private float CalculateCompatibility(Agent other)
        {
            float compatibility = 0f;
            foreach (var trait in dna.personalityTraits)
            {
                float diff = Mathf.Abs(trait.Value - other.dna.personalityTraits[trait.Key]);
                compatibility += 1f - diff;
            }
            return compatibility / dna.personalityTraits.Count;
        }

        private float CalculateIdeologyAlignment(Agent other)
        {
            float alignment = 0f;
            foreach (var ideology in ideologies)
            {
                float diff = Mathf.Abs(ideology.Value - other.ideologies[ideology.Key]);
                alignment += 1f - (diff / 100f);
            }
            return alignment / ideologies.Count;
        }

        private void ExchangeIdeologies(Agent other)
        {
            foreach (var ideology in ideologies.Keys.ToList())
            {
                float myInfluence = socialInfluence / 100f;
                float theirInfluence = other.socialInfluence / 100f;
                float myValue = ideologies[ideology];
                float theirValue = other.ideologies[ideology];

                // 思想交流，影响力越大的个体影响力越强
                float newMyValue = Mathf.Lerp(myValue, theirValue, theirInfluence * 0.1f);
                float newTheirValue = Mathf.Lerp(theirValue, myValue, myInfluence * 0.1f);

                ideologies[ideology] = newMyValue;
                other.ideologies[ideology] = newTheirValue;
            }
        }
        #endregion

        private void InitializeCareer()
        {
            // 根据DNA和社会背景选择初始职业
            float educationLevel = dna.personalityTraits["intelligence"];
            float ambition = dna.personalityTraits["ambition"];
            
            // 初始化基础技能
            skillLevels["physical_labor"] = Random.Range(0.5f, 1.5f);
            skillLevels["communication"] = Random.Range(0.5f, 1.5f);
            skillLevels["problem_solving"] = Random.Range(0.5f, 1.5f);
            
            // 根据教育水平添加额外技能
            if (educationLevel > 0.6f)
            {
                skillLevels["technology"] = Random.Range(0.3f, 1.0f);
                skillLevels["management"] = Random.Range(0.3f, 1.0f);
            }

            // 设置创新驱动力和文化认同
            innovationDrive = (dna.personalityTraits["openness"] + dna.personalityTraits["intelligence"]) / 2f;
            culturalIdentity = Random.Range(0.3f, 1.0f);

            // 选择初始职业
            SelectInitialCareer();
        }

        private void SelectInitialCareer()
        {
            // 根据个人特质选择适合的初始职业
            float education = dna.personalityTraits["intelligence"];
            float social = dna.personalityTraits["extraversion"];
            
            if (education > 0.7f)
            {
                currentCareerId = "programmer"; // 高教育水平选择技术职业
            }
            else if (social > 0.7f)
            {
                currentCareerId = "salesperson"; // 高社交能力选择服务业
            }
            else
            {
                currentCareerId = "factory_worker"; // 默认选择工业职业
            }

            careerExperience = 0f;
            jobSatisfaction = Random.Range(0.5f, 0.8f);
        }

        public void UpdateCareer()
        {
            if (Time.time - lastCareerUpdateTime < careerUpdateInterval)
                return;

            lastCareerUpdateTime = Time.time;

            // 增加职业经验
            careerExperience += Random.Range(0.1f, 0.3f);

            // 更新技能水平
            foreach (var skill in skillLevels.Keys.ToList())
            {
                skillLevels[skill] += Random.Range(0.01f, 0.05f) * jobSatisfaction;
            }

            // 检查是否可以晋升
            string nextCareerId;
            if (CareerSystem.Instance.CanPromote(this, currentCareerId, out nextCareerId))
            {
                ConsiderPromotion(nextCareerId);
            }

            // 更新工作满意度
            UpdateJobSatisfaction();
        }

        private void ConsiderPromotion(string newCareerId)
        {
            var currentCareer = CareerSystem.Instance.careerDefinitions[currentCareerId];
            var newCareer = CareerSystem.Instance.careerDefinitions[newCareerId];

            // 计算接受晋升的概率
            float acceptanceChance = CalculatePromotionAcceptance(currentCareer, newCareer);

            if (Random.value < acceptanceChance)
            {
                // 接受晋升
                currentCareerId = newCareerId;
                careerExperience = 0f;
                jobSatisfaction = Random.Range(0.7f, 1.0f);
                
                // 记录晋升事件
                Debug.Log($"Agent {gameObject.name} promoted to {newCareer.name}");
            }
        }

        private float CalculatePromotionAcceptance(CareerData current, CareerData next)
        {
            float baseChance = 0.7f; // 基础接受概率

            // 根据个性特征调整概率
            baseChance += dna.personalityTraits["ambition"] * 0.2f;
            baseChance += dna.personalityTraits["openness"] * 0.1f;

            // 根据薪资增长调整概率
            float salaryIncrease = (next.baseSalary - current.baseSalary) / current.baseSalary;
            baseChance += salaryIncrease * 0.2f;

            // 根据社会地位变化调整概率
            float statusChange = (next.socialStatus - current.socialStatus) / current.socialStatus;
            baseChance += statusChange * 0.1f;

            return Mathf.Clamp01(baseChance);
        }

        private void UpdateJobSatisfaction()
        {
            var career = CareerSystem.Instance.careerDefinitions[currentCareerId];
            
            // 基于多个因素更新工作满意度
            float satisfactionDelta = 0;

            // 技能匹配度对满意度的影响
            float skillMatch = CalculateSkillMatch(career);
            satisfactionDelta += (skillMatch - 0.5f) * 0.1f;

            // 个性特征对满意度的影响
            float personalityMatch = CalculatePersonalityMatch(career);
            satisfactionDelta += (personalityMatch - 0.5f) * 0.1f;

            // 社会地位对满意度的影响
            satisfactionDelta += (career.socialStatus / 100f - 0.5f) * 0.05f;

            // 更新满意度
            jobSatisfaction = Mathf.Clamp01(jobSatisfaction + satisfactionDelta);
        }

        private float CalculateSkillMatch(CareerData career)
        {
            float totalMatch = 0f;
            int skillCount = 0;

            foreach (var skillReq in career.skillLevels)
            {
                if (skillLevels.ContainsKey(skillReq.Key))
                {
                    totalMatch += Mathf.Min(1f, skillLevels[skillReq.Key] / skillReq.Value);
                    skillCount++;
                }
            }

            return skillCount > 0 ? totalMatch / skillCount : 0f;
        }

        private float CalculatePersonalityMatch(CareerData career)
        {
            float match = 0.5f; // 基础匹配度

            // 根据职业领域调整匹配度
            switch (career.field)
            {
                case CareerField.Art:
                    match += dna.personalityTraits["openness"] * 0.3f;
                    break;
                case CareerField.Technology:
                    match += dna.personalityTraits["intelligence"] * 0.3f;
                    break;
                case CareerField.Service:
                    match += dna.personalityTraits["extraversion"] * 0.3f;
                    break;
                // 添加更多职业领域的匹配规则
            }

            return Mathf.Clamp01(match);
        }

        public float GetSkillLevel(string skillName)
        {
            return skillLevels.ContainsKey(skillName) ? skillLevels[skillName] : 0f;
        }

        public void AddExperience(string skillName, float amount)
        {
            if (!skillLevels.ContainsKey(skillName))
            {
                skillLevels[skillName] = 0f;
            }
            skillLevels[skillName] += amount;
        }

        private void UpdateResourceCollection()
        {
            if (Time.time - lastResourceCheckTime < resourceCheckInterval)
                return;

            lastResourceCheckTime = Time.time;

            // 如果没有目标资源，寻找新的资源
            if (targetResource == null || !targetResource.CanBeHarvested())
            {
                FindNewResource();
            }

            // 如果有目标资源，进行收集
            if (targetResource != null && targetResource.CanBeHarvested())
            {
                // 移动到资源位置
                Vector3 resourcePosition = targetResource.transform.position;
                if (Vector3.Distance(transform.position, resourcePosition) > 1f)
                {
                    Move(resourcePosition);
                }
                else
                {
                    // 收集资源
                    CollectResource();
                }
            }

            // 如果背包接近满了，寻找建筑存放资源
            if (IsInventoryNearFull())
            {
                FindBuildingForDeposit();
            }
        }

        private void FindNewResource()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, resourceDetectionRange);
            float nearestDistance = float.MaxValue;
            ResourceObject nearestResource = null;

            foreach (Collider collider in colliders)
            {
                ResourceObject resource = collider.GetComponent<ResourceObject>();
                if (resource != null && resource.CanBeHarvested())
                {
                    float distance = Vector3.Distance(transform.position, resource.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestResource = resource;
                    }
                }
            }

            if (nearestResource != null)
            {
                targetResource = nearestResource;
                targetResource.SetHarvestState(true);
            }
        }

        private void CollectResource()
        {
            if (targetResource == null)
                return;

            float spaceAvailable = carryCapacity - GetTotalInventoryWeight();
            if (spaceAvailable <= 0)
                return;

            float collected = targetResource.Harvest(resourceCollectionSpeed * Time.deltaTime);
            if (collected > 0)
            {
                // 添加到库存
                if (!inventory.ContainsKey(targetResource.type))
                {
                    inventory[targetResource.type] = 0f;
                }
                inventory[targetResource.type] += collected;

                // 如果资源已经采集完或背包满了，重置目标
                if (targetResource.amount <= 0 || IsInventoryNearFull())
                {
                    targetResource.SetHarvestState(false);
                    targetResource = null;
                }
            }
        }

        private float GetTotalInventoryWeight()
        {
            float total = 0f;
            foreach (var item in inventory)
            {
                total += item.Value;
            }
            return total;
        }

        private bool IsInventoryNearFull()
        {
            return GetTotalInventoryWeight() >= carryCapacity * 0.8f;
        }

        private void FindBuildingForDeposit()
        {
            if (assignedBuilding != null)
                return;

            // 寻找最近的可以接收资源的建筑
            Building[] buildings = FindObjectsOfType<Building>();
            float nearestDistance = float.MaxValue;
            Building nearestBuilding = null;

            foreach (Building building in buildings)
            {
                // 检查建筑是否可以接收我们的资源
                bool canAcceptResources = false;
                foreach (var item in inventory)
                {
                    if (item.Value > 0 && building.CanAcceptResource(item.Key, item.Value))
                    {
                        canAcceptResources = true;
                        break;
                    }
                }

                if (canAcceptResources)
                {
                    float distance = Vector3.Distance(transform.position, building.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestBuilding = building;
                    }
                }
            }

            if (nearestBuilding != null)
            {
                assignedBuilding = nearestBuilding;
                // 移动到建筑位置
                Move(nearestBuilding.transform.position);
            }
        }

        private void DepositResources()
        {
            if (assignedBuilding == null)
                return;

            // 尝试存放所有资源
            foreach (var item in inventory.ToList())
            {
                if (item.Value > 0 && assignedBuilding.CanAcceptResource(item.Key, item.Value))
                {
                    assignedBuilding.AddResource(item.Key, item.Value);
                    inventory[item.Key] = 0f;
                }
            }

            // 完成存放后重置目标建筑
            assignedBuilding = null;
        }
    }
} 