using UnityEngine;
using System.Collections;

public class catanks_base : MonoBehaviour {
	public GameObject[] enemies;
	public Material[] materials;

	// Use this for initialization
	void Awake () {
		materials=new Material[6];
		materials[0]=Resources.Load<Material>("Materials/plasma_ray");
		for(byte i=0;i<5;i++) {
			materials[i+1]=Resources.Load<Material>("Materials/catank"+i.ToString());
		}

	}

	void Start () {
		StartCoroutine(Scan());	
	}

	IEnumerator Scan() {
		enemies=GameObject.FindGameObjectsWithTag("unit");
		yield return new WaitForSeconds(2);
		StartCoroutine(Scan());
	}

	public Transform GiveMeATarget(Vector3 pos) {
		if (enemies==null) return null;
		int mindist=10000;
		Transform pt=null;
		foreach (GameObject x in enemies) {
			if (x==null) continue;
			if (Vector3.Distance(x.transform.position,pos)<=mindist) {
				pt=x.transform;
				mindist=(int)Vector3.Distance(x.transform.position,pos);
			}
		}
		return (pt);
	}
}
