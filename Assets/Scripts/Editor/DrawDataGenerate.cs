using System;
using System.Collections;
using System.Collections.Generic;
using GPUInstance;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace GPUInstance
{
   
    public class DrawDataGenerate : EditorWindow
    {
        [MenuItem("Tools/Export Scene Data to Asset")]
        public static void ExportSceneData()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Map_v1.unity");
            
            // 获取场景中的所有游戏对象
            GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
    
            // 创建一个列表，用于存储游戏对象的数据
            List<GameObjectData> gameObjectDataList = new List<GameObjectData>();
    
            // 遍历所有游戏对象，提取材质和变换等信息
            foreach (GameObject go in allGameObjects)
            {
                MeshFilter meshFilter = go.GetComponent<MeshFilter>();
                Renderer renderer = go.GetComponent<Renderer>();
                if (meshFilter == null || renderer == null)
                    continue;
                GameObjectData gameObjectData = new GameObjectData
                {
                    name = go.name,
                    position = go.transform.position,
                    rotation = go.transform.rotation,
                    scale = go.transform.localScale,
                    mesh = meshFilter.sharedMesh,
                    material = renderer.sharedMaterial
                };
                gameObjectDataList.Add(gameObjectData);
            }
        
            // 创建一个ScriptableObject用于存储数据
            SceneDataAsset sceneDataAsset = ScriptableObject.CreateInstance<SceneDataAsset>();
            sceneDataAsset.gameObjectDataList = gameObjectDataList;
            //EditorSceneManager.OpenScene("Assets/Scenes/Test.unity");
            // 将ScriptableObject保存为.asset文件
            AssetDatabase.CreateAsset(sceneDataAsset,"Assets/Data/test.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
    
            Debug.Log("Scene data exported to test.asset");
        }
    }
    
  
    

}
