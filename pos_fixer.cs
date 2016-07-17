using UnityEngine;
using System.Collections;

public class pos_fixer : MonoBehaviour {
	public GameObject point;
	public Vector3 correction_vector;

	
	// Update is called once per frame
	void Update () {
		transform.position=point.transform.position+transform.TransformDirection(correction_vector);
	}
}
