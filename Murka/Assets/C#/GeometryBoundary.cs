using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GeometryBoundary : MonoBehaviour
{
	#region Fields
	[SerializeField]
	private bool
		_isAddGeometry;

	[SerializeField]
	private DrawLine
		_drawGeometry;

	[SerializeField]
	private GameObject
		_taskLine;

	[SerializeField]
	private GameController
		_gameCntrl;

	private List<List<Vector2>> _geometry;
	private int _counter = 0;
	List <Vector2> _tempList = new List<Vector2> ();
	List <GameObject> _geometryLineList;
	private string _path;
	#endregion

	#region Properties
	public bool IsAddGeometry {
		get { return _isAddGeometry;}
	}

	public List <Vector2> TaskGeometry {
		get { return _geometry [_counter];}
	}
	#endregion

	#region Events
	public event Action LevelUp;

	private void LevelUpHandler ()
	{
		if (LevelUp != null)
			LevelUp ();
	}
	#endregion

	private void Awake ()
	{
		#region NullCheck
		if (!_drawGeometry) {
			Debug.Log ("DrawLine is null");
			return;
		}

		if (!_taskLine) {
			Debug.Log ("LineRenderer is null");
			return;
		}

		if (!_gameCntrl) {
			Debug.Log ("GameController is null");
			return;
		}
		#endregion

		_path = Application.dataPath + "/Data/data.txt"; 

		if (_isAddGeometry)
			_drawGeometry.FinishDrawing += HandleFinishDrawing;

		_geometry = new List<List<Vector2>> ();
		_geometryLineList = new List<GameObject> ();


		Load ();

		_gameCntrl.StartGame += HandleStartGame;
		_gameCntrl.LevelUp += HandleLevelUp;
		;

	}

	void HandleLevelUp ()
	{
		MoveRightCounter ();
		LevelUpHandler ();
	}



	private void Update ()
	{
		if (Input.GetMouseButtonDown (2)) {
			for (int i = 0; i < _geometry.Count; i++) {
				foreach (var item in _geometry[i]) {
					print (item);
				}
			}
		}
	}

	private void OnGUI ()
	{ 
		if (!_isAddGeometry)
			return;

		if (GUI.Button (new Rect (10, 10, 100, 50), "Add")) {
			CreateNewVector2List (_drawGeometry.Points);
			Save ();
		}


		if (GUI.Button (new Rect (120, 10, 100, 50), "Clear")) {
			ClearGrid ();
		}

		if (GUI.Button (new Rect (10, 60, 100, 50), "Remove")) {
			_geometry.RemoveAt (_counter);
			Save ();
		}

		if (GUI.Button (new Rect (Screen.width * 0.5f, 10, 50, 50), "<<")) {
			_counter--;
			_counter = (_counter < 0) ? _geometry.Count - 2 : _counter;
			
			DrawTaskGeometry (_geometry [_counter]);
		}

		if (GUI.Button (new Rect (Screen.width * 0.5f + 60, 10, 50, 50), ">>")) {
			MoveRightCounter ();
		}


	}

	#region Load&Save
	private void Save ()
	{
		string temp = "";
		for (int i = 0; i < _geometry.Count; i++) {
			for (int j = 0; j < _geometry[i].Count; j++) {
				temp += _geometry [i] [j].x.ToString () + "x" + _geometry [i] [j].y.ToString () + "y" + "#";
				
				if (j == _geometry [i].Count - 1) {
					temp += "*";
				}
			}
		}
		Data data = new Data ();
		data.mainString = temp;
	
		DataSerializer.Serialize (data, _path);
	}

	private void Load ()
	{
		Data data = DataSerializer.Deserialize <Data> (_path);
		if (data == null)
			return;
	
		char[] tempArr = data.mainString.ToCharArray ();

		string temp = "";

		Vector2 point = Vector2.zero;
	
		float num = 0;
		int counter = 0;

		//List <Vector2> tempList = new List<Vector2> ();
		_geometry.Add (new List<Vector2> ());

		for (int i = 0; i < tempArr.Length; i++) {				
			if (tempArr [i] == 'x') {

				float.TryParse (temp, out num);
				point.x = num;
				temp = "";
				continue;
			}

			if (tempArr [i] == 'y') {
				float.TryParse (temp, out num);
				point.y = num;
				temp = "";			
				continue;
			}

			if (tempArr [i] == '#') {
				//tempList.Add (point);
				_geometry [_geometry.Count - 1].Add (point);
				continue;
			}


			if (tempArr [i] == '*') {
				_geometry.Add (new List<Vector2> ());			
				continue;
			}


			temp += tempArr [i].ToString ();
		}


	}
	#endregion

	private void MoveRightCounter ()
	{
		_counter++;
		_counter = (_counter == _geometry.Count - 1) ? 0 : _counter;
		
		DrawTaskGeometry (_geometry [_counter]);
		print ("bound");
	}

	private void HandleFinishDrawing ()
	{

	}

	private void HandleStartGame ()
	{
		DrawTaskGeometry (_geometry [_counter]);
	}


	private void CreateNewVector2List (List <Vector2> list)
	{
		if (_geometry == null)
			_geometry = new List<List<Vector2>> ();
		_geometry.Add (new List<Vector2> ());
		for (int i = 0; i < _drawGeometry.Points.Count; i++) {
			Vector2 temp = new Vector2 ();
			temp = _drawGeometry.Points [i];
			_geometry [_geometry.Count - 1].Add (temp);
		}

	}

	private void DrawTaskGeometry (List<Vector2> list)
	{
		if (list.Count == 0)
			return;

		ClearGrid ();

		for (int i = 0; i < list.Count; i++) {
			GameObject obj = _taskLine.Spawn ();
			LineRenderer line = obj.GetComponent <LineRenderer> ();

			if (!_geometryLineList.Contains (obj)) {
				_geometryLineList.Add (obj);
			}

			if (i == list.Count - 1) {
				line.SetPosition (0, list [list.Count - 1]);
				line.SetPosition (1, list [0]);
				break;
			}

			line.SetPosition (0, list [i]);
			line.SetPosition (1, list [i + 1]);
		}

	}

	private void ClearGrid ()
	{
		foreach (var item in _geometryLineList) {
			item.SetActive (false);
		}
	}

	private void OnDestroy ()
	{
		if (!_isAddGeometry)
			return;


		_drawGeometry.FinishDrawing -= HandleFinishDrawing;

		if (_gameCntrl) {
			_gameCntrl.StartGame -= HandleStartGame;
			_gameCntrl.LevelUp -= MoveRightCounter;
		}
	}

}

public class Data
{
	public string mainString;
}
