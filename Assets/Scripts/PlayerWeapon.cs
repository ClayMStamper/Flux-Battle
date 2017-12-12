using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

	public string name = "Glock";

	public int damage = 10;
	public float range = 100f;

    // zero means semi-automatic
    public float fireRate = 0f;

    public GameObject graphics;

}
