using UnityEngine;
using System.Collections;

public class spriter : MonoBehaviour {

	Vector3 origin;
	public float speed=0.0f;
	public float power=0.3f;
	public float timer=0;
	public float time_left;
	bool use_timer=false;
	Vector3 start_scale;
	
	void Start () {
		origin=transform.localScale;
		if (timer>0) {use_timer=true;time_left=timer;start_scale=transform.localScale;}
	}

	void Update () {
		if (!Global.cam) return;
		transform.LookAt(Global.cam.transform.position);
		if (speed!=0) transform.localScale=origin*(1+Mathf.PingPong(Time.time*speed,power));
		if (use_timer) {
			time_left-=Time.deltaTime;
			if (time_left>0) {transform.localScale=start_scale*time_left/timer;}
			else gameObject.SetActive(false);
		}
	}

	public void Reboot() {
		time_left=timer;
	}
}
