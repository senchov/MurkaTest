using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CompareGeometry : MonoBehaviour
{

	#region Fields
	[SerializeField]
	private DrawLine
		_drawGeometry;

	[SerializeField]
	private GeometryBoundary
		_geometryBoundary;

	[SerializeField]
	private GameController
		_gameCntrl;

	[SerializeField]
	[Range(0.1f,20.0f)]
	private float
		_angleOffset;

	private Geometry _taskGeometry, _playerGeometry;
	#endregion

	#region Events
	public event Action Corectly;

	private void CorectlyHandler ()
	{
		if (Corectly != null)
			Corectly ();
	}

	public event Action Incorectly;

	private void IncorectlyHandler ()
	{
		if (Incorectly != null)
			Incorectly ();
	}
	#endregion

	private void Awake ()
	{
		#region NullCheck
		if (!_drawGeometry) {
			Debug.Log ("DrawLine is null");
			return;
		}

		if (!_geometryBoundary) {
			Debug.Log ("GeometryBoundary is null");
			return;
		}

		if (!_gameCntrl) {
			Debug.Log ("GameController is null");
			return;
		}
		#endregion

		if (!_geometryBoundary.IsAddGeometry)
			_drawGeometry.FinishDrawing += HandleFinishDrawing;

		_geometryBoundary.LevelUp += HandleLevelUp;
	}

	private void HandleLevelUp ()
	{
		print ("comp");
		_taskGeometry = CreateGeometry (_geometryBoundary.TaskGeometry);
	}


	private void Start ()
	{
		if (!_geometryBoundary.IsAddGeometry)
			_taskGeometry = CreateGeometry (_geometryBoundary.TaskGeometry);

	}

	private void HandleFinishDrawing ()
	{
		_playerGeometry = CreateGeometry (_drawGeometry.Points);

		print (Compare ());


		if (Compare ())
			CorectlyHandler ();
		else 
			Incorectly ();
	}


	private bool Compare ()
	{
		print (_taskGeometry.verticesAmount + " v " + _playerGeometry.verticesAmount);
		if (_taskGeometry.verticesAmount != _playerGeometry.verticesAmount)
			return false;

		print (_taskGeometry.verticesBL + "bl" + _playerGeometry.verticesBL);
		print (_taskGeometry.verticesTL + "tl" + _playerGeometry.verticesTL);
		print (_taskGeometry.verticesTR + "tr" + _playerGeometry.verticesTR);
		print (_taskGeometry.verticesBR + "br" + _playerGeometry.verticesBR);

		if (_taskGeometry.verticesBL != _playerGeometry.verticesBL ||
			_taskGeometry.verticesTL != _playerGeometry.verticesTL ||
			_taskGeometry.verticesTR != _playerGeometry.verticesTR ||
			_taskGeometry.verticesBR != _playerGeometry.verticesBR
		    ) {
			return false;
		}


		if (!CompareAnglesArr (_taskGeometry.angleArr, _playerGeometry.angleArr, _angleOffset)) {
			return false;
		}

		return true;

	}

	private List<Vector2> PrepareGeometry (List<Vector2> list)
	{
		List <Vector2> tempList = list;
		if (list.Count > 3) {
			tempList = Triangulation.GetResult (list, true);
		}

		if (list.Count > 3 && tempList.Count == 0) {
			tempList = Triangulation.GetResult (list, false);
		}
		return tempList;
	}

	private Vector2 FindGeometryCentr (List<Vector2> list)
	{
		if (list.Count <= 1)
			return list [0];

		if (list.Count == 2) {
			Vector2 temp = new Vector2 ();
			temp.x = (list [0].x + list [1].x) * 0.5f;
			temp.y = (list [0].y + list [1].y) * 0.5f;
			return temp;
		}

		if (list.Count == 3) {
			return TriangleCentr (list [0], list [1], list [2]);
		}

		List<Vector2> tempList = new List<Vector2> ();
		for (int i = 0; i < list.Count-2; i=i+3) {
			Vector2 temp = TriangleCentr (list [i], list [i + 1], list [i + 2]);
			tempList.Add (temp);
		}

		return FindGeometryCentr (tempList);
	}

	private bool CompareAnglesArr (float[] firstArr, float[] secondArr, float offset)
	{
		for (int i = 0; i < firstArr.Length; i++) {
			if (!CompareAngleWithArray (firstArr [i], secondArr, offset))
				return false;
		}

		return true;
	}

	private bool CompareAngleWithArray (float angle, float[] arr, float offset)
	{
		for (int i = 0; i < arr.Length; i++) {
			if (angle > arr [i] - offset && angle < arr [i] + offset)
				return true;
		}

		return false;
	}

	private Geometry CreateGeometry (List<Vector2> list)
	{
		List <Vector2> tempList = new List<Vector2> ();
		for (int i = 0; i < list.Count; i++) {
			Vector2 point = new Vector2 ();
			point.x = list [i].x;
			point.y = list [i].y;
			tempList.Add (point);
		}

		Vector2 centr = FindGeometryCentr (PrepareGeometry (tempList));

		/*GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		obj.transform.position = new Vector3 (centr.x, centr.y, 0);
		obj.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);*/

		int blVerticies = 0, tlVerticies = 0, trVerticies = 0, brVerticies = 0;
		float[] arr = new float[list.Count];

		for (int i = 0; i < list.Count; i++) {
			if (list [i].x < centr.x && list [i].y <= centr.y)
				blVerticies++;

			if (list [i].x < centr.x && list [i].y > centr.y)
				tlVerticies++;

			if (list [i].x >= centr.x && list [i].y > centr.y)
				trVerticies++;

			if (list [i].x >= centr.x && list [i].y <= centr.y)
				brVerticies++;

			int fromInt = i - 1;
			int toInt = i + 1;

			if (i == 0)
				fromInt = arr.Length - 1;

			if (i == list.Count - 1) {
				toInt = 0;
			}

			Vector2 from = list [fromInt] - list [i];
			Vector2 to = list [toInt] - list [i];
			arr [i] = Vector2.Angle (from, to);

		}

		return new Geometry (list.Count, blVerticies, tlVerticies, trVerticies, brVerticies, arr);

	}


	private Vector2 TriangleCentr (Vector2 vertex0, Vector2 vertex1, Vector2 vertex2)
	{
		Vector2 temp = new Vector2 ();
		temp.x = (vertex0.x + vertex1.x + vertex2.x) / 3;
		temp.y = (vertex0.y + vertex1.y + vertex2.y) / 3;
		return temp;
	}

	private void OnDestroy ()
	{
		if (!_drawGeometry)
			_drawGeometry.FinishDrawing -= HandleFinishDrawing;

		if (_gameCntrl)
			_gameCntrl.LevelUp -= HandleLevelUp;
	}

}

public class Geometry
{
	public int verticesAmount;
	public int verticesBL;
	public int verticesTL;
	public int verticesTR;
	public int verticesBR;
	public float[] angleArr;



	public Geometry (int verticesAmount, int verticesBL, int verticesTL, int verticesTR, int verticesBR, float[] angleArr)
	{
		this.verticesAmount = verticesAmount;
		this.verticesBL = verticesBL;
		this.verticesTL = verticesTL;
		this.verticesTR = verticesTR;
		this.verticesBR = verticesBR;
		this.angleArr = angleArr;
	}
}
