using UnityEngine;
using UnityEngine.Networking;

// behaves as an object that is networked by extending networkBehaviour
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
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
    [SerializeField]
    GameObject matchmakingUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;
    [HideInInspector]
    public GameObject matchmakingUIInstance;

	void Start(){

        matchmakingUIInstance = GameObject.Find("Canvas");

		// if isn't local player
		if (!isLocalPlayer) {
			//disable non-local player components
			DisableComponents();
			AssignRemoteLayer ();
		} else {

            //Disable local-player mesh graphics
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab) as GameObject;
            playerUIInstance.name = playerUIPrefab.name; // for a clean hierarchy

            // destroy matchMakingUI
            Destroy(matchmakingUIInstance);

            //configure PlayerUI
            PlayerUI UI = playerUIInstance.GetComponent<PlayerUI>();
            if (UI == null)
                Debug.LogError("No player UI component on player UI prefab: " + playerUIPrefab);
            UI.SetController(GetComponent <PlayerController>());

            //sets player defaults and records active components on player
            GetComponent<Player>().SetupPlayer();

        }
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

        //reactivate scene camera
        if (isLocalPlayer)
            GameManager.GetInstance().ToggleSceneCameraActive(true);

        //instantiate matchMakingUI
   //     matchmakingUIInstance = Instantiate(matchmakingUIPrefab);
   //     matchmakingUIInstance.name = "Canvas";

		//unregister player by netID (name)
		GameManager.GetInstance().UnRegisterPlayer(transform.name);
	}
}
