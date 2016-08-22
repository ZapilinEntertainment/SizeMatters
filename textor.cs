using UnityEngine;
using System.Collections;

public class textor : MonoBehaviour {
	public byte language=0; 
	//0-eng
	//1-ru
	public string m1_briefing="";
	public string m2_briefing="";
	public string m3_briefing="";
	public string m4_briefing="";
	public string m5_briefing="";
	public string m6_briefing="";
	public string m7_briefing="";
	public string m8_briefing="";
	public string m9_briefing="";
	public string m10_briefing="";

	public string data_clean_button="clean";
	public string graphics_quality_button="graphics quality";
	public string step_traces_button="step drawing";
	public string sound_button="sound";
	public string try_again_button="repeat";
	public string to_menu_button="main menu";
	public string to_hangar_button="back to hangar";
	public string dont_stop_button="NEXT!";
	public string continue_button="continue";
	public string settings_button="settings";
	public string language_button="language";
	public string clear_request_label="Really clear data?";

	public string plasmagun_name="plasmagun";
	public string chaingun_name="chaingun";
	public string rlauncher_name="rocket launcher";
	public string pvogun_name="air defence gun";
	public string pvorockets_name="air defence rockets";
	public string shield_gen_name="shield_generator";
	public string artillery_name="artillery";
	public string sh_rockets="rocket launchers";

	public Texture help_tx;

	// Hi, I am Textor!

	void Awake() {
		Global.txtr=this;
	}

	public void SetLanguage(byte l) {
		switch (l) {
		case 0:
			m1_briefing="Welcome GrandPilot! You have been chosen to control our new experimental warmachine." +
				"Unfortunately, our training center is under attack and we hope you can get to work right now. Good luck!";
			m2_briefing="Yes, you come to this position. Now, more to the point. The enemy trying to establish a new base on the reach of this desert so " +
				"your next quest is to startle enemy forces and demolish all that you will find. Get ready!";
			m3_briefing="Our forces need your support for mountain citadel breaching. Victory give us access to make a cleaning operation in mountain dales," +
				"where the enemy helicopter forces is basing. Prepare for hard resist.";
			m4_briefing="Pilot! Today you must destroy all enemy nests. You warmachine can climb on any height, you can use this to gain an advantage and take a convenient position." +
				" After finishing your work head for the exit of the dale for further instructions";
			m5_briefing="Our next targets are three oil platforms, which supplies all nearest mouse fleets. By breaking them down, " +
				"we will force our enemy to save fuel and make most of his fleet uncapacitate. Be careful, enemy air forces detected in action area!";
			m6_briefing="Scouts founded closed enemy science center to the north. Probably mouse military forces are coming and underground passages for enginery preparing. " +
				"Also, we dont know exactly what technologies is using for complex protection. But, anyway this is not important - you must immolate three main buildings and leave area. Proceed.";
			m7_briefing="Enemy forces preparing for a massive srike and getting together in a fortificated factory complex. Attack them and we will damage our enemy so hard that he cannot stand again. Burn all factory building to ashes! Your time has come, sir pilot!";
			m8_briefing="Mouse nation finally agreed to talks. The most important battle will take place in our holy area - in Great Box of our ancestors. Today you will lead our forces to victory - and this is a great honour!";

			data_clean_button="Clear game data";
			graphics_quality_button="Graphics Quality";
			step_traces_button="Draw mech steps?";
			sound_button="Sound";
			try_again_button="Restart!";
			to_menu_button="To main menu";
			to_hangar_button="Back to hangar";
			dont_stop_button="Do not stop";
			continue_button="Continue";
			settings_button="Settings";
			language_button="Set language";
			clear_request_label="Clear mech configuration, walkthrough data and settings? Your slippers may suffer in process!";

			plasmagun_name="Heavy plasmagun";
			chaingun_name="Heavy chaingun";
			rlauncher_name="Heavy rocket launcher";
			pvogun_name="Air defence gun";
			pvorockets_name="Air defence rockets";
			shield_gen_name="Shield generator";
			artillery_name="Artillery tower";
			sh_rockets="Rocket launchers";

			help_tx=Resources.Load<Texture>("help_eng");
			break;
		case 1:
			m1_briefing="Приветствую, мастер-пилот! Вы были избраны для управления нашей новой экспериментальной боевой машиной." +
				"К несчастью, наш центр подготовки атакован, и мы очень надеемся, что вы сможете приступить к работе прямо сейчас. Удачи!";
			m2_briefing="Да, вы нам подходите. А теперь ближе к делу. Враг пытается основать базу на границе пустыни, " +
				"а потому вашим следующим заданием будет обратить вражеские силы в бегство и разрушить все, что попадется вам на глаза. Приготовьтесь!";
			m3_briefing="Наши войска нуждаются в вашей поддержке для прорыва обороны горной цитадели. Победа даст нам возможность провести операцию зачистки в горных долинах, " +
				"где базируется вражеская вертолетная авиация. Приготовьтесь к ожесточенному сопротивлению.";
			m4_briefing="Пилот, сегодня ваша задача - уничтожение вражеских гнезд. Ваша машина  может вскарабкаться на любой склон, вы можете использовать это преимущество, " +
				"чтобы занять выгодную позицию. После зачистки направляйтесь к выходу из долины, там вас введут в дальнейший курс действий.";
			m5_briefing="Наши следующие цели - три нефтяные платформы, которые снабжают топливом все ближайшие флоты мышей. Уничтожив их, " +
				"мы заставим врага экономить топливо и временно выведем большую часть его флота из игры. Осторожно, в районе действий замечена авиация противника!";
			m6_briefing="Разведка обнаружила закрытый научно-исследовательский комплекс севернее от вашей позиции. Вероятно, туда уже стягиваются войска и налаживаются " +
				"подземные путепроводы для техники. Кроме того, мы не знаем, какие технологии используются для защиты комплекса. Но это неважно - просто испепелите три основных здания лаборатории и покиньте комплекс. Приступайте.";
			m7_briefing="Вражеские силы собираются нанести массированный удар по нашим позициям и собираются в укрепленном производственном комплексе. Атаковав их сейчас, мы нанесем врагу ущерб, от которого он уже не оправится. Сожгите дотла всю фабрику! Ваш выход, господин пилот!";
			m8_briefing="Государство мышей наконец согласилось на переговоры. Решающая битва этой войны произойдет в священном месте - в Великой Коробке наших предков. Сегодня вы ведете наши войска  к победе - и это великая честь!";

			data_clean_button="Сбросить данные";
			graphics_quality_button="Качество графики";
			step_traces_button="Следы вашей машины";
			sound_button="Звуки";
			try_again_button="Перезапуск миссии!";
			to_hangar_button="Вернуться в ангар";
			to_menu_button="Вернуться в меню";
			dont_stop_button="Не останавливаться";
			settings_button="Настройки";
			continue_button="Продолжить";
			language_button="Сменить язык";
			clear_request_label="Удалить обвес боевой машины, данные о прохождении игры и все установки? Ваши тапки могут пострадать в процессе!";

			plasmagun_name="Тяжелая плазма";
			chaingun_name="Осадный пулемет";
			rlauncher_name="Ракетный комплекс";
			pvogun_name="Зенитный пулемет";
			pvorockets_name="Ракеты земля-воздух";
			shield_gen_name="Усилитель щита";
			artillery_name="Орудийная башня";
			sh_rockets="Пусковая установка";

			help_tx=Resources.Load<Texture>("help_ru");
			break;
		}
		string s;
		if (PlayerPrefs.HasKey("gdata")) {
			if (l>10) l=(byte)(l%10);
			s=PlayerPrefs.GetString("gdata");
			if (s.Length>1) PlayerPrefs.SetString("gdata",l.ToString()+s.Substring(1,s.Length-1));
			else PlayerPrefs.SetString("gdata",l.ToString());
			}
		else {
			PlayerPrefs.SetString("gdata",l.ToString());
		}
		}
}
