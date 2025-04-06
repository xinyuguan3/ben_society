using UnityEngine;

namespace CamelSociety.Core
{
    public class Skill
    {
        public string name;
        public string description;
        public float level;           // 技能等级
        public float experience;      // 当前经验值
        public float maxExperience;   // 升级所需经验值
        public float learningRate;    // 学习速率
        public float effectiveness;   // 技能效果系数

        public Skill(string name, string description)
        {
            this.name = name;
            this.description = description;
            InitializeSkill();
        }

        private void InitializeSkill()
        {
            level = 1f;
            experience = 0f;
            maxExperience = 100f;
            learningRate = 1f;
            effectiveness = 1f;
        }

        public void GainExperience(float amount)
        {
            experience += amount * learningRate;
            
            // 检查是否可以升级
            while (experience >= maxExperience)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            experience -= maxExperience;
            level += 1f;
            
            // 增加下一级所需经验
            maxExperience *= 1.2f;
            
            // 提高技能效果
            effectiveness *= 1.1f;
        }

        public float GetEffectiveness()
        {
            return effectiveness * (0.5f + level * 0.5f);
        }

        public bool IsNovice()
        {
            return level < 3f;
        }

        public bool IsExpert()
        {
            return level > 8f;
        }

        public bool IsMaster()
        {
            return level > 15f;
        }
    }
} 