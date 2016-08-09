using UnityEngine;
using System.Collections;

public class moduleInfo : MonoBehaviour {
	public string code="00nothing";
	public Texture picture;
	public Vector3 correction_vector;
	public byte type=0; //0-gun, 1-module;
	public Vector3 rot_rf=Vector3.zero; //right_forward;
	public Vector3 rot_lf=Vector3.zero; //left_forward
	public Vector3 rot_rb=Vector3.zero; //right back
	public Vector3 rot_lb=Vector3.zero; //left back


}
