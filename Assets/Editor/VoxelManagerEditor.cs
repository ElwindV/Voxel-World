using Managers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(VoxelManager))]
    public class VoxelManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var voxelManager = (VoxelManager) target;
            
            DrawDefaultInspector();
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate World"))
            {
                var childCount = voxelManager.transform.childCount - 1;
                for (var i = childCount; i >= 0; i--)
                {
                    DestroyImmediate(voxelManager.transform.GetChild(i).gameObject);
                }

                foreach (Transform child in voxelManager.transform)
                {
                    DestroyImmediate(child.gameObject);
                }

                voxelManager.GenerateWorld();
            }
        }
    }
}