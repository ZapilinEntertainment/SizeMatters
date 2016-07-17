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
	void Start () {
		Global.cam=cam;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		float t=Time.deltaTime;
		float rx=Input.GetAxis("Mouse X")*t;
		float ry=Input.GetAxis("Mouse Y")*t;
		if (point!=null) transform.position=point.transform.position+transform.TransformDirection(correction_vector); 
		if (satellite) {
			satellite.RotateAround(transform.position,transform.TransformDirection(Vector3.up),rotation_speed*rx);
			satellite.RotateAround(transform.position,satellite.transform.TransformDirection(Vector3.left),rotation_speed*ry);
		}
		else {
			Vector3 cfwd=transform.forward;
			cfwd.y=0;
			float a=Vector3.Angle(cfwd,transform.root.forward);
			if (cfwd.x<0) a*=-1;
			if (a+rx*rotation_speed>=left_border&&a+rx*rotation_speed<=right_border) {
			   transform.RotateAround(transform.position,transform.root.TransformDirection(Vector3.up),rotation_speed*rx);
			}
			cfwd=transform.forward;
			cfwd.x=0;
			a=Vector3.Angle(cfwd,transform.root.forward);
			if (cfwd.y<0) a*=-1;
			 //ry вверх - плюс, вниз - минус
			if (a+ry*rotation_speed>=down_border&&a+ry*rotation_speed<=up_border) {
			   transform.RotateAround(transform.position,transform.TransformDirection(Vector3.left),rotation_speed*ry);
			}
		}
			float sw=Input.GetAxis("Mouse ScrollWheel");
		if (sw!=0) {cam.transform.Translate(0,0,sw*zoom_speed*t,cam.transform);}
	}
}
