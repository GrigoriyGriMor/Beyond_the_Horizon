using UnityEngine;

[AddComponentMenu("JU TPS/Gameplay/Weapon System/Ammunition Box")]
public class AmmoBox : MonoBehaviour
{
    public int AmmoCount = 32;
    public int WeaponSwitchID = -1;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            var pl = other.GetComponent<ThirdPersonController>();
            if (pl.IsArmed && pl.WeaponInUse != null)
            {
                if (pl.WeaponInUse.WeaponSwitchID == WeaponSwitchID)
                {
                    pl.WeaponInUse.TotalBullets += AmmoCount;
                }
                if (WeaponSwitchID == -1)
                {
                    pl.WeaponInUse.TotalBullets += AmmoCount;
                }


                if(pl.WeaponInUse.WeaponSwitchID == WeaponSwitchID)
                    Destroy(this.gameObject);
            }
        }
    }
}
