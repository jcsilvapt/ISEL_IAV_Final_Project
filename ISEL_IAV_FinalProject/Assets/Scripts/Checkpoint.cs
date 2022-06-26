using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public bool isChecked = false;
    public bool isFinalCheckpoint = false;

    private SphereCollider objectCollider;

    private void Start() {
        objectCollider = GetComponent<SphereCollider>();
        objectCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent<ShipAgent>(out ShipAgent agent)) {
            agent.AddReward(1f);
            isChecked = true;
            transform.gameObject.SetActive(false);
            Debug.Log("Checkpoint Checked!");
            if(isFinalCheckpoint) {
                agent.EndEpisode();
                agent.AddReward(2f);
            }
        }

    }

}
