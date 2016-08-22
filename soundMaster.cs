using UnityEngine;
using System.Collections;

public class soundMaster : MonoBehaviour {
	public bool use_sound=true;
	public AudioClip step_sound_r;
	public AudioClip step_sound_l;
	public AudioClip rightgun_sound;
	public AudioClip leftgun_sound;
	public AudioClip tank_shot;
	public AudioClip tank_crunch;
	public AudioClip cabin_rotation;
	public AudioClip artillery_shot;
	public AudioClip pvogun;
	public AudioClip chopper;
	public AudioClip[] explosions;
	public AudioClip[] hits;
	public bool r_gun_dominating=false;
	public bool l_gun_dominating=false;

	public AudioSource as_mech_right;
	byte mright_clip=0;
	public AudioSource as_mech_left;
	byte mleft_clip=0; //0-nothing, 1- legs moving,2-hits
	public AudioSource as_mech_down;
	public float mech_down_volume=0;
	public float mech_down_clip=0;
	public AudioSource as_gunright;
	byte gunright_clip=0; //0-nothing,1-gun, 2- otherwise
	float gunright_volume=0;
	public AudioSource as_gunleft;
	byte gunleft_clip=0;
	float gunleft_volume=0;
	public AudioSource as_out_fwd;
	byte out_fwd_clip=0; //1-tankshots,2-our artillery,3-explosion;
	public float out_fwd_t=0;
	public AudioSource as_out_back;
	public byte out_back_clip=0; //1-tankshots,2-pvogun;
	public float out_back_t=0;
	public bool outside_off=false;
	public AudioSource as_out_down;
	byte out_down_clip=0; //1-tank crunch
	float out_down_t=0;
	public float hit_timer=0;
	public bool pause=false;

	void Awake() {
		Global.sm=this;
	}


	public void Silence (bool x) {
		if (x) {
			as_mech_right.enabled=false;
			as_mech_left.enabled=false;
			as_mech_down.enabled=false;
			as_gunright.enabled=false;
			as_gunleft.enabled=false;
			as_out_fwd.enabled=false;
			as_out_back.enabled=false;
			as_out_down.enabled=false;
		}
		else {
			as_mech_right.enabled=true;
			as_mech_left.enabled=true;
			as_mech_down.enabled=true;
			as_gunright.enabled=true;
			as_gunleft.enabled=true;
			as_out_fwd.enabled=true;
			as_out_back.enabled=true;
			as_out_down.enabled=true;
		}
	}
	//Brother I am bind here!
	//No!
	//I am bind here!
	//No!
	public void BrotherIAmHit(Vector3 pos) {
		if (hit_timer!=0||!Global.sound) return;
		if (Global.player.transform.InverseTransformPoint(pos).x>0) {
			as_mech_right.clip=hits[Mathf.RoundToInt(Random.value*(hits.Length-1))];
			mright_clip=2;
			as_mech_right.Play();
		}
		else {
			as_mech_left.clip=hits[Mathf.RoundToInt(Random.value*(hits.Length-1))];
			mleft_clip=2;
			as_mech_left.Play();
		}
		hit_timer=0.15f;
	}

	public void PvoFire (bool x) {
		if (!Global.sound) return;
		if (x) {
			out_back_clip=2;
			as_out_back.clip=pvogun;
			as_out_back.loop=true;
			as_out_back.Play();
			out_back_t=0;
		}
		else {
			out_back_clip=0;
			as_out_back.loop=false;
			as_out_back.Stop();
		}
	}

	public void Explosion (Vector3 pos) {
		if (outside_off||!Global.sound) return;
		if (Global.cam.transform.InverseTransformPoint(pos).z>0) {
			if (out_fwd_clip<2) {
				out_fwd_clip=3;
				as_out_fwd.clip=explosions[Mathf.RoundToInt(Random.value*(explosions.Length-1))];
				as_out_fwd.loop=false;
				as_out_fwd.Play();
				out_fwd_t=0.5f;
			}
		}
		else {
			if (out_back_clip<2) {
				out_back_clip=3;
				as_out_back.clip=explosions[Mathf.RoundToInt(Random.value*(explosions.Length-1))];
				as_out_back.loop=false;
				as_out_back.Play();
				out_back_t=0.5f;
			}
		}
	}

	public void CabinRotation (bool x) {
		if (!Global.sound) return;
		if (x) {
			if (mech_down_clip!=0) return;
		as_mech_down.clip=cabin_rotation;
		as_mech_down.loop=true;
		as_mech_down.Play();
			mech_down_clip=1;
		}
		else {
			mech_down_clip=0;
			as_mech_down.Stop();
		}
	}

	public void MyArtShot() {
		if (!Global.sound) return;
		as_out_fwd.clip=artillery_shot;
		out_fwd_clip=2;
		as_out_fwd.Play();
		out_fwd_t=1;
	}

	public void TankCrunch () {
		if (!Global.sound) return;
		if (!use_sound||out_down_t>=0.9f) return;
		as_out_down.clip=tank_crunch;
		as_out_down.Play();
		out_down_clip=1;
		out_down_t=1;
	}

	public void TankShot (byte x) {
		if (!Global.sound) return;
		if (!use_sound||outside_off) return;
		switch (x) {
		case 1://forward
			if (out_fwd_t>0.5f) return;
			as_out_fwd.clip=tank_shot;
			as_out_fwd.Play();
			out_fwd_t=1;
			break;
		case 2://back
			if (out_back_t>0.5f||out_back_clip==2) return;
			as_out_back.clip=tank_shot;
			as_out_back.Play();
			out_back_t=1;
			break;
		}
	}

	public void StepSound(Vector3 pos,bool left) {
		if (!use_sound||!Global.sound) return;
		if (left) {
				mleft_clip=1;
				as_mech_left.PlayOneShot(step_sound_l,1);
		}
		else {
				mright_clip=1;
				as_mech_right.clip=step_sound_r;
				as_mech_right.PlayOneShot(step_sound_r,1);
		}
	}

	public void GunSound(bool right,bool gun,bool start) {
		if (!use_sound||!Global.sound) return;
		if (right) {
				if (start) {
				gunright_clip=1;
				as_gunright.clip=rightgun_sound;
				as_gunright.loop=true;
				as_gunright.Play();
				if (r_gun_dominating) outside_off=true;
				}
				else {
					gunright_clip=0;
				if (r_gun_dominating) outside_off=false;
				}
		}
		else {
				if (start) {
					gunleft_clip=1;
					as_gunleft.clip=leftgun_sound;
					as_gunleft.loop=true;
					as_gunleft.Play();
				if (l_gun_dominating) outside_off=true;
				}
				else {					
					gunleft_clip=0;
				if (l_gun_dominating) outside_off=false;
				}
		}
	}
		

	void Update () {
		if (pause!=Global.pause) {
			if (Global.pause) {
				pause=true;
				as_mech_right.enabled=false;
				as_mech_left.enabled=false;
				as_mech_down.enabled=false;
				as_out_fwd.enabled=false;
				as_out_back.enabled=false;
				as_out_down.enabled=false;
				as_gunright.enabled=false;
				as_gunleft.enabled=false;
			}
			else {
				pause=false;
				as_mech_right.enabled=true;
				as_mech_left.enabled=true;
				as_mech_down.enabled=true;
				as_out_fwd.enabled=true;
				as_out_back.enabled=true;
				as_out_down.enabled=true;
				as_gunright.enabled=true;
				as_gunleft.enabled=true;
			}
		}
		if (Global.pause) return;
		float t=Time.deltaTime;
		if (hit_timer>0) {
			hit_timer-=t;
			if (hit_timer<0) hit_timer=0;
		}
		if (mech_down_clip!=0) {
			if (mech_down_volume<1) {
			mech_down_volume+=3*t;
			as_mech_down.volume=mech_down_volume;
			}
		}
		else {
			if (mech_down_volume>0) {
			mech_down_volume-=3*t;
			if (mech_down_volume<0) mech_down_volume=0;
			as_mech_down.volume=mech_down_volume;
			}
		}
		if (out_down_t>0) {out_down_t-=t;if (out_down_t<0) {out_down_t=0;out_down_clip=0;}}
		if (out_fwd_t>0) {out_fwd_t-=t;if (out_fwd_t<0) {out_fwd_t=0;out_fwd_clip=0;}}
		if (out_back_t>0) {out_back_t-=t;if (out_back_t<0) {out_back_t=0;out_back_clip=0;}}
		if (gunright_clip==1&&gunright_volume<1) {
			gunright_volume+=3*t;
			as_gunright.volume=gunright_volume;
		}
		if (gunright_clip==0&&gunright_volume>0) {
			gunright_volume-=3*t;
			if (gunright_volume<=0) 
			{gunright_volume=0;
				as_gunright.loop=false;
				as_gunright.Stop();
			}
			as_gunright.volume=gunright_volume;
		}
		if (gunleft_clip==1&&gunleft_volume<1) {
			gunleft_volume+=3*t;
			as_gunleft.volume=gunleft_volume;
		}
		if (gunleft_clip==0&&gunleft_volume>0) {
			gunleft_volume-=3*t;
			if (gunleft_volume<=0) {
				gunleft_volume=0;
				as_gunleft.loop=false;
				as_gunleft.Stop();
			}
			as_gunleft.volume=gunleft_volume;
		}
	}
}
