using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Shoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDistance;

    void Update()
    {
        //if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0)) 
        {
            //Fire();
        }
    }

    public void Fire() 
    {
        GameObject bullet =  Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        Destroy(bullet, bulletDistance);
    }
}