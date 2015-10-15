using UnityEngine;
using System.Collections;
using System;

public class GameController : MonoBehaviour
{

	#region Fields
	[SerializeField]
	private DrawLine
		_drawGeometry;

	[SerializeField]
	private CompareGeometry
		_compareGeo;

	[SerializeField]
	private GeometryBoundary
		_geometryBoundary;

	[SerializeField]
	private UILabel
		_timeLeftLabel;

	[SerializeField]
	private UILabel
		_pointsLabel;

	[SerializeField]
	private Grid
		_grid;

	[SerializeField]
	private GameObject
		_gameView;

	[SerializeField]
	private float
		_timeLeft = 10.0f;

	[SerializeField]
	private float
		_levelTimeStep;

	[SerializeField]
	private GameObject
		_corectlyEffect, _countdownEffect, _incorectlyEffect, _loseEffect;

	[SerializeField]
	private GameObject
		_retryButton;

	private bool _isTimeLeft;
	private int _points;
	#endregion

	#region Events
	public event Action StartGame;

	private void StartGameHandler ()
	{
		if (StartGame != null)
			StartGame ();
	}


	public event Action LevelUp;

	private void LevelUpHandler ()
	{
		if (LevelUp != null)
			LevelUp ();
	}

	public event Action LoseGame;

	private void LoseGameHandler ()
	{
		if (LoseGame != null)
			LoseGame ();
	}
	#endregion

	private void Awake ()
	{
		#region NullCheck
		if (!_drawGeometry) {
			Debug.Log ("DrawGeometry is null");
			return;
		}

		if (!_compareGeo) {
			Debug.Log ("CompareGeometry is null");
			return;
		}

		if (!_geometryBoundary) {
			Debug.Log ("Geometry is null");
			return;
		}

		if (!_timeLeftLabel) {
			Debug.Log ("Time label is null");
			return;
		}

		if (!_grid) {
			Debug.Log ("Grid is null");
			return;
		}

		if (!_gameView) {
			Debug.Log ("GameView is null");
			return;
		}

		if (!_pointsLabel) {
			Debug.Log ("PointsLabel is null");
			return;
		}

		if (!_corectlyEffect) {
			Debug.Log ("CorectlyEffect is null");
			return;
		}

		if (!_incorectlyEffect) {
			Debug.Log ("IncorectlyEffect is null");
			return;
		}

		if (!_loseEffect) {
			Debug.Log ("Lose is null");
			return;
		}

		if (!_countdownEffect) {
			Debug.Log ("Countdown is null");
			return;
		}
		#endregion

		_grid.gameObject.SetActive (_geometryBoundary.IsAddGeometry);
		_gameView.SetActive (false);

		_compareGeo.Corectly += HandleCorectly;
		_compareGeo.Incorectly += HandleIncorectly;

		_retryButton.SetActive (false);
	}



	private void Update ()
	{
		_pointsLabel.text = "Points:" + _points.ToString ();
	}

	public void StartGameMethod (GameObject obj)
	{
		GameObject temp = Instantiate (_countdownEffect) as GameObject;
		temp.transform.position = new Vector3 (0, 1, 0);
		Invoke ("CountDownDealay", 4.0f);
		obj.SetActive (false);
	}

	public void ReTry ()
	{
		StopAllCoroutines ();
		_timeLeft += _points * _levelTimeStep;
		StartCoroutine (TimeLeft (_timeLeft));	
		_retryButton.SetActive (false);
	}

	private void Lose ()
	{
		GameObject obj = Instantiate (_loseEffect) as GameObject;
		LoseGameHandler ();
		_retryButton.SetActive (true);
	}

	private void CountDownDealay ()
	{
		_gameView.SetActive (true);
		_grid.gameObject.SetActive (true);	
		StartCoroutine (TimeLeft (_timeLeft));
		StartGameHandler ();

	}

	private IEnumerator TimeLeft (float time)
	{
		_isTimeLeft = true;
		while (time >0) {
			time -= Time.fixedDeltaTime;
			int num = (int)time;
			_timeLeftLabel.text = "Time left:" + num.ToString ();
			yield return null;
		}

		_isTimeLeft = false;
		Lose ();
	}

	private void HandleIncorectly ()
	{
		if (_isTimeLeft) {
			GameObject obj = Instantiate (_incorectlyEffect) as GameObject;
		} else {
			Lose ();
		}
	}
	
	private void HandleCorectly ()
	{
		if (_isTimeLeft) {
			GameObject obj = Instantiate (_corectlyEffect) as GameObject;
			LevelUpHandler ();
			_timeLeft -= _levelTimeStep;
			
			if (_timeLeft < 1)
				_timeLeft = 1;
			
			StopAllCoroutines ();
			StartCoroutine (TimeLeft (_timeLeft));
			_points++;
		} else {
			Lose ();
		}
	}


	private void OnDestroy ()
	{
		if (_compareGeo) {
			_compareGeo.Corectly -= HandleCorectly;
			_compareGeo.Incorectly -= HandleIncorectly;
		}
	}

}
