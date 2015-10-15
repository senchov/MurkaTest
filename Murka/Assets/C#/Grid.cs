using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour
{

	#region Fields
	[SerializeField]
	private Camera
		_myCamera;

	[SerializeField]
	private GameObject
		_test;

	[SerializeField]
	private int
		_width;

	[SerializeField]
	private int
		_height;

	[SerializeField]
	private float
		_offSet;

	[SerializeField]
	private GeometryBoundary
		_geometryBoundary;

	private Vector2[,] _grid;

	Vector3 _bottomLeftScreen, _bottomRightScreen, _topRightScreen;
	Vector2 _gridBorderBL, _gridBorderTR;
	float _cellDimensionX, _cellDimensionY;
	#endregion

	#region Properties
	public Vector2[,] ScreenGrid {
		get { return _grid;}
		set { _grid = value;}
	}

	public float CellWidth {
		get { return _cellDimensionX;}
	}

	public int GridWidth {
		get { return _width;}
	}

	public int GridHeight {
		get { return _height;}
	}
	#endregion

	private void Awake ()
	{
		#region NullCheck
		if (!_geometryBoundary) {
			Debug.Log ("Geometry is null");
			return;
		}

		if (!_myCamera) {
			Debug.Log ("Camera is null");
			return;
		}
		#endregion
	}

	private void Start ()
	{
		GenerateGrid ();
	}


	private void GenerateGrid ()
	{
		_bottomLeftScreen = _myCamera.ScreenToWorldPoint (new Vector3 (0, 0, 0));
		_bottomLeftScreen.z = 0;

		_bottomRightScreen = _myCamera.ScreenToWorldPoint (new Vector3 (Screen.width, 0, 0));
		_bottomRightScreen.z = 0;
	
		_topRightScreen = _myCamera.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0));
		_topRightScreen.z = 0;

		float screenWidth = Vector3.Distance (_bottomLeftScreen, _bottomRightScreen);
		float screenHeight = Vector3.Distance (_bottomRightScreen, _topRightScreen);

		_grid = new Vector2[_width, _height];

		_cellDimensionX = screenWidth / _width;
		_cellDimensionY = screenHeight / _height;

		for (int i = 0; i < _width; i++) {
			for (int j = 0; j < _height; j++) {

				_grid [i, j] = new Vector2 (_bottomLeftScreen.x + _cellDimensionX * 0.5f + _cellDimensionX * i, _bottomLeftScreen.y + _cellDimensionY * 0.5f + _cellDimensionY * j);
			}
		}

		int normalGridOffsetW = Mathf.RoundToInt (_width / 3); 
		int normalGridOffsetH = Mathf.RoundToInt (_height / 3);

		if (_geometryBoundary.IsAddGeometry) {
			CreateGrid (_width - normalGridOffsetW, _height - normalGridOffsetH, _width, _height);
		} else {
			CreateGrid (0, 0, _width - normalGridOffsetW, _height);
		}

	}

	public bool InGridBorders (Vector3 point)
	{
		if (point.x >= _gridBorderBL.x && point.x <= _gridBorderTR.x && point.y >= _gridBorderBL.y && point.y <= _gridBorderTR.y)
			return true;

		return false;
	}

	private void CreateGrid (int xMin, int yMin, int xMax, int yMax)
	{
		for (int j = xMin; j < xMax; j++) {
			for (int i = yMin; i < yMax; i++) {
				GameObject obj = Instantiate (_test) as GameObject;
				Vector3 point = new Vector3 (_grid [j, i].x, _grid [j, i].y, 0);
				obj.transform.position = point;
				obj.name = j.ToString () + " " + i.ToString ();
				obj.transform.SetParent (gameObject.transform);
			}
		}

		_gridBorderBL = LogicToWorld (xMin, yMin);
		_gridBorderTR = LogicToWorld (xMax, yMax);
	}

	public Vector2 LogicToWorld (int i, int j)
	{
		Vector2 pos = Vector2.zero;
		pos.x = _bottomLeftScreen.x + _cellDimensionX * 0.5f + _cellDimensionX * i;
		pos.y = _bottomLeftScreen.y + _cellDimensionY * 0.5f + _cellDimensionY * j;
		return pos;
	}

	public Vector2 WorldToLogic (Vector2 point)
	{
		Vector2 pos = Vector2.zero;
		pos.x = (int)((point.x - _bottomLeftScreen.x) / _cellDimensionX);
		pos.y = (int)((point.y - _bottomLeftScreen.y) / _cellDimensionY);
		return pos;
	}
}
