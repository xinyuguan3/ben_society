using System.Collections.Generic;
using UnityEngine;

namespace CamelSociety.Core
{
    public class Personality
    {
        private Dictionary<PersonalityTrait, float> traits;

        public Personality()
        {
            InitializeTraits();
        }

        private void InitializeTraits()
        {
            traits = new Dictionary<PersonalityTrait, float>();
            
            // 初始化所有性格特征为随机值（0-1）
            foreach (PersonalityTrait trait in System.Enum.GetValues(typeof(PersonalityTrait)))
            {
                traits[trait] = Random.value;
            }
        }

        public float GetTraitValue(PersonalityTrait trait)
        {
            return traits[trait];
        }

        public void ModifyTrait(PersonalityTrait trait, float amount)
        {
            traits[trait] = Mathf.Clamp01(traits[trait] + amount);
        }

        public bool IsExtroverted()
        {
            return traits[PersonalityTrait.Extraversion] > 0.5f;
        }

        public bool IsAgreeable()
        {
            return traits[PersonalityTrait.Agreeableness] > 0.5f;
        }

        public bool IsNeurotic()
        {
            return traits[PersonalityTrait.Neuroticism] > 0.5f;
        }

        public bool IsOpen()
        {
            return traits[PersonalityTrait.Openness] > 0.5f;
        }

        public bool IsConscientious()
        {
            return traits[PersonalityTrait.Conscientiousness] > 0.5f;
        }

        // 计算与另一个性格的兼容性（0-1）
        public float CalculateCompatibility(Personality other)
        {
            float compatibility = 0f;
            int traitCount = System.Enum.GetValues(typeof(PersonalityTrait)).Length;

            foreach (PersonalityTrait trait in System.Enum.GetValues(typeof(PersonalityTrait)))
            {
                float diff = Mathf.Abs(traits[trait] - other.traits[trait]);
                compatibility += 1f - diff;
            }

            return compatibility / traitCount;
        }
    }
} 