using UnityEngine;
using System.Collections.Generic;

namespace CamelSociety.Core
{
    public enum CareerField
    {
        Agriculture,    // 农业
        Industry,      // 工业
        Service,       // 服务业
        Technology,    // 科技
        Art,          // 艺术
        Education,    // 教育
        Medical,      // 医疗
        Finance,      // 金融
        Government,   // 政府
        Entertainment // 娱乐
    }

    public enum CareerType
    {
        // 农业领域
        Farmer,             // 农民
        FarmManager,        // 农场主
        AgriculturalExpert, // 农业专家

        // 工业领域
        FactoryWorker,      // 工厂工人
        Technician,         // 技术工人
        Engineer,           // 工程师
        FactoryManager,     // 工厂经理
        Supervisor,         // 主管
        Manager,            // 经理

        // 服务业
        Waiter,            // 服务员
        Chef,              // 厨师
        RestaurantOwner,   // 餐厅老板
        Salesperson,       // 销售员
        RetailManager,     // 零售经理

        // 科技领域
        Programmer,        // 程序员
        SystemAnalyst,     // 系统分析师
        TechLeader,       // 技术主管
        CTO,              // 技术总监

        // 艺术领域
        Artist,           // 艺术家
        Designer,         // 设计师
        ArtDirector,      // 艺术总监

        // 教育领域
        Teacher,          // 教师
        Professor,        // 教授
        Principal,        // 校长

        // 医疗领域
        Nurse,            // 护士
        Doctor,           // 医生
        Specialist,       // 专科医生
        HospitalDirector, // 医院院长

        // 金融领域
        BankClerk,        // 银行职员
        Accountant,       // 会计师
        FinancialAnalyst, // 金融分析师
        BankManager,      // 银行经理

        // 政府部门
        CivilServant,     // 公务员
        Administrator,    // 行政官员
        Politician,       // 政治家

        // 娱乐业
        Performer,        // 表演者
        Director,        // 导演
        Producer         // 制作人
    }

    public class CareerData
    {
        public string id;                  // 职业ID
        public string name;                // 职业名称
        public string description;         // 职业描述
        public CareerType type;           // 职业类型
        public CareerField field;         // 职业领域
        public float baseSalary;          // 基础薪资
        public float socialStatus;         // 社会地位 (0-100)
        public float workStress;          // 工作压力 (0-100)
        public float jobSatisfaction;     // 工作满意度 (0-100)
        public float minExperience;        // 所需最低经验值
        public Dictionary<string, float> requiredSkills;    // 所需技能及等级
        public Dictionary<string, float> skillGrowthRates;  // 技能成长速率
        public int educationRequirement;   // 所需教育等级
        public List<string> learningSkills;   // 可学习技能
        public List<CareerType> promotionPaths;            // 晋升路径
        public List<BuildingType> workplaces;              // 可工作场所
        public float socialInfluence;      // 社会影响力
        public int techLevelRequired;      // 所需科技等级
        public Dictionary<string, float> personalityPreferences; // 性格偏好
        public Dictionary<ResourceType, float> resourceProduction; // 资源生产

        public CareerData()
        {
            requiredSkills = new Dictionary<string, float>();
            skillGrowthRates = new Dictionary<string, float>();
            promotionPaths = new List<CareerType>();
            workplaces = new List<BuildingType>();
            personalityPreferences = new Dictionary<string, float>();
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
                field = CareerField.Agriculture,
                baseSalary = 1000f,
                skillGrowthRates = new Dictionary<string, float>
                {
                    ["farming"] = 1f,
                    ["botany"] = 0.5f
                },
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
                field = CareerField.Industry,
                baseSalary = 1500f,
                skillGrowthRates = new Dictionary<string, float>
                {
                    ["manufacturing"] = 1.2f,
                    ["machinery"] = 0.8f
                },
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
                field = CareerField.Technology,
                baseSalary = 3000f,
                skillGrowthRates = new Dictionary<string, float>
                {
                    ["engineering"] = 1.5f,
                    ["mathematics"] = 1f
                },
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
                field = CareerField.Service,
                baseSalary = 2000f,
                skillGrowthRates = new Dictionary<string, float>
                {
                    ["cooking"] = 1.3f,
                    ["food_management"] = 0.8f
                },
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