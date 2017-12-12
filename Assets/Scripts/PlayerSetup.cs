using UnityEngine;
using UnityEngine.Networking;

// behaves as an object that is networked by extending networkBehaviour
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

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

            //Disable local-player mesh graphics
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab) as GameObject;
            playerUIInstance.name = playerUIPrefab.name; // for a clean hierarchy

		}

		//sets player defaults and records active components on player
		GetComponent <Player> ().Setup ();
	}

    void SetLayerRecursively(GameObject obj, int newLayer) {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform) {
            SetLayerRecursively(child.gameObject, newLayer);
        }
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

        Destroy(playerUIInstance);

		//re-enable scene camera
		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive (true);
		}

		//unregister player by netID (name)
		GameManager.GetInstance().UnRegisterPlayer(transform.name);
	}
}
