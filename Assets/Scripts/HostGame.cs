using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour {

    [SerializeField]
    private uint roomSize = 6;

    private string roomName;

    [SerializeField]
    private GameObject hostGame;
    private NetworkManager networkManager;

    void Start() {
        networkManager = NetworkManager.singleton;

        // make sure mathmaking mode is the default
        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName () {
        string name = hostGame.GetComponentInChildren<InputField>().text;
        roomName = name;
    }

    public void CreateRoom() {
        if (roomName != "" && roomName != null) {
            Debug.Log("Creating room: " + roomName + " with room for " + roomSize + " players.");

            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
           
        }
    }

}
