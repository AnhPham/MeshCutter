using UnityEngine;
using System.Collections;

public class Point2 {

	public float x;
	public float y;
	
	public Point2(float _x, float _y)
	{
		x = _x;
		y = _y;
	}
	
	public void Print()
	{
		Debug.Log(x + " , " + y);
	}
}
