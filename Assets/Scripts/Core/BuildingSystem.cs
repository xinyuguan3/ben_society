using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

namespace CamelSociety.Core
{
    public class BuildingRequirements
    {
        public Dictionary<ResourceType, float> constructionCosts;
        public int requiredWorkers;
        public Dictionary<string, float> requiredSkills;
        public float constructionTime;
        public float maintenanceCost;

        public BuildingRequirements()
        {
            constructionCosts = new Dictionary<ResourceType, float>();
            requiredSkills = new Dictionary<string, float>();
        }
    }

    public class BuildingSystem : MonoBehaviour
    {
        public static BuildingSystem Instance { get; private set; }

        [Header("Building Settings")]
        public Dictionary<BuildingType, BuildingRequirements> buildingRequirements;
        public Dictionary<BuildingType, GameObject> buildingPrefabs;
        public float minBuildingDistance = 10f;
        public float cityRadius = 100f;

        [Header("Planning")]
        private List<Vector3> plannedLocations = new List<Vector3>();
        private Dictionary<BuildingType, int> buildingCounts = new Dictionary<BuildingType, int>();
        private Dictionary<BuildingType, int> targetBuildingCounts = new Dictionary<BuildingType, int>();

        [Header("Construction")]
        private List<Building> buildingsUnderConstruction = new List<Building>();
        private List<Agent> constructionWorkers = new List<Agent>();
        private float constructionUpdateInterval = 1f;
        private float lastConstructionUpdate;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeBuildingSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeBuildingSystem()
        {
            InitializeBuildingRequirements();
            InitializeTargetCounts();
            buildingCounts.Clear();
            foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
            {
                buildingCounts[type] = 0;
            }
        }

        private void InitializeBuildingRequirements()
        {
            buildingRequirements = new Dictionary<BuildingType, BuildingRequirements>();

            // 初始化每种建筑类型的需求
            foreach (BuildingType type in System.Enum.GetValues(typeof(BuildingType)))
            {
                var req = new BuildingRequirements();
                switch (type)
                {
                    case BuildingType.House:
                        req.constructionCosts[ResourceType.Wood] = 100f;
                        req.constructionCosts[ResourceType.Stone] = 50f;
                        req.requiredWorkers = 2;
                        req.requiredSkills["construction"] = 0.3f;
                        req.constructionTime = 30f;
                        req.maintenanceCost = 5f;
                        break;

                    case BuildingType.Restaurant:
                        req.constructionCosts[ResourceType.Wood] = 150f;
                        req.constructionCosts[ResourceType.Stone] = 100f;
                        req.requiredWorkers = 3;
                        req.requiredSkills["cooking"] = 0.5f;
                        req.constructionTime = 40f;
                        req.maintenanceCost = 10f;
                        break;

                    case BuildingType.Factory:
                        req.constructionCosts[ResourceType.Metal] = 200f;
                        req.constructionCosts[ResourceType.Stone] = 150f;
                        req.requiredWorkers = 5;
                        req.requiredSkills["manufacturing"] = 0.6f;
                        req.constructionTime = 60f;
                        req.maintenanceCost = 20f;
                        break;

                    // 添加其他建筑类型的需求...
                }
                buildingRequirements[type] = req;
            }
        }

        private void InitializeTargetCounts()
        {
            targetBuildingCounts[BuildingType.House] = 10;
            targetBuildingCounts[BuildingType.Restaurant] = 3;
            targetBuildingCounts[BuildingType.Hospital] = 2;
            targetBuildingCounts[BuildingType.Factory] = 3;
            targetBuildingCounts[BuildingType.Farm] = 4;
            targetBuildingCounts[BuildingType.Market] = 2;
            targetBuildingCounts[BuildingType.School] = 2;
            targetBuildingCounts[BuildingType.Entertainment] = 2;
            targetBuildingCounts[BuildingType.CulturalCenter] = 1;
            targetBuildingCounts[BuildingType.SocialCenter] = 2;
            targetBuildingCounts[BuildingType.WoodCutter] = 2;
            targetBuildingCounts[BuildingType.Mine] = 2;
            targetBuildingCounts[BuildingType.Quarry] = 2;
            targetBuildingCounts[BuildingType.Well] = 2;
            targetBuildingCounts[BuildingType.Workshop] = 2;
            targetBuildingCounts[BuildingType.Forge] = 2;
            
        }

        private void Update()
        {
            if (Time.time - lastConstructionUpdate >= constructionUpdateInterval)
            {
                lastConstructionUpdate = Time.time;
                UpdateConstruction();
                PlanNewBuildings();
                AssignWorkersToBuildings();
            }
        }

        private void PlanNewBuildings()
        {
            foreach (var target in targetBuildingCounts)
            {
                if (!buildingCounts.ContainsKey(target.Key))
                    buildingCounts[target.Key] = 0;

                int deficit = target.Value - buildingCounts[target.Key];
                if (deficit > 0)
                {
                    // 检查资源是否足够
                    if (HasSufficientResources(target.Key))
                    {
                        Vector3? location = FindBuildingLocation(target.Key);
                        if (location.HasValue)
                        {
                            StartConstruction(target.Key, location.Value);
                        }
                    }
                }
            }
        }

        private bool HasSufficientResources(BuildingType type)
        {
            var requirements = buildingRequirements[type];
            var societyResources = ResourceSystem.Instance.GetTotalResources();

            foreach (var cost in requirements.constructionCosts)
            {
                if (!societyResources.ContainsKey(cost.Key) || 
                    societyResources[cost.Key] < cost.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private Vector3? FindBuildingLocation(BuildingType type)
        {
            int maxAttempts = 30;
            float buildingSize = 5f; // 假设建筑物占地5x5

            for (int i = 0; i < maxAttempts; i++)
            {
                // 生成随机位置
                float angle = Random.Range(0f, 360f);
                float radius = Random.Range(10f, cityRadius);
                Vector3 position = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    0f,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                );

                // 检查位置是否合适
                if (IsLocationSuitable(position, buildingSize))
                {
                    // 确保位置在NavMesh上
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(position, out hit, 5f, NavMesh.AllAreas))
                    {
                        plannedLocations.Add(hit.position);
                        return hit.position;
                    }
                }
            }

            return null;
        }

        private bool IsLocationSuitable(Vector3 position, float size)
        {
            // 检查是否与其他建筑物太近
            foreach (Vector3 existingLocation in plannedLocations)
            {
                if (Vector3.Distance(position, existingLocation) < minBuildingDistance)
                {
                    return false;
                }
            }

            // 检查地形是否平坦
            if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out RaycastHit hit))
            {
                float maxSlope = 15f;
                return Vector3.Angle(hit.normal, Vector3.up) <= maxSlope;
            }

            return false;
        }

        private void StartConstruction(BuildingType type, Vector3 location)
        {
            // 创建建筑物实例
            GameObject buildingObj = Instantiate(buildingPrefabs[type], location, Quaternion.identity);
            Building building = buildingObj.GetComponent<Building>();
            
            if (building != null)
            {
                building.Initialize(type, buildingRequirements[type]);
                buildingsUnderConstruction.Add(building);
                
                // 扣除建筑成本
                foreach (var cost in buildingRequirements[type].constructionCosts)
                {
                    ResourceSystem.Instance.ConsumeResource(cost.Key, cost.Value);
                }

                buildingCounts[type]++;
                Debug.Log($"开始建造 {type} 在位置 {location}");
            }
        }

        private void UpdateConstruction()
        {
            for (int i = buildingsUnderConstruction.Count - 1; i >= 0; i--)
            {
                Building building = buildingsUnderConstruction[i];
                if (building.constructionProgress >= 1f)
                {
                    building.SetState(BuildingState.Operating);
                    buildingsUnderConstruction.RemoveAt(i);
                    Debug.Log($"建筑 {building.type} 完工");
                }
                else
                {
                    // 更新建造进度
                    float progressIncrease = (1f / building.requirements.constructionTime) * constructionUpdateInterval;
                    building.constructionProgress += progressIncrease;
                }
            }
        }

        private void AssignWorkersToBuildings()
        {
            var allBuildings = FindObjectsOfType<Building>();
            var allAgents = FindObjectsOfType<Agent>();
            var availableWorkers = new List<Agent>(allAgents);

            foreach (var building in allBuildings)
            {
                if (building.state != BuildingState.Operating)
                    continue;

                int workerDeficit = building.requirements.requiredWorkers - building.assignedWorkers.Count;
                if (workerDeficit <= 0)
                    continue;

                // 寻找合适的工人
                for (int i = availableWorkers.Count - 1; i >= 0 && workerDeficit > 0; i--)
                {
                    Agent worker = availableWorkers[i];
                    if (IsWorkerSuitable(worker, building))
                    {
                        building.AssignWorker(worker);
                        availableWorkers.RemoveAt(i);
                        workerDeficit--;
                    }
                }
            }
        }

        private bool IsWorkerSuitable(Agent worker, Building building)
        {
            // 检查工人是否已经被分配
            if (worker.assignedBuilding != null)
                return false;

            // 检查工人的技能是否符合要求
            foreach (var requiredSkill in building.requirements.requiredSkills)
            {
                if (worker.GetSkillLevel(requiredSkill.Key) < requiredSkill.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public void RegisterBuilding(Building building)
        {
            if (!buildingCounts.ContainsKey(building.type))
            {
                buildingCounts[building.type] = 0;
            }
            buildingCounts[building.type]++;
        }

        public void UnregisterBuilding(Building building)
        {
            if (buildingCounts.ContainsKey(building.type))
            {
                buildingCounts[building.type]--;
            }
        }
    }
}