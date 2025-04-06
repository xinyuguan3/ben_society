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
        #endregion

        #region 资源系统
        [Header("Resources")]
        public Dictionary<ResourceType, Resource> resources = new Dictionary<ResourceType, Resource>();
        private Dictionary<ResourceType, float> inventory = new Dictionary<ResourceType, float>();
        #endregion

        #region 行为系统
        [Header("Behavior")]
        public float moveSpeed = 2f;
        public float rotateSpeed = 10f;
        public float resourceDetectionRange = 5f;
        public float interactionRange = 1.5f;
        public float wanderRadius = 10f;
        private State currentState = State.Wandering;
        private GameObject targetResource;
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
            InitializeInventory();
            
            // 设置外观
            CreateVisuals();

            // 初始化DNA和思想体系
            InitializeIdeologySystem();

            // 初始化社会关系网络
            InitializeSocialNetwork();
        }

        private void InitializeInventory()
        {
            foreach (ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
            {
                inventory[resource] = 0f;
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
        }

        private void UpdateNeeds()
        {
            foreach (var need in needs.Values)
            {
                need.Update(Time.deltaTime);
            }

            // 基础需求更新
            hunger = Mathf.Min(hunger + Time.deltaTime * 0.1f, 100f);
            energy = Mathf.Max(energy - Time.deltaTime * 0.05f, 0f);
            happiness = Mathf.Lerp(happiness, 50f, Time.deltaTime * 0.01f);

            if (hunger > 80f)
            {
                if (ConsumeResource(ResourceType.Food, 10f))
                {
                    hunger -= 30f;
                    happiness += 5f;
                }
            }
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
            GameObject closestResource = null;

            foreach (Collider collider in colliders)
            {
                ResourceObject resource = collider.GetComponent<ResourceObject>();
                if (resource != null)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestResource = collider.gameObject;
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
        public void AddResource(ResourceType type, float amount)
        {
            if (inventory.ContainsKey(type))
            {
                inventory[type] += amount;
            }
        }

        public bool ConsumeResource(ResourceType type, float amount)
        {
            if (inventory.ContainsKey(type) && inventory[type] >= amount)
            {
                inventory[type] -= amount;
                return true;
            }
            return false;
        }

        public Dictionary<ResourceType, float> GetInventory()
        {
            return new Dictionary<ResourceType, float>(inventory);
        }
        #endregion

        #region 社交系统
        protected virtual void InitializeNeeds()
        {
            foreach (NeedType needType in System.Enum.GetValues(typeof(NeedType)))
            {
                needs[needType] = new Need(needType);
            }
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
    }
} 