using System.Collections.Generic;
using UnityEngine;
using Voxel;
using Voxel.Config;
using Voxel.JsonObjects;

namespace Managers
{
    public class VoxelManager : MonoBehaviour
    {
        public AtlasSO atlas;
        public WorldGenerationSettingsSO worldGenerationSettings;
        public VoxelSettingsSO voxelSettings;

        public Dictionary<string, JsonBlock> blockData;
        public Chunk[,] chunks = new Chunk[0,0];

        public static VoxelManager instance = null;

        private void Awake()
        {
            if (transform.childCount == 0)
            {
                GenerateWorld();
            }
        }

        public void GenerateWorld()
        {
            instance = this;
            atlas.GenerateAtlas();
            LoadBlockData();
            GenerateChunks();
            GenerateMeshes();
        }

        private void LoadBlockData()
        {            
            var jsonFile = Resources.Load<TextAsset>("Data/blocks");
            var blocksContainer = JsonUtility.FromJson<JsonBlockContainer>(jsonFile.text);

            blockData = new Dictionary<string, JsonBlock>();

            foreach (var block in blocksContainer.blocks)
            {
                blockData[block.name] = block;
            }
        }

        private void GenerateChunks()
        {
            chunks = new Chunk[voxelSettings.chunkCountX, voxelSettings.chunkCountZ];

            for (var x = 0; x < voxelSettings.chunkCountX; x++)
            {
                for (var z = 0; z < voxelSettings.chunkCountZ; z++)
                {
                    var chunk = new GameObject();
                    
                    chunk.transform.SetParent(transform);
                    chunk.name = $"Chunk {x}:{z}";
                    chunk.transform.position = new Vector3(voxelSettings.chunkWidth * x, 0, voxelSettings.chunkDepth * z);

                    var chunkComponent = chunk.AddComponent<Chunk>();
                    chunkComponent.chunkX = x;
                    chunkComponent.chunkZ = z;
                    chunkComponent.AddSettings(voxelSettings, worldGenerationSettings);
                    chunkComponent.Generate();

                    chunkComponent.mesh = chunk.AddComponent<ChunkMesh>();
                    chunkComponent.mesh.chunk = chunkComponent;
                    chunkComponent.mesh.atlas = atlas;

                    chunks[x, z] = chunkComponent;
                }
            }
        }
        
        private void GenerateMeshes()
        {
            for (var x = 0; x < voxelSettings.chunkCountX; x++)
            {
                for (var z = 0; z < voxelSettings.chunkCountZ; z++)
                {
                    var chunkMesh = chunks[x, z].mesh;
                    chunkMesh.Setup();
                    chunkMesh.Create();
                }
            }
        }
    }
}
