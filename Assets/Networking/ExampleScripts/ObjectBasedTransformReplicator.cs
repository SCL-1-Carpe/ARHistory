using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBasedTransformReplicator : ReplicatiorBase
{
    public GameObject CoordinateBaseObject;
    [SerializeField] GameObject Cobase;


    private void Start()
    {
        CoordinateBaseObject = GameObject.Find("COBase");


        if (CoordinateBaseObject == null)
        {
            CoordinateBaseObject = GameObject.FindGameObjectWithTag("COBaseTag");
        }


        if (CoordinateBaseObject==null)
        {
            CoordinateBaseObject = Cobase;

        }


    }

    public override byte[] GetReplicationData()
    {
        float length = (CoordinateBaseObject.transform.position - transform.position).magnitude;
        Vector3 vec = (transform.position - CoordinateBaseObject.transform.position).normalized;
        float yrot = Quaternion.LookRotation(transform.position, CoordinateBaseObject.transform.position).eulerAngles.y;
        return NetworkManager_Server.encoding.GetBytes(Serializer.Vector3ToString(vec, 3) + "," + length + "," + yrot);
    }

    public override byte[] GetAutonomousData()
    {
        float length = (CoordinateBaseObject.transform.position - transform.position).magnitude;
        Vector3 vec = (transform.position - CoordinateBaseObject.transform.position).normalized;
        float yrot = Quaternion.LookRotation(transform.position, CoordinateBaseObject.transform.position).eulerAngles.y;
        return NetworkManager_Server.encoding.GetBytes(Serializer.Vector3ToString(vec, 3) + "," + length + "," + yrot);
    }

    public override void ReceiveReplicationData(byte[] repdata)
    {
        string[] s = NetworkManager_Server.encoding.GetString(repdata).Split(',');
        transform.position = CoordinateBaseObject.transform.position + (Serializer.StringToVector3(s[0], s[1], s[2]) * float.Parse(s[3]));
        transform.eulerAngles =new Vector3(0,float.Parse(s[5]),0);
    }

    public override void ReceiveAutonomousData(byte[] autodata)
    {
        ReceiveReplicationData(autodata);
    }
}
