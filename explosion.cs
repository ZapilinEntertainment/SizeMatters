using UnityEngine;
using System.Collections;

public class explosion : MonoBehaviour {
	public float timer=3;
	// Use this for initialization
	void Start () {
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20);
		int i = 0;
		while (i < hitColliders.Length) {
			hitColliders[i].transform.root.SendMessage("ApplyDamage",new Vector4(transform.position.x,transform.position.y,transform.position.z,500),SendMessageOptions.DontRequireReceiver);
			i++;
		}
		StartCoroutine(Timer());
	}
	
	IEnumerator Timer() {
		yield return new WaitForSeconds(timer);
		Destroy(gameObject);
	}
}
