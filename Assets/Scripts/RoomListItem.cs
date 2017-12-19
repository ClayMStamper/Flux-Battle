using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour {

    public delegate void JoinRoomDelegate(MatchInfoSnapshot match);
    private JoinRoomDelegate joinRoomCallback;

    private MatchInfoSnapshot match;

    [SerializeField]
    private Text roomNameText;

    public void Setup(MatchInfoSnapshot match, JoinRoomDelegate joinRoomCallback) {
        this.match = match;
        this.joinRoomCallback = joinRoomCallback;

        Debug.Log (this.match.name + " (" + this.match.currentSize + "/" + this.match.maxSize + ")");
        roomNameText.text = this.match.name + " (" + this.match.currentSize + "/" + this.match.maxSize + ")";
    }

    public void JoinRoom() {
        if (match != null) {
            joinRoomCallback.Invoke(match);
        }
    }

}
