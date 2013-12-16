using UnityEngine;
using System.Collections.Generic;

public class Polygon
{
	public List<Point2> vertices;
	public float direction;
	
	public Polygon()
	{
		vertices = new List<Point2>();
	}
	
	public Vector2[] ToVector2Array()
	{
		Vector2[] array = new Vector2[vertices.Count];
		for (int i = 0; i < vertices.Count; i++) {
			array[i] = new Vector2(vertices[i].x, vertices[i].y);
		}
		return array;
	}
	
	public void Print()
	{
		foreach (Point2 v in vertices)
		{
			Debug.Log(v.x + " , " + v.y);
		}
	}
}
