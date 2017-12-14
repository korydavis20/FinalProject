using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	public int scene = 0;

	void Start () {

	}

	public void ChangeToScene (){
		
		SceneManager.LoadScene ("_Scene_" + scene);

		StartCoroutine ("Delay");

	}

	public void Exit(){
		Application.Quit ();
	}

	IEnumerator Delay() {
		yield return new WaitForSeconds (1); 
	}

	void Update () {

	}
}
