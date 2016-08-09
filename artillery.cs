using UnityEngine;
using System.Collections;

public class artillery : MonoBehaviour {
	public float speed=140;
	public Transform gun;
	public Transform tower;
	public ParticleSystem fire_ps;
	public bool ready=true;
	public float cooldown=30;
	public float rot_speed=30;
	public int range=3000;
	public int damage=5000;
	float d=10000;
	public float maxhp=1000;
	public float hp;
	public int points=700;
	Texture strike_point;
	Vector3 strike_position;
	bool firing=false;


	void Start () {
		hp=maxhp;
	}
	// Update is called once per frame
	void Update () {
		if (Global.pause||!Global.playable||!ready) return;
		float d=Vector3.Distance(gun.position,Global.player.transform.position)-30;
		Vector3 tf=Quaternion.LookRotation(Global.player.transform.position-tower.transform.position,Vector3.up).eulerAngles;
		tf.x=0;tf.z=0;
		tower.transform.rotation=Quaternion.RotateTowards(tower.transform.rotation,Quaternion.Euler(tf),rot_speed*Time.deltaTime);
		if (Quaternion.Angle(Quaternion.Euler(tf),tower.transform.rotation)<3&&d<range&&d>400) {
			fire_ps.Emit(25);
			if (Global.sound) Global.sm.TankShot(1);
			strike_position=Global.player.transform.position+new Vector3(Random.value*50,0,Random.value*50);
			RaycastHit rh;
			var layerMask=1<<8;
			if (Physics.Raycast(new Vector3(strike_position.x,800,strike_position.z),Vector3.down,out rh,4000,layerMask)) {strike_position.y=rh.point.y;}
			StartCoroutine(Strike(d/200));
			firing=true;
			StartCoroutine(Reloading());
		}
	}

	IEnumerator Reloading() {
		ready=false;
		yield return new WaitForSeconds(cooldown);
		ready=true;
	}
	IEnumerator Strike(float t) {
		yield return new WaitForSeconds(t);
		Collider[] cds=Physics.OverlapSphere(strike_position,12);
		foreach (Collider cd in cds) {
			cd.transform.root.SendMessage("ApplyDamage",new Vector4(strike_position.x,strike_position.y,strike_position.z,damage),SendMessageOptions.DontRequireReceiver);
		}
		Global.ArtilleryStrikeAt(strike_position);
	}

	public void ApplyDamage(Vector4 mg) {
			hp-=mg.w;
			if (hp<=0) {
				Global.ConcreteDustRequest(transform.position);
			Global.SmallExplosionRequest(transform.position);
				Global.score+=points;
				Destroy(gameObject);
	}
}
}
