using UnityEngine;
using UnityEngine.Networking;

// behaves as an object that is networked by extending networkBehaviour
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	Camera sceneCamera;

	void Start(){
		// if isn't local player
		if (!isLocalPlayer) {
			//disable non-local player components
			DisableComponents();
			AssignRemoteLayer ();
		} else {
			//because we dont want each player disabling main camera, only local will
			if (Camera.main != null) {
				sceneCamera = Camera.main;
				sceneCamera.gameObject.SetActive (false);
			}
		}

		//sets player defaults and records active components on player
		GetComponent <Player> ().Setup ();

	}

	public override void OnStartClient(){
		base.OnStartClient ();

		string netID = GetComponent<NetworkIdentity> ().netId.ToString();
		Player player = GetComponent <Player> ();

        Debug.Log("Registering: " + netID);
		GameManager.GetInstance().RegisterPlayer (netID, player);
	}

	void AssignRemoteLayer (){
		gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}

	void DisableComponents(){
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

	// when we are destroyed
	void OnDisable(){

		//re-enable scene camera
		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive (true);
		}

		//unregister player by netID (name)
		GameManager.GetInstance().UnRegisterPlayer(transform.name);
	}
}
