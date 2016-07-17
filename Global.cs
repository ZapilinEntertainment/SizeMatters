using UnityEngine;
using System.Collections;

public static class Global {
	public static GameObject cam;
	public static GameObject player;
	public static int gui_piece;
	public static bool pause;
	public static Vector3 aim;

	public static GameObject[] r_shot_effect;
	public static GameObject[] r_shot_expl_effect;
	public static GameObject[] r_dust_effect;
	public static GameObject r_flatten_tank0;
	public static GameObject r_flatten_tree;
	public static GameObject r_fired_place;
	static ushort shot_i=0;
	static ushort shot_expl_i=0;
	static ushort dust_i=0;

	public static void DustEffectRequest(Vector3 pos) {
		r_dust_effect[dust_i].transform.position=pos;
		r_dust_effect[dust_i].SetActive(true);
		r_dust_effect[dust_i].SendMessage("Reboot",SendMessageOptions.DontRequireReceiver);
		dust_i++;
		if (dust_i>=r_dust_effect.Length) dust_i=0;
	}

	public static void ShotEffectRequest(Vector3 pos) {
		r_shot_effect[shot_i].transform.position=pos;
		r_shot_effect[shot_i].SetActive(true);
		r_shot_effect[shot_i].SendMessage("Reboot",SendMessageOptions.DontRequireReceiver);
		shot_i++;
		if (shot_i>=r_shot_effect.Length) shot_i=0;
	}

	public static void ShotExplEffectRequest(Vector3 pos) {
		r_shot_expl_effect[shot_expl_i].transform.position=pos;
		r_shot_expl_effect[shot_expl_i].SetActive(true);
		r_shot_expl_effect[shot_expl_i].SendMessage("Reboot",SendMessageOptions.DontRequireReceiver);
		shot_expl_i++;
		if (shot_expl_i>=r_shot_expl_effect.Length) shot_expl_i=0;
	}
}
