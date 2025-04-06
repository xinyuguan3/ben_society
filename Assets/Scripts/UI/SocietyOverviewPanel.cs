using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using CamelSociety.Core;

namespace CamelSociety.UI
{
    public class SocietyOverviewPanel : MonoBehaviour
    {
        [Header("文本组件")]
        public TextMeshProUGUI populationText;
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI resourcesText;
        public TextMeshProUGUI socialClassText;

        [Header("更新设置")]
        public float updateInterval = 0.5f;
        private float nextUpdateTime;

        [Header("统计信息")]
        public TextMeshProUGUI classDistributionText;
        public TextMeshProUGUI occupationDistributionText;

        [Header("Agent列表")]
        public Transform agentListContainer;
        public GameObject agentListItemPrefab;
        public AgentInfoPanel agentInfoPanel;

        private void Start()
        {
            UpdateDisplay();
            InvokeRepeating("UpdateDisplay", 1f, 1f);
        }

        private void Update()
        {
            if (Time.time >= nextUpdateTime)
            {
                UpdateDisplay();
                nextUpdateTime = Time.time + updateInterval;
            }
        }

        public void UpdateDisplay()
        {
            if (Core.SocietyManager.Instance == null) return;

            // 更新人口信息
            if (populationText != null)
            {
                populationText.text = $"人口: {Core.SocietyManager.Instance.CurrentPopulation}";
            }

            // 更新时间信息
            if (timeText != null)
            {
                float time = Core.SocietyManager.Instance.SimulationTime;
                int days = Mathf.FloorToInt(time / 24f);
                int hours = Mathf.FloorToInt(time % 24f);
                timeText.text = $"时间: {days}天 {hours}时";
            }

            // 更新资源信息
            if (resourcesText != null)
            {
                string resourceInfo = "资源:\n";
                var resourceCounts = new System.Collections.Generic.Dictionary<ResourceType, int>();

                // 统计所有资源
                foreach (var agent in Core.SocietyManager.Instance.GetAllAgents())
                {
                    var inventory = agent.GetInventory();
                    foreach (var kvp in inventory)
                    {
                        if (!resourceCounts.ContainsKey(kvp.Key))
                            resourceCounts[kvp.Key] = 0;
                        resourceCounts[kvp.Key] += Mathf.RoundToInt(kvp.Value);
                    }
                }

                // 显示资源统计
                foreach (var kvp in resourceCounts)
                {
                    resourceInfo += $"{kvp.Key}: {kvp.Value}\n";
                }
                resourcesText.text = resourceInfo;
            }

            // 更新社会阶层分布
            if (socialClassText != null)
            {
                var classCounts = new System.Collections.Generic.Dictionary<Core.SocialClass, int>();
                foreach (var agent in Core.SocietyManager.Instance.GetAllAgents())
                {
                    if (!classCounts.ContainsKey(agent.socialClass))
                        classCounts[agent.socialClass] = 0;
                    classCounts[agent.socialClass]++;
                }

                string classInfo = "社会阶层:\n";
                foreach (var kvp in classCounts)
                {
                    string className = kvp.Key switch
                    {
                        Core.SocialClass.WorkingClass => "下层",
                        Core.SocialClass.MiddleClass => "中层",
                        Core.SocialClass.UpperClass => "上层",
                        Core.SocialClass.RulingClass => "统治阶层",
                        _ => "未知"
                    };
                    classInfo += $"{className}: {kvp.Value}\n";
                }
                socialClassText.text = classInfo;
            }

            UpdateStatistics(Core.SocietyManager.Instance);
            UpdateAgentList(Core.SocietyManager.Instance);
        }

        private void UpdateStatistics(Core.SocietyManager society)
        {
            var agents = society.GetAllAgents();

            // 更新阶级分布
            var classDistribution = new Dictionary<Core.SocialClass, int>();
            foreach (var agent in agents)
            {
                if (!classDistribution.ContainsKey(agent.socialClass))
                    classDistribution[agent.socialClass] = 0;
                classDistribution[agent.socialClass]++;
            }

            System.Text.StringBuilder classText = new System.Text.StringBuilder("阶级分布:\n");
            foreach (var kv in classDistribution)
            {
                float percentage = (float)kv.Value / agents.Count * 100;
                classText.AppendLine($"{kv.Key}: {percentage:F1}%");
            }
            classDistributionText.text = classText.ToString();

            // 更新职业分布
            var occupationDistribution = new Dictionary<Core.Occupation, int>();
            foreach (var agent in agents)
            {
                if (!occupationDistribution.ContainsKey(agent.occupation))
                    occupationDistribution[agent.occupation] = 0;
                occupationDistribution[agent.occupation]++;
            }

            System.Text.StringBuilder occupationText = new System.Text.StringBuilder("主要职业:\n");
            var topOccupations = occupationDistribution.OrderByDescending(x => x.Value).Take(5);
            foreach (var kv in topOccupations)
            {
                float percentage = (float)kv.Value / agents.Count * 100;
                occupationText.AppendLine($"{kv.Key}: {percentage:F1}%");
            }
            occupationDistributionText.text = occupationText.ToString();
        }

        private void UpdateAgentList(Core.SocietyManager society)
        {
            // 清除现有列表
            foreach (Transform child in agentListContainer)
            {
                Destroy(child.gameObject);
            }

            // 创建新的Agent列表项
            foreach (var agent in society.GetAllAgents())
            {
                GameObject listItem = Instantiate(agentListItemPrefab, agentListContainer);
                
                // 设置按钮文本
                TextMeshProUGUI buttonText = listItem.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = $"{agent.agentName} - {agent.occupation}";

                // 添加点击事件
                Button button = listItem.GetComponent<Button>();
                Core.Agent currentAgent = agent; // 创建局部变量以避免闭包问题
                button.onClick.AddListener(() => OnAgentSelected(currentAgent));
            }
        }

        private void OnAgentSelected(Core.Agent agent)
        {
            if (agentInfoPanel != null)
            {
                agentInfoPanel.gameObject.SetActive(true);
                agentInfoPanel.DisplayAgent(agent);
            }
        }
    }
} 