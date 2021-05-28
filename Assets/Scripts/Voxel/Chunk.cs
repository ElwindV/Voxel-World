using TMPro;
using UnityEngine;
using Voxel.Config;
using Voxel.Enums;

namespace Voxel
{
    public class Chunk : MonoBehaviour
    {
        [HideInInspector]
        public ChunkMesh mesh;
        
        public Blocks[,,] blocks = new Blocks[0, 0, 0];
        
        private VoxelSettingsSO _voxelSettings;
        private WorldGenerationSettingsSO _settings;
        
        [HideInInspector]
        public int chunkX;
        
        [HideInInspector]
        public int chunkZ;

        public uint ChunkWidth => _voxelSettings.chunkWidth;
        public uint ChunkHeight => _voxelSettings.chunkHeight;
        public uint ChunkDepth => _voxelSettings.chunkDepth;

        public void AddSettings(VoxelSettingsSO voxelSettings, WorldGenerationSettingsSO settings)
        {
            _voxelSettings = voxelSettings;
            _settings = settings;
        }

        public void Generate()
        {
            blocks = new Blocks[ChunkWidth, ChunkHeight, ChunkDepth];
            
            for (var x = 0; x < _voxelSettings.chunkWidth; x++)
            {
                for (var z = 0; z < _voxelSettings.chunkDepth; z++)
                {
                    var position = transform.position;
                    
                    var xComponent = _settings.seed + ((position.x + (x * 1f)) * _settings.factor);
                    var yComponent = _settings.seed + ((position.z + (z * 1f)) * _settings.factor);
                    var noiseFactor = Mathf.PerlinNoise(xComponent, yComponent);
                    var stoneLayer = (int)(10f + noiseFactor * 15f);

                    var worldX = Mathf.Pow(-64f + (position.x + (x * 1f)), 2f);
                    var worldZ = Mathf.Pow(-64f + (position.z + (z * 1f)), 2f);
                    var multiplier = 1 - (_settings.islandFactor * worldX + _settings.islandFactor * worldZ);
                    multiplier = Mathf.Clamp(multiplier, 0, 2f);

                    stoneLayer = (int)((stoneLayer * 1f) * multiplier);
                
                    for (var y = 0; y < 32; y++)
                    {
                        if (y == 0)
                        {
                            blocks[x, y, z] = Blocks.Bedrock;
                            
                            continue;
                        }
                        if (y < stoneLayer)
                        {
                            blocks[x, y, z] = Blocks.Stone;
                            
                            continue;
                        }
                        if (y < stoneLayer + 3 && stoneLayer > _settings.waterLevel)
                        {
                            blocks[x, y, z] = Blocks.Dirt;
                            
                            continue;
                        }
                        if (y == 1) {
                            blocks[x, y, z] = Blocks.Sand;
                            
                            continue;
                        }
                        if (y < stoneLayer + 4 && y < _settings.waterLevel)
                        {
                            blocks[x, y, z] = Blocks.Sand;
                            
                            continue;
                        }
                        if (y < _settings.waterLevel)
                        {
                            blocks[x, y, z] = Blocks.Water;
                            
                            continue;
                        }
                        if (y < stoneLayer + 4)
                        {
                            blocks[x, y, z] = (y < _settings.maxGrassLevel) 
                                ? Blocks.Grass 
                                : (y > _settings.snowLevel)
                                    ? Blocks.Snow
                                    : Blocks.Stone;
                            
                            continue;
                        }
                        blocks[x, y, z] = Blocks.Air;
                    }
                }
            }
        }
    }
}