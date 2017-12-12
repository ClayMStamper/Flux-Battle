using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;
    private PlayerWeapon currentWeapon;

    private WeaponGraphics currentGraphics;

	void Start() {
        EquipWeapon(primaryWeapon);
    }

    public PlayerWeapon GetCurrentWeapon() {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics() {
        return currentGraphics;
    }

    void EquipWeapon(PlayerWeapon weapon) {

        currentWeapon = weapon;

        GameObject weaponInstance = Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponInstance.transform.SetParent(weaponHolder);

        currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();
        if (currentGraphics == null) {
            Debug.LogError("No WeaponGraphics component on the weapon object: " + weaponInstance.name);
        }

        if (isLocalPlayer) {
            Util.SetLayerRecursively(weaponInstance, LayerMask.NameToLayer(weaponLayerName));
        }

    }

}
