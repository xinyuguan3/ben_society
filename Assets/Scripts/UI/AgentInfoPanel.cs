using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using CamelSociety.Core;

namespace CamelSociety.UI
{
    public class AgentInfoPanel : MonoBehaviour
    {
        [Header("基本信息")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI ageText;
        public TextMeshProUGUI genderText;
        public TextMeshProUGUI healthText;

        [Header("社会属性")]
        public TextMeshProUGUI socialClassText;
        public TextMeshProUGUI wealthText;
        public TextMeshProUGUI prestigeText;
        public TextMeshProUGUI influenceText;
        public TextMeshProUGUI occupationText;

        [Header("需求状态")]
        public TextMeshProUGUI foodText;
        public TextMeshProUGUI sleepText;
        public TextMeshProUGUI happinessText;
        public TextMeshProUGUI needsText;
        public TextMeshProUGUI needsContainer;

        [Header("资源信息")]
        public TextMeshProUGUI inventoryText;

        [Header("技能信息")]
        public TextMeshProUGUI skillsText;

        [Header("关系信息")]
        public TextMeshProUGUI relationshipsText;
        public TextMeshProUGUI relationshipsContainer;
        private Agent currentAgent;

        public void DisplayAgent(Agent agent)
        {
            currentAgent = agent;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (currentAgent == null) return;

            // 更新基本信息
            if (nameText != null)
                nameText.text = $"姓名: {currentAgent.agentName}";
            
            if (ageText != null)
                ageText.text = $"年龄: {currentAgent.age}岁";
            
            if (genderText != null)
                genderText.text = $"性别: {(currentAgent.gender == Gender.Male ? "男" : "女")}";
            
            if (healthText != null)
                healthText.text = $"健康: {currentAgent.health:F1}";

            // 更新社会属性
            if (socialClassText != null)
            {
                string className = currentAgent.socialClass switch
                {
                    SocialClass.WorkingClass => "下层",
                    SocialClass.MiddleClass => "中层",
                    SocialClass.UpperClass => "上层",
                    SocialClass.RulingClass => "统治阶层",
                    _ => "未知"
                };
                socialClassText.text = $"社会阶层: {className}";
            }

            if (wealthText != null)
                wealthText.text = $"财富: {currentAgent.wealth:F1}";
            
            if (prestigeText != null)
                prestigeText.text = $"声望: {currentAgent.prestige:F1}";
            
            if (influenceText != null)
                influenceText.text = $"影响力: {currentAgent.influence:F1}";

            // 更新需求状态
            if (foodText != null)
                foodText.text = $"饥饿度: {currentAgent.needs[NeedType.Food].value:F1}";
            
            if (sleepText != null)
                sleepText.text = $"睡眠: {currentAgent.needs[NeedType.Sleep].value:F1}";
            
            if (happinessText != null)
                happinessText.text = $"幸福度: {currentAgent.needs[NeedType.Happiness].value:F1}";

            if (needsText != null)
            {
                string needsInfo = "需求状态:\n";
                foreach (var need in currentAgent.needs)
                {
                    needsInfo += $"{need.Key}: {need.Value.value:F1}/{need.Value.maxValue:F1} (紧急度: {need.Value.urgency:F1})\n";
                }
                needsText.text = needsInfo;
            }

            // 更新资源信息
            if (inventoryText != null)
            {
                string inventoryInfo = "物品栏:\n";
                var inventory = currentAgent.GetInventory();
                foreach (var item in inventory)
                {
                    if (item.Value > 0)
                    {
                        inventoryInfo += $"{item.Key}: {item.Value:F1}\n";
                    }
                }
                inventoryText.text = inventoryInfo;
            }

            // 更新技能信息
            if (skillsText != null)
            {
                string skillsInfo = "技能:\n";
                foreach (var skill in currentAgent.skills)
                {
                    skillsInfo += $"{skill.name}: {skill.description}\n";
                }
                skillsText.text = skillsInfo;
            }

            // 更新关系信息
            if (relationshipsText != null)
            {
                string relationshipsInfo = "社会关系:\n";
                foreach (var relation in currentAgent.relationships)
                {
                    var otherAgent = SocietyManager.Instance.GetAgent(relation.Key);
                    if (otherAgent != null)
                    {
                        relationshipsInfo += $"{otherAgent.agentName}: {relation.Value.type}\n";
                    }
                }
                relationshipsText.text = relationshipsInfo;
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
} 