using UnityEngine;
using System.Collections;

public class Trail : MonoBehaviour
{
	[SerializeField]
	private Camera
		myCamera;

	private void Update ()
	{
		Vector3 temp = myCamera.ScreenToWorldPoint (Input.mousePosition);
		temp.z = 0;

		if (Input.GetMouseButton (0)) {
			gameObject.transform.position = temp;
		}
	}
}
