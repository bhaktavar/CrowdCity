using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOutline : MonoBehaviour
{
    private List<GameObject> lineList;
    private LineRenderer lineRend;
    public List<Vector3> roomPoints;
    public GameObject parent;
    public Color color;
    public Material dottedLineMaterial;
    public Material lineMat;
    public GameObject pointer;
    public GameObject sphere;

    private void Start()
    {
        roomPoints = new List<Vector3>();

        lineRend = new LineRenderer();
        lineList = new List<GameObject>();
        parent = new GameObject("floorOutline");

    }

    public void clicker()
    {
        //var x = pointer.transform.position;
        //temp = roomPoints.IndexOf(x);
        //if (temp == -1)
        //{
        //    roomPoints.Add(x);
        //    temp = roomPoints.IndexOf(x);
        //}
        roomPoints.Add(pointer.transform.position);
        var temp = roomPoints.Count - 1;
        Instantiate(sphere, pointer.transform.position, pointer.transform.rotation, parent.transform);
        if (roomPoints.Count >= 2)
        {
            Vector3 firstPoint = roomPoints[temp - 1];
            Vector3 lastPoint = roomPoints[temp];
            //centerPoints.Add((firstPoint + lastPoint) / 2);
            LineDrawer(firstPoint, lastPoint, temp);
        }
    }

    void LineDrawer(Vector3 a, Vector3 b, int count)
    {
        //draw the line
        GameObject line = new GameObject();
        line.transform.parent = parent.transform;
        line.name = count.ToString() + "th line";
        //print(line.name);
        lineRend = line.AddComponent<LineRenderer>();
        //lineRend.material = lineMaterial;
        //lineRend.textureMode = LineTextureMode.Tile;
        lineRend.material = dottedLineMaterial;
        lineRend.SetColors(color, color);
        lineRend.SetWidth(0.1f, 0.1f);
        lineRend.positionCount = 2;
        lineRend.SetPosition(0, a);
        lineRend.SetPosition(1, b);
        lineList.Add(line);
    }

    public void makeRoom()
    {
        if (roomPoints.Count < 3)
        {
            print("not enough points!!");
            return;
        }
        Vector3 firstPoint = roomPoints[0];
        Vector3 lastPoint = roomPoints[roomPoints.Count - 1];
        LineDrawer(firstPoint, lastPoint, 100);
        //centerPoints.Add((firstPoint + lastPoint) / 2);
        //make the lines solid
        foreach (GameObject lin in lineList)
        {
            lin.GetComponent<LineRenderer>().material = lineMat;
        }
        
    }
}
