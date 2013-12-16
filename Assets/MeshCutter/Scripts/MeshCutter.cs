using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCutter : MonoBehaviour 
{
	public delegate void onNoCutDelegate();
	public delegate void onCutDelegate(List<Vector2> cutPoints);
	public delegate void onMovePartsAfterCutDelegate(GameObject part, Vector3 direction);

	public onNoCutDelegate onNoCut;
	public onCutDelegate onCut;
	public onMovePartsAfterCutDelegate onMovePartsAfterCut;
	
	public int manualHeight { get; protected set; }
	public int manualWidth { get; protected set; }
	
	GameObject[] parts;

	List<ResultPoint> lr;
	List<Polygon> lp;
	
	Rectangle rect;

	void Start()
	{
		MeshRenderer ren = GetComponentInChildren<MeshRenderer> ();
		if (ren == null) 
		{
			Debug.LogWarning ("Not found MeshRenderer");
			return;
		}

		GetManualHeight ();
		GetManualWidth ();

		GameObject main = ren.gameObject;
		AddMeshFilter (main);

		parts = new GameObject[2];
		parts [0] = main;
		CopyPart (main);

		Vector2 center = new Vector2 (transform.localPosition.x + manualWidth / 2, transform.localPosition.y + manualHeight / 2);
		rect = new Rectangle (center, parts [0].transform.localScale.x, parts [0].transform.localScale.y);
	}

	void GetManualHeight()
	{
		Transform p = transform.parent;

		while (p != null) 
		{
			Component c1 = p.GetComponent ("UIRoot");
			if (c1 != null) 
			{
				manualHeight = (int)GetPropValue (c1, "manualHeight");
				return;
			}

			Component c2 = p.GetComponent ("DemoUIRoot");
			if (c2 != null) 
			{
				manualHeight = (int)GetPropValue (c2, "manualHeight");
				return;
			}

			p = p.parent;
		}
	}

	void GetManualWidth()
	{
		manualWidth = Mathf.RoundToInt((float)Screen.width / Screen.height * manualHeight);
	}

	public object GetPropValue(object src, string propName)
	{
		return src.GetType ().GetField (propName).GetValue (src);
	}

	void CopyPart(GameObject main)
	{
		parts [1] = Instantiate (main) as GameObject;
		parts [1].name = main.name;
		parts [1].transform.parent = main.transform.parent;
		parts [1].transform.localScale = main.transform.localScale;
		parts [1].SetActive (false);
	}
	
	public void Cut(Vector2 p1, Vector2 p2)
	{
		if (parts == null) return;

		Vector2 center = new Vector2 (transform.localPosition.x + manualWidth / 2, transform.localPosition.y + manualHeight / 2);
		rect = new Rectangle (center, parts [0].transform.localScale.x, parts [0].transform.localScale.y);

		lp = MathLogic.MakeTwoPolygonsByCutting(p1, p2, rect, ref lr);

		if (lp.Count == 0) 
		{
			if (onNoCut != null) onNoCut ();
			return;
		}

		if (onCut != null) 
		{
			List<Vector2> cutPoints = new List<Vector2> ();
			foreach (ResultPoint r in lr) 
			{
				int x = Mathf.RoundToInt(rect.center.x + (r.p.x-0.5f) * rect.width);
				int y = Mathf.RoundToInt(rect.center.y + (r.p.y-0.5f) * rect.height);

				cutPoints.Add (new Vector2 (x, y));
			}
			onCut (cutPoints);
		}

		parts [0].transform.localPosition = Vector3.zero;
		parts [1].transform.localPosition = Vector3.zero;
		parts [1].SetActive (true);

		AddMeshFilter(parts[0], 0);
		AddMeshFilter(parts[1], 1);

		Vector3 v = new Vector3(lr[1].p.x - lr[0].p.x, lr[1].p.y - lr[0].p.y, 0);
		TranslatePart(parts[0], v, (lp[0].direction > 0));
		TranslatePart(parts[1], v, (lp[1].direction > 0));
	}

	
	void TranslatePart(GameObject part, Vector3 vectorCut, bool isBack)
	{
		Vector3 z = (isBack == true) ? Vector3.back : Vector3.forward;
		Vector3 v = Vector3.Cross(vectorCut, z);
		v.Normalize();

		if (onMovePartsAfterCut != null) 
		{
			onMovePartsAfterCut (part, v);
		}
	}

	void AddMeshFilter(GameObject go, int index = -1)
	{		
		go.transform.localScale = new Vector3 (go.renderer.material.mainTexture.width, go.renderer.material.mainTexture.height, 1);

		MeshFilter mf = go.GetComponent<MeshFilter> ();
		if (mf == null) go.AddComponent<MeshFilter> ();

		Mesh mesh = go.GetComponent<MeshFilter>().mesh;
		mesh.Clear();

		Vector2[] vertices2D;
		if (index >= 0) 
		{
			vertices2D = lp[index].ToVector2Array();
		} 
		else 
		{
			vertices2D = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)};
		}

		Vector3[] vertices = new Vector3[vertices2D.Length];
		for (int i = 0; i < vertices2D.Length; i++) 
		{
			vertices [i] = (Vector3) (vertices2D [i] - Vector2.one * 0.5f);
		}

		Triangulator tr = new Triangulator(vertices2D);
		int[] indices = tr.Triangulate();

		mesh.vertices = vertices;
		mesh.uv = vertices2D;
		mesh.triangles = indices;
	}
}