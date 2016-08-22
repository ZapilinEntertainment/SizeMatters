using UnityEngine;
using System.Collections;

public class rocket_launcher : MonoBehaviour {
	public Transform pauldron;
	public ParticleSystem gun_splash;
	public byte rows=4;
	public byte count=4;
	public byte max_passing=25;
	byte passing;
	byte n=0;
	public bool right;
	bool firing=false;
	public float angle_border=60;
	public float duration=1;
	Vector3 startpos=new Vector3(-2,-12,29);
	Quaternion rotateTo;
	public AudioClip sound;
	public byte max_ammo=2;
	byte ammo;
	float time_left=0;
	float shot_time;
	public GameObject ammo_box;
	int k=16;

	void Start () {
		if (Global.nongamescene) {Destroy(this);return;}
		pauldron=transform.parent;
		if (transform.root.InverseTransformPoint(transform.position).x>0) {right=true;} else right=false;
		ammo=max_ammo;
		k=Screen.height/27;
		shot_time=max_passing*rows*duration;
		if (sound) {
			if (right) {Global.sm.rightgun_sound=sound;Global.sm.r_gun_dominating=true;}
			else {Global.sm.leftgun_sound=sound;Global.sm.l_gun_dominating=true;}
		}
	}

	void Update() {
		if (Global.pause||Global.nongamescene||!Global.playable) return;
		float an=Vector3.Angle(Global.aim-transform.position,pauldron.transform.forward);
		if (an<60&&pauldron.transform.InverseTransformPoint(Global.aim).z>0) {
			rotateTo=Quaternion.LookRotation(Global.aim-transform.position);
		}			
		if (rotateTo!=transform.rotation) {transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,45*Time.deltaTime);}
		if (time_left>0) {
			time_left-=Time.deltaTime;
			if (time_left<0) time_left=0;
		}

		if ((Input.GetMouseButtonDown(0)&&!right||Input.GetMouseButtonDown(1)&&right)&&ammo>0) {
			if (!firing) {
				ammo--;
				firing=true;
				time_left=shot_time;
				StartCoroutine(Launch());
				n=0;
				passing=max_passing;
				gun_splash.Play();
				Global.sm.GunSound(right,true,true);
			}
		}
	
	}

	public void Death() {
		Global.sm.GunSound(right,true,false);
	}

	IEnumerator Launch() {
		yield return new WaitForSeconds(duration);
		byte i=0;
		if (n<rows) {
			for (i=0;i<count;i++) {
				Instantiate(Global.r_rocket,transform.TransformPoint(startpos+new Vector3(n,i,0)),transform.rotation);
			}
		}
		n++;
		if (n==rows) {passing--;n=0;}
		if (passing==0) {
			firing=false;
			gun_splash.Stop();
			Global.sm.GunSound(right,true,false);
			if (ammo==0) {
				ammo_box.transform.parent=null;
				ammo_box.AddComponent<BoxCollider>();
				Rigidbody ar=ammo_box.AddComponent<Rigidbody>();
				ar.useGravity=true;
				ar.mass=50;
				ammo_box.AddComponent<foot>();
				ammo_box.AddComponent<timer>();
			}
		}
		else {StartCoroutine(Launch());}
	}

	void OnGUI() {
		if (Global.pause||!Global.playable) return;
		Vector2 mpos=Input.mousePosition;
		mpos.y=Screen.height-mpos.y;
		float t=time_left/shot_time;
		if (right) {
			GUI.DrawTexture(new Rect(mpos.x,mpos.y-k,4*k,2*k),Global.aim_tx_right,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(mpos.x+3*k,mpos.y+k/2-2*k*t,k,k),Global.aim_slider_tx,ScaleMode.StretchToFill);
			GUI.contentColor=Color.black;
			GUI.Label(new Rect(mpos.x+3*k,mpos.y+2*k,k,k),((int)(ammo)).ToString());
		}
		else {
			GUI.DrawTexture(new Rect(mpos.x-4*k,mpos.y-k,4*k,2*k),Global.aim_tx_left,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(mpos.x-4*k,mpos.y+k/2-2*k*t,k,k),Global.aim_slider_tx,ScaleMode.StretchToFill);
			GUI.contentColor=Color.black;
			GUI.Label(new Rect(mpos.x-4*k,mpos.y+2*k,4*k,k),((int)(ammo)).ToString());
		}
	}
}
