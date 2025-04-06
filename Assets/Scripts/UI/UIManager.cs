using UnityEngine;
using UnityEngine.EventSystems;
using CamelSociety.Core;

namespace CamelSociety.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("面板")]
        public SocietyOverviewPanel overviewPanel;
        public AgentInfoPanel agentInfoPanel;

        [Header("设置")]
        public float updateInterval = 0.5f;
        private float nextUpdateTime;

        private Agent selectedAgent;
        private Camera mainCamera;

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

            mainCamera = Camera.main;
        }

        private void Start()
        {
            // 初始化UI
            if (agentInfoPanel != null)
            {
                agentInfoPanel.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            // 定期更新显示
            if (Time.time >= nextUpdateTime)
            {
                UpdateAllDisplays();
                nextUpdateTime = Time.time + updateInterval;
            }

            // 处理Agent选择
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleAgentSelection();
            }

            // 右键取消选择
            if (Input.GetMouseButtonDown(1))
            {
                DeselectAgent();
            }
        }

        private void HandleAgentSelection()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Agent agent = hit.collider.GetComponent<Agent>();
                if (agent != null)
                {
                    SelectAgent(agent);
                }
                else
                {
                    DeselectAgent();
                }
            }
            else
            {
                DeselectAgent();
            }
        }

        public void SelectAgent(Agent agent)
        {
            selectedAgent = agent;
            ShowAgentInfo(agent);
        }

        public void DeselectAgent()
        {
            selectedAgent = null;
            HideAgentInfo();
        }

        public void ShowAgentInfo(Agent agent)
        {
            if (agentInfoPanel != null)
            {
                agentInfoPanel.gameObject.SetActive(true);
                agentInfoPanel.DisplayAgent(agent);
            }
        }

        public void HideAgentInfo()
        {
            if (agentInfoPanel != null)
            {
                agentInfoPanel.gameObject.SetActive(false);
            }
        }

        public void UpdateAllDisplays()
        {
            if (overviewPanel != null)
            {
                overviewPanel.UpdateDisplay();
            }

            if (agentInfoPanel != null && agentInfoPanel.gameObject.activeSelf && selectedAgent != null)
            {
                agentInfoPanel.DisplayAgent(selectedAgent);
            }
        }

        public void ToggleOverviewPanel()
        {
            if (overviewPanel != null)
            {
                overviewPanel.gameObject.SetActive(!overviewPanel.gameObject.activeSelf);
            }
        }

        public void SetSimulationSpeed(float speed)
        {
            if (SocietyManager.Instance != null)
            {
                SocietyManager.Instance.simulationSpeed = speed;
            }
        }

        public void PauseSimulation()
        {
            SetSimulationSpeed(0f);
        }

        public void ResumeSimulation()
        {
            SetSimulationSpeed(1f);
        }

        public void FastForward()
        {
            SetSimulationSpeed(2f);
        }
    }
} 