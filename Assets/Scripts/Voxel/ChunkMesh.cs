using System.Collections.Generic;
using Managers;
using UnityEngine;
using Voxel.Config;
using Voxel.Enums;

namespace Voxel
{
    public class ChunkMesh : MonoBehaviour
    {
        public Chunk chunk;
        public AtlasSO atlas;
        
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        
        private List<Vector3> _vertexList;
        private List<int> _triangleList;
        private List<Vector3> _normalList;
        private List<Vector2> _uvList;
        
        private GameObject _waterObject;
        private MeshFilter _waterMeshFilter;
        private MeshRenderer _waterMeshRenderer;
        
        private List<Vector3> _waterVertexList;
        private List<int> _waterTriangleList;
        private List<Vector3> _waterNormalList;
        private List<Vector2> _waterUvList;
        
        [HideInInspector]
        public Chunk leftChunk;
        [HideInInspector]
        public Chunk rightChunk;
        [HideInInspector]
        public Chunk frontChunk;
        [HideInInspector]
        public Chunk backChunk;
        
        public void Setup()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _meshRenderer.sharedMaterial = atlas.material;
            
            _waterObject = new GameObject();
            _waterObject.transform.parent = transform;
            _waterObject.name = "Water";
            _waterObject.transform.localPosition = Vector3.zero;
            _waterMeshFilter = _waterObject.AddComponent<MeshFilter>();
            _waterMeshRenderer = _waterObject.AddComponent<MeshRenderer>();
            _waterMeshRenderer.sharedMaterial = atlas.waterMaterial;

            leftChunk = (chunk.chunkX - 1 >= 0)
                ? VoxelManager.instance.chunks[chunk.chunkX - 1, chunk.chunkZ]
                : null;
            rightChunk = (chunk.chunkX + 1 < VoxelManager.instance.chunks.GetLength(0))
                ? VoxelManager.instance.chunks[chunk.chunkX + 1, chunk.chunkZ]
                : null;
            backChunk = (chunk.chunkZ + 1 < VoxelManager.instance.chunks.GetLength(1))
                ? VoxelManager.instance.chunks[chunk.chunkX, chunk.chunkZ + 1]
                : null;
            frontChunk = (chunk.chunkZ - 1 >= 0)
                ? VoxelManager.instance.chunks[chunk.chunkX, chunk.chunkZ - 1]
                : null;
        }

        public void Create()
        {
            var mesh = new Mesh();
            var waterMesh = new Mesh();

            _vertexList = new List<Vector3>();
            _triangleList = new List<int>();
            _normalList = new List<Vector3>();
            _uvList = new List<Vector2>();
            
            _waterVertexList = new List<Vector3>();
            _waterTriangleList = new List<int>();
            _waterNormalList = new List<Vector3>();
            _waterUvList = new List<Vector2>();
            
            for (var x = 0; x < chunk.ChunkWidth; x++)
            {
                for (var y = 0; y < chunk.ChunkHeight; y++)
                {
                    for (var z = 0; z < chunk.ChunkDepth; z++)
                    {
                        var block = chunk.blocks[x, y, z];
                        var currentPosition = new Vector3(x, y, z);

                        if (block == Blocks.Air)
                        {
                            continue;
                        }

                        var rightBlock = ((x + 1 < chunk.ChunkWidth) ? chunk.blocks[x + 1, y, z]
                            : ((rightChunk != null) ? rightChunk.blocks[0, y, z] : Blocks.Air));
                        var leftBlock = ((x - 1 >= 0) ? chunk.blocks[x - 1, y, z]
                            : ((leftChunk != null) ? leftChunk.blocks[leftChunk.ChunkWidth - 1, y, z] : Blocks.Air));

                        var topBlock = ((y + 1 < chunk.ChunkHeight) ? chunk.blocks[x, y + 1, z] : Blocks.Air);
                        var bottomBlock = ((y - 1 >= 0) ? chunk.blocks[x, y - 1, z] : Blocks.Air);

                        var backBlock = ((z + 1 < chunk.ChunkDepth) ? chunk.blocks[x, y, z + 1]
                            : ((backChunk != null) ? backChunk.blocks[x, y, 0] : Blocks.Air));
                        var frontBlock = ((z - 1 >= 0) ? chunk.blocks[x, y, z - 1]
                            : ((frontChunk != null) ? frontChunk.blocks[x, y, frontChunk.ChunkDepth - 1] : Blocks.Air));

                        AddVertices(ref block, ref rightBlock, ref currentPosition, Sides.Right, Vector3.right);
                        AddVertices(ref block, ref leftBlock, ref currentPosition, Sides.Left, Vector3.left);
                        AddVertices(ref block, ref topBlock, ref currentPosition, Sides.Top, Vector3.up);
                        if (y != 0) AddVertices(ref block, ref bottomBlock, ref currentPosition, Sides.Bottom, Vector3.down);
                        AddVertices(ref block, ref backBlock, ref currentPosition, Sides.Back, Vector3.forward);
                        AddVertices(ref block, ref frontBlock, ref currentPosition, Sides.Front, Vector3.back);
                    }
                }
            }

            _meshFilter.mesh = mesh;
            mesh.vertices = _vertexList.ToArray();
            mesh.triangles = _triangleList.ToArray();
            mesh.normals = _normalList.ToArray();
            mesh.uv = _uvList.ToArray();
            _meshCollider.sharedMesh = mesh;
            
            _waterMeshFilter.mesh = waterMesh;
            waterMesh.vertices = _waterVertexList.ToArray();
            waterMesh.triangles = _waterTriangleList.ToArray();
            waterMesh.normals = _waterNormalList.ToArray();
            waterMesh.uv = _waterUvList.ToArray();
        }

        private void AddVertices(ref Blocks block, ref Blocks neighborBlock, ref Vector3 currentPosition, Sides side, Vector3 normal)
        {
            var blockData = VoxelManager.instance.blockData[block.ToString()];
            var neighborBlockData = VoxelManager.instance.blockData[neighborBlock.ToString()];

            var isVisible = !blockData.transparent && neighborBlockData.transparent || neighborBlock == Blocks.Air;

            if (!isVisible)
            {
                return;
            }
            
            var isWater = block == Blocks.Water;
            var size = isWater ? _waterVertexList.Count : _vertexList.Count;
            
            var directions = GetDirections(side);
            if (isWater)
            {
                foreach (var direction in directions)
                {
                    _waterVertexList.Add(currentPosition + direction);
                }
            }
            else
            {
                foreach (var direction in directions)
                {
                    _vertexList.Add(currentPosition + direction);
                }
            }

            AddFaceNormals(normal, isWater);
            AddTriangles(size, isWater);
            AddUVs(block, side, isWater);
        }
        
        private void AddFaceNormals(Vector3 direction, bool isWater = false)
        {
            for (var i = 0; i < 4; i++)
            {
                if (isWater)
                {
                    _waterNormalList.Add(direction);
                    continue;
                }

                _normalList.Add(direction);
            }
        }
        
        private void AddTriangles(int baseVertexNumber, bool isWater = false)
        {
            int[] offsets = {0, 3, 1, 0, 2, 3};
            
            if (isWater) 
            {
                foreach (var i in offsets) 
                {
                    _waterTriangleList.Add(baseVertexNumber + i);
                }

                return;
            }

            foreach (var offset in offsets) {
                _triangleList.Add(baseVertexNumber + offset);
            }
        }
        
        private void AddUVs(Blocks block, Sides side, bool isWater = false)
        {
            if (isWater)
            {
                _waterUvList.Add(Vector2.zero);
                _waterUvList.Add(Vector2.right);
                _waterUvList.Add(Vector2.up);
                _waterUvList.Add(Vector2.right + Vector2.up);

                return;
            }
            
            var blockData = VoxelManager.instance.blockData[block.ToString()];
            var texture = side switch
            {
                Sides.Top => blockData.textures.top,
                Sides.Bottom => blockData.textures.bottom,
                _ => blockData.textures.side
            };
            var textureStart = atlas.uvs[texture];

            var divider = 1f / (atlas.dimensions);
            _uvList.Add(textureStart + divider * Vector2.zero);
            _uvList.Add(textureStart + divider * Vector2.right);
            _uvList.Add(textureStart + divider * Vector2.up);
            _uvList.Add(textureStart + divider * Vector2.right + divider * Vector2.up);
        }
        
        private static Vector3[] GetDirections(Sides side)
        {
            switch (side)
            {
                case Sides.Right:
                {
                    Vector3[] rightDirections =
                    {
                        new Vector3(1, 0, 0), 
                        new Vector3(1, 0, 1), 
                        new Vector3(1, 1, 0), 
                        new Vector3(1, 1, 1)
                    };
                    return rightDirections;
                }
                case Sides.Left:
                {
                    Vector3[] leftDirections = 
                    { 
                        new Vector3(0, 0, 1), 
                        new Vector3(0, 0, 0), 
                        new Vector3(0, 1, 1), 
                        new Vector3(0, 1, 0)
                    };
                    return leftDirections;
                }
                case Sides.Top:
                {
                    Vector3[] topDirections = 
                    { 
                        new Vector3(0, 1, 0), 
                        new Vector3(1, 1, 0), 
                        new Vector3(0, 1, 1), 
                        new Vector3(1, 1, 1)
                    };
                    return topDirections;
                }
                case Sides.Bottom:
                {
                    Vector3[] bottomDirections = 
                    { 
                        new Vector3(1, 0, 0), 
                        new Vector3(0, 0, 0), 
                        new Vector3(1, 0, 1), 
                        new Vector3(0, 0, 1)
                    };
                    return bottomDirections;
                }
                case Sides.Back:
                {
                    Vector3[] backDirections = 
                    { 
                        new Vector3(1, 0, 1), 
                        new Vector3(0, 0, 1), 
                        new Vector3(1, 1, 1), 
                        new Vector3(0, 1, 1)
                    };
                    return backDirections;
                }
                case Sides.Front:
                {
                    Vector3[] frontDirections = 
                    { 
                        new Vector3(0, 0, 0), 
                        new Vector3(1, 0, 0), 
                        new Vector3(0, 1, 0), 
                        new Vector3(1, 1, 0)
                    };
                    return frontDirections;
                }
                default:
                {
                    Vector3[] directions = { };
                    return directions;
                }
            }
        }
    }
}