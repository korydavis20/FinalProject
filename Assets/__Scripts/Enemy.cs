﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	/*public GameObject projectileEnemyPrefab; //Prefab for enemy projectiles
	public float lastShotTime;
	static public Transform PROJECTILE_ANCHOR;
	*/

	[Header("Set in Inspector: Enemy")]
	public float speed = 10f; // the Speed in m/s
	public float fireRate = 0.3f; //Seconds/shot (unused)
	public float health = 10;
	public float boss_health = 1000;
	public int score = 100; // points earned for destroying this
	public float showDamageDuration = 0.1f; //# seconds to show damage
	public float powerUpDropChance = 1f; //chance to drop a powerup

	[Header ("Set Dynamically: Enemy")]
	public Color[] originalColors;
	public Material[] materials;// All the Materials of this & its children
	public bool showingDamage = false;
	public float damageDoneTime; // Time to stop showing damage
	public bool notifiedOfDestruction = false; // Will be used later

	protected BoundsCheck bndCheck;


	//this is a property: a method that acts like a field
	public Vector3 pos{
		get{
			return(this.transform.position);
		}
		set{
			this.transform.position = value;
		}
	}

	void Awake(){
		bndCheck = GetComponent<BoundsCheck> ();
		//get materials and colors for this GameObject and its children
		materials = Utils.GetAllMaterials(gameObject);
		originalColors = new Color[materials.Length];
		for(int i = 0; i < materials.Length; i++){
			originalColors[i] = materials[i].color;
		}

		/* Dynamically create an anchor for all Projectiles
		if (PROJECTILE_ANCHOR == null) { 
			GameObject go = new GameObject("_ProjectileAnchor");
			PROJECTILE_ANCHOR = go.transform;
		}*/

	}
	
	// Update is called once per frame
	void Update () {
		Move ();

		if (showingDamage && Time.time > damageDoneTime) {
			UnShowDamage ();
		}

		if(bndCheck != null && bndCheck.offDown){
				//we're off the bottom so destroy this GameObject
			Destroy(gameObject);
		}
			
	}

	/*public void Fire(){

		// If it hasn't been enough time between shots, return
		if (Time.time - lastShotTime < 3) { 
			return;
		}

		Projectile p;
		Vector3 vel = Vector3.up * 30; 

		if (transform.up.y < 0) {
			vel.y = -vel.y;
		}

		p = MakeProjectileEnemy() ;

	}*/

	public virtual void Move(){
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}

	void OnCollisionEnter(Collision coll){
		GameObject otherGO = coll.gameObject;
		switch (otherGO.tag){
		case "ProjectileHero":
			Projectile p = otherGO.GetComponent<Projectile> ();
				
			//if this enemy is off screen, don't damage it
			if (!bndCheck.isOnScreen) {
				Destroy (otherGO);
				break;
			}

			//hurt this enemy
			ShowDamage ();
			Destroy (p);

			//get the damage amount from the Main WEAP_DICT
			if (Main.S.boss_spawned == false) {
				health -= Main.GetWeaponDefinition (p.type).damageOnHit;
				health -= Main.GetWeaponDefinition (p.type).continuousDamage;
			} else {
				boss_health-= Main.GetWeaponDefinition (p.type).damageOnHit;
				boss_health -= Main.GetWeaponDefinition (p.type).continuousDamage;
				Main.S.uitBoss_health.text = "Boss Health: " + boss_health;
			}
			/*if (Main.GetWeaponDefinition (p.type).continuousDamage != 0) {
				//(Time.time - timeStart)/timeDuration
				float startTime = Time.time;
				float timeDuration = 3;
				Debug.Log ("Time: " + Time.time);

				while ((Time.time - startTime)/timeDuration <= 1) {
					
					health -= Main.GetWeaponDefinition (p.type).continuousDamage;
					ShowDamage ();
				}
			}*/

			if (health <= 0 || boss_health <= 0) {
				
				// Tell the Main singleton that this ship was destroyed // b
				if (!notifiedOfDestruction){
					Main.S.shipDestroyed( this );
				}

				notifiedOfDestruction = true;

				/*for (int i = 0; this.gameObject != Main.S.prefabEnemies [i]; i++) {
					score = 25 * i;
				}*/

				// Destroy this Enemy
				Destroy(this.gameObject);

				Main.S.UpdateGUI ();

			}

			Destroy (otherGO);
			break;

		default:
			print ("Enemy hit by non-ProjectileHero: " + otherGO.name);
			break;
		}
	}

	void ShowDamage(){
		foreach (Material m in materials) {
			m.color = Color.red;
		}

		showingDamage = true;
		damageDoneTime = Time.time + showDamageDuration;
	}

	void UnShowDamage(){
		for (int i = 0; i < materials.Length; i++) {
			materials [i].color = originalColors [i];
		}
		showingDamage = false;
	}

/*	public Projectile MakeProjectileEnemy() {
		GameObject go = Instantiate<GameObject> (projectileEnemyPrefab);

		if (def.type == WeaponType.missile) {
			go = Instantiate<GameObject> (def.missilePrefab);
		}

		if ( transform.parent.gameObject.tag == "Enemy" ) { 
			go.tag = "ProjectileEnemy";
			go.layer = LayerMask.NameToLayer("ProjectileEnemy");
		}
			
		go.transform.SetParent( PROJECTILE_ANCHOR, true );
		Projectile p = go.GetComponent<Projectile>();

		lastShotTime = Time.time; 
		return( p );
	} */
		
}