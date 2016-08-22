using UnityEngine;
using System.Collections;

public class tower : MonoBehaviour {
	public float maxhp=10000;
	float hp;
	public int points=5000;
	public float tall=70;
	public GameObject upperObject;
	// Use this for initialization
	void Start () {
		hp=maxhp;
		if (transform.parent!=null) transform.parent=null;
	}


	public void ApplyDamage(Vector4 mg) {
		hp-=mg.w;
		if (hp<=0) {
			Vector3 pt=transform.position+Vector3.down*tall/2;
			while (tall>0) {
				Global.ConcreteDustRequest(pt);
				pt+=Vector3.up*10;
				tall-=10;
			}
			Global.SmallExplosionRequest(transform.position);
			Global.score+=points;
			if (upperObject!=null) Destroy(upperObject);
			Destroy(gameObject);
		}
	}
}
