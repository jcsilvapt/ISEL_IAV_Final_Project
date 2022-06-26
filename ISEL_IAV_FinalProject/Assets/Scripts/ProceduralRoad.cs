using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ProceduralRoad : MonoBehaviour {

    public static ProceduralRoad instance;

    [Header("Road Settings")]
    public int numSegments;
    public float distanceBetweenAnchors;
    public PathSpace pathSpace;
    public bool closePath;

    [Header("Perlin Noise Settings")]
    public float smooth;
    public Vector2 offsetRange;
    public float offset = 24000f;

    [Header("References")]
    public Transform ship;
    float angleOffset;

    [Header("Developer Settings")]
    public List<Checkpoint> checkPointsList = new List<Checkpoint>();
    public int currentCheckpointIndex = 0;

    private RoadMesh pathMesh;

    private GameObject checkpointsHolder;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {

        angleOffset = ship.localEulerAngles.y * Mathf.Deg2Rad - PerlinOrientation(ship.position);

        offset = Random.Range(offsetRange.x, offsetRange.y);

        RebuiltPath();
    }

    public void RebuiltPath() {
    
        if(pathMesh == null) {
            pathMesh = GetComponent<RoadMesh>();
        }

        BuildPath(ship.position - (ship.forward * 4));
        ship.position += Vector3.up;

    }


    private void BuildPath(Vector3 initialPosition) {

        List<Vector3> anchors = new List<Vector3>();
        anchors.Add(initialPosition);

        for (int i = 0; i < numSegments; i++) {


            Vector3 nAnchor = NextAnchor(anchors[i]);
            anchors.Add(nAnchor);
            if (i == numSegments - 1) {
                CreateCheckPoint(nAnchor, true, false);
            } else {
                if (i % 2 == 0 && i != 0) {
                    CreateCheckPoint(nAnchor, false, true);
                } else {
                    CreateCheckPoint(nAnchor, false, false);
                }
            }
        }

        VertexPath vertexPath = GeneratePath(anchors, 0.2f, closePath);
        pathMesh.BuildRoad(vertexPath);

    }

    private void CreateCheckPoint(Vector3 position, bool isFinalCheckpoint, bool physical) {

        if (checkpointsHolder == null) {
            checkpointsHolder = new GameObject("Checkpoints Holder");
            checkpointsHolder.transform.SetParent(transform);
        }

        // Create Checkpoint
        GameObject checkpoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // Position the sphere in the place
        checkpoint.transform.position = position;
        // Disable Mesh Renderer;
        checkpoint.GetComponent<MeshRenderer>().enabled = physical;
        // Set the collider size
        checkpoint.GetComponent<SphereCollider>().radius = 2f;

        checkpoint.gameObject.name = "Checkpoint";
        checkpoint.tag = "Checkpoints";

        Checkpoint tempData = checkpoint.AddComponent<Checkpoint>();
        tempData.isFinalCheckpoint = isFinalCheckpoint;

        checkpoint.transform.SetParent(checkpointsHolder.transform);


        checkPointsList.Add(tempData);

    }

    private Vector3 NextAnchor(Vector3 currentAnchor) {

        float angle = PerlinOrientation(currentAnchor);
        angle += angleOffset;
        Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        Vector2 anchorxz = new Vector2(currentAnchor.x, currentAnchor.z);
        anchorxz += direction * distanceBetweenAnchors;
        return new Vector3(anchorxz.x, 0f, anchorxz.y);
    }

    private float PerlinOrientation(Vector3 currentPosition) {
        return 2f * Mathf.PI * Mathf.PerlinNoise((currentPosition.x + offset) * smooth, (currentPosition.z + offset) * smooth);
    }

    private VertexPath GeneratePath(List<Vector3> anchors, float vertexSpacing, bool closePath) {

        BezierPath bezierPath = new BezierPath(anchors, closePath, pathSpace);
        return new VertexPath(bezierPath, this.transform, vertexSpacing);
    }

    private Vector3 NextCheckPoint(Vector3 currentPosition) {
        if (checkPointsList.Count > 0) {
            return checkPointsList[currentCheckpointIndex].GetComponent<Transform>().position;
        }
        return Vector3.zero;
    }

    private void CheckpointReset() {
        foreach (Checkpoint point in checkPointsList) {
            point.isChecked = false;
            point.gameObject.SetActive(true);
        }
        currentCheckpointIndex = 0;
    }

    public static Vector3 GetNextCheckpoint(Vector3 position) {
        if (instance != null) {
            return instance.NextCheckPoint(position);
        } else {
            return Vector3.zero;
        }
    }

    public static void UpdateCheckpoint() {
        if (instance != null) {
            instance.currentCheckpointIndex++;
        }
    }

    public static void ResetCheckpoints() {
        if (instance != null) {
            instance.CheckpointReset();
        }
    }

}
