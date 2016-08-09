using UnityEngine;
using System.Collections;

public class rocket : MonoBehaviour {
	public Vector3 speed;
	public float damage=500;
	public float timer=30;
	// Use this for initialization
	void Update () {
		transform.Translate(speed*Time.deltaTime,Space.Self);
		timer-=Time.deltaTime;
		if (timer<=0) {
			Global.SmallExplosionRequest(transform.position);Destroy(gameObject);
			Collider[] cds=Physics.OverlapSphere(transform.position,10);
			foreach (Collider cd in cds) {
				cd.transform.root.SendMessage("ApplyDamage",new Vector4(transform.position.x,transform.position.y,transform.position.z,damage),SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void OnCollisionEnter (Collision c) {
		Collider[] cds=Physics.OverlapSphere(transform.position,10);
		foreach (Collider cd in cds) {
		cd.transform.root.SendMessage("ApplyDamage",new Vector4(transform.position.x,transform.position.y,transform.position.z,damage),SendMessageOptions.DontRequireReceiver);
		}
			Global.SmallExplosionRequest(transform.position);
		Destroy(gameObject);
	}
}
