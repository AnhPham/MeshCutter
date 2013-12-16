using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// We find intersection of a line to edges of a polygon.
/// pA, pB: 2 vertices of a edge. with square (0,0), (0,1), (1,0), (1,1)
/// p: the cut point. (x,y) (0 -> 1)
/// </summary>
public class ResultPoint
{
	public Point2 p;
	public Point2 pA;
	public Point2 pB;
	
	public ResultPoint(Point2 _p, Point2 _pA, Point2 _pB)
	{
		p = _p;
		pA = _pA;
		pB = _pB;
	}
}

public class MathLogic
{	
	#region Public
	
	public static List<Polygon> MakeTwoPolygonsByCutting(Vector2 v1, Vector2 v2, Polygon p, ref List<ResultPoint> v)
	{
		Point2 p1 = new Point2(v1.x, v1.y);
		Point2 p2 = new Point2(v2.x, v2.y);
		v = IntersectionLineAndPolygon(p1, p2, p);
		List<Polygon> lp = new List<Polygon>();
		
		if (v.Count == 0) return lp;
		
		ConvertToNormal(ref p, ref v);
		
		Polygon poly1 = new Polygon();
		Polygon poly2 = new Polygon();
		
		lp.Add(poly1);
		lp.Add(poly2);
		
		int index = 0;
		while (index < p.vertices.Count)
		{
			Point2 curVertex = p.vertices[index];
			
			if (v[0].pB == curVertex) {
				poly1.vertices.Add(v[0].p);
				poly1.vertices.Add(v[1].p);
				
				index = p.vertices.IndexOf(v[1].pB);
				
				if (index == 0) break;
				else poly1.vertices.Add(v[1].pB);
			} else {
				poly1.vertices.Add(curVertex);
				
				if (IsSame(poly1.direction, 0f)) {
					poly1.direction = PointAndLine(v[0].p, v[1].p, curVertex);
					poly2.direction = - poly1.direction;
				}
			}
			
			index++;
		}
		
		index = 0;
		poly2.vertices.Add(v[0].p);
		index = p.vertices.IndexOf(v[0].pB);		
		while (index < p.vertices.Count)
		{
			Point2 curVertex = p.vertices[index];
			
			if (v[1].pA == curVertex) {
				poly2.vertices.Add(curVertex);
				poly2.vertices.Add(v[1].p);
				break;
			} else {
				poly2.vertices.Add(curVertex);
			}
			
			index++;
		}
		
		return lp;
	}
	
	public static List<ResultPoint> IntersectionLineAndPolygon(Point2 p1, Point2 p2, Polygon p)
	{
		float x = 0;
		float y = 0;
		int prev_cut = 0;
		int edge_count = 0;
		
		List<ResultPoint> v = new List<ResultPoint>();
		
		Equation e = AssignLine(p1.x, p1.y, p2.x, p2.y);
		
		for (edge_count = 0; edge_count < p.vertices.Count; edge_count++)
		{
			Point2 pA = p.vertices[edge_count];
			Point2 pB = (edge_count + 1) < p.vertices.Count ? p.vertices[edge_count + 1] : p.vertices[0];
			Equation line = AssignLine(pA.x, pA.y, pB.x, pB.y);
			
			if (IntersectionLines(e, line, ref x, ref y) && InRange(pA.x, pA.y, pB.x, pB.y, x, y))
			{
				ResultPoint newCutPoint = new ResultPoint(new Point2(x, y), pA, pB);
				
				if (v.Count != 0 && prev_cut == edge_count - 1) 
				{
					if (!IsSame(v[v.Count - 1], newCutPoint)) 
					{
						prev_cut = edge_count;
						v.Add(newCutPoint);
						continue;
					}
				} 
				else if (v.Count != 0 && v[0].pA == p.vertices[0] && edge_count == p.vertices.Count - 1)
				{
					if (!IsSame(v[0], newCutPoint))
					{
						prev_cut = edge_count;
						v.Add(newCutPoint);
						continue;
					}				
				}
				else 
				{
					prev_cut = edge_count;
					v.Add(newCutPoint);
				}
			}
		}
		
		if (v.Count < 2) v.Clear();
		return v;
	}
		
	public static float PointAndLine(Vector2 p0, Vector2 p1, Vector2 p)
	{
		float x0 = p0.x;
		float y0 = p0.y;
		
		float x1 = p1.x;
		float y1 = p1.y;
		
		float x = p.x;
		float y = p.y;
		
		return (y - y0) * (x1 - x0) - (x - x0) * ( y1 - y0);
	}
	#endregion
	
	class Equation 
	{ 
		public float a, b, c; 
	}
	
	static private Equation AssignLine(float xA, float yA, float xB, float yB)
	{
		Equation e = new Equation();
	    e.a = yA - yB;
	    e.b = xB - xA;
	    e.c = xB * yA - xA * yB;
	    return e;
	}
	
	static bool IntersectionLines(Equation e1, Equation e2, ref float x, ref float y)
	{
	    float d  = e1.a * e2.b - e2.a * e1.b;
	    float dx = e1.c * e2.b - e2.c * e1.b;
	    float dy = e1.a * e2.c - e2.a * e1.c;
	    if (d != 0)
	    {
	        x = dx / d;
	        y = dy / d;
			
	        return true;
	    }
	    return false;
	}
	
	static bool InRange(float xA, float yA, float xB, float yB, float x, float y)
	{
		return (InRange(xA, xB, x) && InRange(yA, yB, y));
	}
	
	static bool InRange(float lim1, float lim2, float v)
	{
		return (IsSame(lim1, v) || IsSame(lim2, v) || (lim1 <= v && v <= lim2) || (lim2 <= v && v <= lim1));
	}
	
	static bool IsSame(float x, float y)
	{
		return (Mathf.Abs(x - y) <= 0.0001f);
	}
	
	static bool IsSame(Point2 p1, Point2 p2)
	{
		return (IsSame(p1.x, p2.x) && IsSame(p1.y, p2.y));
	}
	
	static bool IsSame(ResultPoint p1, ResultPoint p2)
	{
		return (IsSame(p1.p.x, p2.p.x) && IsSame(p1.p.y, p2.p.y));
	}
	
	static void ConvertToNormal (ref Polygon p, ref List<ResultPoint> lr)
	{
		float left, right, top, bot, w, h;
		left = right = top = bot = 0;
		FindRectAroundPolygon(p, ref left, ref bot, ref right, ref top);
		w = right - left;
		h = top - bot;
		
		for (int i = 0; i < p.vertices.Count; i++) {
			Point2 v = p.vertices[i];
			ConvertAPointToNormal(ref v, left, bot, w, h);
		}
		
		for (int i = 0; i < lr.Count; i++) {
			Point2 v = lr[i].p;
			ConvertAPointToNormal(ref v, left, bot, w, h);
		}
	}
	
	static void ConvertAPointToNormal(ref Point2 v, float left, float bot, float w, float h)
	{
		v.x = (v.x - left) / w;
		v.y = (v.y - bot) / h;
	}
	
	static void FindRectAroundPolygon(Polygon p, ref float left, ref float bot, ref float right, ref float top)
	{
		left = FindLeft(p);
		bot = FindBot(p);
		right = FindRight(p);
		top = FindTop(p);
	}
	
	static float FindLeft(Polygon p)
	{
		float left = p.vertices[0].x;
		for (int i = 1; i < p.vertices.Count; i++) {
			if (p.vertices[i].x < left) left = p.vertices[i].x;
		}
		return left;
	}
	
	static float FindRight(Polygon p)
	{
		float right = p.vertices[0].x;
		for (int i = 1; i < p.vertices.Count; i++) {
			if (p.vertices[i].x > right) right = p.vertices[i].x;
		}
		return right;
	}
	
	static float FindBot(Polygon p)
	{
		float bot = p.vertices[0].y;
		for (int i = 1; i < p.vertices.Count; i++) {
			if (p.vertices[i].y < bot) bot = p.vertices[i].y;
		}
		return bot;
	}
	
	static float FindTop(Polygon p)
	{
		float top = p.vertices[0].y;
		for (int i = 1; i < p.vertices.Count; i++) {
			if (p.vertices[i].y > top) top = p.vertices[i].y;
		}
		return top;
	}
	
	static float PointAndLine(Point2 p0, Point2 p1, Point2 p)
	{
		float x0 = p0.x;
		float y0 = p0.y;
		
		float x1 = p1.x;
		float y1 = p1.y;
		
		float x = p.x;
		float y = p.y;
		
		return (y - y0) * (x1 - x0) - (x - x0) * ( y1 - y0);
	}
}
