using UnityEngine;
using System.Collections;

public class shoulder_launcher : MonoBehaviour {
	public ParticleSystem launch_ps;
	public Vector3 gun_point=new Vector3(-2,-2,7);
	public byte rows=2;
	public byte count=4;
	Texture icon=null;
	bool ready=true;
	public float cooldown=30;
	float t=0;
	byte slot_number=0;
	int k=16;
	public float duration=0.1f;
	byte n=0;
	public byte maxpassing=10;
	byte passing=0;
	// Use this for initialization
	void Awake () {
		if (Global.nongamescene) Destroy(this);
	}

	void Start () {
		icon=gameObject.GetComponent<moduleInfo>().picture;
		k=Screen.height/27;
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.pause||!Global.playable) return;
		if (t>0) {
			t-=Time.deltaTime;
			if (t<=0) {t=0;ready=true;}
		}
		if (ready&&Input.GetKeyDown("2")) {
			ready=false;
			launch_ps.Play();
			StartCoroutine(Launch());
			passing=maxpassing;
			n=0;
		}
	}

	IEnumerator Launch() {
		GameObject x=null;
		byte i=0;
		if (n<rows) {
			for (i=0;i<count;i++) {
				yield return new WaitForSeconds(duration);
				x=Instantiate(Global.r_simple_rocket,transform.TransformPoint(gun_point+new Vector3(n,i,0)),transform.rotation) as GameObject;
				x.transform.forward=Global.aim-x.transform.position;
			}
		}
		n++;
		if (n==rows) {passing--;n=0;}
		if (passing==0) {
			launch_ps.Stop();
			yield return new WaitForSeconds(cooldown);
			ready=true;
		}
		else {StartCoroutine(Launch());}
	}

	public void SlotNumber (byte x) {
		slot_number=x;
	}

	void OnGUI () {
		if (Global.pause||!Global.playable) return;
		GUI.DrawTexture(new Rect(slot_number*2*k,0,k,k),icon,ScaleMode.StretchToFill);
		if (ready) {
			GUI.DrawTexture(new Rect(slot_number*2*k+k,k,k,k),Global.ind_green_tx,ScaleMode.StretchToFill);
		}
		else {
			GUI.DrawTexture(new Rect(slot_number*2*k+k,k,k,k),Global.ind_red_tx,ScaleMode.StretchToFill);
		}
	}
}
