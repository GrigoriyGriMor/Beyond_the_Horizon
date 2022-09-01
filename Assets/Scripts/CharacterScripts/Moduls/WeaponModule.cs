using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeaponModule : CharacterBase
{
    [Header("Weapon Setting")]
    [SerializeField] private WeaponInfo[] weapon = new WeaponInfo[2];
    [SerializeField] private float waitGetWTime = 0.10f;
    [SerializeField] private float waitOutWTime = 0.10f;

    private int gunInHandNumber = -1;

    [SerializeField] private int upBodyLayerIndex = 2;

    [Header("Riggin Detals")]
    [SerializeField] private Transform rightHandPos;

    [Header("UI Elements")]
    [SerializeField] private Color usingPanelColor = Color.green;
    [SerializeField] private Color defaultPanelColor = Color.white;

    [Header("Weapon_1")]
    [SerializeField] private Image weapon_1_panel;
    [SerializeField] private Image weapon_1;

    [SerializeField] private Text weaponName_1;
    [SerializeField] private Text bulletCount_1;

    [Header("Weapon_1")]
    [SerializeField] private Image weapon_2_panel;
    [SerializeField] private Image weapon_2;

    [SerializeField] private Text weaponName_2;
    [SerializeField] private Text bulletCount_2;


    void Start()
    {
        if (weaponName_1) weaponName_1.text = "";
        if (weaponName_2) weaponName_2.text = "";
        if (bulletCount_1) bulletCount_1.text = "";
        if (bulletCount_2) bulletCount_2.text = "";

        for (int i = 0; i < weapon.Length; i++)
        {
            if (weapon[i].ItemInfo != null)
            {
                weapon[i].weaponGameObject = Instantiate(weapon[i].ItemInfo.GetVisualForPlayer(), weapon[i].weapon_Pos.position, weapon[i].weapon_Pos.rotation, weapon[i].weapon_Pos);
                weapon[i].weaponGameObject.SetActive(false);
            }

            gunInHandNumber = -1;
        }

        if (weapon[0].ItemInfo != null) {
            if (weapon_1) weapon_1.sprite = weapon[0].ItemInfo.GetItemIcon_2x1();
            if (weaponName_1) weaponName_1.text = weapon[0].ItemInfo.GetItemName();
            weapon[0].weaponGameObject.GetComponent<WeaponController>().SetBulletCountText(bulletCount_1);
        }
        else
            if (weapon_1) weapon_1.sprite = defaultWeaponImage;

        if (weapon[1].ItemInfo != null) {
            if (weapon_2) weapon_2.sprite = weapon[1].ItemInfo.GetItemIcon_2x1();
            weaponName_2.text = weapon[1].ItemInfo.GetItemName();
            weapon[1].weaponGameObject.GetComponent<WeaponController>().SetBulletCountText(bulletCount_2);
        }
        else
            if (weapon_2) weapon_2.sprite = defaultWeaponImage;
    }

    [SerializeField] private Sprite defaultWeaponImage;
    public void SetWeapon(int wNumber, ItemBaseParametrs _weapon) {
        if (_weapon == null) {
            switch (wNumber) {
                case 0:
                    weapon[0].ItemInfo = null;
                    if (weapon[0].weaponGameObject != null) Destroy(weapon[0].weaponGameObject);
                    weapon[0].weaponGameObject = null;
                    if (weapon_1) weapon_1.sprite = defaultWeaponImage;
                    if (weaponName_1) weaponName_1.text = "";
                    if (weapon_1_panel) weapon_1_panel.color = defaultPanelColor;
                    if (bulletCount_1) bulletCount_1.text = "";

                    if (gunInHandNumber != -1) {
                        if (weapon[1].ItemInfo != null) GetWeapon(1);
                        else {
                            gunInHandNumber = -1;
                            playerAnim.SetBool("UseWeapon", false);
                            playerAnim.SetLayerWeight(upBodyLayerIndex, 0);
                            _player.SetUsedWeapon(null);
                        }
                    }
                    break;
                case 1:
                    weapon[1].ItemInfo = null;
                    if (weapon[1].weaponGameObject != null) Destroy(weapon[1].weaponGameObject);
                    weapon[1].weaponGameObject = null;
                    if (weapon_2) weapon_2.sprite = defaultWeaponImage;
                    if (weaponName_2) weaponName_2.text = "";
                    if (weapon_2_panel) weapon_2_panel.color = defaultPanelColor;
                    bulletCount_2.text = "";

                    if (gunInHandNumber != -1) {
                        if (weapon[0].ItemInfo != null) GetWeapon(0);
                        else {
                            gunInHandNumber = -1;
                            playerAnim.SetBool("UseWeapon", false);
                            playerAnim.SetLayerWeight(upBodyLayerIndex, 0);
                            _player.SetUsedWeapon(null);
                        }
                    }
                    break;
            }
            return;
        }

        switch (wNumber) {
            case 0:
                weapon[0].ItemInfo = _weapon;
                if (weapon[0].weaponGameObject != null) Destroy(weapon[0].weaponGameObject);
                weapon[0].weaponGameObject = Instantiate(weapon[0].ItemInfo.GetVisualForPlayer(), weapon[0].weapon_Pos.position, weapon[0].weapon_Pos.rotation, weapon[0].weapon_Pos);
                weapon[0].weaponGameObject.SetActive(false);

                if (weapon_1) weapon_1.sprite = weapon[0].ItemInfo.GetItemIcon_2x1();
                if (weaponName_1) weaponName_1.text = weapon[0].ItemInfo.GetItemName();
                if (bulletCount_1) {
                    weapon[0].weaponGameObject.GetComponent<WeaponController>().SetBulletCountText(bulletCount_1);
                    bulletCount_1.color = Color.white;
                }
            break;
            case 1:
                weapon[1].ItemInfo = _weapon;
                if (weapon[1].weaponGameObject != null) Destroy(weapon[1].weaponGameObject);
                weapon[1].weaponGameObject = Instantiate(weapon[1].ItemInfo.GetVisualForPlayer(), weapon[1].weapon_Pos.position, weapon[1].weapon_Pos.rotation, weapon[1].weapon_Pos);
                weapon[1].weaponGameObject.SetActive(false);

                if (weapon_2) weapon_2.sprite = weapon[1].ItemInfo.GetItemIcon_2x1();
                if (weaponName_2) weaponName_2.text = weapon[1].ItemInfo.GetItemName();
                if (bulletCount_2) {
                    weapon[1].weaponGameObject.GetComponent<WeaponController>().SetBulletCountText(bulletCount_2);
                    bulletCount_2.color = Color.white;
                }
                break;
        }
    }

    private Coroutine activeCoroutine;
    private int lastWeaponIndex = -1;
    private bool animCast = false;
    public void GetWeapon(int wNumber = -1)
    {
        if (activeCoroutine != null) return;

        if (wNumber != -1 && weapon[wNumber].ItemInfo == null)
            return;

        if (wNumber < 2 && (wNumber != -1 && wNumber != gunInHandNumber))
        {
            lastWeaponIndex = gunInHandNumber;
            gunInHandNumber = wNumber;
            activeCoroutine = StartCoroutine(GetNextWeapon());
        }
        else
        {
            lastWeaponIndex = gunInHandNumber;
            gunInHandNumber = -1;
            activeCoroutine = StartCoroutine(BackWeapon());
        }
    }

    Coroutine reloadCoroutine;
    public void ReloadGun()
    {
        if (gunInHandNumber <= -1 || reloadCoroutine != null)
            return;

        reloadCoroutine = StartCoroutine(ReloadingGun());
    }

    private Coroutine gunShotCoroutine;
    [SerializeField] private GameObject otherVignette;
    public void GunShoot()
    {
        if (gunShotCoroutine != null) StopCoroutine(gunShotCoroutine);
        gunShotCoroutine = StartCoroutine(GunShot());
    }
     
    private IEnumerator GunShot() {
        playerAnim.SetTrigger("OneShot");
        mainCamera.transform.localPosition = new Vector3(mainCamera.transform.localPosition.x, mainCamera.transform.localPosition.y, mainCamera.transform.localPosition.z - 0.05f);

        yield return new WaitForEndOfFrame();
    }

    [HideInInspector] public bool reload = false;
    private IEnumerator ReloadingGun()
    {
        reload = true;

        yield return new WaitForFixedUpdate();
        WeaponReloadInfo rT = weapon[gunInHandNumber].weaponGameObject.GetComponent<WeaponController>().UseReloadGun();

        _player.SetNewAnimLayerW(1);
        _player.SetNewBoneWeight(0);

        if (playerAnim != null) playerAnim.SetTrigger(rT.reloadAnimTriggerName);
        yield return new WaitForSeconds(rT.reloadTime);

        _player.SetNewAnimLayerW(0);
        _player.SetNewBoneWeight(1);

        reload = false;
        reloadCoroutine = null;
    }

    public void EndAnimCast()
    {
        animCast = false;
    }

    public IEnumerator GetNextWeapon() {
        if (reloadCoroutine != null) {
            StopCoroutine(reloadCoroutine);
            reload = false;
            reloadCoroutine = null;
            weapon[lastWeaponIndex].weaponGameObject.GetComponent<WeaponController>().BreakReload();
        }

        animCast = true;

        _player.SetNewBoneWeight(0);
        //переключаем приоритет слоя в аниматоре  на UpBody
        _player.SetNewAnimLayerW(1);

        float time = 0;
        playerAnim.SetTrigger("GetWeapon");
        while (time < waitGetWTime) {
            time += 0.02f;
            yield return new WaitForFixedUpdate();
        }

        if (lastWeaponIndex != -1 && weapon[lastWeaponIndex].weaponGameObject != null) {
            weapon[lastWeaponIndex].weaponGameObject.transform.position = weapon[lastWeaponIndex].weapon_Pos.position;
            weapon[lastWeaponIndex].weaponGameObject.transform.rotation = weapon[lastWeaponIndex].weapon_Pos.rotation;
            weapon[lastWeaponIndex].weaponGameObject.transform.SetParent(weapon[lastWeaponIndex].weapon_Pos);
            weapon[lastWeaponIndex].weaponGameObject.GetComponent<WeaponController>().reload.RemoveAllListeners();
            weapon[lastWeaponIndex].weaponGameObject.GetComponent<WeaponController>().oneShot.RemoveAllListeners();
            weapon[lastWeaponIndex].weaponGameObject.SetActive(false);

            if (weapon_1 != null && weapon_2 != null)
                if (gunInHandNumber == 1) {
                    if (weapon_1_panel) weapon_1_panel.color = defaultPanelColor;
                }
                else
                    if (weapon_2_panel) weapon_2_panel.color = defaultPanelColor;
        }

        weapon[gunInHandNumber].weaponGameObject.transform.position = rightHandPos.position;
        weapon[gunInHandNumber].weaponGameObject.transform.rotation = rightHandPos.rotation;
        weapon[gunInHandNumber].weaponGameObject.transform.SetParent(rightHandPos);
        weapon[gunInHandNumber].weaponGameObject.SetActive(true);
        weapon[gunInHandNumber].weaponGameObject.GetComponent<WeaponController>().reload.AddListener(() => ReloadGun());
        weapon[gunInHandNumber].weaponGameObject.GetComponent<WeaponController>().oneShot.AddListener(() => GunShoot());

        if (weapon_1 != null && weapon_2 != null)
            if (gunInHandNumber == 0) {
                if (weapon_1_panel) weapon_1_panel.color = usingPanelColor;
            }
            else
                if (weapon_2_panel) weapon_2_panel.color = usingPanelColor;

        while (animCast)
            yield return new WaitForFixedUpdate();

        playerAnim.SetBool("UseWeapon", true);

        _player.SetNewAnimLayerW(0);
        _player.SetNewBoneWeight(1);

        _player.SetUsedWeapon(weapon[gunInHandNumber].weaponGameObject.GetComponent<WeaponController>());
        activeCoroutine = null;
    }

    public IEnumerator BackWeapon() {
        if (reloadCoroutine != null) {
            StopCoroutine(reloadCoroutine);
            reload = false;
            reloadCoroutine = null;
            weapon[lastWeaponIndex].weaponGameObject.GetComponent<WeaponController>().BreakReload();
        }

        animCast = true;

        _player.SetNewBoneWeight(0);
        _player.SetNewAnimLayerW(1);

        playerAnim.SetTrigger("BackWeapon");

        float time = 0;
        while (time < waitOutWTime)
        {
            time += 0.02f;
            yield return new WaitForFixedUpdate();
        }

        if (lastWeaponIndex != -1 && weapon[lastWeaponIndex].weaponGameObject != null)
        {
            weapon[lastWeaponIndex].weaponGameObject.transform.position = weapon[lastWeaponIndex].weapon_Pos.position;
            weapon[lastWeaponIndex].weaponGameObject.transform.rotation = weapon[lastWeaponIndex].weapon_Pos.rotation;
            weapon[lastWeaponIndex].weaponGameObject.transform.SetParent(weapon[lastWeaponIndex].weapon_Pos);
            weapon[lastWeaponIndex].weaponGameObject.GetComponent<WeaponController>().reload.RemoveAllListeners();
            weapon[lastWeaponIndex].weaponGameObject.GetComponent<WeaponController>().oneShot.RemoveAllListeners();
            weapon[lastWeaponIndex].weaponGameObject.SetActive(false);


            if (weapon_1_panel) weapon_1_panel.color = defaultPanelColor;
            if (weapon_2_panel) weapon_2_panel.color = defaultPanelColor;
        }

        while (animCast)
            yield return new WaitForFixedUpdate();

        playerAnim.SetBool("UseWeapon", false);

        _player.SetNewAnimLayerW(0);

        playerAnim.SetLayerWeight(upBodyLayerIndex, 0);
        _player.SetUsedWeapon(null);
        activeCoroutine = null;
    }

    public byte GetUsedGunNumber() {
        switch (gunInHandNumber) {
            case -1:
                return 2;
            case 0:
                return 0;
            case 1:
                return 1;
            default:
                return 2;
        }
    }

    public byte GetUsedGun() {
        return (byte)gunInHandNumber;
    }
}