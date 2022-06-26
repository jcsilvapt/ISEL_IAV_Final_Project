using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [Header("Settings")]
    public float bulletSpeed = 15f;

    private void Start() {
        transform.parent = null;
    }

    private void Update() {
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }

}
