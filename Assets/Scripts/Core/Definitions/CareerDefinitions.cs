using UnityEngine;
using System.Collections.Generic;

namespace CamelSociety.Core
{
    public enum CareerType
    {
        // 基础职业
        Unemployed,         // 无业
        Student,           // 学生
        
        // 资源采集
        Farmer,            // 农民
        Miner,            // 矿工
        WoodCutter,       // 伐木工
        Hunter,           // 猎人
        Fisher,           // 渔民
        
        // 手工业
        Carpenter,         // 木匠
        Blacksmith,       // 铁匠
        Tailor,           // 裁缝
        Potter,           // 陶工
        Weaver,           // 织工
        
        // 工业
        FactoryWorker,    // 工厂工人
        Engineer,         // 工程师
        Technician,       // 技术员
        Mechanic,         // 机械师
        
        // 服务业
        Merchant,         // 商人
        Trader,           // 贸易商
        Shopkeeper,       // 店主
        Chef,             // 厨师
        Waiter,           // 服务员
        
        // 专业职业
        Doctor,           // 医生
        Nurse,            // 护士
        Teacher,          // 教师
        Professor,        // 教授
        Scientist,        // 科学家
        Researcher,       // 研究员
        
        // 艺术文化
        Artist,           // 艺术家
        Musician,         // 音乐家
        Writer,           // 作家
        Actor,            // 演员
        
        // 管理职业
        Manager,          // 经理
        Administrator,    // 行政人员
        Supervisor,       // 主管
        
        // 特殊职业
        Priest,           // 牧师
        Soldier,          // 军人
        Guard,            // 守卫
        Explorer         // 探险家
    }

    public enum CareerField
    {
        Agriculture,      // 农业
        Industry,         // 工业
        Service,          // 服务业
        Culture,          // 文化
        Art,              // 艺术
    }

    public class CareerData
    {
        public string name;                 // 职业名称
        public string description;          // 职业描述
        public CareerType type;            // 职业类型
        public int tier;                    // 职业等级（1-5）
        public float baseSalary;           // 基础薪资
        public CareerField field;          // 职业领域
        public Dictionary<string, float> requiredSkills;  // 所需技能
        public List<string> learningSkills;  // 可学习技能
        public float skillGainRate;        // 技能获取速率
        public int educationRequirement;   // 教育要求
        public List<CareerType> promotionPaths;  // 晋升路径
        public float socialStatus;         // 社会地位（0-100）
        public float workStress;           // 工作压力（0-100）
        public float jobSatisfaction;      // 工作满意度基数
        public List<BuildingType> workplaces;  // 可工作的建筑类型
        public Dictionary<ResourceType, float> resourceProduction;  // 资源生产效率

        public CareerData()
        {
            requiredSkills = new Dictionary<string, float>();
            learningSkills = new List<string>();
            promotionPaths = new List<CareerType>();
            workplaces = new List<BuildingType>();
            resourceProduction = new Dictionary<ResourceType, float>();
        }
    }

    public static class CareerDatabase
    {
        private static Dictionary<CareerType, CareerData> careerData;

        public static void Initialize()
        {
            careerData = new Dictionary<CareerType, CareerData>();

            // 农民
            var farmer = new CareerData
            {
                name = "农民",
                description = "从事农业生产的工作者",
                type = CareerType.Farmer,
                tier = 1,
                baseSalary = 1000f,
                skillGainRate = 1f,
                educationRequirement = 0,
                socialStatus = 30f,
                workStress = 40f,
                jobSatisfaction = 60f
            };
            farmer.requiredSkills["farming"] = 0.1f;
            farmer.learningSkills.Add("farming");
            farmer.learningSkills.Add("botany");
            farmer.workplaces.Add(BuildingType.Farm);
            farmer.resourceProduction[ResourceType.Food] = 1.2f;
            farmer.resourceProduction[ResourceType.RawMaterial] = 0.8f;
            careerData[CareerType.Farmer] = farmer;

            // 工厂工人
            var factoryWorker = new CareerData
            {
                name = "工厂工人",
                description = "在工厂从事生产的工人",
                type = CareerType.FactoryWorker,
                tier = 2,
                baseSalary = 1500f,
                skillGainRate = 1.2f,
                educationRequirement = 1,
                socialStatus = 40f,
                workStress = 60f,
                jobSatisfaction = 50f
            };
            factoryWorker.requiredSkills["manufacturing"] = 0.2f;
            factoryWorker.learningSkills.Add("manufacturing");
            factoryWorker.learningSkills.Add("machinery");
            factoryWorker.promotionPaths.Add(CareerType.Supervisor);
            factoryWorker.workplaces.Add(BuildingType.Factory);
            factoryWorker.resourceProduction[ResourceType.ProcessedMaterial] = 1.5f;
            careerData[CareerType.FactoryWorker] = factoryWorker;

            // 工程师
            var engineer = new CareerData
            {
                name = "工程师",
                description = "负责技术设计和生产优化的专业人员",
                type = CareerType.Engineer,
                tier = 4,
                baseSalary = 3000f,
                skillGainRate = 1.5f,
                educationRequirement = 3,
                socialStatus = 75f,
                workStress = 70f,
                jobSatisfaction = 80f
            };
            engineer.requiredSkills["engineering"] = 0.6f;
            engineer.requiredSkills["mathematics"] = 0.5f;
            engineer.learningSkills.Add("engineering");
            engineer.learningSkills.Add("innovation");
            engineer.promotionPaths.Add(CareerType.Manager);
            engineer.workplaces.Add(BuildingType.Factory);
            engineer.workplaces.Add(BuildingType.ResearchCenter);
            engineer.resourceProduction[ResourceType.Innovation] = 0.5f;
            careerData[CareerType.Engineer] = engineer;

            // 厨师
            var chef = new CareerData
            {
                name = "厨师",
                description = "专业的烹饪人员",
                type = CareerType.Chef,
                tier = 3,
                baseSalary = 2000f,
                skillGainRate = 1.3f,
                educationRequirement = 2,
                socialStatus = 60f,
                workStress = 65f,
                jobSatisfaction = 75f
            };
            chef.requiredSkills["cooking"] = 0.4f;
            chef.learningSkills.Add("cooking");
            chef.learningSkills.Add("food_management");
            chef.promotionPaths.Add(CareerType.Manager);
            chef.workplaces.Add(BuildingType.Restaurant);
            chef.resourceProduction[ResourceType.ProcessedFood] = 2f;
            careerData[CareerType.Chef] = chef;

            // 继续添加其他职业...
        }

        public static CareerData GetCareerData(CareerType type)
        {
            if (careerData == null)
            {
                Initialize();
            }
            return careerData.ContainsKey(type) ? careerData[type] : null;
        }

        public static List<CareerType> GetAvailableCareers(int educationLevel, Dictionary<string, float> skills)
        {
            if (careerData == null)
            {
                Initialize();
            }

            List<CareerType> availableCareers = new List<CareerType>();
            foreach (var career in careerData)
            {
                if (career.Value.educationRequirement <= educationLevel)
                {
                    bool hasRequiredSkills = true;
                    foreach (var requiredSkill in career.Value.requiredSkills)
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
                        availableCareers.Add(career.Key);
                    }
                }
            }
            return availableCareers;
        }
    }
} 