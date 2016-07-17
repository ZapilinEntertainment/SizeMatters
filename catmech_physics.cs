using UnityEngine;
using System.Collections;

public class catmech_physics : MonoBehaviour {
	public GameObject cabin;
	public GameObject aim_go;
	public float cabin_rot_speed=30;
	public float rot_speed=20;
	Quaternion rotateTo;
	public GameObject outcam;
	public GameObject incam;
	bool inside_camera=false;
	bool move=false;
	bool rotate_cabin=true;
	public Animator anim;
	int walkHash = Animator.StringToHash("walk");
	int standHash = Animator.StringToHash("stand");
	float speed=0;
	Vector3 mpos;
	RaycastHit camhit;
	Camera cam;

	// Use this for initialization
	void Start () {
		anim.SetBool("move",move);
		Global.player=cabin;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("w")) {if (!move) {move=true;anim.SetTrigger (walkHash);anim.SetBool("move",true);StartCoroutine(AwaitForMoving(1));}}
		if (Input.GetKeyDown("s")) {if (move) {move=false;speed=0;anim.SetTrigger (standHash);anim.SetBool("move",false);}}
		if (Input.GetKey("a")&&speed>0) {transform.Rotate(new Vector3(0,-20*Time.deltaTime,0));}
		if (Input.GetKey("d")&&speed>0) {transform.Rotate(new Vector3(0,20*Time.deltaTime,0));}
		if (Input.GetKeyDown(KeyCode.Tab)) rotate_cabin=!rotate_cabin;
		if (Input.GetKeyDown("v")) {
			if (!inside_camera) {
			Global.cam.SetActive(false);
				Global.cam=incam;
				Global.cam.SetActive(true);
				inside_camera=true;
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

		if (!inside_camera&&rotate_cabin) {
			Quaternion rt=Quaternion.LookRotation(new Vector3(Global.aim.x,cabin.transform.position.y,Global.aim.z)-cabin.transform.position);
			rt=Quaternion.Euler(0,rt.eulerAngles.y,0);
			cabin.transform.rotation=Quaternion.RotateTowards(cabin.transform.rotation,rt,cabin_rot_speed*Time.deltaTime);
	}
		if (speed>0) {
			transform.Translate(new Vector3(0,0,speed*Time.deltaTime),Space.Self);
		}

	}

	IEnumerator AwaitForMoving(float t) {
		yield return new WaitForSeconds(t);
		speed=30;
	}


}
