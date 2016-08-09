using UnityEngine;
using System.Collections;

public class artbattery : MonoBehaviour {
	public Transform tower;
	public Transform gunwheel;
	public Transform guns;
	public ParticleSystem fire_ps;
	public float rot_speed=70;
	public int range=4000;
	public float damage=1000;
	public bool ready=true;
	public float reload_time=25;
	byte slot_number=0;
	Texture icon;
	int k=16;

	void Awake () {
		if (Global.nongamescene) Destroy(this);
		icon=transform.root.GetComponent<moduleInfo>().picture;
	}

	void Start () {
		k=Screen.height/27;
	}
	
	// Update is called once per frame
	void Update () {
		if (Global.pause||!Global.playable) return;
		Vector3 rt=Quaternion.LookRotation(Global.aim-transform.position).eulerAngles;
		tower.rotation=Quaternion.RotateTowards(tower.rotation,Quaternion.Euler(new Vector3(0,rt.y,0)),rot_speed*Time.deltaTime);
		rt.z=0;rt.y=tower.rotation.eulerAngles.y;
		gunwheel.rotation=Quaternion.RotateTowards(gunwheel.rotation,Quaternion.Euler(rt),rot_speed*Time.deltaTime);
		if (ready) {
			if (Input.GetKeyDown("1")) {
				ready=false;
				StartCoroutine(Fire());
			}
		}
	}

	IEnumerator Fire() {
		if (Global.sm&&Global.sound) Global.sm.MyArtShot();
		ready=false;
		yield return new WaitForSeconds(0.2f);
		RaycastHit rh;
		Vector3 endpos=Vector3.zero;
			fire_ps.Emit(25);
		var layerMask = 1 << 10;
		layerMask = ~layerMask;
			if (Physics.Raycast(guns.position,Global.aim-guns.position,out rh,range,layerMask)) {
				endpos=rh.point;
			Collider[] cds=Physics.OverlapSphere(rh.point,30);
			foreach (Collider cd in cds) {
				cd.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,damage),SendMessageOptions.DontRequireReceiver);
			}
			}
			else endpos=guns.position+guns.forward*range;
		Global.SmallExplosionRequest(endpos);
		yield return new WaitForSeconds(reload_time);
		ready=true;
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
