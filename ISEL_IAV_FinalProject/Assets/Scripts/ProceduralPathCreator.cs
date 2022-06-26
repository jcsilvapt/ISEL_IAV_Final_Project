using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class ProceduralPathCreator : MonoBehaviour {

    private Vector3[] anchors;
    private GameObject go;
    private float speed = 2f;
    private float dist = 0f;
    private VertexPath path;
    private RoadMesh roadMesh;


    private void Start() {

        go = GetComponentInChildren<MeshRenderer>().gameObject;
        roadMesh = GetComponent<RoadMesh>();
        RandomAnchors();
        path = GeneratePath(anchors, false);
        roadMesh.BuildRoad(path);
    }

    private void Update() {

        go.transform.position = path.GetPointAtDistance(dist);
        go.transform.rotation = path.GetRotationAtDistance(dist);

        Debug.DrawRay(go.transform.position, path.GetDirectionAtDistance(dist), Color.yellow);
        Debug.DrawRay(go.transform.position, path.GetNormalAtDistance(dist), Color.blue);

        dist += speed * Time.deltaTime;
    }


    private void RandomAnchors() {

        anchors = new Vector3[5];

        for (int i = 0; i < 5; i++) {
            anchors[i] = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = anchors[i];
            g.transform.localScale = Vector3.one * 0.2f;
        }

    }



    VertexPath GeneratePath(Vector3[] anchors, bool closePath) {

        BezierPath bezierPath = new BezierPath(anchors, closePath, PathSpace.xyz);

        return new VertexPath(bezierPath, this.transform, 0.05f);
    }

}
