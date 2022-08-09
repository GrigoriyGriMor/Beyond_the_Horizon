using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAtSceneController : AbstractIO
{
    [Header("Visual Setting")]
    public Rigidbody _rb;
    [SerializeField] private GameObject visualTraker;

    [SerializeField] private ItemBaseParametrs itemRef;
    [SerializeField] private int count;

    private Vector3 moveVector;

    private void Start() {
        SetName(itemRef.GetItemName());
        moveVector = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
    }


    public void SetItemCount(int itemCount)
    {
        count = itemCount;
    }

    //Если используется данная функция значит итем был поднят и перенесен в инвентарь
    public ItemTransaction GetItemInfo() {
        ItemTransaction newData = new ItemTransaction();
        newData.item = itemRef;
        newData.itemCount = count;

        Destroy(gameObject);

        return newData;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 31) {
            _rb.isKinematic = true;
            visualTraker.SetActive(true);
            visualTraker.transform.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.GetComponent<ItemAtSceneController>())
            transform.Translate(moveVector * Time.deltaTime);
    }
}
