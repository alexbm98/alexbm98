// Copyright (C) 2018-2021 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using Unity.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using System;

namespace Mercuna
{
    public class MerGeometryCollector
    {
        public void GetSourceInfo(int numSources, List<NavMeshBuildSource> sources, MerAABB bounds, MerGeometryInfo[] infos)
        {
            for (int i = 0; i < numSources; ++i)
            {
                infos[i].shape = sources[i].shape;
                infos[i].size = sources[i].size;
                infos[i].transform = sources[i].transform.transpose;

                if (infos[i].shape == NavMeshBuildSourceShape.Terrain)
                {
                    // For terrains need to know the number of width and height they will use, so that the right
                    // amount of memory can be allocated for when the vertices are collected
                    Vector3 position = new Vector3(sources[i].transform[0, 3], sources[i].transform[1, 3], sources[i].transform[2, 3]);
                    TerrainData terrain = (TerrainData)sources[i].sourceObject;

                    int xBase = Mathf.Max(0, (int)((bounds.minx - position.x) / terrain.heightmapScale.x));
                    int yBase = Mathf.Max(0, (int)((bounds.minz - position.z) / terrain.heightmapScale.z));
                    infos[i].width = Mathf.Min(terrain.heightmapResolution - xBase, (int)((bounds.maxx - bounds.minx) / terrain.heightmapScale.x) + 1);
                    infos[i].height = Mathf.Min(terrain.heightmapResolution - yBase, (int)((bounds.maxz - bounds.minz) / terrain.heightmapScale.z) + 1);
                }
                else
                {
                    infos[i].width = 0;
                    infos[i].height = 0;
                }
            }
        }

        public IntPtr CollectMesh(IntPtr pGeometryCollector, NavMeshBuildSource source)
        {
            IntPtr pTriMesh;

            Mesh mesh = (Mesh)source.sourceObject;
            using (var meshDatas = Mesh.AcquireReadOnlyMeshData(mesh))
            {
                Assert.IsTrue(meshDatas.Length == 1);

                int subMeshCount = meshDatas[0].subMeshCount;

                NativeArray<Vector3> vertices = new NativeArray<Vector3>(mesh.vertexCount, Allocator.Temp);
                meshDatas[0].GetVertices(vertices);
                MerNativeArray merVertices = new MerNativeArray();
                merVertices.len = mesh.vertexCount;

                NativeArray<int>[] indices = new NativeArray<int>[subMeshCount];
                MerNativeArray[] merIndices = new MerNativeArray[subMeshCount];

                unsafe
                {
                    merVertices.ptr = NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(vertices);

                    for (int j = 0; j < subMeshCount; ++j)
                    {
                        int indexCount = meshDatas[0].GetSubMesh(j).indexCount;
                        indices[j] = new NativeArray<int>(indexCount, Allocator.Temp);
                        meshDatas[0].GetIndices(indices[j], j);

                        merIndices[j] = new MerNativeArray();
                        merIndices[j].ptr = NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr(indices[j]);
                        merIndices[j].len = indexCount;
                    }

                    pTriMesh = ProcessGeometryMesh(pGeometryCollector, source.transform.transpose, merVertices, merIndices, subMeshCount);
                }

                for (int j = 0; j < subMeshCount; ++j)
                {
                    indices[j].Dispose();
                }

                vertices.Dispose();
            }

            return pTriMesh;
        }

        public void CollectTerrain(NavMeshBuildSource source, MerAABB bounds, float[] pVertices)
        {
            Vector3 position = new Vector3(source.transform[0, 3], source.transform[1, 3], source.transform[2, 3]);
            TerrainData terrain = (TerrainData)source.sourceObject;

            int xBase = Mathf.Max(0, (int)((bounds.minx - position.x) / terrain.heightmapScale.x));
            int yBase = Mathf.Max(0, (int)((bounds.minz - position.z) / terrain.heightmapScale.z));
            int width = Mathf.Min(terrain.heightmapResolution - xBase, (int)((bounds.maxx - bounds.minx) / terrain.heightmapScale.x) + 1);
            int height = Mathf.Min(terrain.heightmapResolution - yBase, (int)((bounds.maxz - bounds.minz) / terrain.heightmapScale.z) + 1);

            float[,] heights = terrain.GetHeights(xBase, yBase, width, height);

            for (int j = 0; j < height; ++j)
            {
                for (int i = 0; i < width; ++i)
                {
                    pVertices[(j * width + i) * 3] = (xBase + i) * terrain.heightmapScale.x + position.x;
                    pVertices[(j * width + i) * 3 + 1] = heights[j, i] * terrain.size.y + position.y;
                    pVertices[(j * width + i) * 3 + 2] = (yBase + j) * terrain.heightmapScale.z + position.z;
                }
            }
        }

        // Calls to native
        [DllImport(Mercuna.MERCUNA_DLL_NAME, CallingConvention = Mercuna.MERCUNA_CALLING_CONVENTION)]
        private static extern IntPtr ProcessGeometryMesh(IntPtr pGeometryCollector, Matrix4x4 transform, MerNativeArray vertices,
                                                         [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] MerNativeArray[] pIndices, int numIndices);
    }
}
