using UnityEngine;

namespace CamelSociety.Core
{
    public class Need
    {
        public NeedType type;
        public float value;        // 当前值
        public float maxValue;     // 最大值
        public float minValue;     // 最小值
        public float decayRate;    // 衰减速率
        public float urgency;      // 紧急程度
        public float priority;     // 优先级

        public Need(NeedType type)
        {
            this.type = type;
            InitializeNeedParameters();
        }

        private void InitializeNeedParameters()
        {
            // 根据需求类型设置不同的参数
            switch (type)
            {
                case NeedType.Food:
                    maxValue = 100f;
                    minValue = 0f;
                    decayRate = 0.5f;
                    priority = 1f;
                    break;
                case NeedType.Shelter:
                    maxValue = 100f;
                    minValue = 0f;
                    decayRate = 0.1f;
                    priority = 0.9f;
                    break;
                case NeedType.Safety:
                    maxValue = 100f;
                    minValue = 0f;
                    decayRate = 0.2f;
                    priority = 0.8f;
                    break;
                // ... 其他需求类型的参数设置
            }

            value = maxValue;
            UpdateUrgency();
        }

        public void Update(float deltaTime)
        {
            // 更新需求值
            value = Mathf.Max(minValue, value - decayRate * deltaTime);
            UpdateUrgency();
        }

        public void Satisfy(float amount)
        {
            value = Mathf.Min(maxValue, value + amount);
            UpdateUrgency();
        }

        private void UpdateUrgency()
        {
            // 计算紧急程度
            float normalizedValue = (value - minValue) / (maxValue - minValue);
            urgency = (1f - normalizedValue) * priority;
        }

        public bool IsCritical()
        {
            return value <= minValue + (maxValue - minValue) * 0.2f;
        }
    }
} 