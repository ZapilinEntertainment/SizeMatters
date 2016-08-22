using UnityEngine;
using System.Collections;

public class pvo_launcher : MonoBehaviour {
	public int range=2200;
	public float lift_speed=15;
	public float reload_time=25;
	public Vector3[] start_points;
	GameObject[] rockets;
	public float height=5;
	bool ready_ru=false;
	bool ready_rd=false;
	bool ready_lu=false;
	bool ready_ld=false;
	byte i=0;
	Transform tld=null;Transform trd=null;Transform tru=null;Transform tlu=null;

	void Awake () {
		if (Global.nongamescene) Destroy(this);
		rockets=new GameObject[8];
	}

	//    2  1 
	//    3  4
	// Update is called once per frame
	void Update () {
		if (Global.pause) return;
		for (i=0;i<8;i++) {
			if (rockets[i]==null) {
				rockets[i]=Instantiate(Global.r_simple_hmissile,transform.TransformPoint(start_points[i/2]+new Vector3(0,i%2,0)),transform.rotation) as GameObject;
				rockets[i].transform.parent=transform;
			}
			else {
				if (rockets[i].transform.localPosition.y<start_points[i/2].y+height-i%2) rockets[i].transform.Translate(Vector3.up*lift_speed*Time.deltaTime,Space.Self);
				else {
					switch (i/2) {
					case 1:ready_rd=true;break;
					case 2:ready_ld=true;break;
					case 3:ready_lu=true;break;
					case 4: ready_ru=true;break;
					}
				}
			}
		}
		if (Input.GetKeyDown("3")) {
			if (ready_rd) {trd=Scan(1);if (trd!=null) Fire (1);}
			if (ready_ld) {tld=Scan(2);if (tld!=null) Fire(2);}
			if (ready_lu) {tlu=Scan(3);if (tlu!=null) Fire(3);}
			if (ready_ru) {tru=Scan(4);if (tru!=null) Fire(4);}
		}
	}

	Transform Scan(byte id) {
		Collider[] cds=Physics.OverlapSphere(transform.position,range);
		if (cds.Length==0) return(null);
		Vector3 inpos=Vector3.zero;
		float mindist=range;
		Transform pt=null;
		Transform ctr=null;
		foreach (Collider cd in cds) {
			ctr=cd.transform.root;
			inpos=transform.InverseTransformPoint(ctr.position);
			if (ctr.GetComponent<batcopter>()==null&&ctr.GetComponent<plane>()==null||inpos.z<0) continue;
			if ((id==1||id==4)&&inpos.x<0||(id==2||id==3)&&inpos.x>0) continue;
			if (inpos.magnitude<mindist) {
				if (ctr==trd||ctr==tru||ctr==tlu||ctr==tld) continue;
				mindist=inpos.magnitude;
				pt=ctr;
			}
		}
		return (pt);
	}



	void Fire(byte id) {
		simple_hmissile hmc1=null;
		simple_hmissile hmc2=null;
		switch (id) {
		case 1:
			if (trd==null||!ready_rd) return;
			hmc1=rockets[0].GetComponent<simple_hmissile>();
			hmc2=rockets[1].GetComponent<simple_hmissile>();
			hmc1.target=trd;
			hmc2.target=trd;
			trd=null;
			ready_rd=false;
			break;
		case 2:
			if (tld==null||!ready_ld) return;
			hmc1=rockets[2].GetComponent<simple_hmissile>();
			hmc2=rockets[3].GetComponent<simple_hmissile>();
			hmc1.target=tld;
			hmc2.target=tld;
			tld=null;
			ready_ld=false;
			break;
		case 3:
			if (tlu==null||!ready_lu) return;
			hmc1=rockets[4].GetComponent<simple_hmissile>();
			hmc2=rockets[5].GetComponent<simple_hmissile>();
			hmc1.target=tlu;
			hmc2.target=tlu;
			tlu=null;
			ready_lu=false;
			break;
		case 4:
			if (tru==null||!ready_ru) return;
			hmc1=rockets[6].GetComponent<simple_hmissile>();
			hmc2=rockets[7].GetComponent<simple_hmissile>();
			hmc1.target=tru;
			hmc2.target=tru;
			tru=null;
			ready_ru=false;
			break;
		}
		hmc1.enabled=true;
		hmc1.gameObject.transform.parent=null;
		hmc2.enabled=true;
		hmc2.gameObject.transform.parent=null;
	}




}
