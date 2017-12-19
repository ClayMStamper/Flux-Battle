using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
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

    [SerializeField]
    private GameObject[] disableGameobjectsOnDeath;

	//keep in mind: colliders components can't be stored as a behaviour
	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

    [SerializeField]
    private GameObject deathEffectPrefab;
    [SerializeField]
    private GameObject spawnEffectPrefab;

    public  bool firstSetup = true;

    //replaces start methods so the player start
    //function can be synced with the setup class
    public void SetupPlayer(){

        //switch cameras and canvas
        if (isLocalPlayer) {
            GameManager.GetInstance().ToggleSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadcastNewPlayerSetup();

	}

    [Command]
    private void CmdBroadcastNewPlayerSetup() {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients() {
       

        if (firstSetup == true) {
            wasEnabled = new bool[disableOnDeath.Length];
            //recording which behaviours are enabled on the player form the start
            for (int i = 0; i < wasEnabled.Length; i++) {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;

        }

        SetDefaults();
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

        //disable components
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = false;
		}

        //disable Gameobjects
        for (int i = 0; i < disableGameobjectsOnDeath.Length; i++) {
            disableGameobjectsOnDeath[i].SetActive(false);
        }

        //disable the collider
        Collider col = GetComponent <Collider> ();
		if (col != null) {
			col.enabled = false;
		}

        //spawn death effect
        Destroy (Instantiate(deathEffectPrefab, transform.position, Quaternion.identity), 2f);

        // switch camera to scene camera and toggle UI
        if (isLocalPlayer) {
            GameManager.GetInstance().ToggleSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

		Debug.Log (transform.name + " is dead");

		//Respawn
		StartCoroutine("Respawn");
	}
		
	IEnumerator Respawn(){
        float respawnTime = GameManager.GetInstance().matchSettings.respawnTime;
		yield return new WaitForSeconds (respawnTime);

		Transform spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;

        // making sure we move the player before instantiating particles: (hack)
        yield return new WaitForSeconds(.1f);

        SetupPlayer();

        Debug.Log (transform.name + " respawned");
	}

	public void SetDefaults(){
		isDead = false;
		currentHealth = maxHealth;

        //Debug.Log("Setting defaults");

        //set components active
		for (int i = 0; i < disableOnDeath.Length; i++) {
        //    Debug.Log("Setting " + disableOnDeath[i] + ": " + wasEnabled[i]);
			disableOnDeath [i].enabled = wasEnabled [i];
		}

        //enable disabled Gameobjects includeing player mesh
        for (int i = 0; i < disableGameobjectsOnDeath.Length; i++) {
            disableGameobjectsOnDeath[i].SetActive(true);
        }

        Collider col = GetComponent <Collider> ();
		if (col != null) {
			col.enabled = true;
		}

        //spawn effect
        Destroy(Instantiate(spawnEffectPrefab, transform.position, Quaternion.identity), 2f);

    }

}
