using UnityEngine;
using System.Collections;

public class catmech_physics : MonoBehaviour {
	public GameObject cabin;
	public GameObject aim_go;
	public float maxhp=50000;
	float hp;
	public float maxshield=50000;
	float shield;
	public float shield_reg_speed=500;
	public float cabin_rot_speed=30;
	public float rot_speed=20;
	Quaternion rotateTo;
	public GameObject outcam;
	public GameObject incam;
	bool inside_camera=false;
	bool move=false;
	public Animator anim;
	int walkHash = Animator.StringToHash("walk");
	int standHash = Animator.StringToHash("stand");
	public float maxspeed=5;
	 float speed=0;
	Vector3 mpos;
	RaycastHit camhit;
	Camera cam;
	float cax; //cabin_angle_x
	public float deep=25;
	bool cabin_rotation=false;
	bool use_sound=true;

	// Use this for initialization
	void Start () {
		Global.score=0;
		anim.SetBool("move",move);
		Global.player=incam;
		hp=maxhp;
		shield=maxshield;
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.pause||!Global.playable) return;
		if (Global.sm) use_sound=Global.sm.use_sound;
		if (!Global.mission_end) {
		if (Input.GetKeyDown("w")) {if (!move) {move=true;anim.SetTrigger (walkHash);anim.SetBool("move",true);StartCoroutine(AwaitForMoving(1));}}
		if (Input.GetKeyDown("s")) {if (move) {move=false;speed=0;anim.SetTrigger (standHash);anim.SetBool("move",false);}}
		if (Input.GetKey("a")&&speed>0) {transform.Rotate(new Vector3(0,-20*Time.deltaTime,0));}
		if (Input.GetKey("d")&&speed>0) {transform.Rotate(new Vector3(0,20*Time.deltaTime,0));}
		}
		else {
			Vector3 rt=Vector3.zero;
			if (Vector3.Distance(transform.position,Global.endpoint)>30) {
				rt=Quaternion.FromToRotation(transform.forward,Global.endpoint-transform.position).eulerAngles;
				rt.x=0;rt.z=0;
				transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(rt),rot_speed*Time.deltaTime);
			}
			else {
				rt=Quaternion.FromToRotation(transform.forward,Vector3.forward).eulerAngles;
				rt.x=0;rt.z=0;
				transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(rt),rot_speed*Time.deltaTime);
			}
			if (!move) {move=true;anim.SetTrigger (walkHash);anim.SetBool("move",true);StartCoroutine(AwaitForMoving(1));}
		}
		cax=Vector3.Angle(new Vector3(transform.forward.x,0,transform.forward.z),new Vector3(cabin.transform.forward.x,0,cabin.transform.forward.z));
		if (transform.InverseTransformDirection(cabin.transform.forward).x<0) cax*=-1;
		bool a=cabin_rotation;
		cabin_rotation=false;
		if (Input.GetKey("q")) {cabin.transform.Rotate(new Vector3(0,-20*Time.deltaTime,0));if (!cabin_rotation) {cabin_rotation=true;if (use_sound) Global.sm.CabinRotation(true);}}
		if (Input.GetKey("e")) {cabin.transform.Rotate(new Vector3(0,20*Time.deltaTime,0));if (!cabin_rotation) {cabin_rotation=true;if (use_sound) Global.sm.CabinRotation(true);}}
		if (a==true&&cabin_rotation==false&&use_sound) {Global.sm.CabinRotation(false);}
		if (Input.GetKeyDown("v")) {
			if (!inside_camera) {
				if (incam!=null) {
			Global.cam.SetActive(false);
				Global.cam=incam;
				Global.cam.SetActive(true);
				inside_camera=true;
				}
				}
			else {
				Global.cam.SetActive(false);
				Global.cam=outcam;
				Global.cam.SetActive(true);
				inside_camera=false;
			}
		}
		mpos=Input.mousePosition;
		//mpos.y=Screen.height-mpos.y;
		cam=Global.cam.GetComponent<Camera>();
		Ray ray = cam.ScreenPointToRay(new Vector3(mpos.x,mpos.y, 0));
		var layerMask = 1 << 10;
		// This would cast rays only against colliders in layer 8.
		// But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
		layerMask = ~layerMask;
		if (Physics.Raycast(ray.origin,ray.direction,out camhit,cam.farClipPlane,layerMask)) {
			Global.aim=camhit.point;
		}
		else Global.aim=ray.origin+ray.direction.normalized*cam.farClipPlane;
		aim_go.transform.position=Global.aim;

		RaycastHit rh;
		layerMask = 1 << 8;
		if (Physics.Raycast(cabin.transform.position,Vector3.down,out rh,200,layerMask)) {
			float dt=rh.point.y-cabin.transform.position.y+deep;
			if (Mathf.Abs(dt)>0.1f) {
				if (Mathf.Abs(dt)<7) {
					transform.Translate(dt*Vector3.up);
					if (speed>0) {
						transform.Translate(new Vector3(0,0,speed*Time.deltaTime),Space.Self);
					}
				}
				else {
					if (dt<0) transform.Translate(Vector3.down*Time.deltaTime*10);
				}
			}
			else {
				if (speed>0) {
					transform.Translate(new Vector3(0,0,speed*Time.deltaTime),Space.Self);
				}
			}
		}			

	}

	IEnumerator AwaitForMoving(float t) {
		yield return new WaitForSeconds(t);
		speed=maxspeed;
	}

	public void ApplyDamage (Vector4 v) {
		if (Global.sound&&Global.sm.hit_timer==0) {
			Global.sm.BrotherIAmHit(new Vector3(v.x,v.y,v.z));
		}
	}
		
}
