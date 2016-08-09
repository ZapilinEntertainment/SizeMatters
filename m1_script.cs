using UnityEngine;
using System.Collections;

public class m1_script : MonoBehaviour {
	public GameObject mech;
	public float speed=30;
	public GameObject sand_ps;
	public GameObject sand2_ps;
	public AudioClip roar;
	public GameObject coolcam;
	public GameObject realcam;
	// Use this for initialization
	void Start () {
		realcam=Global.cam;
		Global.cam.SetActive(false);
		coolcam.SetActive(true);
		Global.cam=coolcam;
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.cam!=coolcam) {
			realcam=Global.cam;
			Global.cam.SetActive(false);
			Global.cam=coolcam;
			coolcam.SetActive(true);
		}
		if (Global.playable) {
			Destroy(sand_ps);
			Destroy(sand2_ps);
			realcam.SetActive(true);
			Global.cam=realcam;
			Destroy(coolcam);
			Destroy(this);
		}
		else {
			mech.transform.Translate(Vector3.up*speed*Time.deltaTime);
		}
	}
}
