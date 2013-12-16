using UnityEngine;
using System.Collections.Generic;

public class Rectangle : Polygon
{
	public Vector2 center;
	public float width;
	public float height;

	public Rectangle(Vector2 c, float w, float h)
	{
		center = c;
		width = w;
		height = h;

		vertices = new List<Point2>();
		
		float left = c.x - w/2;
		float right= c.x + w/2;
		float top = c.y + h/2;
		float bot = c.y - h/2;
				
		vertices.Add(new Point2(left, bot));
		vertices.Add(new Point2(left, top));
		vertices.Add(new Point2(right, top));
		vertices.Add(new Point2(right, bot));
	}
}
