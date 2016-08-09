using UnityEngine;
using System.Collections;

public class homingMissile : MonoBehaviour {
	public Transform target;
	public float rotation_speed=60;
	public float speed=300;
	public float timer=15;
	public float damage=350;

	void Start () {
		TrailRenderer tr=gameObject.GetComponent<TrailRenderer>();
		if (tr) tr.enabled=true;
		GetComponent<Collider>().enabled=true;
	}

	void Update () {
		if (Global.pause) return;
		timer-=Time.deltaTime;
		if (timer<=0) {
			Global.SmallExplosionRequest(transform.position);
			Destroy(gameObject);
		}
		if (target!=null)	{
			transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(target.position-transform.position),rotation_speed*Time.deltaTime);
			if (Vector3.Distance(transform.position,target.position)<speed*Time.deltaTime) {
				target.root.SendMessage("ApplyDamage",new Vector4(transform.position.x,transform.position.y,transform.position.z,damage),SendMessageOptions.DontRequireReceiver);
				Global.SmallExplosionRequest(transform.position);
				Destroy(gameObject);
			}
		}
		transform.Translate(new Vector3(0,0,speed*Time.deltaTime),Space.Self);

	}

	void OnCollisionEnter(Collision c) {
		if (c.collider.GetComponent<homingMissile>()==null) {
			c.collider.transform.root.SendMessage("ApplyDamage",new Vector4(transform.position.x,transform.position.y,transform.position.z,damage),SendMessageOptions.DontRequireReceiver);
			Global.SmallExplosionRequest(transform.position);
			Destroy(gameObject);
		}
	}
}
