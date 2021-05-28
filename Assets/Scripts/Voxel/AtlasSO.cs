using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Voxel
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Voxel/New Atlas", order = 1)]
    public class AtlasSO : ScriptableObject
    {
        [Header("Settings")]
        public Material material;

        [HideInInspector] public Texture2D atlasTexture;

        public Dictionary<string, Vector2> uvs;

        [HideInInspector]
        public Texture2D[] textures;
        
        [HideInInspector]
        public int dimensions = 0;

        private const int TextureWidth = 16;
        private const int TextureHeight = 16;

        private const string TexturePath = "Assets/Resources/Materials/Textures/Voxel.asset";
        
        public void GenerateAtlas()
        {
            uvs = new Dictionary<string, Vector2>();
            textures = Resources.LoadAll<Texture2D>("Atlas");
            
            var textureCount = textures.Length;

            dimensions = GetAtlasDimension(textureCount);

            atlasTexture = new Texture2D(TextureWidth * dimensions, TextureHeight * dimensions)
            {
                anisoLevel = 1,
                filterMode = FilterMode.Point
            };

            for (var i = 0; i < textures.Length; i++) {
                var texture = textures[i];

                var horizontalAtlasOffset = (i % dimensions) * TextureWidth;
                var verticalAtlasOffset = (i / dimensions) * TextureHeight;

                var textureX = i % dimensions;
                var textureY = i / dimensions;

                uvs.Add(
                    texture.name,
                    new Vector2((textureX * 1f) / (dimensions * 1f), (textureY * 1f) / (dimensions * 1f))
                );

                var pixels = texture.GetPixels(0, 0, texture.width, texture.height);
                for (var y = 0; y < texture.height; y++)
                {
                    for (var x = 0; x < texture.width; x++)
                    {
                        atlasTexture.SetPixel(x + horizontalAtlasOffset, y + verticalAtlasOffset, pixels[x + y * 16]);
                    }
                }
            }
            atlasTexture.Apply();
            
            AssetDatabase.CreateAsset(atlasTexture, TexturePath);
            material.mainTexture = atlasTexture;
        }
        
        private static int GetAtlasDimension(int count) => (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(count) / Mathf.Log(4)));
    }
}
