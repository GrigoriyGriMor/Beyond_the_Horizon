using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Animator gunAnim;

    [Header("Стрельба")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject fireObjPrefabs;
    [SerializeField] private float damage = 10;
    [SerializeField] private float maxFireDistance = 100.0f;
    [SerializeField] private float effectFireDistance = 50.0f;

    [Header("")]
    [SerializeField] private float shootGup = 0.1f;
    private float shootGupCurrent = 0;
    
    [Header("")]
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private GameObject gunCartridge;//гильза
    [SerializeField] private Transform cartridgeStartPos;
    [SerializeField] private AudioClip fireAudio;

    private List<GameObject> fireObjPool = new List<GameObject>();
    private List<GameObject> gunCartridgePool = new List<GameObject>();

    [Header("Перезарядка")]
    [SerializeField] private float gunMagazineCount = 20;
    [SerializeField] private float gunMagazine = 0;
    [SerializeField] private ReloadParam reloadInfo;
    [SerializeField] private AudioClip reloadAudio;
    private Coroutine reloadCoroutine;

    [Header("Прочее")]
    public Transform weaponForearm; //цевье оружия
    [SerializeField] private GameObject defaultShotCollisionObject;

    public UnityEvent reload = new UnityEvent();
    public UnityEvent oneShot = new UnityEvent();

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject go = Instantiate(fireObjPrefabs, transform.position, transform.rotation);
            fireObjPool.Add(go);
            go.SetActive(false);

            go = Instantiate(gunCartridge, transform.position, transform.rotation);
            gunCartridgePool.Add(go);
            go.SetActive(false);
        }

        shootGupCurrent = 0;
        gunMagazine = gunMagazineCount;
        reloadCoroutine  = null;

        if (reloadInfo == null) Debug.LogError("В оружии " + gameObject.name + " отсутствует ссылка на Reload Param, ВАЖНО!!!");
    }

    private UnityEngine.UI.Text _bulletText;
    public void SetBulletCountText(UnityEngine.UI.Text bulletText) {
        _bulletText = bulletText;
        _bulletText.text = gunMagazineCount.ToString();
    }

    public void Fire()
    {
        if (reloadCoroutine != null) return;

        if (shootGupCurrent <= 0)
        {
            if (gunMagazine > 0)
            {
                shootGupCurrent = shootGup;
                Shot();
            }
            else
                if (reloadCoroutine == null)
                {
                   // reloadCoroutine = StartCoroutine(ReloadGun());
                    reload.Invoke();
                }
        }
        else
            shootGupCurrent -= Time.deltaTime;
    }

    //выстрел
    private void Shot()
    {
        gunMagazine -= 1;
        if (_bulletText != null)
        {
            _bulletText.text = gunMagazine.ToString();

            if (gunMagazine < 5) _bulletText.color = Color.red;
        }

        if (fireParticle != null) fireParticle.Play();

        oneShot.Invoke();
        if (SoundManagerAllControll.Instance && fireAudio != null) SoundManagerAllControll.Instance.ClipPlay(fireAudio);

        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, maxFireDistance))
        {
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<InDamageModule>())
                {
                    if (Vector3.Distance(hit.point, muzzlePoint.position) < effectFireDistance) 
                        hit.collider.GetComponent<InDamageModule>().InDamage(damage, hit, transform);
                    else
                        hit.collider.GetComponent<InDamageModule>().InDamage(damage * 0.5f, hit, transform);  //если дистанция больше эффективной, то урон снижается
                }
                else
                {
                    GameObject go = Instantiate(defaultShotCollisionObject, hit.point, Quaternion.identity);
                    go.transform.LookAt(transform.position);
                }
            }
        }

        //патроны
        for (int i = 0; i < fireObjPool.Count; i++)
        {
            if (!fireObjPool[i].activeInHierarchy)
            {
                fireObjPool[i].transform.position = muzzlePoint.position;
                fireObjPool[i].transform.rotation = muzzlePoint.rotation; 
                fireObjPool[i].SetActive(true);
                break;
            }
            else
            if (i == fireObjPool.Count - 1)
            {
                GameObject go = Instantiate(fireObjPrefabs, muzzlePoint.position, muzzlePoint.rotation);
                fireObjPool.Add(go);
                break;
            }
        }

        //гильзы
        for (int i = 0; i < gunCartridgePool.Count; i++)
        {
            if (!gunCartridgePool[i].activeInHierarchy)
            {
                gunCartridgePool[i].SetActive(true);
                gunCartridgePool[i].transform.position = cartridgeStartPos.position;
                gunCartridgePool[i].transform.rotation = cartridgeStartPos.rotation;
                break;
            }
            else
            if (i == fireObjPool.Count - 1)
            {
                gunCartridgePool[0].SetActive(false);
                gunCartridgePool[0].SetActive(true);

                gunCartridgePool[0].transform.position = cartridgeStartPos.position;
                gunCartridgePool[0].transform.rotation = cartridgeStartPos.rotation;
                break;
            }
        }
    }

    public WeaponReloadInfo UseReloadGun()
    {
        if (reloadCoroutine == null)
            reloadCoroutine = StartCoroutine(ReloadGun());

        return reloadInfo.GetInfo();
    }

    private IEnumerator ReloadGun()
    {
        if (!gameObject.activeInHierarchy) {
            reloadCoroutine = null;
            yield break;
        }

        if (SoundManagerAllControll.Instance && reloadAudio != null) SoundManagerAllControll.Instance.ClipPlay(reloadAudio);

        yield return new WaitForSeconds(reloadInfo.GetInfo().reloadTime);

        gunMagazine = gunMagazineCount;
        if (_bulletText) {
            _bulletText.text = gunMagazine.ToString();
            _bulletText.color = Color.white;
        }

        reloadCoroutine = null;
    }

    public void BreakReload() {
        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        reloadCoroutine = null;
    }
}

[System.Serializable]
public class WeaponReloadInfo
{
    public string reloadAnimTriggerName = "ReloadGun";
    public float reloadTime = 1.5f;
}