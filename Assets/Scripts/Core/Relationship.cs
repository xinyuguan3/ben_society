using UnityEngine;

namespace CamelSociety.Core
{
    public class Relationship
    {
        public string targetAgentId;
        public RelationType type;
        public float intimacy;        // 亲密度
        public float trust;           // 信任度
        public float influence;       // 影响力
        public float mutualBenefit;   // 互利程度
        public float conflict;        // 冲突指数

        private float decayRate = 0.1f;
        private float maxValue = 100f;
        private float minValue = -100f;

        public Relationship(string targetId, RelationType type)
        {
            this.targetAgentId = targetId;
            this.type = type;
            InitializeRelationship();
        }

        private void InitializeRelationship()
        {
            // 根据关系类型初始化各项属性
            switch (type)
            {
                case RelationType.Family:
                    intimacy = 80f;
                    trust = 70f;
                    influence = 60f;
                    mutualBenefit = 50f;
                    conflict = 20f;
                    break;
                case RelationType.Friend:
                    intimacy = 50f;
                    trust = 50f;
                    influence = 30f;
                    mutualBenefit = 40f;
                    conflict = 10f;
                    break;
                // ... 其他关系类型的初始化
            }
        }

        public void Update(float deltaTime)
        {
            // 关系自然衰减
            intimacy = Mathf.Max(minValue, intimacy - decayRate * deltaTime);
            trust = Mathf.Max(minValue, trust - decayRate * deltaTime);
            
            // 更新其他属性
            UpdateRelationshipStatus();
        }

        private void UpdateRelationshipStatus()
        {
            // 根据各属性值更新关系状态
            if (conflict > 80f)
            {
                // 高冲突导致关系恶化
                intimacy -= 0.5f;
                trust -= 0.5f;
            }

            if (mutualBenefit > 70f)
            {
                // 高互利促进关系发展
                intimacy += 0.2f;
                trust += 0.2f;
            }

            // 确保所有值在合理范围内
            ClampValues();
        }

        private void ClampValues()
        {
            intimacy = Mathf.Clamp(intimacy, minValue, maxValue);
            trust = Mathf.Clamp(trust, minValue, maxValue);
            influence = Mathf.Clamp(influence, minValue, maxValue);
            mutualBenefit = Mathf.Clamp(mutualBenefit, minValue, maxValue);
            conflict = Mathf.Clamp(conflict, 0, maxValue);
        }

        public void ModifyRelationship(float intimacyDelta, float trustDelta, float influenceDelta, float benefitDelta, float conflictDelta)
        {
            intimacy += intimacyDelta;
            trust += trustDelta;
            influence += influenceDelta;
            mutualBenefit += benefitDelta;
            conflict += conflictDelta;

            ClampValues();
            UpdateRelationshipStatus();
        }

        public bool IsPositive()
        {
            return (intimacy + trust) / 2 > 0;
        }

        public bool IsStable()
        {
            return conflict < 30f;
        }

        public float GetOverallStrength()
        {
            return (intimacy + trust + influence + mutualBenefit - conflict) / 4;
        }
    }
} 