using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpgratedObjPool
{
	private static UpgratedObjPool instance;
    
	public static UpgratedObjPool Instance {
		get {
			if (instance == null)
				instance = new UpgratedObjPool ();

			return instance;
		}
        
	}

	private UpgratedObjPool ()
	{
	}

	private Dictionary<GameObject, List<GameObject>> pool;

	public void PooleObjects (GameObject prefab, int qauntity)
	{
		if (pool == null)
			pool = new Dictionary<GameObject, List<GameObject>> ();

		if (!pool.ContainsKey (prefab)) {
			pool.Add (prefab, new List<GameObject> ());
		}

		List<GameObject> tempList;

		if (pool.TryGetValue (prefab, out tempList)) {
			if (tempList == null) {
				tempList = new List<GameObject> ();
			}

			for (int i = 0; i < qauntity; i++) {
				tempList.Add (CreateObj (prefab));
			}
		}
	}

	public GameObject GetObject (GameObject prefab)
	{
		if (pool == null) {
			pool = new Dictionary<GameObject, List<GameObject>> ();
		}

		if (!pool.ContainsKey (prefab)) {
			pool.Add (prefab, new List<GameObject> ());
		}

		List<GameObject> tempList;

		if (pool.TryGetValue (prefab, out tempList)) {
			if (tempList == null) {
				tempList = new List<GameObject> ();
				//  return null;
			}

			foreach (var item in tempList) {
				if (!item.activeSelf) {
					item.SetActive (true);
					return item;
				}
			}

			var temp = CreateObj (prefab);

			if (temp == null)
				return null;

			tempList.Add (temp);
			temp.SetActive (true);
			return temp;
		}
		return null;
	}


	private GameObject CreateObj (GameObject obj)
	{
		var temp = MonoBehaviour.Instantiate (obj) as GameObject;
	

		if (temp == null)
			return null;
		temp.SetActive (false);
		return temp;
	}

	public void ClaenObj (GameObject prefab, int quantity = -1)
	{
		if (pool == null) {
			return;
		}

		if (!pool.ContainsKey (prefab)) {
			return;
		}


		List<GameObject> tempList;


		if (pool.TryGetValue (prefab, out tempList)) {
			if (tempList == null) {
				return;
			}

			int temp = quantity == -1 ? tempList.Count : quantity;

			temp = Mathf.Clamp (temp, 0, tempList.Count);

			for (int i = 0; i <temp; i++) {
				MonoBehaviour.Destroy (tempList [0]);
				tempList.RemoveAt (0);
			}

           
		}
	} 
}


public static class PoolExetentions
{
	public static void Pooled (this GameObject obj, int quantity)
	{
		if (UpgratedObjPool.Instance != null)
			UpgratedObjPool.Instance.PooleObjects (obj, quantity);
	}

	public static GameObject Spawn (this GameObject obj)
	{
		if (UpgratedObjPool.Instance != null)
			return  UpgratedObjPool.Instance.GetObject (obj);

		return null;
	}

	public static void Recicle (this GameObject obj)
	{
		obj.SetActive (false);
	}

	public static void CleanPool (this GameObject obj, int quantity = -1)
	{
		if (UpgratedObjPool.Instance != null)
			UpgratedObjPool.Instance.ClaenObj (obj, quantity);
	}
}