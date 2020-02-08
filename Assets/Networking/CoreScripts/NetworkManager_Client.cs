using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;


public class NetworkManager_Client : MonoBehaviour
{
    public IPAddress OwnIP;
    public string DOwnIP, TargetIP;
    public string m_TargetIP
    {
        get { return TargetIP; }
        set { TargetIP = value; }
    }

    UdpClient OwnUdpClient;
    TcpClient OwnTcpSocket;
    [SerializeField]
    bool LaunchOnStart;
    public byte NetworkId;
    [SerializeField]
    int TcpPortNum = 7890, UdpPortNum = 7891;
    /// <summary>
    /// Object List of Autonomous Object.
    /// </summary>
    List<ReplicatiorBase> AutonomausObjects;
    /// <summary>
    /// Object Dirctionary of Replicated by server.
    /// </summary>
    Dictionary<int, ReplicatiorBase> RepObjPairs;
    int buffersize = 512;
    Encoding encoding = Encoding.ASCII;
    byte[] databuffer;

    public delegate void ConnectionNotification(NetworkManager_Client client);
    public delegate void NetworkDataHandler(byte[] data);
    public delegate void ReplicatedObjectNotification(ReplicatiorBase replicatior);

    public ConnectionNotification OnConnectedToServer;
    public ConnectionNotification OnAssignedNetwork;
    public ReplicatedObjectNotification OnNewRepObjectAdded;
    public ReplicatedObjectNotification OnNewAutonomousObjectAdmitted;
    public NetworkDataHandler OnTcpPacketReceived;
    public NetworkDataHandler OnUdpPacketReceived;

    // Start is called before the first frame update
    void Start()
    {
        if (LaunchOnStart)
        {
            LaunchNetworkClient();
        }
    }

    public void LaunchNetworkClient()
    {
        databuffer = new byte[buffersize];
        OwnTcpSocket = new TcpClient();
        OwnUdpClient = new UdpClient(UdpPortNum);
        AutonomausObjects = new List<ReplicatiorBase>();
        RepObjPairs = new Dictionary<int, ReplicatiorBase>();
        try
        {
            OwnTcpSocket.BeginConnect(TargetIP, TcpPortNum, ConnectedToServerCallback, 0);
        }
        catch
        {
            Debug.Log("Couldnt find Server");
        }
    }

    void ConnectedToServerCallback(System.IAsyncResult ar)
    {
        OwnTcpSocket.EndConnect(ar);
        if (OwnTcpSocket.Connected)
            Debug.Log("Client: Connected to Server");
        SendTcpPacket(encoding.GetBytes("Init$"));
        OwnUdpClient.Send(encoding.GetBytes("InitRep$"), encoding.GetByteCount("InitRep$"), new IPEndPoint(IPAddress.Parse(TargetIP), UdpPortNum));
        if (OnConnectedToServer != null)
            OnConnectedToServer.Invoke(this);
        //OwnUdpClient.Connect(TargetIP, UdpPortNum);
    }

    public void SendTcpPacket(byte[] data)
    {
        OwnTcpSocket.Client.Send(data);
    }

    void NetworkInitialize(byte NewId)
    {
        NetworkId = NewId;
        if (OnAssignedNetwork != null)
            OnAssignedNetwork.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (OwnTcpSocket == null || !OwnTcpSocket.Connected)
            return;
        if (OwnTcpSocket.Available > 0)
        {
            OwnTcpSocket.Client.Receive(databuffer);
            Debug.Log("Tcp Received :" + encoding.GetString(databuffer));
            DecompServerMessage(databuffer);
            if (OnTcpPacketReceived != null)
                OnTcpPacketReceived.Invoke(databuffer);
        }
        if (OwnUdpClient.Available > 0)
        {
            IPEndPoint endPoint = null;
            byte[] Udpdatabuffer = OwnUdpClient.Receive(ref endPoint);
            Debug.Log("Udp Received : " + encoding.GetString(Udpdatabuffer));
            DecompReplicationData(Udpdatabuffer);
            if (OnUdpPacketReceived != null)
                OnUdpPacketReceived.Invoke(Udpdatabuffer);
        }
        ReplicateAutonomousObject();
    }
    /// <summary>
    /// Send creating autonomous object request to server <!waring!> Autonomous Object must have One and Only Name!
    /// </summary>
    /// <param name="replicatior">Autonomous obj</param>
    /// <param name="ObjName">Identity of autonomous obj</param>
    /// <param name="ReplicatedPrefabName">Name of prefab replicated on others</param>
    /// <param name="pos">replicated prefab initial position</param>
    /// <param name="eular">replicated prefab initial eularAngle</param>
    /// <param name="ParentName">replicated prefab initial parent name</param>
    public void RequestCreatingNewAutonomousObject(ReplicatiorBase replicatior, string ReplicatedPrefabName, Vector3 pos, Vector3 eular, string ParentName)
    {
        SendTcpPacket(encoding.GetBytes("NewAutoObj," + ReplicatedPrefabName + "," + replicatior.gameObject.name + "," + Serializer.Vector3ToString(pos) +
            "," + Serializer.Vector3ToString(eular) + "," + ParentName));
    }

    public void RequestRPCOnServer(string ServerObjectName, string MethodName, string arg)
    {
        SendTcpPacket(encoding.GetBytes("RPCOS," + ServerObjectName + "," + MethodName + "," + arg));
    }

    public void RequestRPCOnOtherClient(string ObjectName, string MethodName, string arg, byte ClientId)
    {
        if (ClientId == NetworkId)
            ProcessRPC(ObjectName, MethodName, arg);
        else
        {
            SendTcpPacket(encoding.GetBytes("RPCOC," + ClientId + "," + ObjectName + "," + MethodName + "," + arg));
        }
    }

    public void RequestRPCMultiCast(string ObjectName,string MethodName,string arg)
    {
        SendTcpPacket(encoding.GetBytes("RPCMC," + ObjectName + "," + MethodName + "," + arg));
    }

    void AddNewReplicatedObject(ReplicatiorBase replicatior, int Id, byte OwnerId)
    {
        replicatior.Id = Id;
        replicatior.OwnerNetId = OwnerId;
        replicatior.LocalHostNetId = NetworkId;
        RepObjPairs.Add(Id, replicatior);
        if (OnNewRepObjectAdded != null)
            OnNewRepObjectAdded.Invoke(replicatior);
    }

    void AddAdmittedAutonomousObject(string ObjName, int ObjId)
    {
        GameObject obj = GameObject.Find(ObjName);
       
        if (obj == null)
            return;

        ReplicatiorBase replicatior = obj.GetComponent<ReplicatiorBase>();

        Debug.Log("FF");
        Debug.Log(replicatior);

        AddNewReplicatedObject(replicatior, ObjId, NetworkId);

        Debug.Log("A");

        AutonomausObjects.Add(replicatior);

        

        Debug.Log(OnNewAutonomousObjectAdmitted);

        if (OnNewAutonomousObjectAdmitted != null)
        {
            OnNewAutonomousObjectAdmitted.Invoke(replicatior);
           

        }


        Debug.Log("New Autonomous Object : " + ObjName);
    }

    void DecompServerMessage(byte[] data)
    {
        string datastr = encoding.GetString(data);
        string[] vs = datastr.Contains('$') ? datastr.Split('$') : new string[] { datastr };
        foreach (string s in vs)
        {
            ProcessServerMessage(s);
        }
    }

    void ProcessServerMessage(string mes)
    {
        string[] vs = mes.Contains(',') ? mes.Split(',') : new string[] { mes };
        switch (vs[0])
        {
            case "NewRepObj":
                CreateReplicatedPrefab(vs[1], Serializer.StringToVector3(vs[2], vs[3], vs[4]), Serializer.StringToVector3(vs[5], vs[6], vs[7]), vs[8], int.Parse(vs[9]), byte.Parse(vs[10]));
                break;
            case "AutoObjAdded":
                AddAdmittedAutonomousObject(vs[1], int.Parse(vs[2]));
                break;
            case "RPCOC":
                ProcessRPC(vs[1], vs[2], vs[3]);
                break;
            case "Assign":
                NetworkInitialize(byte.Parse(vs[1]));
                break;
        }
    }

    GameObject CreateReplicatedPrefab(string PrefabName, Vector3 pos, Vector3 eular, string ParentObj, int ObjId, byte OwnerId)
    {
        string path = "Prefabs/" + PrefabName;
        GameObject Pobj = (GameObject)Resources.Load(path), parentobj = GameObject.Find(ParentObj), obj;
        if (parentobj != null)
            obj = Instantiate(Pobj, pos, Quaternion.Euler(eular.x, eular.y, eular.z), parentobj.transform);
        else
            obj = Instantiate(Pobj, pos, Quaternion.Euler(eular.x, eular.y, eular.z));
        ReplicatiorBase replicatior = obj.GetComponent<ReplicatiorBase>();
        AddNewReplicatedObject(replicatior, ObjId, OwnerId);
        return obj;
    }

    void ProcessRPC(string ObjectName, string MethodName, string arg)
    {
        GameObject obj = GameObject.Find(ObjectName);
        if (obj == null)
        {
            Debug.Log("Object couldnt find. RPC failed.");
            return;
        }
        obj.SendMessage(MethodName, arg, SendMessageOptions.DontRequireReceiver);
    }

    void DecompReplicationData(byte[] data)
    {
        string[] vs = encoding.GetString(data).Split('$');
        foreach (string s in vs)
        {
            if (s.Length < 1)
                return;
            if (s.IndexOf(':') < 0)
                return;
            int Id = int.Parse(s.Substring(0, s.IndexOf(':')));
            HandOutReplicationData(Id, encoding.GetBytes(s.Substring(s.IndexOf(':') + 1)));
        }
    }

    void HandOutReplicationData(int id, byte[] data)
    {
        ReplicatiorBase Target;
        if (RepObjPairs.TryGetValue(id, out Target))
        {
            Target.ReceiveReplicationData(data);
        }
    }

    byte[] CreateAutonomousData()
    {
        byte[] data = new byte[0];
        AutonomausObjects.ForEach((obj) =>
        {
            if (obj.DoesServerNeedReplication())
            {
                byte[] vs = obj.GetAutonomousData();
                if (vs != null)
                    data = data.Concat(encoding.GetBytes(obj.Id + ":")).Concat(vs).Concat(encoding.GetBytes("$")).ToArray();
            }
        });
        return data;
    }

    void ReplicateAutonomousObject()
    {
        byte[] data = CreateAutonomousData();
        if (data.Length < 1)
            return;
        OwnUdpClient.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(TargetIP), UdpPortNum));
        Debug.Log("Rep : " + TargetIP + " / " + data.Length + "  " + encoding.GetString(data));
    }

    private void OnDestroy()
    {
        if (OwnTcpSocket != null)
            OwnTcpSocket.Close();
        if (OwnUdpClient != null)
            OwnUdpClient.Close();
    }
}
