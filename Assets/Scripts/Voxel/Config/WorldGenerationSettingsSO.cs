using UnityEngine;

namespace Voxel.Config
{
    [CreateAssetMenu(fileName = "WorldGenerationSettings", menuName = "ScriptableObjects/Voxel/New World Settings", order = 1)]
    public class WorldGenerationSettingsSO : ScriptableObject
    {
        [Header("Settings")]
        
        [Range(-10000, 10000)] public int seed = 4113;
        [Range(0, 1)] public float factor = .07f;
        public float islandFactor = .0002f;
        [Range(0, 32)] public int waterLevel = 12;
        [Range(0, 32)] public int maxGrassLevel = 18;
        [Range(0, 32)] public int snowLevel = 21;
        
        [Range(0, 10)] public int treesPerChunk = 4;
    }
}