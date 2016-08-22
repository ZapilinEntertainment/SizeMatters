using UnityEngine;
using System.Collections;

public class spot_optimizer : MonoBehaviour {
	public SpriteRenderer sr;
	bool off=false;
	float d=0;
	public int border=1000;

	void Start() {
		StartCoroutine(OPT());
	}
	// Update is called once per frame
	IEnumerator OPT () {
		yield return new WaitForSeconds(1);
		d=Vector3.Distance(Global.cam.transform.position,transform.position);
		if (d>border) {
			if (!off) {
				sr.enabled=false;
				off=true;
			}
		}
		else {
			if (off) {
				sr.enabled=true;
				off=false;
			}
		}
		while ( Global.pause) {
				yield return null; 
			}

		StartCoroutine(OPT());
	}
}
