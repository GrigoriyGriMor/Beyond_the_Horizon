using UnityEngine;
using System.Collections;
using UnityEditor;

public class CreaterColliders : MonoBehaviour
{
    [SerializeField]
    private Transform prefabDamagerCollider;

    [SerializeField]
    private string[] bones;

    //public string path;

    public void Create()
    {
        for (int index = 0; index < bones.Length; index++)
        {
            Transform findGameObject = GameObject.Find(bones[index]).transform;

            Debug.Log(findGameObject.name);

            if (findGameObject)
            {
                Transform tempGameObject = Instantiate(prefabDamagerCollider);
                tempGameObject.SetParent(findGameObject);
                tempGameObject.position = findGameObject.position;

                Debug.Log("Create Damage Collider");

            }
        }
    }
}