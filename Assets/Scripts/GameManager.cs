using UnityEngine;
using System.Collections.Generic;
using CamelSociety.Core;
namespace CamelSociety
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // 社会系统配置
        [Header("Society Configuration")]
        public int initialAgentCount = 10;
        public float simulationSpeed = 1f;
        
        // 资源系统
        [Header("Resource System")]
        public List<ResourceType> availableResources;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeSociety();
        }

        private void InitializeSociety()
        {
            Debug.Log("Initializing Camel Society...");
            // TODO: 初始化社会系统
        }

        public void AdjustSimulationSpeed(float speed)
        {
            simulationSpeed = speed;
            Time.timeScale = speed;
        }
    }
}