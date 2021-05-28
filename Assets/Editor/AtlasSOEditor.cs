using UnityEditor;
using UnityEngine;
using Voxel;
using Voxel.Config;

namespace Editor
{
    [CustomEditor(typeof(AtlasSO))]
    public class AtlasSOEditor : UnityEditor.Editor
    {
        private bool _showUvs = false;
        private bool _showTextures = false;
        
        public override void OnInspectorGUI()
        {
            var atlasSO = (AtlasSO) target;

            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Viewable", EditorStyles.boldLabel);
            
            _showUvs = EditorGUILayout.Foldout(_showUvs, "Show UVs");
            if (_showUvs)
            {
                foreach (var uv in atlasSO.uvs)
                {
                    EditorGUILayout.Vector2Field(uv.Key, uv.Value);
                }
            }

            _showTextures = EditorGUILayout.Foldout(_showTextures, "Show Textures");
            if (_showTextures)
            {
                foreach (var texture in atlasSO.textures)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(texture);
                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.IntField("Dimensions", atlasSO.dimensions);
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Atlas"))
            {
                atlasSO.GenerateAtlas();
            }
        }
    }
}
