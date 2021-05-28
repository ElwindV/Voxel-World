using UnityEngine;

namespace Voxel.Config
{
    [CreateAssetMenu(fileName = "VoxelSettings", menuName = "ScriptableObjects/Voxel/New Voxel Settings", order = 1)]
    public class VoxelSettingsSO : ScriptableObject
    {
        [Header("Chunk Count")] 
        public uint chunkCountX = 4;
        public uint chunkCountZ = 4;
        
        [Header("Chunk Settings")]
        public uint chunkWidth = 16;
        public uint chunkHeight = 32;
        public uint chunkDepth = 16;
    }
}
