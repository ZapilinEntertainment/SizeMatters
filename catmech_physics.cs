using UnityEngine;
using System.Collections;

public class catmech_physics : MonoBehaviour {
	public GameObject cabin;
	public GameObject aim_go;
	public float maxhp=50000;
	float hp=0;
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
	GameObject chassis_icon;
	GameObject cabin_icon;
	GameObject cabin_ind;
	Texture green_line;
	Texture lblue_line;

	// Use this for initialization
	void Start () {
		Global.score=0;
		anim.SetBool("move",move);
		Global.player=incam;
		hp=maxhp;
		shield=maxshield;
		cabin_ind=new GameObject("cabin_indicator");
		cabin_ind.transform.position=Global.cam.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.1f,0.5f,0.32f));
		cabin_ind.transform.parent=Global.cam.transform;
		cabin_ind.transform.forward=-Global.cam.transform.forward;
		chassis_icon=Instantiate(Resources.Load<GameObject>("chassis_icon"));
		chassis_icon.transform.parent=cabin_ind.transform;
		chassis_icon.transform.localPosition=Vector3.zero;
		chassis_icon.transform.localRotation=Quaternion.Euler(0,0,0);
		cabin_icon=Instantiate(Resources.Load<GameObject>("cabin_icon"));
		cabin_icon.transform.parent=cabin_ind.transform;
		cabin_icon.transform.localPosition=new Vector3(0,0,0.001f);
		cabin_icon.transform.localRotation=Quaternion.Euler(0,0,0);

		green_line=Resources.Load<Texture>("horizontal_green_line");
		lblue_line=Resources.Load<Texture>("horizontal_lightblue_line");
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.pause||!Global.playable) return;
		if (Global.sm) use_sound=Global.sm.use_sound;
		if (!Global.mission_end) {
			anim.speed=(Global.bonus-1)/8+1;
		if (Input.GetKeyDown("w")) {if (!move) {move=true;anim.SetTrigger (walkHash);anim.SetBool("move",true);StartCoroutine(AwaitForMoving(1));}}
		if (Input.GetKeyDown("s")) {if (move) {move=false;speed=0;anim.SetTrigger (standHash);anim.SetBool("move",false);}}
			if (Input.GetKey("a")&&speed>0) {transform.Rotate(new Vector3(0,-20*Time.deltaTime*((Global.bonus-1)/4+1),0));}
			if (Input.GetKey("d")&&speed>0) {transform.Rotate(new Vector3(0,20*Time.deltaTime*((Global.bonus-1)/4+1),0));}
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
		if (Input.GetKey("q")) {
			cabin.transform.Rotate(new Vector3(0,-20*Time.deltaTime*((Global.bonus-1)/4+1),0));
			if (!cabin_rotation) {
				cabin_rotation=true;
				if (use_sound) Global.sm.CabinRotation(true);
			}
		}
		if (Input.GetKey("e")) {
			cabin.transform.Rotate(new Vector3(0,20*Time.deltaTime*((Global.bonus-1)/4+1),0));
			if (!cabin_rotation) {
				cabin_rotation=true;
				if (use_sound) Global.sm.CabinRotation(true);
			}
		}
		if (a==true&&cabin_rotation==false&&use_sound) {Global.sm.CabinRotation(false);}
		if (Input.GetKeyDown("v")) {
			if (!inside_camera) {
				if (incam!=null) {
			    Global.cam.SetActive(false);
				Global.cam=incam;
				Global.cam.SetActive(true);
					cabin_ind.transform.position=Global.cam.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.1f,0.5f,0.32f));
					cabin_ind.transform.parent=Global.cam.transform;
					cabin_ind.transform.forward=-Global.cam.transform.forward;
					chassis_icon.transform.parent=cabin_ind.transform;
					chassis_icon.transform.localPosition=Vector3.zero;
					chassis_icon.transform.localRotation=Quaternion.Euler(0,0,0);
					cabin_icon.transform.parent=cabin_ind.transform;
					cabin_icon.transform.localPosition=new Vector3(0,0,0.001f);
					cabin_icon.transform.localRotation=Quaternion.Euler(0,0,0);
				inside_camera=true;
				}
				}
			else {
				Global.cam.SetActive(false);
				Global.cam=outcam;
				Global.cam.SetActive(true);
				cabin_ind.transform.position=Global.cam.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.1f,0.5f,0.32f));
				cabin_ind.transform.parent=Global.cam.transform;
				cabin_ind.transform.forward=-Global.cam.transform.forward;
				chassis_icon.transform.parent=cabin_ind.transform;
				chassis_icon.transform.localPosition=Vector3.zero;
				chassis_icon.transform.localRotation=Quaternion.Euler(0,0,0);
				cabin_icon.transform.parent=cabin_ind.transform;
				cabin_icon.transform.localPosition=new Vector3(0,0,0.001f);
				cabin_icon.transform.localRotation=Quaternion.Euler(0,0,0);
				inside_camera=false;
			}
		}
		mpos=Input.mousePosition;
		//mpos.y=Screen.height-mpos.y;
		cam=Global.cam.GetComponent<Camera>();
		Ray ray = cam.ScreenPointToRay(new Vector3(mpos.x,mpos.y, 0));
		var layerMask = 1 << 10;
		// This would cast rays only against colliders in layer 10.
		// But instead we want to collide against everything except layer 10. The ~ operator does this, it inverts a bitmask.
		layerMask = ~layerMask;
		if (Physics.Raycast(ray.origin,ray.direction,out camhit,cam.farClipPlane,layerMask)) {
			Global.aim=camhit.point;
		}
		else Global.aim=ray.origin+ray.direction.normalized*cam.farClipPlane;
		aim_go.transform.position=Global.aim;
		if (Vector3.Distance(aim_go.transform.position,Global.cam.transform.position)<100) {aim_go.SetActive(false);}
		else {
			aim_go.SetActive(true);
		}

		RaycastHit rh;
		layerMask = 1 << 8;
		if (Physics.Raycast(cabin.transform.position,Vector3.down,out rh,200,layerMask)) {
			float dt=rh.point.y-cabin.transform.position.y+deep;
			if (Mathf.Abs(dt)>0.1f) {
				if (Mathf.Abs(dt)<7) {
					transform.Translate(dt*Vector3.up);
					if (speed>0) {
						transform.Translate(new Vector3(0,0,speed*Time.deltaTime*((Global.bonus-1)/4+1)),Space.Self);
					}
				}
				else {
					if (dt<0) transform.Translate(Vector3.down*Time.deltaTime*10);
				}
			}
			else {
				if (speed>0) {
					transform.Translate(new Vector3(0,0,speed*Time.deltaTime*((Global.bonus-1)/4+1)),Space.Self);
				}
			}
		}			
		if (shield<maxshield) {shield+=shield_reg_speed*Time.deltaTime*Global.bonus;}
	}

	void LateUpdate () {
		cabin_icon.transform.localRotation=Quaternion.Euler(0,0,cabin.transform.localRotation.eulerAngles.y);
	}

	IEnumerator AwaitForMoving(float t) {
		yield return new WaitForSeconds(t);
		speed=maxspeed;
	}

	public void ApplyDamage (Vector4 v) {
		if (Global.sound&&Global.sm.hit_timer==0) {
			Global.sm.BrotherIAmHit(new Vector3(v.x,v.y,v.z));
		}
		if (Global.invincible||Global.mission_end) return;
		if (shield>0) {
			shield-=v.w*0.7f;
		}
		else {
			hp-=v.w*0.3f;
			if (hp<0) {
				if (inside_camera) 
				{
				Global.cam.SetActive(false);
				Global.cam=outcam;
				Global.cam.SetActive(true);
				inside_camera=false;
				}
				Global.cam.transform.parent=null;
				RaycastHit rc;
				var lm=1<<8;
				Vector3 dir=new Vector3(Random.Range(-1,1)*200,Random.Range(-1,1)*10,Random.Range(-1,1)*200);
				if (Physics.Raycast(transform.position,dir,out rc,500,lm)) {
					Global.cam.transform.position=rc.point+Vector3.up*200;
				}
				else Global.cam.transform.position=transform.position+dir;
				Global.cam.transform.forward=transform.position-Global.cam.transform.position;
				BroadcastMessage("Death",SendMessageOptions.DontRequireReceiver);
				Global.sm.CabinRotation(false);
				Global.bonus=1;
				Destroy(cabin_ind);
				gameObject.SetActive(false);
				Instantiate(Resources.Load<GameObject>("dead_mech"),transform.position,transform.rotation);
				Global.menu_script.fail=true;
				Destroy(gameObject);
			}
		}
	}

	void OnGUI() {
		if (Global.mission_end||Global.invincible||!Global.playable) return;
		float x=hp/maxhp*6*Global.gui_piece;
		GUI.DrawTexture(new Rect(0,0,x,Global.gui_piece/2),green_line,ScaleMode.StretchToFill);
		x=shield/(maxshield*Global.bonus)*6*Global.gui_piece;
		GUI.DrawTexture(new Rect(0,Global.gui_piece/2,x,Global.gui_piece/2),lblue_line,ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(0,0,Global.gui_piece*6,Global.gui_piece/2),Global.menu_script.frame_tx,ScaleMode.StretchToFill);
		GUI.DrawTexture(new Rect(0,Global.gui_piece/2,Global.gui_piece*6,Global.gui_piece/2),Global.menu_script.frame_tx,ScaleMode.StretchToFill);
	}
		
}
