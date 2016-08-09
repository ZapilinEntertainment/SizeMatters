using UnityEngine;
using System.Collections;

public class art_projectile : MonoBehaviour {
	public float speed=200;
	public float damage=1500;
	public Rigidbody rbody;

	void Start () {
		rbody=gameObject.GetComponent<Rigidbody>();
	}
	// Update is called once per frame
	void Update () {
		//transform.Translate(new Vector3(0,0,speed*Time.deltaTime),Space.Self);
		if (transform.position.y<-200) {
			rbody.velocity=Vector3.zero;
			rbody.angularVelocity=Vector3.zero;
			gameObject.SetActive(false);
		}
	}

	void OnCollisionEnter (Collision c) {
		Collider[] cds=Physics.OverlapSphere(transform.position,30);
		foreach (Collider cd in cds) {
			cd.transform.root.SendMessage("ApplyDamage",new Vector4(transform.position.x,transform.position.y,transform.position.z,damage),SendMessageOptions.DontRequireReceiver);
		}
		Global.SmallExplosionRequest(transform.position);
		rbody.velocity=Vector3.zero;
		rbody.angularVelocity=Vector3.zero;
		gameObject.SetActive(false);
	}
}
