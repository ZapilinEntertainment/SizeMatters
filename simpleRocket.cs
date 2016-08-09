using UnityEngine;
using System.Collections;

public class simpleRocket : MonoBehaviour {
	public float speed=200;
	public float damage=200;
	public float timer=15;
	RaycastHit rh;
	// Update is called once per frame
	void Start() {
		StartCoroutine(Timer());
	}

	void Update () {
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
