using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CreateFloor : MonoBehaviour
{
    public Mesh mesh, mesh2;
    public int[] triangles;
    public Vector3[] vertices;
    public GameObject sphere;
    public Transform centee;
    private float x = 0, y = 0, z = 0;
    public Material mat, transparentMat;
    public float height = 8f;
    List<Vector3> roomPoints;
    private void Initialise()
    {
        var count = GetComponent<DrawOutline>().roomPoints.Count;
        mesh = new Mesh();
        mesh2 = new Mesh();
        vertices = new Vector3[count + 1];
        vertices[0] = Vector3.zero;
        //uv = new Vector2[0];
        triangles = new int[count * 3];
    }

    public void createFloor()
    {
        roomPoints = GetComponent<DrawOutline>().roomPoints;
        feedValueOnClick(roomPoints.ToArray());
    }
    
    public void makeWalls()
    {
        GameObject.Find("floorOutline").SetActive(false);

        List<Vector3> allRoomPoints = new List<Vector3>();
        for (int i = 0; i < roomPoints.Count; i++)
        {
            if (i == roomPoints.Count - 1)
            {
                allRoomPoints.Add(roomPoints[i]);
                allRoomPoints.Add(roomPoints[0]);
                allRoomPoints.Add(new Vector3(roomPoints[i].x, height, roomPoints[i].z));
                allRoomPoints.Add(new Vector3(roomPoints[0].x, height, roomPoints[0].z));
            }
            else
            {
                allRoomPoints.Add(roomPoints[i]);
                allRoomPoints.Add(roomPoints[i + 1]);
                allRoomPoints.Add(new Vector3(roomPoints[i].x, height, roomPoints[i].z));
                allRoomPoints.Add(new Vector3(roomPoints[i + 1].x, height, roomPoints[i + 1].z));
            }
        }
        var list = new List<List<Vector3>>();

        for (int i = 0; i < allRoomPoints.Count; i += 4)
        {
            list.Add(allRoomPoints.GetRange(i, Mathf.Min(4, allRoomPoints.Count - i)));
        }
        for (int i = 0; i < list.Count; i++)
        {
            generateMesh(list[i].ToArray(), i);
        }
    }


    Vector3 CalcBounds(Vector3[] points)
    {
        float b = 0.01f, a1 = 0f, c1 = 0f, a2 = 100f, c2 = 100f;
        foreach(Vector3 point in points)
        {
            //largest
            if (point.x > a1) a1 = point.x;
            if (point.z > c1) c1 = point.z;
            //smallest
            if (point.x < a2) a2 = point.x;
            if (point.x < c2) c2 = point.z;
        }
        return new Vector3(a1-a2, b, c1-c2);
    }

    //helper functions

    public void feedValueOnClick(Vector3[] points)
    {
        Initialise();

        //mesh.vertices = points;
        GameObject floor = new GameObject("floor", typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider));
        floor.GetComponent<MeshFilter>().mesh = mesh;
        var toprint = floor.GetComponent<BoxCollider>().size = CalcBounds(points);
        print(toprint);
        GetComponent<LevelGenerator>().width = ((int)toprint.x);
        GetComponent<LevelGenerator>().height = ((int)toprint.z);
        floor.tag = "Road";
        floor.layer = 9;
        //GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.NavigationStatic);

        var center = FindCenter(points);
        var cent = Instantiate(sphere, center, Quaternion.identity);
        centee = cent.transform;
        cent.name = "center";

        foreach (Vector3 point in points)
        {
            print(point);
            x += point.x;
            y += point.y;
            z += point.z;
        }
        print(x + " , " + y + " , " + z);

        for (int i = 0; i < points.Length; i++)
        {
            if (y == 0)
            {
                points[i].y = 0.0f;
            }
            else if (x == 0)
            {
                points[i].x = 0.0f;
            }
            else if (z == 0)
            {
                points[i].z = 0.0f;
            }
            vertices[i + 1] = points[i] - center;
        }
        //Quaternion rotation = Quaternion.Euler(0f, 0f, 180f);
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    vertices[i] = rotation * (vertices[i] - center) + center;
        //}
        mesh.vertices = vertices;
        mesh2.vertices = vertices;
        for (int i = 0; i < points.Length - 1; i++)
        {
            triangles[i * 3] = i + 2;
            triangles[i * 3 + 1] = 0;
            triangles[i * 3 + 2] = i + 1;
        }

        triangles[(points.Length - 1) * 3] = 1;
        triangles[(points.Length - 1) * 3 + 1] = 0;
        triangles[(points.Length - 1) * 3 + 2] = points.Length;

        Vector3[] normals = new Vector3[mesh.normals.Length];
        Vector3[] normals2 = new Vector3[mesh.normals.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = Vector3.up;
            normals2[i] = Vector3.down;
        }

        mesh.triangles = triangles;
        mesh2.triangles = triangles;
        Bounds bounds = mesh.bounds;
        //mesh2.uv = mesh.uv = UVbuilder(vertices, bounds);
        mesh2.uv = mesh.uv = BuildUVs(vertices);
        //ceiling = Instantiate(floor);
        //ceiling.name = "ceiling";

        mesh.normals = normals;
        //mesh2.normals = normals2;
        //mesh.triangles = mesh.triangles.Reverse().ToArray();
        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        //ceiling.GetComponent<MeshFilter>().mesh = mesh2;
        //var normals = mesh.normals;
        //for (int i = 0; i < normals.Length; i++)
        //    normals[i] = rotation * normals[i];
        //mesh.normals = normals;

        floor.GetComponent<MeshRenderer>().material = mat;
        //ceiling.GetComponent<MeshRenderer>().material = mat2;
        //var celing = Instantiate(floor, ceiling.transform);

        floor.transform.position = center;
        //ceiling.transform.position = center;
    }
    //copied from https://answers.unity.com/questions/546473/create-a-plane-from-points.html
    Vector3 FindCenter(Vector3[] poly)
    {
        Vector3 center = Vector3.zero;
        foreach (Vector3 v3 in poly)
        {
            center += v3;
        }
        return center / poly.Length;
    }

    Vector2[] BuildUVs(Vector3[] vertices)
    {

        float xMin = Mathf.Infinity;
        float zMin = Mathf.Infinity;
        float xMax = -Mathf.Infinity;
        float zMax = -Mathf.Infinity;

        foreach (Vector3 v3 in vertices)
        {
            if (v3.x < xMin)
                xMin = v3.x;
            if (v3.z < zMin)
                zMin = v3.z;
            if (v3.x > xMax)
                xMax = v3.x;
            if (v3.z > zMax)
                zMax = v3.z;
        }

        float xRange = xMax - xMin;
        float zRange = zMax - zMin;

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i].x = (vertices[i].x - xMin) / xRange;
            uvs[i].y = (vertices[i].z - zMin) / zRange;

        }
        return uvs;
    }

    void generateMesh(Vector3[] points, int i)
    {
        var plane = new GameObject((i + 1).ToString() + "th wall", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        Mesh mesh = new Mesh();
        plane.GetComponent<MeshFilter>().mesh = mesh;

        int[] Triangles = new int[6] { 0, 3, 1, 0, 2, 3 };

        Vector3[] normals = new Vector3[4]
        {
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward
        };
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.vertices = points;
        mesh.triangles = Triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        //mesh.triangles = mesh.triangles.Reverse().ToArray();
        //mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        plane.GetComponent<MeshRenderer>().material = transparentMat;
    }
}
