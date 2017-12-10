using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Hero : MonoBehaviour {
	static public Hero S;


	[Header ("Set in Inspector")]
	//these fields control the movement of the ship
	public float speed = 30;
	public float rollMult = 45;
	public float pitchMult = 30;
	public float gameRestartDelay = 2f;
	public GameObject projectilePrefab;
	public float projectileSpeed = 40;
	public Weapon[] weapons;

	[Header ("Set Dynamically")]
	private GameObject lastTriggerGo = null;
	public Text uitAmmo;
	public int current_ammo; // the current ammo of the equipped weapon
	//public int max_ammo; //ammo capacity of the equipped weapon

	// Declare a new delegate type WeaponFireDelegate
	public delegate void WeaponFireDelegate(); 
	// Create a WeaponFireDelegate field named fireDelegate.
	public WeaponFireDelegate fireDelegate;
	private bool keyup = true;

	[SerializeField]
	private float _shieldLevel = 1;

	void Start(){
		if (S == null) {
			S = this; //set singleton
		} else {
			Debug.LogError("Hero.Awake() - Attempted to assign a second Hero.S!");
		}
		//reset the weapons to start _Hero with 1 blaster
		ClearWeapons();
		weapons [0].SetType (WeaponType.blaster);
		current_ammo = 9999999;
	}

	void Update(){
		//Pull in information from the input class
		float xAxis = Input.GetAxis ("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		//Change transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;
		//rotate the ship to make it feel more dynamic
		transform.rotation = Quaternion.Euler (yAxis * pitchMult, xAxis * rollMult, 0);
		//allow the ship to fire
		/*if(Input.GetKeyDown(KeyCode.Space)){
			TempFire ();
		}*/

		// Use the fireDelegate to fire Weapons
		// First, make sure the button is pressed: Axis("Jump")
		// Then ensure that fireDelegate isn't null to avoid an error

		if (Input.GetKeyUp("space")){
			print("space key was released");
			keyup = true;
		}

			
		if (Input.GetAxis ("Jump") == 1 && fireDelegate != null && keyup == true) { 

			keyup = false;
			fireDelegate ();

			if (weapons [0].def.type != WeaponType.blaster || weapons [1].def.type == WeaponType.blaster) {
				current_ammo--;
				UpdateAmmo ();
			}
				


		}
	}

/*	void TempFire(){
		GameObject projGO = Instantiate<GameObject> (projectilePrefab);
		projGO.transform.position = transform.position;
		Rigidbody rigidB = projGO.GetComponent<Rigidbody> ();
		rigidB.velocity = Vector3.up * projectileSpeed;

		Projectile proj = projGO.GetComponent<Projectile>(); 
		proj.type = WeaponType.blaster;
		float tSpeed = Main.GetWeaponDefinition( proj.type ).velocity;
		rigidB.velocity = Vector3.up * tSpeed;
	}*/


	void OnTriggerEnter(Collider other){
		Transform rootT = other.gameObject.transform.root;
		GameObject go = rootT.gameObject;
		//print ("Triggered: " + go.name);

		//make sure it's not the same triggering go as last time
		if (go == lastTriggerGo) {
			return;
		}
		lastTriggerGo = go;

		if (go.tag == "Enemy") { //if the shield was triggered by an enemy
			shieldLevel--; //decrease the shieldlevel by 1
			Destroy (go);
		}else if(go.tag == "PowerUp"){
			//if the shield was triggered by a PowerUp
			AbsorbPowerUp(go);
		}else {
			print ("Triggered by non-Enemy: " + go.name);
		}
	}

	public void AbsorbPowerUp(GameObject go){
		PowerUp pu = go.GetComponent<PowerUp> ();
		switch (pu.type) {
			
			case WeaponType.shield:
				shieldLevel++;
				break;

		default:
			if (pu.type == weapons[0].type) { // if it is the same type
				Weapon w = GetEmptyWeaponSlot ();
				current_ammo += 100;
				UpdateAmmo ();
				if (w != null) {
					//set it to pu.type
					w.SetType(pu.type);
				}
			} else { //if this is a different weapon type
				ClearWeapons();
				weapons[0].SetType (pu.type);
				current_ammo = 100;
				UpdateAmmo ();
			}
			break;
		}
		pu.AbsorbedBy (this.gameObject);
	}

	public float shieldLevel{
		get{
			return (_shieldLevel);
		}
		set{
			_shieldLevel = Mathf.Min (value, 4);
			//if the shield is going to be set to less than zero
			if(value < 0){
				Destroy (this.gameObject);
				//tell Main.S to restart the game after a delay
				Main.S.DelayedRestart(gameRestartDelay);
			}
		}
	}

	Weapon GetEmptyWeaponSlot(){
		for(int i = 0; i < weapons.Length; i++){
			if (weapons[i].type == WeaponType.none) {
				return (weapons[i]);
			}
		}
		return (null);
	}

	void ClearWeapons(){
		foreach (Weapon w in weapons) {
			w.SetType (WeaponType.none);
		}
	}

	public void UpdateAmmo(){
		Debug.Log ("Ammo: " + current_ammo); //whyyyy doesn't this worrrkk
		uitAmmo.text = "Ammo: " + current_ammo;
		return;

	}


}