using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour {
	static public Main S; //singleton for main
	static Dictionary <WeaponType, WeaponDefinition> WEAP_DICT;

	[Header("Set in Inspector")]
	public GameObject[] prefabEnemies; //an array of enemy prefabs
	public float enemySpawnPerSecond = 0.5f; // # Enemies/second
	public float enemyDefaultPadding = 1.5f; // Padding for position
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public Text uitLevel; // The GT_Level GUIText
	public Text uitScore; // The GT_Score GUIText

	public WeaponType[] powerUpFrequency = new WeaponType[] { 
		WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield,  WeaponType.laser
	};

	private BoundsCheck bndCheck;

	[Header("Set Dynamically")]
	public int level;
	public int Totalscore;

	public void shipDestroyed(Enemy e){
		
		if (Random.value <= e.powerUpDropChance) { // d
			// Choose which PowerUp to pick
			// Pick one from the possibilities in powerUpFrequency
			int ndx = Random.Range(0,powerUpFrequency.Length); 
			WeaponType puType = powerUpFrequency[ndx];

			// Spawn a PowerUp
			GameObject go = Instantiate( prefabPowerUp ) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp>();

			// Set it to the proper WeaponType
			pu.SetType( puType ); // f
			// Set it to the position of the destroyed ship
			pu.transform.position = e.transform.position;
		}
	}
		
	void Awake () {
		S = this;
		//set bndCheck to reference the BoundsCheck component on this GameObject
		bndCheck = GetComponent<BoundsCheck>();
		//invoke SpawnEnemy() once (in 2 seconds, based on default values)
		Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);

		WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition> ();
		foreach(WeaponDefinition def in weaponDefinitions){
			WEAP_DICT[def.type] = def;
		}
	}
	
	public void SpawnEnemy(){
		//Pick a random Enemy prefab to instantiate
		int ndx = Random.Range(0, level*2); //the range of enemy types scales based on the level
		print("enemy #: " + ndx);
		GameObject go = Instantiate<GameObject> (prefabEnemies [ndx]);

		//position the enemy above the screen with a random x position
		float enemyPadding = enemyDefaultPadding;
		if (go.GetComponent<BoundsCheck> () != null) {
			enemyPadding = Mathf.Abs (go.GetComponent<BoundsCheck>().radius);
		}

		//set the initial position for the spawned enemy
		Vector3 pos = Vector3.zero;
		float xMin = -bndCheck.camWidth + enemyPadding;
		float xMax = bndCheck.camWidth - enemyPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = bndCheck.camHeight + enemyPadding;
		go.transform.position = pos;

		//invoke SpawnEnemy() again
		Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);

	}

	public void DelayedRestart(float delay){
		//invoke the restart() method in delay seconds
		Invoke("Restart", delay);
	}

	public void Restart(){
		//reload _Scene_0 to restart the game 
		SceneManager.LoadScene ("_Scene_0");
		Totalscore = 0;
		level = 1;
		UpdateLevel ();

	}
	/// <summary>
	/// Static function that gets a WeaponDefinition from the WEAP_DICT static
	/// protected field of the Main class.
	/// </summary>
	/// <returns>The WeaponDefinition or, if there is no WeaponDefinition with
	/// the WeaponType passed in, returns a new WeaponDefinition with a
	/// WeaponType of none..</returns>
	/// <param name="wt">The WeaponType of the desired WeaponDefinition</param>
	static public WeaponDefinition GetWeaponDefinition( WeaponType wt )
	{ 
		// Check to make sure that the key exists in the Dictionary
		// Attempting to retrieve a key that didn't exist, would throw an error,
		// so the following if statement is important.
		if (WEAP_DICT.ContainsKey(wt))
		{ 
			return( WEAP_DICT[wt] );
		}
		// This returns a new WeaponDefinition with a type of WeaponType.none,
		// which means it has failed to find the right WeaponDefinition
		return( new WeaponDefinition() ); // c
	}

	public void UpdateGUI( ){
		Totalscore += 100;
		uitScore.text = "Score: " + Totalscore;
		if (Totalscore == 2000) {
			level++;
			UpdateLevel ();
		}
	}
	public void UpdateLevel(){
		Totalscore = 0;
		if (level > 5) {
			Restart ();
		}
		uitLevel.text = "Level: " + level + " of 5";
	}
}
