using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CamelSociety.Core
{
    public enum PersonalityDimension
    {
        // 基础性格维度 (Big Five)
        Openness,           // 开放性
        Conscientiousness,  // 尽责性
        Extraversion,       // 外向性
        Agreeableness,     // 宜人性
        Neuroticism,       // 神经质

        // 价值观维度
        Tradition,          // 传统价值观
        Innovation,         // 创新精神
        Collectivism,      // 集体主义
        Individualism,      // 个人主义
        Materialism,       // 物质主义
        Spiritualism,       // 精神追求

        // 兴趣维度
        ArtisticInterest,   // 艺术兴趣
        ScientificInterest, // 科学兴趣
        SocialInterest,     // 社交兴趣
        PhysicalInterest,   // 体育兴趣
        IntellectualInterest, // 知识兴趣

        // 情感维度
        EmotionalStability, // 情感稳定性
        Empathy,           // 同理心
        Sensitivity,       // 敏感度
        EmotionalExpression, // 情感表达
        EmotionalDepth,    // 情感深度

        // 社交维度
        SocialConfidence,  // 社交自信
        Leadership,        // 领导力
        Cooperation,       // 合作性
        Independence,      // 独立性
        SocialInfluence,   // 社交影响力

        // 道德维度
        Honesty,           // 诚实度
        Loyalty,           // 忠诚度
        Responsibility,    // 责任感
        Altruism,         // 利他主义
        Justice,          // 正义感

        // 生活态度维度
        Optimism,          // 乐观度
        Ambition,          // 野心
        RiskTaking,        // 冒险精神
        Persistence,       // 坚持性
        Adaptability,      // 适应性

        // 文化维度
        CulturalOpenness,  // 文化开放度
        Traditionalism,    // 传统主义
        Cosmopolitanism,   // 世界主义
        LocalIdentity,     // 本土认同
        CulturalSensitivity, // 文化敏感度

        // 认知维度
        AnalyticalThinking, // 分析思维
        CreativeThinking,   // 创造思维
        PracticalThinking,  // 实践思维
        AbstractThinking,   // 抽象思维
        IntuitiveThinking,  // 直觉思维

        // 动机维度
        Achievement,        // 成就动机
        Power,             // 权力动机
        Affiliation,       // 亲和动机
        Recognition,       // 认可需求
        Growth             // 成长动机
    }

    public class PersonalityMatrix
    {
        private const int VECTOR_SIZE = 1024;  // 向量维度
        private float[] vector;                // 特征向量
        private Dictionary<PersonalityDimension, float> dimensionValues; // 维度值
        private Dictionary<PersonalityDimension, int[]> dimensionEmbeddings; // 每个维度在向量中的映射位置

        public PersonalityMatrix()
        {
            vector = new float[VECTOR_SIZE];
            dimensionValues = new Dictionary<PersonalityDimension, float>();
            dimensionEmbeddings = new Dictionary<PersonalityDimension, int[]>();
            InitializeEmbeddings();
        }

        private void InitializeEmbeddings()
        {
            System.Random random = new System.Random(42); // 固定种子以确保一致性
            foreach (PersonalityDimension dimension in System.Enum.GetValues(typeof(PersonalityDimension)))
            {
                // 为每个维度分配20个随机位置
                int[] positions = Enumerable.Range(0, VECTOR_SIZE)
                    .OrderBy(x => random.Next())
                    .Take(20)
                    .ToArray();
                dimensionEmbeddings[dimension] = positions;
            }
        }

        public void SetDimensionValue(PersonalityDimension dimension, float value)
        {
            value = Mathf.Clamp01(value);
            dimensionValues[dimension] = value;

            // 更新向量中对应的位置
            foreach (int position in dimensionEmbeddings[dimension])
            {
                vector[position] = value;
            }
        }

        public float GetDimensionValue(PersonalityDimension dimension)
        {
            return dimensionValues.ContainsKey(dimension) ? dimensionValues[dimension] : 0f;
        }

        public float[] GetVector()
        {
            return vector;
        }

        public float CalculateSimilarity(PersonalityMatrix other)
        {
            return CosineSimilarity(this.vector, other.vector);
        }

        public float CalculateCompatibility(PersonalityMatrix other, RelationType relationType)
        {
            float baseSimilarity = CalculateSimilarity(other);
            
            switch (relationType)
            {
                case RelationType.Friend:
                    return CalculateFriendshipCompatibility(other, baseSimilarity);
                case RelationType.Lover:
                    return CalculateRomanticCompatibility(other, baseSimilarity);
                case RelationType.Mentor:
                    return CalculateMentorshipCompatibility(other, baseSimilarity);
                case RelationType.Business:
                    return CalculateBusinessCompatibility(other, baseSimilarity);
                default:
                    return baseSimilarity;
            }
        }

        private float CalculateFriendshipCompatibility(PersonalityMatrix other, float baseSimilarity)
        {
            float interestMatch = CalculateInterestSimilarity(other);
            float valueMatch = CalculateValueSimilarity(other);
            float socialMatch = CalculateSocialCompatibility(other);

            return (baseSimilarity * 0.4f + interestMatch * 0.3f + valueMatch * 0.2f + socialMatch * 0.1f);
        }

        private float CalculateRomanticCompatibility(PersonalityMatrix other, float baseSimilarity)
        {
            float emotionalMatch = CalculateEmotionalCompatibility(other);
            float valueMatch = CalculateValueSimilarity(other);
            float complementarity = CalculateComplementarity(other);

            return (baseSimilarity * 0.3f + emotionalMatch * 0.3f + valueMatch * 0.2f + complementarity * 0.2f);
        }

        private float CalculateMentorshipCompatibility(PersonalityMatrix other, float baseSimilarity)
        {
            float experienceGap = CalculateExperienceGap(other);
            float teachingAptitude = GetDimensionValue(PersonalityDimension.Leadership);
            float learningAptitude = other.GetDimensionValue(PersonalityDimension.Growth);

            return (baseSimilarity * 0.2f + experienceGap * 0.3f + teachingAptitude * 0.25f + learningAptitude * 0.25f);
        }

        private float CalculateBusinessCompatibility(PersonalityMatrix other, float baseSimilarity)
        {
            float professionalMatch = CalculateProfessionalCompatibility(other);
            float trustFactor = CalculateTrustFactor(other);
            float complementarity = CalculateComplementarity(other);

            return (baseSimilarity * 0.2f + professionalMatch * 0.3f + trustFactor * 0.25f + complementarity * 0.25f);
        }

        private float CalculateInterestSimilarity(PersonalityMatrix other)
        {
            PersonalityDimension[] interestDimensions = {
                PersonalityDimension.ArtisticInterest,
                PersonalityDimension.ScientificInterest,
                PersonalityDimension.SocialInterest,
                PersonalityDimension.PhysicalInterest,
                PersonalityDimension.IntellectualInterest
            };

            return CalculateDimensionsAverage(other, interestDimensions);
        }

        private float CalculateValueSimilarity(PersonalityMatrix other)
        {
            PersonalityDimension[] valueDimensions = {
                PersonalityDimension.Tradition,
                PersonalityDimension.Innovation,
                PersonalityDimension.Collectivism,
                PersonalityDimension.Individualism,
                PersonalityDimension.Materialism,
                PersonalityDimension.Spiritualism
            };

            return CalculateDimensionsAverage(other, valueDimensions);
        }

        private float CalculateEmotionalCompatibility(PersonalityMatrix other)
        {
            PersonalityDimension[] emotionalDimensions = {
                PersonalityDimension.EmotionalStability,
                PersonalityDimension.Empathy,
                PersonalityDimension.Sensitivity,
                PersonalityDimension.EmotionalExpression,
                PersonalityDimension.EmotionalDepth
            };

            return CalculateDimensionsAverage(other, emotionalDimensions);
        }

        private float CalculateSocialCompatibility(PersonalityMatrix other)
        {
            PersonalityDimension[] socialDimensions = {
                PersonalityDimension.SocialConfidence,
                PersonalityDimension.Cooperation,
                PersonalityDimension.Independence,
                PersonalityDimension.SocialInfluence
            };

            return CalculateDimensionsAverage(other, socialDimensions);
        }

        private float CalculateProfessionalCompatibility(PersonalityMatrix other)
        {
            PersonalityDimension[] professionalDimensions = {
                PersonalityDimension.AnalyticalThinking,
                PersonalityDimension.CreativeThinking,
                PersonalityDimension.PracticalThinking,
                PersonalityDimension.Achievement,
                PersonalityDimension.Power
            };

            return CalculateDimensionsAverage(other, professionalDimensions);
        }

        private float CalculateTrustFactor(PersonalityMatrix other)
        {
            PersonalityDimension[] trustDimensions = {
                PersonalityDimension.Honesty,
                PersonalityDimension.Loyalty,
                PersonalityDimension.Responsibility
            };

            return CalculateDimensionsAverage(other, trustDimensions);
        }

        private float CalculateExperienceGap(PersonalityMatrix other)
        {
            float thisExperience = GetDimensionValue(PersonalityDimension.Achievement);
            float otherExperience = other.GetDimensionValue(PersonalityDimension.Achievement);
            return Mathf.Abs(thisExperience - otherExperience);
        }

        private float CalculateComplementarity(PersonalityMatrix other)
        {
            // 计算互补性，某些特质的差异可能会带来正面效果
            float complementarity = 0f;
            
            // 外向-内向互补
            float extraversionComp = Mathf.Abs(
                GetDimensionValue(PersonalityDimension.Extraversion) - 
                other.GetDimensionValue(PersonalityDimension.Extraversion));
            
            // 分析思维-直觉思维互补
            float thinkingComp = Mathf.Abs(
                GetDimensionValue(PersonalityDimension.AnalyticalThinking) - 
                other.GetDimensionValue(PersonalityDimension.IntuitiveThinking));
            
            // 实践-理论互补
            float practicalComp = Mathf.Abs(
                GetDimensionValue(PersonalityDimension.PracticalThinking) - 
                other.GetDimensionValue(PersonalityDimension.AbstractThinking));

            complementarity = (extraversionComp + thinkingComp + practicalComp) / 3f;
            return complementarity;
        }

        private float CalculateDimensionsAverage(PersonalityMatrix other, PersonalityDimension[] dimensions)
        {
            float sum = 0f;
            foreach (var dimension in dimensions)
            {
                float diff = Mathf.Abs(GetDimensionValue(dimension) - other.GetDimensionValue(dimension));
                sum += 1f - diff; // 转换为相似度
            }
            return sum / dimensions.Length;
        }

        private float CosineSimilarity(float[] a, float[] b)
        {
            float dotProduct = 0f;
            float normA = 0f;
            float normB = 0f;
            
            for (int i = 0; i < VECTOR_SIZE; i++)
            {
                dotProduct += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }
            
            normA = Mathf.Sqrt(normA);
            normB = Mathf.Sqrt(normB);
            
            if (normA == 0 || normB == 0)
                return 0f;
                
            return dotProduct / (normA * normB);
        }

        public void RandomizePersonality(float variance = 0.2f)
        {
            System.Random random = new System.Random();
            foreach (PersonalityDimension dimension in System.Enum.GetValues(typeof(PersonalityDimension)))
            {
                float baseValue = 0.5f;
                float randomOffset = (float)(random.NextDouble() - 0.5) * 2 * variance;
                SetDimensionValue(dimension, Mathf.Clamp01(baseValue + randomOffset));
            }
        }

        public void InheritFromParents(PersonalityMatrix parent1, PersonalityMatrix parent2, float mutationRate = 0.1f)
        {
            System.Random random = new System.Random();
            foreach (PersonalityDimension dimension in System.Enum.GetValues(typeof(PersonalityDimension)))
            {
                float parentValue = random.Next(2) == 0 
                    ? parent1.GetDimensionValue(dimension) 
                    : parent2.GetDimensionValue(dimension);

                // 添加随机变异
                float mutation = (float)(random.NextDouble() - 0.5) * 2 * mutationRate;
                SetDimensionValue(dimension, Mathf.Clamp01(parentValue + mutation));
            }
        }
    }
} 