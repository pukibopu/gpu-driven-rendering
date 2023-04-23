using System;
using System.Collections.Generic;
using GPUInstance;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class DrawDataMono : MonoBehaviour
{
    public SceneDataAsset sceneDataAsset;
    private Dictionary<Mesh, List<Matrix4x4>> meshToTransforms;
    private Dictionary<Mesh, Material> meshToMaterial;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer positionBuffer;
    public Shader instanceShader;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private void Start()
    {
        AssetDatabase.Refresh();
        sceneDataAsset = AssetDatabase.LoadAssetAtPath<SceneDataAsset>("Assets/Data/test.asset");
       
        meshToTransforms = new Dictionary<Mesh, List<Matrix4x4>>();
        meshToMaterial = new Dictionary<Mesh, Material>();

        // 从sceneDataAsset中提取游戏对象的数据并填充meshToTransforms字典
        foreach (var gameObjectData in sceneDataAsset.gameObjectDataList)
        {
            Mesh mesh = gameObjectData.mesh;
            Matrix4x4 transformMatrix4X4 = Matrix4x4.TRS(gameObjectData.position,
                gameObjectData.rotation,
                gameObjectData.scale);
            
            if (!meshToTransforms.ContainsKey(mesh))
            {
                meshToMaterial.Add(mesh,gameObjectData.material);
                meshToTransforms.Add(mesh,new List<Matrix4x4>());
            }
            meshToTransforms[mesh].Add(transformMatrix4X4);
            Debug.Log(mesh.subMeshCount);
           
            
        }

        // 初始化indirect rendering参数缓冲区
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        
        
    }

    private void Update()
    {
        Draw();
        
         
    }

    private void Draw()
    {
        foreach (KeyValuePair<Mesh, List<Matrix4x4>> entry in meshToTransforms)
        {
            
            Mesh mesh = entry.Key;
            List<Matrix4x4> transforms = entry.Value;
            Material material = meshToMaterial[mesh];

            int subMeshIndex = mesh.subMeshCount-1;
            // 设置indirect rendering参数
            args[0] = mesh.GetIndexCount(0);
            args[1] = (uint)transforms.Count;
            args[2] = mesh.GetIndexStart(subMeshIndex);
            args[3] = mesh.GetBaseVertex(subMeshIndex);
            argsBuffer.SetData(args);
            
            int matrixSize = sizeof(float) * 16; // A Matrix4x4 has 16 float values
            int bufferSize = meshToTransforms[mesh].Count * matrixSize;
            positionBuffer = new ComputeBuffer(meshToTransforms[mesh].Count, matrixSize);
            positionBuffer.SetData(meshToTransforms[mesh].ToArray());

            material.shader = instanceShader;
            material.SetBuffer("positionBuffer",positionBuffer);
            
           
        
        
            // 使用Graphics.DrawMeshInstancedIndirect绘制游戏对象
            Graphics.DrawMeshInstancedIndirect(mesh, subMeshIndex, material, new Bounds(Vector3.zero, new Vector3(300.0f, 300.0f, 300.0f)), argsBuffer);
            
            //positionBuffer.Dispose();
        }
    }
    private void OnDestroy()
    {
        // 释放缓冲区
        if (argsBuffer != null)
            argsBuffer.Release();
        if (positionBuffer != null)
            positionBuffer.Release();
    }
}