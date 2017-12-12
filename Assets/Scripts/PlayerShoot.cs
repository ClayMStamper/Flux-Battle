using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

	private const string PLAYER_TAG = "Player";

	private PlayerWeapon currentWeapon;

	private GameManager gameManager;
    private WeaponManager weaponManager;

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	void Start(){

		gameManager = GameManager.GetInstance ();
        weaponManager = GetComponent<WeaponManager>();

		if (cam == null) {
			Debug.LogError ("PlayerShoot: No camera referenced");
			this.enabled = false;
		}

	}

	void Update(){

        currentWeapon = weaponManager.GetCurrentWeapon();

        // isnt automatic
        if (currentWeapon.fireRate <= 0) {
            if (Input.GetButtonDown("Fire1")) {
                Shoot();
            }
        } else { //is automatic
            if (Input.GetButtonDown("Fire1")) {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            } else if (Input.GetButtonUp("Fire1")) {
                CancelInvoke("Shoot");
            }
        }
	}

	[Client] //local method: only called on client, never on server
	private void Shoot(){

        Debug.Log("SHOOT");

		RaycastHit hit;

		//if we hit something
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask)) {
            //if what we hit is a player
            Debug.Log("We hit " + hit.collider.name);
			if (hit.collider.tag == PLAYER_TAG) {
				GameObject playerHit = hit.collider.gameObject;
				CmdPlayerIsShot(playerHit, currentWeapon.damage);
			}
		}
	}

	[Command] // Called only on server
	void CmdPlayerIsShot(GameObject playerHit, int damage){
        //print(gameManager.GetPlayer("Player 1"));
        playerHit.GetComponent<Player>().RpcTakeDamage (damage);
	}

}
