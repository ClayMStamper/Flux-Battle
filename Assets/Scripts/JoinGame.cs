using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    private void Start() {
        networkManager = NetworkManager.singleton;

        //making sure the matchmaker is setup before using it
        if (networkManager.matchMaker == null) {
            networkManager.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList() {

        ClearRoomList();

        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
        status.text = "";

        if (matches == null) {
            status.text = "Couldn't get room list";
            return;
        }

        for (int i = 0; i < matches.Count; i++) {
            GameObject roomListItemGameObject = Instantiate(roomListItemPrefab) as GameObject;
            roomListItemGameObject.transform.SetParent(roomListParent);

            roomList.Add(roomListItemGameObject);

            RoomListItem roomListItem = roomListItemGameObject.GetComponent<RoomListItem>();
            if (roomListItem != null) {
                roomListItem.Setup(matches[i], JoinRoom);
            } else {
                Debug.LogError("RoomListItem component = null");
            }

        }

        if (roomList.Count == 0) {
            status.text = "No rooms available";
        }

    }

    private void ClearRoomList() {
        for (int i = 0; i < roomList.Count; i++) {
            Destroy(roomList[i]);
        }

        roomList.Clear();

    }

    public void JoinRoom(MatchInfoSnapshot match) {
        // Debug.Log("Joining " + match.name);
        networkManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";
    }
}
