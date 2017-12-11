using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";

	public PlayerWeapon weapon;
	private GameManager gameManager;

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	void Start(){
		gameManager = GameManager.GetInstance ();
		if (cam == null) {
			Debug.LogError ("PlayerShoot: No camera referenced");
			this.enabled = false;
		}
	}

	void Update(){
		if (Input.GetButtonDown ("Fire1")) {
			Shoot ();
		}
	}

	[Client] //local method: only called on client, never on server
	private void Shoot(){
		RaycastHit hit;

		//if we hit something
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask)) {
            //if what we hit is a player
            Debug.Log("We hit " + hit.collider.name);
			if (hit.collider.tag == PLAYER_TAG) {
				GameObject playerHit = hit.collider.gameObject;
				CmdPlayerIsShot(playerHit, weapon.damage);
			}
		}
	}

	[Command] // Called only on server
	void CmdPlayerIsShot(GameObject playerHit, int damage){
        //print(gameManager.GetPlayer("Player 1"));
        playerHit.GetComponent<Player>().RpcTakeDamage (damage);
	}

}
