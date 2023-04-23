using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstance
{


    [CreateAssetMenu(fileName = "SceneDataAsset", menuName = "Scene Data/Scene Data Asset")]
    public class SceneDataAsset : ScriptableObject
    {
        public List<GameObjectData> gameObjectDataList;
    }
    [System.Serializable]
    public class GameObjectData
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Material material;
        public Mesh mesh;
    }
}
