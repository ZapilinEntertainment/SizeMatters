using UnityEngine;
using System.Collections;

public class envcam_sync : MonoBehaviour {


	// Update is called once per frame
	void Update () {
		if (!Global.cam) return;
		transform.rotation=Global.cam.transform.rotation;
	}
}
