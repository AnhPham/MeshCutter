using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoCutController : MonoBehaviour 
{
	[SerializeField]
	MeshCutter chara;

	Vector2 pressPos;
	Vector2 releasePos;

	int manualHeight;
	int manualWidth;

	void OnEnable()
	{
		chara.onNoCut += OnNoCut;
		chara.onCut += OnCut;
		chara.onMovePartsAfterCut += OnMovePartsAfterCut;
	}

	void OnDisable()
	{
		if (chara == null) return;

		chara.onNoCut -= OnNoCut;
		chara.onCut -= OnCut;
		chara.onMovePartsAfterCut -= OnMovePartsAfterCut;
	}

	void Update()
	{
		manualHeight = chara.manualHeight;
		manualWidth = chara.manualWidth;

		if (Input.GetMouseButtonDown (0)) 
		{
			pressPos = (Vector2)Input.mousePosition;
			ConvertToNGUIPos (ref pressPos);
		}

		if (Input.GetMouseButtonUp (0)) 
		{
			releasePos = (Vector2)Input.mousePosition;
			ConvertToNGUIPos (ref releasePos);

			Cut(pressPos, releasePos);
		}
	}

	void Cut(Vector2 from, Vector2 to)
	{
		chara.Cut (from, to);
	}

	void ConvertToNGUIPos(ref Vector2 v)
	{
		float rate = (float)manualHeight / Screen.height;

		v.x *= rate;
		v.y *= rate;
	}

	void OnNoCut()
	{
		Debug.Log ("Miss");
	}

	void OnCut(List<Vector2> cutPoints)
	{
		string s = "LeftBottom=(0,0). RightTop=({0},{1}). ";
		s = string.Format (s, manualWidth, manualHeight);

		Debug.Log (s + "p1: " + cutPoints [0] + ", " + "p2: " + cutPoints [1]);
	}

	void OnMovePartsAfterCut(GameObject part, Vector3 direction)
	{
		part.transform.localPosition += direction * 20f ;
	}
}
