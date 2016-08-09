using UnityEngine;
using System.Collections;

public class cam_control : MonoBehaviour {
	public GameObject point;
	public Transform satellite;
	public GameObject cam;
	public float rotation_speed=70;
	public float zoom_speed=50;
	public float left_border=-45;
	public float right_border=45;
	public float up_border=-45;
	public float down_border=45;
	public Vector3 correction_vector=Vector3.zero;
	// Use this for initialization
	void Awake () {
		Global.cam=cam;
	}
	
	// Update is called once per frame
	void Update () {
		
		float sw=Input.GetAxis("Mouse ScrollWheel");
		if (sw!=0) {cam.transform.Translate(0,0,sw*zoom_speed*Time.deltaTime,cam.transform);}
	}
}
