using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CamelSociety.Core
{
    public class CareerSystem : MonoBehaviour
    {
        public static CareerSystem Instance { get; private set; }

        [Header("Career System Settings")]
        public float baseExperienceGain = 0.1f;        // 基础经验获取速率
        public float skillGrowthMultiplier = 1f;       // 技能成长倍率
        public float promotionExperienceThreshold = 100f; // 晋升所需经验阈值
        public float personalityMatchWeight = 0.3f;     // 性格匹配权重
        public float skillMatchWeight = 0.4f;          // 技能匹配权重
        public float socialStatusWeight = 0.3f;        // 社会地位权重

        public Dictionary<string, CareerData> careerDefinitions;
        private Dictionary<CareerField, List<string>> careersByField;
        private Dictionary<int, List<string>> careersByTechLevel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeCareerSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeCareerSystem()
        {
            careerDefinitions = new Dictionary<string, CareerData>();
            careersByField = new Dictionary<CareerField, List<string>>();
            careersByTechLevel = new Dictionary<int, List<string>>();

            // 初始化职业数据
            InitializeCareerData();

            // 组织职业数据
            OrganizeCareers();
        }

        private void InitializeCareerData()
        {
            // 农民
            var farmer = new CareerData
            {
                id = "farmer",
                name = "农民",
                description = "从事农业生产的基础工作者",
                type = CareerType.Farmer,
                field = CareerField.Agriculture,
                baseSalary = 2000f,
                socialStatus = 30f,
                minExperience = 0f,
                workStress = 40f,
                jobSatisfaction = 60f,
                socialInfluence = 20f,
                techLevelRequired = 0
            };
            farmer.requiredSkills["farming"] = 0.1f;
            farmer.requiredSkills["physical_labor"] = 0.2f;
            farmer.skillGrowthRates["farming"] = 0.02f;
            farmer.skillGrowthRates["physical_labor"] = 0.01f;
            farmer.promotionPaths.Add(CareerType.FarmManager);
            farmer.workplaces.Add(BuildingType.Farm);
            farmer.personalityPreferences["openness"] = 0.3f;
            farmer.personalityPreferences["conscientiousness"] = 0.7f;
            careerDefinitions[farmer.id] = farmer;

            // 工厂工人
            var factoryWorker = new CareerData
            {
                id = "factory_worker",
                name = "工厂工人",
                description = "在工厂从事生产工作的工人",
                type = CareerType.FactoryWorker,
                field = CareerField.Industry,
                baseSalary = 2500f,
                socialStatus = 35f,
                minExperience = 0f,
                workStress = 60f,
                jobSatisfaction = 50f,
                socialInfluence = 25f,
                techLevelRequired = 1
            };
            factoryWorker.requiredSkills["manufacturing"] = 0.2f;
            factoryWorker.requiredSkills["machine_operation"] = 0.3f;
            factoryWorker.skillGrowthRates["manufacturing"] = 0.02f;
            factoryWorker.skillGrowthRates["machine_operation"] = 0.03f;
            factoryWorker.promotionPaths.Add(CareerType.Technician);
            factoryWorker.workplaces.Add(BuildingType.Factory);
            factoryWorker.personalityPreferences["conscientiousness"] = 0.8f;
            factoryWorker.personalityPreferences["extraversion"] = 0.4f;
            careerDefinitions[factoryWorker.id] = factoryWorker;

            // 程序员
            var programmer = new CareerData
            {
                id = "programmer",
                name = "程序员",
                description = "从事软件开发的技术人员",
                type = CareerType.Programmer,
                field = CareerField.Technology,
                baseSalary = 5000f,
                socialStatus = 60f,
                minExperience = 0f,
                workStress = 70f,
                jobSatisfaction = 70f,
                socialInfluence = 40f,
                techLevelRequired = 2
            };
            programmer.requiredSkills["programming"] = 0.5f;
            programmer.requiredSkills["problem_solving"] = 0.4f;
            programmer.skillGrowthRates["programming"] = 0.03f;
            programmer.skillGrowthRates["problem_solving"] = 0.02f;
            programmer.promotionPaths.Add(CareerType.SystemAnalyst);
            programmer.promotionPaths.Add(CareerType.TechLeader);
            programmer.workplaces.Add(BuildingType.Laboratory);
            programmer.personalityPreferences["openness"] = 0.7f;
            programmer.personalityPreferences["conscientiousness"] = 0.6f;
            careerDefinitions[programmer.id] = programmer;

            // 继续添加更多职业...
        }

        private void OrganizeCareers()
        {
            foreach (var career in careerDefinitions.Values)
            {
                // 按领域组织
                if (!careersByField.ContainsKey(career.field))
                {
                    careersByField[career.field] = new List<string>();
                }
                careersByField[career.field].Add(career.id);

                // 按科技等级组织
                if (!careersByTechLevel.ContainsKey(career.techLevelRequired))
                {
                    careersByTechLevel[career.techLevelRequired] = new List<string>();
                }
                careersByTechLevel[career.techLevelRequired].Add(career.id);
            }
        }

        public bool CanPromote(Agent agent, string currentCareerId, out string nextCareerId)
        {
            nextCareerId = null;
            if (!careerDefinitions.ContainsKey(currentCareerId))
                return false;

            var currentCareer = careerDefinitions[currentCareerId];
            if (agent.careerExperience < promotionExperienceThreshold)
                return false;

            // 获取所有可能的晋升路径
            var possiblePromotions = currentCareer.promotionPaths
                .Select(type => careerDefinitions.Values.FirstOrDefault(c => c.type == type))
                .Where(c => c != null)
                .ToList();

            if (possiblePromotions.Count == 0)
                return false;

            // 找到最适合的晋升职业
            CareerData bestPromotion = null;
            float bestScore = 0f;

            foreach (var promotion in possiblePromotions)
            {
                float score = CalculateCareerSuitability(agent, promotion);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestPromotion = promotion;
                }
            }

            if (bestPromotion != null && bestScore > 0.6f)
            {
                nextCareerId = bestPromotion.id;
                return true;
            }

            return false;
        }

        public float CalculateCareerSuitability(Agent agent, CareerData career)
        {
            // 计算技能匹配度
            float skillMatch = CalculateSkillMatch(agent, career);
            
            // 计算性格匹配度
            float personalityMatch = CalculatePersonalityMatch(agent, career);
            
            // 计算社会地位影响
            float socialMatch = CalculateSocialStatusMatch(agent, career);

            // 综合评分
            return skillMatch * skillMatchWeight + 
                   personalityMatch * personalityMatchWeight + 
                   socialMatch * socialStatusWeight;
        }

        private float CalculateSkillMatch(Agent agent, CareerData career)
        {
            if (career.requiredSkills.Count == 0)
                return 1f;

            float totalMatch = 0f;
            foreach (var skill in career.requiredSkills)
            {
                float agentSkill = agent.GetSkillLevel(skill.Key);
                float requiredSkill = skill.Value;
                totalMatch += Mathf.Max(0, agentSkill / requiredSkill);
            }

            return totalMatch / career.requiredSkills.Count;
        }

        private float CalculatePersonalityMatch(Agent agent, CareerData career)
        {
            if (career.personalityPreferences.Count == 0)
                return 1f;

            float totalMatch = 0f;
            foreach (var pref in career.personalityPreferences)
            {
                if (agent.dna.personalityTraits.ContainsKey(pref.Key))
                {
                    float diff = Mathf.Abs(agent.dna.personalityTraits[pref.Key] - pref.Value);
                    totalMatch += 1f - diff;
                }
            }

            return totalMatch / career.personalityPreferences.Count;
        }

        private float CalculateSocialStatusMatch(Agent agent, CareerData career)
        {
            float currentStatus = agent.prestige;
            float targetStatus = career.socialStatus;
            
            // 如果当前地位接近或高于目标职业要求，给予高分
            if (currentStatus >= targetStatus * 0.8f)
                return 1f;
            
            // 否则根据差距计算匹配度
            return currentStatus / targetStatus;
        }

        public List<string> GetAvailableCareers(CareerField field, int techLevel)
        {
            if (!careersByField.ContainsKey(field))
                return new List<string>();

            return careersByField[field]
                .Where(id => careerDefinitions[id].techLevelRequired <= techLevel)
                .ToList();
        }

        public void UpdateCareerSkills(Agent agent, float deltaTime)
        {
            if (!careerDefinitions.ContainsKey(agent.currentCareerId))
                return;

            var career = careerDefinitions[agent.currentCareerId];
            
            // 更新相关技能
            foreach (var skillGrowth in career.skillGrowthRates)
            {
                float growth = skillGrowth.Value * skillGrowthMultiplier * deltaTime;
                growth *= agent.jobSatisfaction; // 工作满意度影响技能成长
                agent.AddExperience(skillGrowth.Key, growth);
            }
        }

        public float CalculateIncome(Agent agent)
        {
            if (!careerDefinitions.ContainsKey(agent.currentCareerId))
                return 0f;

            var career = careerDefinitions[agent.currentCareerId];
            float income = career.baseSalary;

            // 根据技能水平调整收入
            float skillBonus = CalculateSkillMatch(agent, career);
            income *= (1f + skillBonus * 0.5f);

            // 根据工作满意度调整收入
            income *= (0.8f + agent.jobSatisfaction * 0.4f);

            return income;
        }
    }
} 