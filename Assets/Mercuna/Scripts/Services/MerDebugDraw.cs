// Copyright (C) 2018-2020 Mercuna Developments Limited - All rights reserved
// This source file is part of the Mercuna Middleware
// Use, modification and distribution of this file or any portion thereof
// is only permitted as specified in your Mercuna License Agreement.
using UnityEngine;
using System.Runtime.InteropServices;

namespace Mercuna
{
    public class MerDebugDraw
    {         
        private Material m_debugMaterial = null;
        private Material debugMaterial
        {
            get
            {
                if (m_debugMaterial == null)
                {
                    m_debugMaterial = new Material(Shader.Find("Mercuna/UnlitDebug"));
                }
                return m_debugMaterial;
            }
        }

        private Mesh m_sphereMesh = null;
        private Mesh sphereMesh
        {
            get
            {
                if (m_sphereMesh == null)
                {
                    // Generated using https://schneide.wordpress.com/2016/07/15/generating-an-icosphere-in-c/
                    m_sphereMesh = new Mesh();
                    m_sphereMesh.vertices = new[] { new Vector3(-0.525731f, 0f, 0.850651f), new Vector3(0.525731f, 0f, 0.850651f), new Vector3(-0.525731f, 0f, -0.850651f), new Vector3(0.525731f, 0f, -0.850651f), new Vector3(0f, 0.850651f, 0.525731f), new Vector3(0f, 0.850651f, -0.525731f), new Vector3(0f, -0.850651f, 0.525731f), new Vector3(0f, -0.850651f, -0.525731f), new Vector3(0.850651f, 0.525731f, 0f), new Vector3(-0.850651f, 0.525731f, 0f), new Vector3(0.850651f, -0.525731f, 0f), new Vector3(-0.850651f, -0.525731f, 0f), new Vector3(-0.309017f, 0.5f, 0.809017f), new Vector3(0.309017f, 0.5f, 0.809017f), new Vector3(0f, 0f, 1f), new Vector3(-0.809017f, 0.309017f, 0.5f), new Vector3(-0.5f, 0.809017f, 0.309017f), new Vector3(-0.5f, 0.809017f, -0.309017f), new Vector3(0f, 1f, 0f), new Vector3(0.5f, 0.809017f, -0.309017f), new Vector3(0.5f, 0.809017f, 0.309017f), new Vector3(0.809017f, 0.309017f, 0.5f), new Vector3(1f, 0f, 0f), new Vector3(0.809017f, -0.309017f, 0.5f), new Vector3(0.809017f, 0.309017f, -0.5f), new Vector3(0.809017f, -0.309017f, -0.5f), new Vector3(0.309017f, 0.5f, -0.809017f), new Vector3(-0.309017f, 0.5f, -0.809017f), new Vector3(0f, 0f, -1f), new Vector3(-0.309017f, -0.5f, -0.809017f), new Vector3(0.309017f, -0.5f, -0.809017f), new Vector3(0.5f, -0.809017f, -0.309017f), new Vector3(0f, -1f, 0f), new Vector3(0.5f, -0.809017f, 0.309017f), new Vector3(-0.5f, -0.809017f, -0.309017f), new Vector3(-0.5f, -0.809017f, 0.309017f), new Vector3(-0.809017f, -0.309017f, 0.5f), new Vector3(-0.309017f, -0.5f, 0.809017f), new Vector3(0.309017f, -0.5f, 0.809017f), new Vector3(-1f, 0f, 0f), new Vector3(-0.809017f, -0.309017f, -0.5f), new Vector3(-0.809017f, 0.309017f, -0.5f), new Vector3(-0.433889f, 0.259892f, 0.862669f), new Vector3(-0.16246f, 0.262866f, 0.951057f), new Vector3(-0.273266f, 0f, 0.961938f), new Vector3(0.160622f, 0.702046f, 0.69378f), new Vector3(0f, 0.525731f, 0.850651f), new Vector3(-0.160622f, 0.702046f, 0.69378f), new Vector3(0.273266f, 0f, 0.961938f), new Vector3(0.16246f, 0.262866f, 0.951057f), new Vector3(0.433889f, 0.259892f, 0.862669f), new Vector3(-0.69378f, 0.160622f, 0.702046f), new Vector3(-0.587785f, 0.425325f, 0.688191f), new Vector3(-0.702046f, 0.69378f, 0.160622f), new Vector3(-0.688191f, 0.587785f, 0.425325f), new Vector3(-0.862669f, 0.433889f, 0.259892f), new Vector3(-0.425325f, 0.688191f, 0.587785f), new Vector3(-0.259892f, 0.862669f, 0.433889f), new Vector3(-0.702046f, 0.69378f, -0.160622f), new Vector3(-0.525731f, 0.850651f, 0f), new Vector3(0f, 0.961938f, -0.273266f), new Vector3(-0.262866f, 0.951057f, -0.16246f), new Vector3(-0.259892f, 0.862669f, -0.433889f), new Vector3(-0.262866f, 0.951057f, 0.16246f), new Vector3(0f, 0.961938f, 0.273266f), new Vector3(0.262866f, 0.951057f, 0.16246f), new Vector3(0.259892f, 0.862669f, 0.433889f), new Vector3(0.259892f, 0.862669f, -0.433889f), new Vector3(0.262866f, 0.951057f, -0.16246f), new Vector3(0.702046f, 0.69378f, 0.160622f), new Vector3(0.525731f, 0.850651f, 0f), new Vector3(0.702046f, 0.69378f, -0.160622f), new Vector3(0.425325f, 0.688191f, 0.587785f), new Vector3(0.862669f, 0.433889f, 0.259892f), new Vector3(0.688191f, 0.587785f, 0.425325f), new Vector3(0.587785f, 0.425325f, 0.688191f), new Vector3(0.69378f, 0.160622f, 0.702046f), new Vector3(0.961938f, 0.273266f, 0f), new Vector3(0.951057f, 0.16246f, 0.262866f), new Vector3(0.862669f, -0.433889f, 0.259892f), new Vector3(0.951057f, -0.16246f, 0.262866f), new Vector3(0.961938f, -0.273266f, 0f), new Vector3(0.850651f, 0f, 0.525731f), new Vector3(0.69378f, -0.160622f, 0.702046f), new Vector3(0.862669f, 0.433889f, -0.259892f), new Vector3(0.951057f, 0.16246f, -0.262866f), new Vector3(0.69378f, -0.160622f, -0.702046f), new Vector3(0.850651f, 0f, -0.525731f), new Vector3(0.69378f, 0.160622f, -0.702046f), new Vector3(0.951057f, -0.16246f, -0.262866f), new Vector3(0.862669f, -0.433889f, -0.259892f), new Vector3(0.160622f, 0.702046f, -0.69378f), new Vector3(0.425325f, 0.688191f, -0.587785f), new Vector3(0.587785f, 0.425325f, -0.688191f), new Vector3(0.433889f, 0.259892f, -0.862669f), new Vector3(0.688191f, 0.587785f, -0.425325f), new Vector3(-0.160622f, 0.702046f, -0.69378f), new Vector3(0f, 0.525731f, -0.850651f), new Vector3(-0.273266f, 0f, -0.961938f), new Vector3(-0.16246f, 0.262866f, -0.951057f), new Vector3(-0.433889f, 0.259892f, -0.862669f), new Vector3(0.16246f, 0.262866f, -0.951057f), new Vector3(0.273266f, 0f, -0.961938f), new Vector3(-0.433889f, -0.259892f, -0.862669f), new Vector3(-0.16246f, -0.262866f, -0.951057f), new Vector3(0.160622f, -0.702046f, -0.69378f), new Vector3(0f, -0.525731f, -0.850651f), new Vector3(-0.160622f, -0.702046f, -0.69378f), new Vector3(0.16246f, -0.262866f, -0.951057f), new Vector3(0.433889f, -0.259892f, -0.862669f), new Vector3(0.259892f, -0.862669f, -0.433889f), new Vector3(0.425325f, -0.688191f, -0.587785f), new Vector3(0.688191f, -0.587785f, -0.425325f), new Vector3(0.702046f, -0.69378f, -0.160622f), new Vector3(0.587785f, -0.425325f, -0.688191f), new Vector3(0f, -0.961938f, -0.273266f), new Vector3(0.262866f, -0.951057f, -0.16246f), new Vector3(0.259892f, -0.862669f, 0.433889f), new Vector3(0.262866f, -0.951057f, 0.16246f), new Vector3(0f, -0.961938f, 0.273266f), new Vector3(0.525731f, -0.850651f, 0f), new Vector3(0.702046f, -0.69378f, 0.160622f), new Vector3(-0.259892f, -0.862669f, -0.433889f), new Vector3(-0.262866f, -0.951057f, -0.16246f), new Vector3(-0.702046f, -0.69378f, 0.160622f), new Vector3(-0.525731f, -0.850651f, 0f), new Vector3(-0.702046f, -0.69378f, -0.160622f), new Vector3(-0.262866f, -0.951057f, 0.16246f), new Vector3(-0.259892f, -0.862669f, 0.433889f), new Vector3(-0.862669f, -0.433889f, 0.259892f), new Vector3(-0.688191f, -0.587785f, 0.425325f), new Vector3(-0.433889f, -0.259892f, 0.862669f), new Vector3(-0.587785f, -0.425325f, 0.688191f), new Vector3(-0.69378f, -0.160622f, 0.702046f), new Vector3(-0.425325f, -0.688191f, 0.587785f), new Vector3(-0.160622f, -0.702046f, 0.69378f), new Vector3(-0.16246f, -0.262866f, 0.951057f), new Vector3(0.433889f, -0.259892f, 0.862669f), new Vector3(0.16246f, -0.262866f, 0.951057f), new Vector3(0f, -0.525731f, 0.850651f), new Vector3(0.160622f, -0.702046f, 0.69378f), new Vector3(0.425325f, -0.688191f, 0.587785f), new Vector3(0.587785f, -0.425325f, 0.688191f), new Vector3(0.688191f, -0.587785f, 0.425325f), new Vector3(-0.951057f, 0.16246f, 0.262866f), new Vector3(-0.961938f, 0.273266f, 0f), new Vector3(-0.850651f, 0f, 0.525731f), new Vector3(-0.961938f, -0.273266f, 0f), new Vector3(-0.951057f, -0.16246f, 0.262866f), new Vector3(-0.951057f, 0.16246f, -0.262866f), new Vector3(-0.862669f, 0.433889f, -0.259892f), new Vector3(-0.862669f, -0.433889f, -0.259892f), new Vector3(-0.951057f, -0.16246f, -0.262866f), new Vector3(-0.69378f, 0.160622f, -0.702046f), new Vector3(-0.850651f, 0f, -0.525731f), new Vector3(-0.69378f, -0.160622f, -0.702046f), new Vector3(-0.688191f, 0.587785f, -0.425325f), new Vector3(-0.587785f, 0.425325f, -0.688191f), new Vector3(-0.425325f, 0.688191f, -0.587785f), new Vector3(-0.425325f, -0.688191f, -0.587785f), new Vector3(-0.587785f, -0.425325f, -0.688191f), new Vector3(-0.688191f, -0.587785f, -0.425325f) };
                    m_sphereMesh.triangles = new [] { 0, 44, 42, 12, 42, 43, 14, 43, 44, 42, 44, 43, 4, 47, 45, 13, 45, 46, 12, 46, 47, 45, 47, 46, 1, 50, 48, 14, 48, 49, 13, 49, 50, 48, 50, 49, 12, 43, 46, 13, 46, 49, 14, 49, 43, 46, 43, 49, 0, 42, 51, 15, 51, 52, 12, 52, 42, 51, 42, 52, 9, 55, 53, 16, 53, 54, 15, 54, 55, 53, 55, 54, 4, 57, 47, 12, 47, 56, 16, 56, 57, 47, 57, 56, 15, 52, 54, 16, 54, 56, 12, 56, 52, 54, 52, 56, 9, 53, 58, 17, 58, 59, 16, 59, 53, 58, 53, 59, 5, 62, 60, 18, 60, 61, 17, 61, 62, 60, 62, 61, 4, 64, 57, 16, 57, 63, 18, 63, 64, 57, 64, 63, 17, 59, 61, 18, 61, 63, 16, 63, 59, 61, 59, 63, 4, 66, 64, 18, 64, 65, 20, 65, 66, 64, 66, 65, 5, 60, 67, 19, 67, 68, 18, 68, 60, 67, 60, 68, 8, 71, 69, 20, 69, 70, 19, 70, 71, 69, 71, 70, 18, 65, 68, 19, 68, 70, 20, 70, 65, 68, 65, 70, 4, 45, 66, 20, 66, 72, 13, 72, 45, 66, 45, 72, 8, 69, 73, 21, 73, 74, 20, 74, 69, 73, 69, 74, 1, 76, 50, 13, 50, 75, 21, 75, 76, 50, 76, 75, 20, 72, 74, 21, 74, 75, 13, 75, 72, 74, 72, 75, 8, 73, 77, 22, 77, 78, 21, 78, 73, 77, 73, 78, 10, 81, 79, 23, 79, 80, 22, 80, 81, 79, 81, 80, 1, 83, 76, 21, 76, 82, 23, 82, 83, 76, 83, 82, 22, 78, 80, 23, 80, 82, 21, 82, 78, 80, 78, 82, 8, 77, 84, 24, 84, 85, 22, 85, 77, 84, 77, 85, 3, 88, 86, 25, 86, 87, 24, 87, 88, 86, 88, 87, 10, 90, 81, 22, 81, 89, 25, 89, 90, 81, 90, 89, 24, 85, 87, 25, 87, 89, 22, 89, 85, 87, 85, 89, 5, 67, 91, 26, 91, 92, 19, 92, 67, 91, 67, 92, 3, 94, 88, 24, 88, 93, 26, 93, 94, 88, 94, 93, 8, 84, 71, 19, 71, 95, 24, 95, 84, 71, 84, 95, 26, 92, 93, 24, 93, 95, 19, 95, 92, 93, 92, 95, 5, 91, 96, 27, 96, 97, 26, 97, 91, 96, 91, 97, 2, 100, 98, 28, 98, 99, 27, 99, 100, 98, 100, 99, 3, 102, 94, 26, 94, 101, 28, 101, 102, 94, 102, 101, 27, 97, 99, 28, 99, 101, 26, 101, 97, 99, 97, 101, 2, 98, 103, 29, 103, 104, 28, 104, 98, 103, 98, 104, 7, 107, 105, 30, 105, 106, 29, 106, 107, 105, 107, 106, 3, 109, 102, 28, 102, 108, 30, 108, 109, 102, 109, 108, 29, 104, 106, 30, 106, 108, 28, 108, 104, 106, 104, 108, 7, 105, 110, 31, 110, 111, 30, 111, 105, 110, 105, 111, 10, 113, 90, 25, 90, 112, 31, 112, 113, 90, 113, 112, 3, 86, 109, 30, 109, 114, 25, 114, 86, 109, 86, 114, 31, 111, 112, 25, 112, 114, 30, 114, 111, 112, 111, 114, 7, 110, 115, 32, 115, 116, 31, 116, 110, 115, 110, 116, 6, 119, 117, 33, 117, 118, 32, 118, 119, 117, 119, 118, 10, 121, 113, 31, 113, 120, 33, 120, 121, 113, 121, 120, 32, 116, 118, 33, 118, 120, 31, 120, 116, 118, 116, 120, 7, 115, 122, 34, 122, 123, 32, 123, 115, 122, 115, 123, 11, 126, 124, 35, 124, 125, 34, 125, 126, 124, 126, 125, 6, 128, 119, 32, 119, 127, 35, 127, 128, 119, 128, 127, 34, 123, 125, 35, 125, 127, 32, 127, 123, 125, 123, 127, 11, 124, 129, 36, 129, 130, 35, 130, 124, 129, 124, 130, 0, 133, 131, 37, 131, 132, 36, 132, 133, 131, 133, 132, 6, 135, 128, 35, 128, 134, 37, 134, 135, 128, 135, 134, 36, 130, 132, 37, 132, 134, 35, 134, 130, 132, 130, 134, 0, 131, 44, 14, 44, 136, 37, 136, 131, 44, 131, 136, 1, 48, 137, 38, 137, 138, 14, 138, 48, 137, 48, 138, 6, 140, 135, 37, 135, 139, 38, 139, 140, 135, 140, 139, 14, 136, 138, 38, 138, 139, 37, 139, 136, 138, 136, 139, 6, 117, 140, 38, 140, 141, 33, 141, 117, 140, 117, 141, 1, 137, 83, 23, 83, 142, 38, 142, 137, 83, 137, 142, 10, 79, 121, 33, 121, 143, 23, 143, 79, 121, 79, 143, 38, 141, 142, 23, 142, 143, 33, 143, 141, 142, 141, 143, 9, 145, 55, 15, 55, 144, 39, 144, 145, 55, 145, 144, 0, 51, 133, 36, 133, 146, 15, 146, 51, 133, 51, 146, 11, 129, 147, 39, 147, 148, 36, 148, 129, 147, 129, 148, 15, 144, 146, 36, 146, 148, 39, 148, 144, 146, 144, 148, 9, 150, 145, 39, 145, 149, 41, 149, 150, 145, 150, 149, 11, 147, 151, 40, 151, 152, 39, 152, 147, 151, 147, 152, 2, 155, 153, 41, 153, 154, 40, 154, 155, 153, 155, 154, 39, 149, 152, 40, 152, 154, 41, 154, 149, 152, 149, 154, 9, 58, 150, 41, 150, 156, 17, 156, 58, 150, 58, 156, 2, 153, 100, 27, 100, 157, 41, 157, 153, 100, 153, 157, 5, 96, 62, 17, 62, 158, 27, 158, 96, 62, 96, 158, 41, 156, 157, 27, 157, 158, 17, 158, 156, 157, 156, 158, 7, 122, 107, 29, 107, 159, 34, 159, 122, 107, 122, 159, 2, 103, 155, 40, 155, 160, 29, 160, 103, 155, 103, 160, 11, 151, 126, 34, 126, 161, 40, 161, 151, 126, 151, 161, 29, 159, 160, 40, 160, 161, 34, 161, 159, 160, 159, 161 };
                }
                return m_sphereMesh;
            }
        }

        private Mesh m_coneMesh = null;
        private Mesh coneMesh
        {
            get
            {
                if (m_coneMesh == null)
                {
                    m_coneMesh = new Mesh();
                    m_coneMesh.vertices = new[] { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.000000f, -1.0f, 1.000000f), new Vector3(0.195090f, -1.0f, 0.980785f), new Vector3(0.382684f, -1.0f, 0.923880f), new Vector3(0.555570f, -1.0f, 0.831470f), new Vector3(0.707107f, -1.0f, 0.707107f), new Vector3(0.831470f, -1.0f, 0.555570f), new Vector3(0.923880f, -1.0f, 0.382683f), new Vector3(0.980785f, -1.0f, 0.195090f), new Vector3(1.000000f, -1.0f, 0.000000f), new Vector3(0.980785f, -1.0f, -0.195090f), new Vector3(0.923880f, -1.0f, -0.382684f), new Vector3(0.831470f, -1.0f, -0.555570f), new Vector3(0.707107f, -1.0f, -0.707107f), new Vector3(0.555570f, -1.0f, -0.831470f), new Vector3(0.382683f, -1.0f, -0.923880f), new Vector3(0.195090f, -1.0f, -0.980785f), new Vector3(0.000000f, -1.0f, -1.000000f), new Vector3(-0.195091f, -1.0f, -0.980785f), new Vector3(-0.382683f, -1.0f, -0.923880f), new Vector3(-0.555570f, -1.0f, -0.831470f), new Vector3(-0.707107f, -1.0f, -0.707107f), new Vector3(-0.831470f, -1.0f, -0.555570f), new Vector3(-0.923880f, -1.0f, -0.382683f), new Vector3(-0.980785f, -1.0f, -0.195090f), new Vector3(-1.000000f, -1.0f, 0.000000f), new Vector3(-0.980785f, -1.0f, 0.195090f), new Vector3(-0.923879f, -1.0f, 0.382684f), new Vector3(-0.831470f, -1.0f, 0.555570f), new Vector3(-0.707107f, -1.0f, 0.707107f), new Vector3(-0.555570f, -1.0f, 0.831470f), new Vector3(-0.382683f, -1.0f, 0.923880f), new Vector3(-0.195090f, -1.0f, 0.980785f) };
                    m_coneMesh.triangles = new[] { 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 7, 0, 7, 8, 0, 8, 9, 0, 9, 10, 0, 10, 11, 0, 11, 12, 0, 12, 13, 0, 13, 14, 0, 14, 15, 0, 15, 16, 0, 16, 17, 0, 17, 18, 0, 18, 19, 0, 19, 20, 0, 20, 21, 0, 21, 22, 0, 22, 23, 0, 23, 24, 0, 24, 25, 0, 25, 26, 0, 26, 27, 0, 27, 28, 0, 28, 29, 0, 29, 30, 0, 30, 31, 0, 31, 32, 0, 32, 33, 0, 33, 2, 1, 3, 2, 1, 4, 3, 1, 5, 4, 1, 6, 5, 1, 7, 6, 1, 8, 7, 1, 9, 8, 1, 10, 9, 1, 11, 10, 1, 12, 11, 1, 13, 12, 1, 14, 13, 1, 15, 14, 1, 16, 15, 1, 17, 16, 1, 18, 17, 1, 19, 18, 1, 20, 19, 1, 21, 20, 1, 22, 21, 1, 23, 22, 1, 24, 23, 1, 25, 24, 1, 26, 25, 1, 27, 26, 1, 28, 27, 1, 29, 28, 1, 30, 29, 1, 31, 30, 1, 32, 31, 1, 33, 32, 1, 2, 33 };
                }
                return m_coneMesh;
            }
        }

        internal void DrawLine(MerVector start, MerVector end, MerColor color, float thickness)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);
        }

        internal void DrawPolyline(float[] vertices, int numVertices, MerColor color, float thickness)
        {
            if (numVertices > 1)
            {
                Gizmos.color = color;

                Vector3 v0 = new Vector3(vertices[0], vertices[1], vertices[2]);
                for (int i = 0; i < numVertices - 3; i += 3) 
                {
                    Vector3 v1 = new Vector3(vertices[i + 3], vertices[i + 4], vertices[i + 5]);
                    Gizmos.DrawLine(v0, v1);
                    v0 = v1;
                }
            }
        }

        internal void DrawSphere(MerVector pos, float radius, MerColor color)
        {
            debugMaterial.SetColor("_Color", color);
            debugMaterial.SetPass(0);
            Graphics.DrawMeshNow(sphereMesh, Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(radius, radius, radius)));
        }

        internal void DrawCone(MerVector pos, float height, float radius, MerVector dir, MerColor color)
        {
            debugMaterial.SetColor("_Color", color);
            debugMaterial.SetPass(0);
            Graphics.DrawMeshNow(coneMesh, Matrix4x4.TRS(pos, Quaternion.FromToRotation(new Vector3(0,1,0), dir), new Vector3(radius, height, radius)));
        }

        internal void DrawMesh([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] float[] vertices, int numVertices,
                               [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] [In] uint[] triIndices, int numTriIndices,
                               MerColor color)
        {
            Mesh mesh = new Mesh();
            Vector3[] meshVertices = new Vector3[numVertices / 3];
            int[] meshTriangles = new int[numTriIndices];

            for (int i = 0, iEnd = numVertices / 3; i < iEnd; i++)
            {
                meshVertices[i].Set(vertices[3 * i], vertices[3 * i + 1], vertices[3 * i + 2]);
            }

            for (int i = 0; i < numTriIndices; i++)
            {
                meshTriangles[i] = (int)triIndices[i];
            }

            mesh.vertices = meshVertices;
            mesh.triangles = meshTriangles;

            if (numVertices > 0)
            {
                debugMaterial.SetColor("_Color", color);
                debugMaterial.SetPass(0);
                Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity);
            }
        }

        internal void DrawText(string text, MerVector pos, float offset, MerColor color)
        {
#if UNITY_EDITOR
            UnityEditor.SceneView view = UnityEditor.SceneView.currentDrawingSceneView;
            Camera camera = view ? view.camera : Camera.main;

            if (camera == null)
                return;

            Vector3 screenPos = camera.WorldToScreenPoint(pos);

            if (screenPos.y >= 0 && screenPos.y < Screen.height && screenPos.x >= 0 && screenPos.x < Screen.width && screenPos.z > 0)
            {
                UnityEditor.Handles.BeginGUI();

                var restoreColor = GUI.color;
                GUI.color = color;

                Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));

                Vector2 shiftedPos = new Vector2(screenPos.x , camera.pixelHeight - screenPos.y);
                Vector2 guiPos = UnityEditor.EditorGUIUtility.PixelsToPoints(shiftedPos);

                GUI.Label(new Rect(guiPos.x - size.x / 2, guiPos.y + offset, size.x, size.y), text);

                GUI.color = restoreColor;
                UnityEditor.Handles.EndGUI();
            }
#endif
        }

        internal void Draw2DText(string text, float x, float y, MerColor color)
        {
#if UNITY_EDITOR
            UnityEditor.SceneView view = UnityEditor.SceneView.currentDrawingSceneView;
            Camera camera = view ? view.camera : Camera.main;

            if (camera == null)
                return;

            UnityEditor.Handles.BeginGUI();

            var restoreColor = GUI.color;
            GUI.color = color;

            if (x < 0.0f) x = camera.pixelWidth + x;
            if (y < 0.0f) y = camera.pixelHeight + y;

            Vector2 guiPos = UnityEditor.EditorGUIUtility.PixelsToPoints(new Vector2(x, y));

            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(guiPos.x, guiPos.y, size.x, size.y), text);

            GUI.color = restoreColor;
            UnityEditor.Handles.EndGUI();
#endif
        }

        internal void GetViewInfo(out MerVector position, out MerVector forward, out MerVector up)
        {
#if UNITY_EDITOR
            if (UnityEditor.SceneView.lastActiveSceneView)
            {
                position = new MerVector(UnityEditor.SceneView.lastActiveSceneView.camera.transform.position);
                forward = new MerVector(UnityEditor.SceneView.lastActiveSceneView.camera.transform.forward);
                up = new MerVector(UnityEditor.SceneView.lastActiveSceneView.camera.transform.up);
            }
            else
#endif
            if (Camera.main)
            {
                position = new MerVector(Camera.main.transform.position);
                forward = new MerVector(Camera.main.transform.forward);
                up = new MerVector(Camera.main.transform.up);
            }
            else
            {
                position = Vector3.zero;
                forward = Vector3.forward;
                up = Vector3.up;
            }
        }
    }
}
