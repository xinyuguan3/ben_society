namespace CamelSociety.Core
{
    public enum Gender
    {
        Male,
        Female
    }

    public enum NeedType
    {
        Food,
        Shelter,
        Safety,
        Social,
        Development,
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
        Food,
        Water,
        Energy,
        RawMaterial,

        Wood,
        Stone,
        Gold,
        
        // 加工资源
        ProcessedFood,
        IndustrialProduct,
        MedicalSupply,
        LuxuryItem,
        
        // 虚拟资源
        Money,
        Stock,
        Bond,
        Data,
        Knowledge
    }
} 