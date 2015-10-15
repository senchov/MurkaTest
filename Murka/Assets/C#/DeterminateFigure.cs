using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeterminateFigure : MonoBehaviour
{
	[SerializeField]
	private Camera
		_myCamera;
	
	[SerializeField]
	private float
		_offSet = 0.1f;

	private List <Vector3> _points;
	private Vector3 _startPoint, _nextPoint, _direction, _currentDir;
	private bool _isAlongX = true;

	private void Awake ()
	{
		_points = new List<Vector3> ();
	}



}
