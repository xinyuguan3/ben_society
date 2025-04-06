using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class SceneSetup
{
    [MenuItem("CamelSociety/Setup Scene")]
    public static void SetupScene()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建EventSystem
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 创建SocietyOverviewPanel
        GameObject overviewPanel = CreateOverviewPanel(canvasObj);
        
        // 创建AgentInfoPanel
        GameObject agentInfoPanel = CreateAgentInfoPanel(canvasObj);

        // 创建预制体
        CreatePrefabs();

        // 创建SocietyManager
        GameObject societyManager = new GameObject("SocietyManager");
        societyManager.AddComponent<CamelSociety.Core.SocietyManager>();

        // 保存场景
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
    }

    private static GameObject CreateOverviewPanel(GameObject parent)
    {
        GameObject panel = CreateUIObject("SocietyOverviewPanel", parent);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0.3f, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // 添加组件
        var overviewPanel = panel.AddComponent<CamelSociety.UI.SocietyOverviewPanel>();

        // 创建Header
        GameObject header = CreateUIObject("Header", panel);
        overviewPanel.populationText = CreateText(header, "Population", "总人口: 0");

        // 创建Statistics
        GameObject statistics = CreateUIObject("Statistics", panel);
        overviewPanel.classDistributionText = CreateText(statistics, "ClassDistribution", "阶级分布:\n");
        overviewPanel.occupationDistributionText = CreateText(statistics, "OccupationDistribution", "职业分布:\n");

        // 创建AgentList
        GameObject scrollView = CreateScrollView("AgentList", panel);
        overviewPanel.agentListContainer = scrollView.GetComponentInChildren<ScrollRect>().content;

        return panel;
    }

    private static GameObject CreateAgentInfoPanel(GameObject parent)
    {
        GameObject panel = CreateUIObject("AgentInfoPanel", parent);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.3f, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // 添加组件
        var agentInfoPanel = panel.AddComponent<CamelSociety.UI.AgentInfoPanel>();

        // 创建BasicInfo
        GameObject basicInfo = CreateUIObject("BasicInfo", panel);
        agentInfoPanel.nameText = CreateText(basicInfo, "Name", "姓名: ");
        agentInfoPanel.ageText = CreateText(basicInfo, "Age", "年龄: ");
        agentInfoPanel.genderText = CreateText(basicInfo, "Gender", "性别: ");
        agentInfoPanel.occupationText = CreateText(basicInfo, "Occupation", "职业: ");
        agentInfoPanel.socialClassText = CreateText(basicInfo, "SocialClass", "阶级: ");

        // 创建StatusInfo
        GameObject statusInfo = CreateUIObject("StatusInfo", panel);
        agentInfoPanel.healthText = CreateText(statusInfo, "Health", "健康: ");
        agentInfoPanel.wealthText = CreateText(statusInfo, "Wealth", "财富: ");
        agentInfoPanel.prestigeText = CreateText(statusInfo, "Prestige", "声望: ");
        agentInfoPanel.influenceText = CreateText(statusInfo, "Influence", "影响力: ");

        // 创建NeedsPanel
        GameObject needsScrollView = CreateScrollView("NeedsPanel", panel);
        agentInfoPanel.needsContainer.GetComponent<RectTransform>().SetParent(needsScrollView.GetComponent<RectTransform>(), false);

        // 创建RelationshipsPanel
        GameObject relationshipsScrollView = CreateScrollView("RelationshipsPanel", panel);
        agentInfoPanel.relationshipsContainer.GetComponent<RectTransform>().SetParent(relationshipsScrollView.GetComponent<RectTransform>(), false);

        return panel;
    }

    private static void CreatePrefabs()
    {
        // 创建AgentListItemPrefab
        GameObject agentListItem = new GameObject("AgentListItem");
        Button button = agentListItem.AddComponent<Button>();
        TextMeshProUGUI buttonText = CreateText(agentListItem, "Text", "Agent Name");
        PrefabUtility.SaveAsPrefabAsset(agentListItem, "Assets/Prefabs/UI/AgentListItemPrefab.prefab");
        Object.DestroyImmediate(agentListItem);

        // 创建NeedItemPrefab
        GameObject needItem = new GameObject("NeedItem");
        TextMeshProUGUI needText = CreateText(needItem, "Text", "Need Info");
        PrefabUtility.SaveAsPrefabAsset(needItem, "Assets/Prefabs/UI/NeedItemPrefab.prefab");
        Object.DestroyImmediate(needItem);

        // 创建RelationshipItemPrefab
        GameObject relationshipItem = new GameObject("RelationshipItem");
        TextMeshProUGUI relationshipText = CreateText(relationshipItem, "Text", "Relationship Info");
        PrefabUtility.SaveAsPrefabAsset(relationshipItem, "Assets/Prefabs/UI/RelationshipItemPrefab.prefab");
        Object.DestroyImmediate(relationshipItem);
    }

    private static GameObject CreateUIObject(string name, GameObject parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent.transform, false);
        return obj;
    }

    private static TextMeshProUGUI CreateText(GameObject parent, string name, string text)
    {
        GameObject obj = CreateUIObject(name, parent);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 14;
        return tmp;
    }

    private static GameObject CreateScrollView(string name, GameObject parent)
    {
        GameObject scrollView = CreateUIObject(name, parent);
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        
        GameObject viewport = CreateUIObject("Viewport", scrollView);
        GameObject content = CreateUIObject("Content", viewport);
        
        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = content.GetComponent<RectTransform>();
        
        viewport.AddComponent<Mask>();
        viewport.AddComponent<Image>();
        
        content.AddComponent<VerticalLayoutGroup>();
        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        return scrollView;
    }
} 