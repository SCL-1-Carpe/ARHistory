using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text;

public class NetworkManagerBase : MonoBehaviour
{
    public static NetworkManagerBase LocalInst;
    public IPAddress OwnIP;
    public byte NetworkId;
    /// <summary>
    /// Encoding class of Server and Replicators
    /// </summary>
    public static Encoding encoding = Encoding.ASCII;

    public virtual void Launch()
    {

    }
}
