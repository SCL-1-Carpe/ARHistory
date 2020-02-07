using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBasedTransformReplicator : ReplicatiorBase
{
    public GameObject CoordinateBaseObject;

    private void Start()
    {
        CoordinateBaseObject = GameObject.Find("COBase");
    }

    public override byte[] GetReplicationData()
    {
        float length = (CoordinateBaseObject.transform.position - transform.position).magnitude;
        Vector3 vec = (transform.position - CoordinateBaseObject.transform.position).normalized;
        Vector3 eular = transform.eulerAngles - CoordinateBaseObject.transform.eulerAngles;
        return NetworkManager_Server.encoding.GetBytes(Serializer.Vector3ToString(vec) + "," + length + "," + Serializer.Vector3ToString(eular));
    }

    public override byte[] GetAutonomousData()
    {
        return GetReplicationData();
    }

    public override void ReceiveReplicationData(byte[] repdata)
    {
        string[] s = NetworkManager_Server.encoding.GetString(repdata).Split(',');
        transform.position = CoordinateBaseObject.transform.position + (Serializer.StringToVector3(s[0], s[1], s[2]) * float.Parse(s[3]));
        transform.eulerAngles = CoordinateBaseObject.transform.eulerAngles + Serializer.StringToVector3(s[4], s[5], s[6]);
    }

    public override void ReceiveAutonomousData(byte[] autodata)
    {
        ReceiveReplicationData(autodata);
    }
}
