using UnityEngine;
using System.Collections;

public class plane : MonoBehaviour {
	public float speed=300;
	public float acceleration=5;
	public int attack_range=4000;
	public int bombing_distance=200;
	public int bombs=10;
	public float rocket_timer=20;
	public bool reloading=false;
	public bool bombing=false;
	public float maxhp=500;
	public float hp;
	bool dead=false;
	public int points=4000;
	AudioSource sounder;
	bool pause=false;

	void Start() {
		hp=maxhp;
		sounder=gameObject.AddComponent<AudioSource>();
	//	sounder.clip=Global.sm.chopper;
		sounder.loop=true;
		sounder.volume=0.1f;
		sounder.Play();
	}
	// Update is called once per frame
	void Update () {
		if (!dead&&Global.player) {
			RaycastHit rt;
			var layerMask=1<<8;
			Vector3 rtt=transform.rotation.eulerAngles;
			Vector3 inpos= transform.InverseTransformPoint(Global.player.transform.position);
			if (inpos.magnitude>attack_range) {
				if (!reloading) {
					rtt=Quaternion.LookRotation(Global.player.transform.position-transform.position).eulerAngles;
					rtt.x=0;rtt.z=0;
					transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(rtt),acceleration*Time.deltaTime);
				}
			}
			else {
				if (!reloading) {
					rtt=Quaternion.LookRotation(Global.player.transform.position-transform.position).eulerAngles;
					rtt.x=0;rtt.z=0;
					transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(rtt),acceleration*Time.deltaTime);
					if (inpos.z>0&&!Physics.Raycast(transform.position,Global.player.transform.position-transform.position,out rt,inpos.magnitude,layerMask)) {
						StartCoroutine(RocketTime());
						reloading=true;
					}
				}
				if (new Vector3(inpos.x,0,inpos.z).magnitude<bombing_distance&&!bombing) {
					StartCoroutine(Bombing());
					bombing=true;
				}
			}
			RaycastHit rh;
			float h=transform.position.y;
			if (Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out rh,speed)) {
				transform.Translate(Vector3.up*acceleration,Space.Self);
			}
			transform.Translate(Vector3.forward*speed*Time.deltaTime,Space.Self);
		}
		else {
			if (transform.position.y<-200) Destroy(gameObject);
		}
	}

	IEnumerator Bombing () {
		RaycastHit rt;
		var layerMask=1<<8;
		for (byte j=0;j<bombs;j++) {
			yield return new WaitForSeconds(0.1f);
			if (Physics.Raycast(transform.position,Vector3.down,out rt,10000,layerMask)){
				Global.ArtilleryStrikeAt(rt.point);
			}
		}
		bombing=false;
	}

	IEnumerator RocketTime() {
		for (byte i=0;i<5;i++) {
		yield return new WaitForSeconds(0.1f);
			GameObject x=Instantiate(Global.r_homing_missile,transform.position+transform.TransformDirection(new Vector3(-2+i,-10,10)),transform.rotation) as GameObject;
			x.GetComponent<homingMissile>().target=Global.player.transform;
			x.GetComponent<homingMissile>().enabled=true;
			x.GetComponent<homingMissile>().speed=450;
		}
		yield return new WaitForSeconds(rocket_timer);
		reloading=false;
	}

	public void ApplyDamage(Vector4 mg) {
		if (dead) return;
		Vector3 point=new Vector3(mg.x,mg.y,mg.z);
		hp-=mg.w;
		if (hp<0) {
			Global.score+=points;
			Global.SmallExplosionRequest(transform.position);
			if (hp<-100) {
				Destroy(gameObject);
			}
			else {
				dead=true;
				Destroy(sounder);
				Rigidbody r=gameObject.AddComponent<Rigidbody>();
				r.mass=70;
				r.velocity=new Vector3(0,0,speed/4);
				gameObject.tag="decoration";
				decoration d=gameObject.AddComponent<decoration>();
				d.maxhp=700;
				d.hp=700;
				d.sound=2;
				Destroy(this);
			}
		}
	}
}
