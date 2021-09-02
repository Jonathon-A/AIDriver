using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples
{
    public class RoadMeshCreator : PathSceneTool
    {

        [Header("Road settings")]
        public float roadWidth = .4f;
        [Range(0, .5f)]
        public float thickness = .15f;
        public bool flattenSurface;

        [Header("Material settings")]
        public Material roadMaterial;
        public Material undersideMaterial;
        public float textureTiling = 1;

        [SerializeField, HideInInspector]
        GameObject meshHolder;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        Mesh mesh;

        [Header("Prefab settings")]
        public bool IncludePrefabs;
        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                foreach (GameObject Waypoint in GameObject.FindGameObjectsWithTag("Waypoint"))
                {
                    DestroyImmediate(Waypoint);
                }
                foreach (GameObject Barrier in GameObject.FindGameObjectsWithTag("Barrier"))
                {
                    DestroyImmediate(Barrier);
                }

                AssignMeshComponents();
                AssignMaterials();
                CreateRoadMesh();
            }
        }

        public GameObject Barrier;
        public MeshCollider ThisMeshCollider;
        public Transform BarrierParent;

        public GameObject Waypoint;
        public int ApproxWaypointNumber;
        public Transform WaypointParent;

        void CreateRoadMesh()
        {
            Vector3[] verts = new Vector3[path.NumPoints * 8];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] roadTriangles = new int[numTris * 3];
            int[] underRoadTriangles = new int[numTris * 3];
            int[] sideOfRoadTriangles = new int[numTris * 2 * 3];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 8  9
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
            int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

            for (int i = 0; i < path.NumPoints; i++)
            {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
                Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);

                if (IncludePrefabs)
                {


                    int NewIndex = i + 1;
                    if (NewIndex >= path.NumPoints)
                    {
                        NewIndex = 0;
                    }

                    Vector3 NewlocalUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(NewIndex), path.GetNormal(NewIndex)) : path.up;
                    Vector3 NewlocalRight = (usePathNormals) ? path.GetNormal(NewIndex) : Vector3.Cross(NewlocalUp, path.GetTangent(NewIndex));

                    // Find position to left and right of current path vertex
                    Vector3 NewvertSideA = path.GetPoint(NewIndex) - NewlocalRight * Mathf.Abs(roadWidth);
                    Vector3 NewvertSideB = path.GetPoint(NewIndex) + NewlocalRight * Mathf.Abs(roadWidth);

                    GameObject NewBarrier = Instantiate(Barrier, vertSideA, new Quaternion(0, 0, 0, 0));

                    Vector3 direction = NewvertSideA - NewBarrier.transform.position;
                    if (direction.magnitude != 0)
                    {
                        NewBarrier.transform.rotation = Quaternion.LookRotation(direction);
                        NewBarrier.transform.position += direction / 2;
                        NewBarrier.transform.localScale = new Vector3(Barrier.transform.localScale.x, Barrier.transform.localScale.y, direction.magnitude);
                        NewBarrier.transform.position += -NewBarrier.transform.right * NewBarrier.transform.lossyScale.x / 2;
                        NewBarrier.transform.parent = BarrierParent;
                    }
                    else { DestroyImmediate(NewBarrier); }


                    NewBarrier = Instantiate(Barrier, vertSideB, new Quaternion(0, 0, 0, 0));

                    direction = NewvertSideB - NewBarrier.transform.position;
                    if (direction.magnitude != 0)
                    {


                        NewBarrier.transform.rotation = Quaternion.LookRotation(direction);
                        NewBarrier.transform.position += direction / 2;
                        NewBarrier.transform.localScale = new Vector3(Barrier.transform.localScale.x, Barrier.transform.localScale.y, direction.magnitude);
                        NewBarrier.transform.position += NewBarrier.transform.right * NewBarrier.transform.lossyScale.x / 2;
                        NewBarrier.transform.parent = BarrierParent;
                    }
                    else { DestroyImmediate(NewBarrier); }
                }
                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;
                // Add bottom of road vertices
                verts[vertIndex + 2] = vertSideA - localUp * thickness;
                verts[vertIndex + 3] = vertSideB - localUp * thickness;

                // Duplicate vertices to get flat shading for sides of road
                verts[vertIndex + 4] = verts[vertIndex + 0];
                verts[vertIndex + 5] = verts[vertIndex + 1];
                verts[vertIndex + 6] = verts[vertIndex + 2];
                verts[vertIndex + 7] = verts[vertIndex + 3];

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, path.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, path.times[i]);

                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;
                // Bottom of road normals
                normals[vertIndex + 2] = -localUp;
                normals[vertIndex + 3] = -localUp;
                // Sides of road normals
                normals[vertIndex + 4] = -localRight;
                normals[vertIndex + 5] = localRight;
                normals[vertIndex + 6] = -localRight;
                normals[vertIndex + 7] = localRight;

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                        // reverse triangle map for under road so that triangles wind the other way and are visible from underneath
                        underRoadTriangles[triIndex + j] = (vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2) % verts.Length;
                    }
                    for (int j = 0; j < sidesTriangleMap.Length; j++)
                    {
                        sideOfRoadTriangles[triIndex * 2 + j] = (vertIndex + sidesTriangleMap[j]) % verts.Length;
                    }

                }

                vertIndex += 8;
                triIndex += 6;
            }

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = 3;
            mesh.SetTriangles(roadTriangles, 0);
            mesh.SetTriangles(underRoadTriangles, 1);
            mesh.SetTriangles(sideOfRoadTriangles, 2);
            mesh.RecalculateBounds();

            if (ApproxWaypointNumber > 0 && IncludePrefabs)
            {
                int num = 0;

                int WaypointStep = path.NumPoints / ApproxWaypointNumber;
                // print(path.NumPoints);
                // print(WaypointStep);
                for (int i = WaypointStep; i < path.NumPoints; i += WaypointStep)
                {
                    Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                    Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                    Vector3 vertSide = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);

                    GameObject NewWaypoint = Instantiate(Waypoint, path.GetPoint(i), new Quaternion(0, 0, 0, 0));
                    Vector3 direction = vertSide - NewWaypoint.transform.position;
                    NewWaypoint.transform.rotation = Quaternion.LookRotation(direction);
                    NewWaypoint.transform.localScale = new Vector3(Waypoint.transform.localScale.x, Waypoint.transform.localScale.y, direction.magnitude * 2);

                    NewWaypoint.transform.parent = WaypointParent;

                    num++;
                    NewWaypoint.name = "Waypoint: " + num;


                }

            }
            ThisMeshCollider.sharedMesh = null;
            ThisMeshCollider.sharedMesh = mesh;
        }

        List<GameObject> TempWaypoints = new List<GameObject>();
        List<GameObject> TempBarriers = new List<GameObject>();



        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents()
        {

            if (meshHolder == null)
            {
                meshHolder = new GameObject("Road Mesh Holder");
            }

            meshHolder.transform.rotation = Quaternion.identity;
            meshHolder.transform.position = Vector3.zero;
            meshHolder.transform.localScale = Vector3.one;

            // Ensure mesh renderer and filter components are assigned
            if (!meshHolder.gameObject.GetComponent<MeshFilter>())
            {
                meshHolder.gameObject.AddComponent<MeshFilter>();
            }
            if (!meshHolder.GetComponent<MeshRenderer>())
            {
                meshHolder.gameObject.AddComponent<MeshRenderer>();
            }

            meshRenderer = meshHolder.GetComponent<MeshRenderer>();
            meshFilter = meshHolder.GetComponent<MeshFilter>();
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            meshFilter.sharedMesh = mesh;
        }

        void AssignMaterials()
        {
            if (roadMaterial != null && undersideMaterial != null)
            {
                meshRenderer.sharedMaterials = new Material[] { roadMaterial, undersideMaterial, undersideMaterial };
                meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3(1, textureTiling);
            }
        }

    }
}