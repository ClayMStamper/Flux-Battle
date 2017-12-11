using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	#region Singleton
	private static GameManager instance;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else {
            Debug.LogError("More than one " + transform.name + " in the scene");
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	public static GameManager GetInstance(){
		return instance;
	}

    #endregion

    public MatchSettings matchSettings;


    #region Player tracking
    private const string PLAYER_ID_PREFIX = "Player ";

    public Dictionary<string, Player> players = new Dictionary<string, Player>();

    public void RegisterPlayer(string netID, Player playerRef){
		//attach "Player " prefix to netID
		string playerID = PLAYER_ID_PREFIX + netID;
		//add to 'players' dictionary
		players.Add (playerID, playerRef);
		//assign name to player
		playerRef.transform.name = playerID;
	}

	public void UnRegisterPlayer(string playerID){
		players.Remove (playerID);
	}

	public Player GetPlayer (string ID){
		if (ID.Contains ("Player")) {
			return players [ID];
		} else {
			ID = PLAYER_ID_PREFIX + ID;
			return players [ID];
		}
	}
    /*
        void OnGUI(){
            GUILayout.BeginArea (new Rect (200, 200, 200, 500));
            GUILayout.BeginVertical ();

            foreach (string playerID in players.Keys) {
    //			GUI.Label (new Rect (200, 200, 200, 500), playerID + " - " + players [playerID].transform.name);
            }

            GUILayout.EndVertical ();
            GUILayout.EndArea ();

        }
    */
    #endregion

}
