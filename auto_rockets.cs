using UnityEngine;
using System.Collections;

public class auto_rockets : MonoBehaviour {
	public ParticleSystem launch_ps;
	public Vector3 gun_point=new Vector3(-2,-2,7);
	public byte rows=2;
	public byte count=4;
	bool ready=true;
	public float cooldown=30;
	float t=0;
	byte slot_number=0;
	int k=16;
	public float duration=0.1f;
	byte n=0;
	public byte maxpassing=10;
	byte passing=0;



	// Update is called once per frame
	void Update () {
		if (Global.pause||!Global.playable) return;
		if (t>0) {
			t-=Time.deltaTime;
			if (t<=0) {t=0;ready=true;}
		}
		Vector3 rt=Quaternion.LookRotation(Global.player.transform.position-transform.position).eulerAngles;
		rt.x=0;rt.z=0;
		transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(rt),70*Time.deltaTime);
		if (ready&&Quaternion.Angle(transform.rotation,Quaternion.Euler(rt))<3) {
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
				x.transform.forward=Global.player.transform.position-x.transform.position;
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
		
}
