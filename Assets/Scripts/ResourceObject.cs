using UnityEngine;

namespace CamelSociety.Core
{
    public class ResourceObject : MonoBehaviour
    {
        public ResourceType type;
        public float amount;
        
        private void OnTriggerEnter(Collider other)
        {
            Agent agent = other.GetComponent<Agent>();
            if (agent != null)
            {
                // 将资源添加到Agent的库存中
                agent.AddResource(type, amount);
                
                // 销毁资源对象
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // 添加触发器碰撞体
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }
    }
}