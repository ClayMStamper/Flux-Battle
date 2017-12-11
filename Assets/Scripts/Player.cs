using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

	[SyncVar]
	private bool isDeadValue = false;
	public bool isDead {
		get {return isDeadValue; }
		protected set {isDeadValue = value; }
	}

	[SerializeField]
	private int maxHealth = 100;

	//anytime the value changes it's pushed out to all of the clients
	[SyncVar]
	private int currentHealth;

	//keep in mind: colliders components can't be stored as a behaviour
	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	//replaces start methods so the player start
	//function can be synced with the setup class
	public void Setup(){
		wasEnabled = new bool[disableOnDeath.Length];

		//recording which behaviours are enabled on the player form the start
		for (int i = 0; i < wasEnabled.Length; i++) {
			wasEnabled [i] = disableOnDeath [i].enabled;
		}

		SetDefaults ();
	}
/* Debug: kill function*/
	void Update(){
		if (Input.GetKeyDown (KeyCode.K)) {
			RpcTakeDamage (9999);
		}
	}
    

	[ClientRpc] //invokes on the client from the server
	public void RpcTakeDamage(int amount){
		if (isDead) {
			return;
		}

		currentHealth -= amount;
		Debug.Log (transform.name + " now has " + currentHealth + " health");

		if (currentHealth <= 0) {
			Die ();
		}
	}

	private void Die(){
		isDead = true;

		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = false;
		}

		Collider col = GetComponent <Collider> ();
		if (col != null) {
			col.enabled = false;
		}

		Debug.Log (transform.name + " is dead");

		//Respawn
		StartCoroutine("Respawn");
	}
		
	IEnumerator Respawn(){
        float respawnTime = GameManager.GetInstance().matchSettings.respawnTime;
		yield return new WaitForSeconds (respawnTime);

		SetDefaults ();
		Transform spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;

		Debug.Log (transform.name + " respawned");
	}

	public void SetDefaults(){
		isDead = false;
		currentHealth = maxHealth;
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = wasEnabled [i];
		}

		Collider col = GetComponent <Collider> ();
		if (col != null) {
			col.enabled = true;
		}

	}

}
