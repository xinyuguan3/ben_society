using UnityEngine;

namespace CamelSociety.Core
{

    public class ResourceObject : MonoBehaviour
    {
        public ResourceType type;
        public float amount;
        public float maxAmount = 100f;
        public float regenerationRate = 0.1f;
        public float harvestRate = 1f;
        
        private bool isBeingHarvested = false;
        private float lastHarvestTime;
        private float harvestCooldown = 1f;

        private void Start()
        {
            // 初始化资源对象
            InitializeResource();
        }

        private void InitializeResource()
        {
            // 根据资源类型设置参数
            switch (type)
            {
                case ResourceType.Food:
                    maxAmount = 200f;
                    regenerationRate = 0.5f;
                    harvestRate = 2f;
                    break;
                case ResourceType.Wood:
                    maxAmount = 300f;
                    regenerationRate = 0.3f;
                    harvestRate = 1.5f;
                    break;
                case ResourceType.Stone:
                    maxAmount = 400f;
                    regenerationRate = 0.1f;
                    harvestRate = 1f;
                    break;
                case ResourceType.Metal:
                    maxAmount = 100f;
                    regenerationRate = 0.05f;
                    harvestRate = 0.5f;
                    break;
                case ResourceType.Gold:
                    maxAmount = 50f;
                    regenerationRate = 0.02f;
                    harvestRate = 0.2f;
                    break;
            }
        }

        public float Harvest(float requestedAmount)
        {
            if (Time.time - lastHarvestTime < harvestCooldown)
                return 0f;

            if (amount <= 0)
                return 0f;

            float harvestedAmount = Mathf.Min(requestedAmount, amount, harvestRate);
            amount -= harvestedAmount;
            lastHarvestTime = Time.time;

            return harvestedAmount;
        }

        public void SetHarvestState(bool state)
        {
            isBeingHarvested = state;
        }

        public bool CanBeHarvested()
        {
            return amount > 0 && !isBeingHarvested && Time.time - lastHarvestTime >= harvestCooldown;
        }

        private void OnDrawGizmos()
        {
            // 可视化资源状态
            if (amount > 0)
            {
                float scale = Mathf.Lerp(0.3f, 1f, amount / maxAmount);
                Gizmos.color = GetResourceColor();
                Gizmos.DrawWireSphere(transform.position, scale);
            }
        }

        private Color GetResourceColor()
        {
            return type switch
            {
                ResourceType.Food => Color.green,
                ResourceType.Wood => new Color(0.5f, 0.3f, 0.2f),
                ResourceType.Stone => Color.gray,
                ResourceType.Metal => Color.blue,
                ResourceType.Gold => Color.yellow,
                _ => Color.white
            };
        }
    }
}