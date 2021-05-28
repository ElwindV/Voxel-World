using UnityEngine;

namespace Voxel
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Voxel/New Settings", order = 1)]
    public class VoxelSettingsSO : ScriptableObject
    {
        public int voxelSize = 1;
    }
}
