using UnityEngine;
using System.Collections;

public class batcopter : MonoBehaviour {
	public float speed=120;
	public float acceleration=3;
	float cspeed=0;
	float goal=0;
	public int attack_range=700;
	public float damage=150;
	public float rocket_timer=8;
	public bool attacking=false;
	public float maxhp=500;
	public float hp;
	public GameObject attacking_sprite;
	bool dead=false;
	public int points=1000;
	AudioSource sounder;
	bool pause=false;

	void Start() {
		hp=maxhp;
		sounder=gameObject.AddComponent<AudioSource>();
		sounder.clip=Global.sm.chopper;
		sounder.loop=true;
		sounder.volume=0.1f;
		sounder.Play();
	}
	// Update is called once per frame
	void Update () {
		if (Global.pause) {
			if (pause==false) {
				pause=true;
				sounder.enabled=false;
			}
			return;
		}
		else {
			if (pause) {
				pause=false;
				sounder.enabled=true;
			}
		}
		if (!dead&&Global.player) {
		int d=(int)Vector3.Distance(transform.position,Global.player.transform.position);
		if (d<attack_range) {
			if (!attacking) {
				attacking=true;
				StopCoroutine(RocketTime());
				StartCoroutine(RocketTime());
			}
			else {
				if (transform.InverseTransformPoint(Global.player.transform.position).z>0) 
					attacking_sprite.SetActive(true);
				}
			if (d<100) goal=-speed;
			}
		else {
			if (attacking) {
				attacking=false;
				StopCoroutine(RocketTime());
				attacking_sprite.SetActive(false);
			}
			goal=speed;
		}
		RaycastHit rh;
		float h=transform.position.y;
		if (Physics.Raycast(transform.position,Vector3.down,out rh,500)) {
			if ((transform.position.y-rh.point.y)<200) transform.Translate(Vector3.up*30*Time.deltaTime);
			h=rh.point.y-transform.position.y;
		}
		else {
			if (transform.position.y>100) transform.Translate(Vector3.down*30*Time.deltaTime);
		}
		if (goal!=0) {
			if (goal>cspeed) cspeed+=acceleration*Time.deltaTime;
			if (goal<cspeed) cspeed-=acceleration*Time.deltaTime;
			transform.Translate(Vector3.forward*cspeed*Time.deltaTime,Space.Self);
		}
		transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(Global.player.transform.position-transform.position),speed/2*Time.deltaTime);
	}
		else {
			if (transform.position.y<-200) Destroy(gameObject);
		}
	}

	IEnumerator RocketTime() {
		yield return new WaitForSeconds(rocket_timer);
		if (Random.value>0.5f&&transform.InverseTransformPoint(Global.player.transform.position).z>0) {
			GameObject x=Instantiate(Global.r_homing_missile,transform.position+transform.TransformDirection(new Vector3(0,0,10)),transform.rotation) as GameObject;
			x.GetComponent<homingMissile>().target=Global.player.transform;
			x.GetComponent<homingMissile>().enabled=true;
		}
		if (attacking) {
			StartCoroutine(RocketTime());
		}
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
				Destroy(attacking_sprite);
				dead=true;
				Destroy(sounder);
				Rigidbody r=gameObject.AddComponent<Rigidbody>();
				r.mass=30;
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
