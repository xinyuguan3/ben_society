namespace CamelSociety.Core
{
    public enum Gender
    {
        Male,
        Female
    }

    public enum NeedType
    {
        None,
        Food,
        Sleep,
        Health,
        Social,
        Culture,
        Entertainment,
        SelfActualization
    }

    public enum SocialClass
    {
        WorkingClass,
        MiddleClass,
        UpperClass,
        RulingClass
    }

        public enum RelationType
    {
        Family,         // 家庭关系
        Friend,         // 朋友关系
        Mentor,         // 师徒关系
        Colleague,      // 同事关系
        Superior,       // 上下级
        Subordinate,    // 下属
        Rival,          // 竞争对手
        Partner,        // 合作伙伴
        Enemy,          // 敌人
        Lover,          // 恋人
        Spouse,         // 配偶
        Student,        // 学生
        Teacher         // 老师
    }


    public enum PersonalityTrait
    {
        Openness,
        Conscientiousness,
        Extraversion,
        Agreeableness,
        Neuroticism
    }

    public enum Occupation
    {
        // 工业
        Factory_Worker,
        Engineer,
        Technician,
        
        // 金融
        Banker,
        Investor,
        Accountant,
        
        // 医疗
        Doctor,
        Nurse,
        Pharmacist,
        
        // 娱乐
        Artist,
        Performer,
        Musician,
        
        // 食品
        Chef,
        Farmer,
        FoodVendor,
        
        // 执法
        Police,
        Detective,
        SecurityGuard,
        
        // 奢侈品
        JewelryMaker,
        FashionDesigner,
        LuxuryDealer,
        
        // 旅游
        TourGuide,
        HotelManager,
        TravelAgent,
        
        // 管理
        Manager,
        Executive,
        Administrator,
        
        // 教育
        Teacher,
        Professor,
        Researcher,
        
        // 服务
        ServiceWorker,
        Salesperson,
        Consultant,
        
        // 特殊
        Unemployed,
        Student,
        Retired
    }

    public enum ResourceType
    {
        // 基础资源
        Money,          // 金钱
        Food,           // 食物
        Water,          // 水
        Wood,           // 木材
        Stone,          // 石材
        Metal,          // 金属
        Gold,           // 黄金
        Coal,           // 煤炭
        Energy,         // 能源
        
        // 加工资源
        ProcessedFood,  // 加工食品
        RawMaterial,   // 原材料
        ProcessedMaterial, // 加工材料
        Furniture,     // 家具
        Clothes,       // 衣物
        Tools,         // 工具
        
        // 高级资源
        Electronics,   // 电子产品
        Machinery,     // 机械
        Medicine,      // 药品
        Luxury,        // 奢侈品
        Artwork,       // 艺术品
        Vehicle,       // 交通工具
        Data,          // 数据
        Computer,      // 计算机
        Robot,         // 机器人
        
        // 特殊资源
        Knowledge,     // 知识
        Culture,       // 文化
        Innovation,    // 创新
        Entertainment, // 娱乐
        Education,     // 教育
        Healthcare,    // 医疗服务
        Stock,         // 股票
        Bond,          // 债券
        
        // 社会资源
        Influence,     // 影响力
        Reputation,    // 声望
        SocialCapital  // 社会资本
    }
} 