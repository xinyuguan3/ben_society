using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CamelSociety.Core
{
    public class Building : MonoBehaviour
    {
        [Header("Building Info")]
        public BuildingType type;
        public BuildingState state;
        public BuildingData data;
        public float constructionProgress;
        public float efficiency = 1f;

        [Header("Resources")]
        public Dictionary<ResourceType, float> inventory = new Dictionary<ResourceType, float>();
        public float productionProgress;
        public float lastProductionTime;

        [Header("Workers")]
        public List<Agent> assignedWorkers = new List<Agent>();
        private float lastWorkerUpdate;
        private float workerUpdateInterval = 1f;

        [Header("Visuals")]
        public Material constructionMaterial;
        public Material operatingMaterial;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }
            InitializeInventory();
        }

        private void Start()
        {
            // 从数据库加载建筑数据
            data = BuildingDatabase.GetBuildingData(type);
            if (data == null)
            {
                Debug.LogError($"找不到建筑类型 {type} 的数据");
                return;
            }

            UpdateVisuals();
        }

        private void Update()
        {
            switch (state)
            {
                case BuildingState.UnderConstruction:
                    UpdateConstruction();
                    break;
                case BuildingState.Operating:
                    UpdateProduction();
                    UpdateWorkers();
                    break;
                case BuildingState.Upgrading:
                    UpdateUpgrade();
                    break;
            }
        }

        public void Initialize()
        {
            InitializeInventory();
        }

        private void InitializeInventory()
        {
            inventory.Clear();
            foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
            {
                inventory[type] = 0f;
            }
        }

        private void UpdateConstruction()
        {
            if (constructionProgress >= 1f)
            {
                SetState(BuildingState.Operating);
            }
            UpdateVisuals();
        }

        public void UpdateProduction()
        {
            if (Time.time - lastProductionTime < data.productionTime)
                return;

            lastProductionTime = Time.time;
            
            // 计算实际效率
            CalculateEfficiency();

            // 根据建筑类型进行生产
            foreach (var producedResource in data.producedResources)
            {
                bool canProduce = true;
                float productionAmount = efficiency;

                // 检查是否有足够的原材料
                foreach (var component in ResourceDatabase.GetResourceData(producedResource).components)
                {
                    float required = 1f; // 每单位产出需要的原材料数量
                    if (!HasResource(component, required))
                    {
                        canProduce = false;
                        break;
                    }
                }

                if (canProduce)
                {
                    // 消耗原材料
                    foreach (var component in ResourceDatabase.GetResourceData(producedResource).components)
                    {
                        ConsumeResource(component, 1f);
                    }

                    // 生产新资源
                    AddResource(producedResource, productionAmount);
                }
            }

            // 维护成本
            ConsumeMaintenanceResources();
        }

        private void UpdateUpgrade()
        {
            // 升级逻辑
        }

        private void UpdateWorkers()
        {
            if (Time.time - lastWorkerUpdate < workerUpdateInterval)
                return;

            lastWorkerUpdate = Time.time;

            // 更新工人技能和经验
            foreach (var worker in assignedWorkers)
            {
                foreach (var skill in data.requiredSkills)
                {
                    worker.AddExperience(skill.Key, 0.1f * efficiency);
                }
            }
        }

        private void CalculateEfficiency()
        {
            if (assignedWorkers.Count < data.maxWorkers)
            {
                efficiency = (float)assignedWorkers.Count / data.maxWorkers;
            }
            else
            {
                efficiency = 1f;
                // 计算工人技能加成
                float skillBonus = 0f;
                foreach (var worker in assignedWorkers)
                {
                    foreach (var requiredSkill in data.requiredSkills)
                    {
                        skillBonus += Mathf.Max(0, worker.GetSkillLevel(requiredSkill.Key) - requiredSkill.Value);
                    }
                }
                efficiency += skillBonus * 0.1f; // 技能加成最多提升50%效率
            }
        }

        private void ConsumeMaintenanceResources()
        {
            float maintenanceCost = data.maintenanceCost * Time.deltaTime;
            if (HasResource(ResourceType.Money, maintenanceCost))
            {
                ConsumeResource(ResourceType.Money, maintenanceCost);
            }
            else
            {
                efficiency *= 0.9f; // 效率下降
            }
        }

        public void SetState(BuildingState newState)
        {
            state = newState;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (meshRenderer != null)
            {
                switch (state)
                {
                    case BuildingState.UnderConstruction:
                        meshRenderer.material = constructionMaterial;
                        transform.localScale = Vector3.one * (0.5f + constructionProgress * 0.5f);
                        break;
                    case BuildingState.Operating:
                        meshRenderer.material = operatingMaterial;
                        transform.localScale = Vector3.one;
                        break;
                }
            }
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

            float availableSpace = data.storageCapacity - GetTotalInventory();
            float actualAmount = Mathf.Min(amount, availableSpace);
            
            inventory[type] += actualAmount;
        }

        public bool ConsumeResource(ResourceType type, float amount)
        {
            if (HasResource(type, amount))
            {
                inventory[type] -= amount;
                return true;
            }
            return false;
        }

        public float GetTotalInventory()
        {
            return inventory.Values.Sum();
        }

        public bool CanAcceptResource(ResourceType type, float amount)
        {
            if (!data.acceptedResources.Contains(type))
                return false;

            return GetTotalInventory() + amount <= data.storageCapacity;
        }

        public float RequestResource(ResourceType type, float amount)
        {
            if (HasResource(type, amount))
            {
                ConsumeResource(type, amount);
                return amount;
            }
            return 0f;
        }

        public void AssignWorker(Agent worker)
        {
            if (!assignedWorkers.Contains(worker) && assignedWorkers.Count < data.maxWorkers)
            {
                assignedWorkers.Add(worker);
                worker.assignedBuilding = this;
            }
        }

        public void UnassignWorker(Agent worker)
        {
            if (assignedWorkers.Contains(worker))
            {
                assignedWorkers.Remove(worker);
                if (worker.assignedBuilding == this)
                {
                    worker.assignedBuilding = null;
                }
            }
        }

        private void OnDestroy()
        {
            // 解除所有工人的分配
            foreach (var worker in assignedWorkers.ToList())
            {
                UnassignWorker(worker);
            }
        }
    }
} 