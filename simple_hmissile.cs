using UnityEngine;
using System.Collections;

public class simple_hmissile : MonoBehaviour {
	public float speed=200;
	public float damage=2000;
	public float timer=30;
	public float rotation_speed=30;
	public Transform target;
	public TrailRenderer tr1;
	public TrailRenderer tr2;
	RaycastHit rh;
	// Update is called once per frame
	void Start() {
		if (tr1) tr1.enabled=true;
		if (tr2) tr2.enabled=true;
		StartCoroutine(Timer());
	}

	void Update () {
		if (target!=null)	{
			transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(target.position-transform.position),rotation_speed*Time.deltaTime);
		}
		if (Physics.Raycast(transform.position,transform.forward,out rh,speed*Time.deltaTime)) {
			Global.SmallExplosionRequest(rh.point);
			Collider[] cds=Physics.OverlapSphere(rh.point,10);
			foreach (Collider cd in cds) {
				cd.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,damage),SendMessageOptions.DontRequireReceiver);
			}
			Destroy(gameObject);
		}
		else transform.Translate(Vector3.forward*speed*Time.deltaTime,Space.Self);
	}

	IEnumerator Timer() {
		yield return new WaitForSeconds(timer);
		Destroy(gameObject);
	}
}
