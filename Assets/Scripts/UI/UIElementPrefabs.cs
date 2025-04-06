using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;

namespace CamelSociety.UI
{
    public class UIElementPrefabs
    {
        [MenuItem("CamelSociety/Create UI Prefabs")]
        public static void CreateUIPrefabs()
        {
            CreateAgentListItemPrefab();
            CreateNeedItemPrefab();
            CreateRelationshipItemPrefab();
            AssetDatabase.SaveAssets();
        }

        private static void CreateAgentListItemPrefab()
        {
            // 创建Agent列表项预制体
            GameObject agentListItem = new GameObject("AgentListItem");
            RectTransform rt = agentListItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 50);

            // 添加按钮组件
            Button button = agentListItem.AddComponent<Button>();
            Image image = agentListItem.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // 创建文本对象
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(agentListItem.transform, false);
            RectTransform textRT = textObj.AddComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = new Vector2(10, 5);
            textRT.offsetMax = new Vector2(-10, -5);

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 14;
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.text = "Agent Name - Occupation";

            // 保存预制体
            string prefabPath = "Assets/Prefabs/UI/AgentListItem.prefab";
            PrefabUtility.SaveAsPrefabAsset(agentListItem, prefabPath);
            Object.DestroyImmediate(agentListItem);
        }

        private static void CreateNeedItemPrefab()
        {
            // 创建需求项预制体
            GameObject needItem = new GameObject("NeedItem");
            RectTransform rt = needItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 30);

            TextMeshProUGUI tmp = needItem.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 12;
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.text = "Need: Value/MaxValue (Urgency)";

            // 保存预制体
            string prefabPath = "Assets/Prefabs/UI/NeedItem.prefab";
            PrefabUtility.SaveAsPrefabAsset(needItem, prefabPath);
            Object.DestroyImmediate(needItem);
        }

        private static void CreateRelationshipItemPrefab()
        {
            // 创建关系项预制体
            GameObject relationshipItem = new GameObject("RelationshipItem");
            RectTransform rt = relationshipItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 40);

            TextMeshProUGUI tmp = relationshipItem.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 12;
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.text = "Relationship with Agent (Type)\nIntimacy: X Trust: Y";

            // 保存预制体
            string prefabPath = "Assets/Prefabs/UI/RelationshipItem.prefab";
            PrefabUtility.SaveAsPrefabAsset(relationshipItem, prefabPath);
            Object.DestroyImmediate(relationshipItem);
        }
    }
}
#endif 