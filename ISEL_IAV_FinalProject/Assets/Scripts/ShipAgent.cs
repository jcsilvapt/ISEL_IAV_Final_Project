using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
public class ShipAgent : Agent {

    [Header("Ship Settings")]
    public float movementSpeed = 5f;
    public float rotationSpeed = 1f;
    public Transform shipTransform;
    private Rigidbody shipRb;

    private Rigidbody rb;

    private Vector3 initialSpawn;
    private Quaternion initialRotation;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        initialSpawn = transform.position;
        initialRotation = transform.rotation;
        shipRb = shipTransform.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin() {

        ProceduralRoad.ResetCheckpoints();
        

        transform.position = initialSpawn;
        transform.rotation = initialRotation;

    }

    public override void CollectObservations(VectorSensor sensor) {
        Vector3 nextCheckpoint = ProceduralRoad.GetNextCheckpoint(this.transform.position);
        float direction = Vector3.Dot(transform.forward, nextCheckpoint);
        sensor.AddObservation(direction);


    }

    public override void OnActionReceived(ActionBuffers actions) {

        int moveX = actions.DiscreteActions[0]; // Front, Stopped, Backwards
        int moveZ = actions.DiscreteActions[1]; // Left, Foward, Right

        Vector3 force = Vector3.zero;
        Vector3 rot = Vector3.zero;

        switch (moveX) {
            case 0: rot.y = 0f; break;
            case 1: rot.y = 1f; break;
            case 2: rot.y = -1f; break;
        }

        transform.Rotate(rot);

        switch (moveZ) {
            case 0: force = Vector3.zero; break;
            case 1: force = transform.forward; break;
            case 2: force = -transform.forward; break;
        }

        rb.velocity = force * movementSpeed;
        /*
        float yaw = rotationSpeed * Time.deltaTime * moveX;
        float roll = rotationSpeed * Time.deltaTime * moveX;
        */
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        switch (horizontal) {
            case 0: discreteActions[0] = 0; break;
            case 1: discreteActions[0] = 1; break;
            case -1: discreteActions[0] = 2; break;
        }

        switch (vertical) {
            case 0: discreteActions[1] = 0; break;
            case 1: discreteActions[1] = 1; break;
            case -1: discreteActions[1] = 2; break;
        }
    }

    private void OnCollisionExit(Collision collision) {

        // Check if lost the ground
        if (collision.gameObject.CompareTag("Path")) {
            AddReward(-1f);
            EndEpisode();
        }

    }

}
