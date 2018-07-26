using BLINDED_AM_ME;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Examples;

public class Cuttable : MonoBehaviour {

    public Material cutMaterial;
    public Collider collider;
    public float timeCreated;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        timeCreated = Time.time;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(Time.time < timeCreated + 0.5f) {
            return;
        }

        var sword = collision.attachedRigidbody.GetComponent<CuttingSword>();
        if(sword == null) {
            return;
        }

        var mat = cutMaterial == null ? sword.cutMaterial : cutMaterial;

        var objs = MeshCut.Cut(gameObject, sword.transform.position, sword.transform.right, mat);

        PreparePart(objs[0]).AddForce(-transform.right * sword.splitStrenght);
        PreparePart(objs[1]).AddForce(transform.right * sword.splitStrenght);

        if(collider != null) {
            Destroy(gameObject);
        }
    }

    private Rigidbody PreparePart(GameObject o)
    {
        o.layer = gameObject.layer;
        var mc = o.AddComponent<MeshCollider>();
        mc.sharedMesh = o.GetComponent<MeshFilter>().sharedMesh;
        mc.convex = true;
        var cuttable = o.AddComponent<Cuttable>();
        cuttable.cutMaterial = cutMaterial;
        return o.AddComponent<Rigidbody>();
    }
}
