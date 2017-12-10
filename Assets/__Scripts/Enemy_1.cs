using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy_1 extends the Enemy Class
public class Enemy_1 : Enemy {
	[Header("Set in Inspector: Enemy_1")]
	//# seconds for a full sine wave
	public float waveFrequency = 2;
	//sine wave width in meters
	public float waveWidth = 4;
	public float waveRotY = 45;

	private float x0; // the initial x value of pos
	private float birthTime;

	// Use this for initialization
	void Start () {
		//set x0 to the initial x position of enemy_1
		x0 = pos.x;

		birthTime = Time.time;
	}

	// Override the Move function On Enemy
	public override void Move () {
		//Because pos is a property, you can't directly set pox.x
		// so get the pos as an editable vector 3
		Vector3 tempPos = pos;
		//theta adjusts based on time
		float age =  Time.time - birthTime;
		float theta = Mathf.PI * 2 * age / waveFrequency;
		float sin = Mathf.Sin(theta);
		tempPos.x = x0 + waveWidth * sin;
		pos = tempPos;

		//rotate a bit about the y
		Vector3 rot = new Vector3(0, sin*waveRotY, 0);
		this.transform.rotation = Quaternion.Euler (rot);

		//base.Move() still handles the movement down in y
		base.Move();

		//print (bndCheck.isOnScreen);
	}
}
