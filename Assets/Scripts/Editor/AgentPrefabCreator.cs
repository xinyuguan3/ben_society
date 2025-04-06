using UnityEngine;
using UnityEditor;

public class AgentPrefabCreator
{
    [MenuItem("CamelSociety/Create Agent Prefab")]
    public static void CreateAgentPrefab()
    {
        // 创建Agent游戏对象
        GameObject agentObject = new GameObject("Agent");
        
        // 添加必要的组件
        agentObject.AddComponent<CamelSociety.Core.Agent>();
        
        // 添加可视化组件
        SpriteRenderer spriteRenderer = agentObject.AddComponent<SpriteRenderer>();
        
        // 创建一个简单的方形精灵
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.white;
        }
        texture.SetPixels(colors);
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f);

        // 保存预制体
        string prefabPath = "Assets/Prefabs/AgentPrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(agentObject, prefabPath);
        
        // 清理临时对象
        Object.DestroyImmediate(agentObject);
        
        Debug.Log("Agent预制体已创建: " + prefabPath);
    }
} 