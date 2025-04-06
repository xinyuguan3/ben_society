using UnityEngine;

namespace CamelSociety.Core
{
    public class Resource
    {
        public ResourceType type;
        public string name;
        public string description;
        public float quantity;
        public float quality;
        public float baseValue;
        public float currentValue;

        public Resource(ResourceType type, float quantity = 0, float quality = 1)
        {
            this.type = type;
            this.quantity = quantity;
            this.quality = quality;
            InitializeResource();
        }

        private void InitializeResource()
        {
            name = type.ToString();
            baseValue = GetBaseValue();
            UpdateCurrentValue();
        }

        private float GetBaseValue()
        {
            // 根据资源类型设置基础价值
            switch (type)
            {
                case ResourceType.Food:
                case ResourceType.Water:
                    return 10f;
                case ResourceType.Energy:
                case ResourceType.RawMaterial:
                    return 20f;
                case ResourceType.ProcessedFood:
                case ResourceType.IndustrialProduct:
                    return 50f;
                case ResourceType.MedicalSupply:
                    return 100f;
                case ResourceType.LuxuryItem:
                    return 200f;
                case ResourceType.Money:
                    return 1f;
                case ResourceType.Stock:
                case ResourceType.Bond:
                    return 100f;
                case ResourceType.Data:
                case ResourceType.Knowledge:
                    return 150f;
                default:
                    return 1f;
            }
        }

        public void UpdateCurrentValue()
        {
            // 根据品质调整当前价值
            currentValue = baseValue * quality;
        }

        public bool IsVirtual()
        {
            return type == ResourceType.Money || 
                   type == ResourceType.Stock || 
                   type == ResourceType.Bond || 
                   type == ResourceType.Data || 
                   type == ResourceType.Knowledge;
        }

        public bool IsConsumable()
        {
            return type == ResourceType.Food || 
                   type == ResourceType.Water || 
                   type == ResourceType.Energy || 
                   type == ResourceType.ProcessedFood || 
                   type == ResourceType.MedicalSupply;
        }

        public float GetTotalValue()
        {
            return currentValue * quantity;
        }
    }
} 