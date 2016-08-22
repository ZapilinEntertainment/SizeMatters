using UnityEngine;
using System.Collections;

public class pvogun : MonoBehaviour {
	public int range=700;
	public float damage=600;
	public float damage_radius=3;
	public float rot_speed=60;
	public Transform gunwheel;
	public ParticleSystem fire_ps;
	public Transform target;
	public GameObject splash;
	bool emit=false;
	float t=0;
	// Use this for initialization
	void Awake() {
		if (Global.nongamescene) Destroy(this);
	}
	void Start () {
		StartCoroutine(Scan());
	}

	IEnumerator Scan() {
		yield return new WaitForSeconds(1);
		Collider[] ct=Physics.OverlapSphere(transform.position,range);
		Transform ve=null;
		if (ct.Length!=0) {
			Vector3 inpoint=Vector3.zero;
			float mindist=range;
			foreach (Collider c in ct) {
				if (c.tag=="unit"&&(c.transform.root.GetComponent<batcopter>()||c.transform.root.GetComponent<plane>())) {
					inpoint=transform.InverseTransformPoint(c.transform.position);
					if (inpoint.y>0&&inpoint.magnitude<mindist) {
						ve=c.transform;
					}
				}
			}
			}
		if (ve!=null) target=ve;
		StartCoroutine(Scan());
	}


	public void Death() {
		Global.sm.PvoFire(false);
	}

	// Update is called once per frame
	void Update () {
		if (Global.pause) return;
		if (target) {
			if (transform.InverseTransformPoint(target.position).y<=0||target.tag!="unit"||Vector3.Distance(transform.position,target.position)>range) {target=null;return;}	
			if (!emit) {
				emit=true;
				if (Global.sm&&Global.sound&&Global.sm.out_back_clip!=2) {
					Global.sm.PvoFire(true);
				}
				fire_ps.Play();
			}
			Vector3 lv=Quaternion.LookRotation(target.position-transform.position,Vector3.up).eulerAngles;
			lv.x=0;lv.z=0;
			transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.Euler(lv),rot_speed*Time.deltaTime);
			gunwheel.LookAt(target);
			RaycastHit rh;
			var layerMask = 1 << 10;
			layerMask = ~layerMask;
			if (Physics.SphereCast(transform.position,damage_radius,gunwheel.forward,out rh,range,layerMask)) {
				if (t==0) {Instantiate(splash,rh.point,Quaternion.identity);t=1;if (rh.collider.tag=="unit") {rh.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,damage*Time.deltaTime),SendMessageOptions.DontRequireReceiver);}}
			}
			else {
				if (t==0) {Instantiate(splash,transform.position+transform.forward*range,Quaternion.identity);t=1;}
			}
		}
		else {
			if (emit) {
				emit=false;
				fire_ps.Stop();
				if (Global.sm&&Global.sm.out_back_clip==2) {
					Global.sm.PvoFire(false);
				}
			}
			transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(transform.forward,Vector3.up),rot_speed*Time.deltaTime);
			gunwheel.transform.rotation=Quaternion.RotateTowards(gunwheel.transform.rotation,Quaternion.LookRotation(transform.forward,Vector3.up),rot_speed*Time.deltaTime);
		}
		if (t>0) {t-=Time.deltaTime;if(t<0) t=0;}
	}
}
