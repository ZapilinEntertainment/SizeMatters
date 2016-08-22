using UnityEngine;
using System.Collections;

public class cursor : MonoBehaviour {
	public Renderer rr;

	void Start () {rr=gameObject.GetComponent<MeshRenderer>();}

	void Update () {
		if (Vector3.Distance(Global.cam.transform.position,transform.position)<100) {
			rr.enabled=false;
		}
		else {
			rr.enabled=true;
		}
	}
}
