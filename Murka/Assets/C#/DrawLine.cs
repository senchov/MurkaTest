using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent (typeof(LineRenderer))]
public class DrawLine : MonoBehaviour
{
	[SerializeField]
	private Camera
		_myCamera;

	[SerializeField]
	private GameObject
		_trail;

	[SerializeField]
	private float
		_offSet = 0.1f, _angleOffset;

	[SerializeField]
	private Grid
		_grid;

	[SerializeField]
	private GeometryBoundary
		_geometryBoundary;

	[SerializeField]
	private GameController
		_gameCntrl;

	private LineRenderer _line;
	private int _counter;
	private List <Vector2> _points, _checkPoints, _anotherList;
	private Vector3 _startPoint, _nextPoint, _direction, _currentDir;
	private bool _isAlongX = true;

	public event Action FinishDrawing;

	private void FinishDrawingHandler ()
	{
		if (FinishDrawing != null)
			FinishDrawing ();
	}

	public List<Vector2> Points {
		get { return _points;}
	}

	private void Awake ()
	{
		#region NullCheck
		if (!_myCamera) {
			Debug.Log ("Camera is null");
			return;
		}

		if (!_trail) {
			Debug.Log ("Trail is null");
			return;
		}

		if (!_grid) {
			Debug.Log ("Grid is null");
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

		_line = GetComponent <LineRenderer> ();
		_points = new List<Vector2> ();
		_checkPoints = new List<Vector2> ();
		_anotherList = new List<Vector2> ();

		_trail.SetActive (false);

		_gameCntrl.StartGame += HandleStartGame;
	}



	private void Update ()
	{
		Vector3 mousePos = _myCamera.ScreenToWorldPoint (Input.mousePosition);

		if (Input.GetMouseButtonDown (0)) {
			if (!_geometryBoundary.IsAddGeometry)
				_points.Clear ();

			_startPoint = mousePos;
			_nextPoint = mousePos;

			if (_grid.InGridBorders (mousePos))
				_points.Add (GridOperation (_startPoint));
		}

		if (Input.GetMouseButton (0)) {
		
			float temp = Vector3.Distance (_nextPoint, mousePos);

			if (temp <= _offSet || !_grid.InGridBorders (mousePos))
				return;

			_nextPoint = mousePos;
			Vector2 point = GridOperation (mousePos);
		
			float t = Vector2.Distance (mousePos, point); 

			if (t < _grid.CellWidth * _offSet && !_points.Contains (point)) {
				_points.Add (point);
				_trail.transform.position = new Vector3 (point.x, point.y, 0);
			}

		}


		if (Input.GetMouseButtonUp (0)) {
			if (!_grid.InGridBorders (mousePos))
				return;

			DeletePoints (_points, 0);		
			DeletePoints2 ();
			FinishDrawingHandler ();


		}

		if (_geometryBoundary.IsAddGeometry)
			DrawFigure (_points, _line);



		if (Input.GetMouseButton (1) && _geometryBoundary.IsAddGeometry) {
			_points.Clear ();
		} 
	}

	private void HandleStartGame ()
	{
		_trail.SetActive (true);
	}

	private Vector2 GridOperation (Vector2 point)
	{
		Vector2 temp = _grid.WorldToLogic (point);
		Vector2 temp2 = _grid.LogicToWorld ((int)temp.x, (int)temp.y);
		return temp2;
	}



	private List<Vector2> DeletePoints (List <Vector2> list, int startIndex)
	{
		if (startIndex == list.Count - 1)
			return list;

		for (int i = startIndex; i < list.Count-1; i++) {


			if (list [i] == list [i + 1])
				list.Remove (list [i + 1]);
		}
		startIndex++;
		DeletePoints (list, startIndex);
		return null;
	}

	private void AddPointsToGridArr (List <Vector2> points)
	{
		for (int i = 0; i < points.Count; i++) {
			Vector2 temp = _grid.WorldToLogic (points [i]);
			_grid.ScreenGrid [(int)temp.x, (int)temp.y] = points [i];
		}
	}

	/*private void DeletePoints2 ()
	{
		for (int i = 0; i < _grid.GridWidth; i++) {
			for (int j = 0; j < _grid.GridHeight; j++) {
				if (_grid.ScreenGrid[i,j] != Vector2.zero){

				}
			}
		}

	}*/

	private void DeletePoints2 ()
	{
		List <Vector2> tempList = new List<Vector2> ();

		for (int i = 0; i < _points.Count-2; i++) {
			FindPointsToDelete (tempList, _points [i], _points [i + 1], _points [i + 2]);
		}

		RemoveFrom_points (tempList);

		if (_points [0] == _points [_points.Count - 1]) {
			_points.Remove (_points [_points.Count - 1]);
		}

		CheckTheEnds ();
	}

	private void RemoveFrom_points (List <Vector2> tempList)
	{
		if (tempList.Count > 0) {
			
			foreach (var item in tempList) {
				
				Vector2 temp = _grid.LogicToWorld ((int)item.x, (int)item.y);
				_points.Remove (temp);
			}
			
			tempList.Clear ();
		}
	}

	private void FindPointsToDelete (List <Vector2> tempList, Vector2 point, Vector2 point1, Vector2 point2)
	{
		Vector2 temp = _grid.WorldToLogic (point);
		Vector2 temp1 = _grid.WorldToLogic (point1);
		Vector2 temp2 = _grid.WorldToLogic (point2);
		if ((int)temp.x == (int)temp1.x && (int)temp.x == (int)temp2.x) {
			tempList.Add (temp1);
		}
		
		
		if ((int)temp.y == (int)temp1.y && (int)temp.y == (int)temp2.y) {
			tempList.Add (temp1);
		}		


		if ((int)(temp.x + temp.y) == (int)(temp1.x + temp1.y) && (int)(temp.x + temp.y) == (int)(temp2.x + temp2.y)) {
			tempList.Add (temp1);
		}

		if ((int)(temp.x - temp.y) == (int)(temp1.x - temp1.y) && (int)(temp.x - temp.y) == (int)(temp2.x - temp2.y)) {
			tempList.Add (temp1);
		}
	} 

	private void CheckTheEnds ()
	{
		if (_points.Count <= 2)
			return;

		List <Vector2> tempList = new List<Vector2> ();
		FindPointsToDelete (tempList, _points [_points.Count - 1], _points [0], _points [1]);
		FindPointsToDelete (tempList, _points [_points.Count - 2], _points [_points.Count - 1], _points [0]);
		RemoveFrom_points (tempList);
	}


	public void DrawFigure (List <Vector2> list, LineRenderer line)
	{
		line.SetVertexCount (list.Count);

		for (int i = 0; i < list.Count; i++) {
			line.SetPosition (i, list [i]);
		}

	}

	private void OnDestroy ()
	{
		if (_gameCntrl)
			_gameCntrl.StartGame -= HandleStartGame;
	}

}
