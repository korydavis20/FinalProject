using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// This is an enum of the various possible weapon types
/// it also includes a "shield" type to allow a shield power-up
/// items marked [NI] below are not implemented in the IGDPD book
/// </summary>


public enum WeaponType {
	none, // the default / no weapon
	blaster, // a simple blaster
	spread, // two shots simultaneously
	phaser, // [NI] shots that move in waves
	missile, // [NI] Homing Missiles
	laser, // [NI] Damage over time
	shield // Raise shieldLevel
}

[System.Serializable]
public class WeaponDefinition{
	public WeaponType type = WeaponType.none;
	public string letter; // leter to show on the power-up
	public Color color = Color.white; // color of collar & power-up
	public GameObject projectilePrefab; //Prefab for projectiles
	//public GameObject missilePrefab; //prefab for missiles
	public Color projectileColor = Color.white;
	public float damageOnHit = 0; //Amount of damage caused
	public float continuousDamage = 0; //Damage per second (Laser)
	public float delayBetweenShots = 0; 
	public float velocity = 20; //Speed of projectiles
	//public int current_ammo; // the current ammo of the equipped weapon
	//public int max_ammo; //ammo capacity of the equipped weapon

}

public class Weapon : MonoBehaviour{
	static public Transform PROJECTILE_ANCHOR;

	[Header("Set Dynamically")] [SerializeField]
	private WeaponType _type = WeaponType.none;
	public WeaponDefinition def;
	public GameObject collar;
	public float lastShotTime; // Time last shot was fired
	private Renderer collarRend;

		
	void Start() {
		collar = transform.Find("Collar").gameObject;
		collarRend = collar.GetComponent<Renderer>();
		// Call SetType() for the default _type of WeaponType.none
		SetType( _type ); 

		// Dynamically create an anchor for all Projectiles
		if (PROJECTILE_ANCHOR == null) { 
			GameObject go = new GameObject("_ProjectileAnchor");
			PROJECTILE_ANCHOR = go.transform;
		}

		// Find the fireDelegate of the root GameObject
		GameObject rootGO = transform.root.gameObject; 
		if ( rootGO.GetComponent<Hero>() != null ) { 
			rootGO.GetComponent<Hero>().fireDelegate += Fire;
		}

	}


	public WeaponType type {
		get { return( _type ); }
		set { SetType( value ); }
	}

	public void SetType( WeaponType wt ) {
		_type = wt;

		if (type == WeaponType.none) { 
			this.gameObject.SetActive(false);
			return;
		} else {
			this.gameObject.SetActive(true);
		}

		def = Main.GetWeaponDefinition(_type); 

		collarRend.material.color = def.color;
		lastShotTime = 0; // You can fire immediately after _type is set. 
	}

	public void Fire() {
		// If this.gameObject is inactive, return
		if (!gameObject.activeInHierarchy) return; 

		// If it hasn't been enough time between shots, return
		if (Time.time - lastShotTime < def.delayBetweenShots) { 
			return;
		}

		Projectile p;
		Vector3 vel = Vector3.up * def.velocity; 

		if (transform.up.y < 0) {
			vel.y = -vel.y;
		}

		switch (type) { 
		case WeaponType.blaster:
			p = MakeProjectile ();
			p.rigid.velocity = vel;
			break;

		case WeaponType.spread: 
			p = MakeProjectile (); // Make middle Projectile
			p.rigid.velocity = vel;
			p = MakeProjectile (); // Make right Projectile
			p.transform.rotation = Quaternion.AngleAxis (10, Vector3.back);
			p.rigid.velocity = p.transform.rotation * vel;
			p = MakeProjectile (); // Make left Projectile
			p.transform.rotation = Quaternion.AngleAxis (-10, Vector3.back);
			p.rigid.velocity = p.transform.rotation * vel;
			break;

		case WeaponType.laser:
			p = MakeProjectile ();
			p.rigid.velocity = vel;
			break;
		

		case WeaponType.missile:
			p = MakeProjectile ();
			p.rigid.velocity = vel;
			break;
		
		}

	}

	public Projectile MakeProjectile() {
			GameObject go = Instantiate<GameObject> (def.projectilePrefab);

		/*if (def.type == WeaponType.missile) {
			go = Instantiate<GameObject> (def.missilePrefab);
		}*/

		Vector3 temp;

		if (type == WeaponType.laser) {
			temp = new Vector3 (0.25f, 4f, 0.5f);
			go.transform.localScale = temp;
		}

		if (type == WeaponType.missile) {
			temp = new Vector3 (0.4f, 2f, 0.5f);
			go.transform.localScale = temp;
		}

		if ( transform.parent.gameObject.tag == "Hero" ) { 
			go.tag = "ProjectileHero";
			go.layer = LayerMask.NameToLayer("ProjectileHero");
		} else {
			go.tag = "ProjectileEnemy";
			go.layer = LayerMask.NameToLayer("ProjectileEnemy");
		}

		go.transform.position = collar.transform.position;
		go.transform.SetParent( PROJECTILE_ANCHOR, true );
		Projectile p = go.GetComponent<Projectile>();

		p.type = type;
		lastShotTime = Time.time; 
		return( p );
	}

}