using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using UnityEngine;

public struct ClientDataContainer
{
    public IPAddress address;
    public TcpClient TcpSocket;
    public List<ReplicatiorBase> AutonomousObjects;
    public byte NetworkId;
}

public class NetworkManager_Server : NetworkManagerBase
{
    public static NetworkManager_Server server;

    public string DOwnIP;
    TcpListener listener;
    UdpClient UdpSocket;
    public List<ClientDataContainer> ClientDataList = new List<ClientDataContainer>();
    [SerializeField]
    bool LaunchOnStart;
    [SerializeField]
    int TcpPortNum = 7890, UdpPortNum = 7891;
    int buffersize = 512;
    byte[] buffer;
    /// <summary>
    /// Dictionary of Replication targets Key=ReplicatorBase.Id
    /// </summary>
    public Dictionary<int, ReplicatiorBase> RepObjPairs = new Dictionary<int, ReplicatiorBase>();
    /// <summary>
    /// List of Replication targets
    /// </summary>
    public List<ReplicatiorBase> RepObjects = new List<ReplicatiorBase>();
    int ObjIdBuffer = 0;
    byte NetIdBuffer = 1;

    public delegate void ClientNotification(ClientDataContainer clientData);
    public delegate void NetworkDataHandler(byte[] data, ClientDataContainer clientData);
    public delegate void ReplicatedObjectNotification(ReplicatiorBase replicatior);
    public ReplicatedObjectNotification OnNewRepObjectAdded;
    public ReplicatedObjectNotification OnNewAutonomousObjectAdded;
    public ClientNotification OnNewClientConnected;
    public ClientNotification OnClientDisconnected;
    public NetworkDataHandler OnTcpPacketReceived;
    public NetworkDataHandler OnUdpPacketReceived;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            foreach (var I in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (I.AddressFamily == AddressFamily.InterNetwork)
                {
                    OwnIP = I;
                    DOwnIP = I.ToString();
                }
            }
        }
        catch
        {
            OwnIP = IPAddress.Parse(DOwnIP);
        }

        if (LaunchOnStart)
        {
            LaunchNetworkServer();
        }
    }

    public override void Launch()
    {
        LaunchNetworkServer();
    }

    /// <summary>
    /// Launch Server System. Start Listen on TcpPortNum.
    /// </summary>
    public void LaunchNetworkServer()
    {
        buffer = new byte[buffersize];
        try
        {
            listener = new TcpListener(IPAddress.Any, TcpPortNum);
            listener.Start();
            System.IAsyncResult result = listener.BeginAcceptTcpClient(AcceptedClientCallback, listener);
        }
        catch
        {
            Debug.Log("Couldnt Launch Server");
            return;
        }
        UdpSocket = new UdpClient(UdpPortNum);
        server = this;
        LocalInst = this;
    }

    void AcceptedClientCallback(System.IAsyncResult ar)
    {
        Debug.Log("Server: Client Connected");
        TcpClient client = (ar.AsyncState as TcpListener).EndAcceptTcpClient(ar);
        ClientDataContainer c = new ClientDataContainer() { TcpSocket = client, address = ((IPEndPoint)client.Client.RemoteEndPoint).Address, AutonomousObjects = new List<ReplicatiorBase>(), NetworkId = NetIdBuffer++ };
        Debug.Log("Client IPAddress : " + c.address);
        ClientDataList.Add(c);
        if (OnNewClientConnected != null)
            OnNewClientConnected.Invoke(c);
        listener.BeginAcceptTcpClient(AcceptedClientCallback, listener);
    }

    void SendInitialMessage(ClientDataContainer client)
    {
        string InitRepData = "Assign," + client.NetworkId + "$";
        RepObjects.ForEach((obj) =>
        {
            InitRepData += "NewRepObj" + "," + obj.RepPrefabName + "," + Serializer.Vector3ToString(obj.transform.position) + "," +
            Serializer.Vector3ToString(obj.transform.eulerAngles) + "," + obj.transform.parent.gameObject.name + "," + obj.Id + "," + obj.OwnerNetId + "$";
        });
        SendTcpPacket(client, encoding.GetBytes(InitRepData));
    }

    void SendTcpPacket(ClientDataContainer client, byte[] data)
    {
        try
        {
            client.TcpSocket.Client.Send(data);
        }
        catch
        {
            ClientDisconnected(client);
        }
    }

    void SendFile(ClientDataContainer client, string FilePath)
    {
        client.TcpSocket.Client.SendFile(FilePath);
    }

    void ClientDisconnected(ClientDataContainer client)
    {
        ClientDataList.Remove(client);
        if (OnClientDisconnected != null)
            OnClientDisconnected.Invoke(client);
        Debug.Log("Client Disconnected : " + client.address);
    }

    void RegistNewReplicationObject(ReplicatiorBase replicatior, string PrefabName)
    {
        RepObjects.Add(replicatior);
        replicatior.Id = ObjIdBuffer;
        replicatior.RepPrefabName = PrefabName;
        RepObjPairs.Add(ObjIdBuffer++, replicatior);
        if (OnNewRepObjectAdded != null)
            OnNewRepObjectAdded.Invoke(replicatior);
    }

    void RegistNewAutonomousObject(ClientDataContainer client, ReplicatiorBase replicatior, string PrefabName)
    {
        RegistNewReplicationObject(replicatior, PrefabName);
        replicatior.OwnerNetId = client.NetworkId;
        client.AutonomousObjects.Add(replicatior);
        if (OnNewAutonomousObjectAdded != null)
            OnNewAutonomousObjectAdded.Invoke(replicatior);
    }

    byte[] CreateReplicationData(ClientDataContainer client)
    {
        byte[] data = new byte[0];
        RepObjects.ForEach((obj) =>
        {
            if (obj.DoesClientNeedReplication(client))
            {
                byte[] vs = obj.GetReplicationData();
                if (vs != null)
                    data = data.Concat(encoding.GetBytes(obj.Id + ":")).Concat(vs).Concat(encoding.GetBytes("$")).ToArray();
            }
        });
        return data;
    }

    /// <summary>
    /// Send ReplicationData to all client. Dont recommend manually call.
    /// </summary>
    public void Replicate()
    {
        ClientDataList.ForEach((client) =>
        {
            byte[] data = CreateReplicationData(client);
            if (data.Length < 1)
                return;
            UdpSocket.Send(data, data.Length, new IPEndPoint(client.address, UdpPortNum));
            Debug.Log("Rep : " + client.address + " / " + data.Length + "  " + encoding.GetString(data));
        });
    }

    void DecompClientRequest(byte[] data, ClientDataContainer client)
    {
        string datastr = encoding.GetString(data);
        string[] vs = datastr.Contains('$') ? datastr.Split('$') : new string[] { datastr };
        foreach (string s in vs)
        {
            ClientRequest(s, client);
        }
    }

    void ClientRequest(string request, ClientDataContainer client)
    {
        string[] vs = request.Contains(',') ? request.Split(',') : new string[] { request };
        switch (vs[0])
        {
            case "NewAutoObj":
                CreateAutonomousPrefab(vs[1], vs[2], Serializer.StringToVector3(vs[3], vs[4], vs[5]), Serializer.StringToVector3(vs[6], vs[7], vs[8]), vs[9], client);
                break;
            case "Init":
                SendInitialMessage(client);
                break;
            case "RPCOS": //RPC On Server
                ProcessRPC(vs[1], vs[2], vs[3]);
                break;
            case "RPCOC": //RPC On Client
                HandOutRPC(byte.Parse(vs[1]), vs[2], vs[3], vs[4]);
                break;
            case "RPCMC": //MultiCast RPC
                MultiCastRPC(vs[1], vs[2], vs[3]);
                break;
        }
    }

    void DecompClientAutonomousData(byte[] data, ClientDataContainer client)
    {
        string datastr = encoding.GetString(data);
        string[] vs = datastr.Contains('$') ? datastr.Split('$') : new string[] { datastr };
        foreach (string s in vs)
        {
            if (s.Length < 1)
                return;
            if (s.IndexOf(':') < 0)
                return;
            int Id = int.Parse(s.Substring(0, s.IndexOf(':')));
            HandOutAutonomousData(Id, encoding.GetBytes(s.Substring(s.IndexOf(':') + 1)));
        }
    }

    void HandOutAutonomousData(int Id, byte[] data)
    {
        if (RepObjPairs.TryGetValue(Id, out ReplicatiorBase Target))
        {
            Target.ReceiveAutonomousData(data);
        }
    }
    /// <summary>
    /// Create Gameobject replicated On all Client <!warning> Prefab must contain Replicator!
    /// </summary>
    /// <param name="PrefabName">Resources/Prefabs/...</param>
    /// <param name="pos"></param>
    /// <param name="eular"></param>
    /// <param name="ParentObjName"></param>
    /// <returns></returns>
    public GameObject CreateNetworkPrefab(string PrefabName, Vector3 pos, Vector3 eular, string ParentObjName)
    {
        string path = "Prefabs/" + PrefabName;
        GameObject Pobj = (GameObject)Resources.Load(path), parentobj = GameObject.Find(ParentObjName), obj;
        if (parentobj != null)
            obj = Instantiate(Pobj, pos, Quaternion.Euler(eular.x, eular.y, eular.z), parentobj.transform);
        else
            obj = Instantiate(Pobj, pos, Quaternion.Euler(eular.x, eular.y, eular.z));
        ReplicatiorBase replicatior = obj.GetComponent<ReplicatiorBase>();
        if (replicatior == null)
        {
            Debug.Log("CreatingNetworkPrefab Request Refused! Attach Replicator To Prefab!");
            Destroy(obj);
            return null;
        }
        RegistNewReplicationObject(replicatior, PrefabName);
        ClientDataList.ForEach((c) =>
        {
            SendTcpPacket(c, encoding.GetBytes("NewRepObj," + PrefabName + "," + Serializer.Vector3ToString(pos) + "," +
                Serializer.Vector3ToString(eular) + "," + ParentObjName + "," + replicatior.Id + "," + replicatior.OwnerNetId));
        });
        return obj;
    }

    /// <summary>
    /// Create Gameobject replicated On all Client. On LocalHost, Object beact as LocalPrefab. On Client, Object beact as NetworkPrefab. <!warning> Prefab must contain Replicator!
    /// </summary>
    /// <paramref name="LocalPrefabName"/> Local Replicated Object. Search in Resources/Prefabs/...
    /// <param name="NetworkPrefabName"> Replicated Object On Networking. Search in Resources/Prefabs/... </param>
    /// <param name="pos"></param>
    /// <param name="eular"></param>
    /// <param name="ParentObjName"></param>
    /// <returns></returns>
    public GameObject CreateNetworkPrefab(string LocalPrefabName,string NetworkPrefabName, Vector3 pos, Vector3 eular, string ParentObjName)
    {
        string path = "Prefabs/" + LocalPrefabName;
        GameObject Pobj = (GameObject)Resources.Load(path), parentobj = GameObject.Find(ParentObjName), obj;
        if (parentobj != null)
            obj = Instantiate(Pobj, pos, Quaternion.Euler(eular.x, eular.y, eular.z), parentobj.transform);
        else
            obj = Instantiate(Pobj, pos, Quaternion.Euler(eular.x, eular.y, eular.z));
        ReplicatiorBase replicatior = obj.GetComponent<ReplicatiorBase>();
        if (replicatior == null)
        {
            Debug.Log("CreatingNetworkPrefab Request Refused! Attach Replicator To Prefab!");
            Destroy(obj);
            return null;
        }
        RegistNewReplicationObject(replicatior, NetworkPrefabName);
        ClientDataList.ForEach((c) =>
        {
            SendTcpPacket(c, encoding.GetBytes("NewRepObj," + NetworkPrefabName + "," + Serializer.Vector3ToString(pos) + "," +
                Serializer.Vector3ToString(eular) + "," + ParentObjName + "," + replicatior.Id + "," + replicatior.OwnerNetId));
        });
        return obj;
    }

    /// <summary>
    /// Replicate Object as RepPrefabObj
    /// </summary>
    /// <param name="replicatior"></param>
    /// <param name="RepPrefabName"></param>
    public void StartReplicateObject(ReplicatiorBase replicatior, string RepPrefabName)
    {
        RegistNewReplicationObject(replicatior, RepPrefabName);
        ClientDataList.ForEach((c) =>
        {
            SendTcpPacket(c, encoding.GetBytes("NewRepObj," + RepPrefabName + "," + Serializer.Vector3ToString(replicatior.transform.position) + "," +
                Serializer.Vector3ToString(replicatior.transform.eulerAngles) + "," + replicatior.transform.parent.gameObject.name + "," + replicatior.Id + "," + replicatior.OwnerNetId));
        });
    }

    void CreateAutonomousPrefab(string PrefabName, string ObjName, Vector3 pos, Vector3 eular, string ParentObjName, ClientDataContainer Owner)
    {
        string path = "Prefabs/" + PrefabName;
        GameObject Pobj = (GameObject)Resources.Load(path), parentobj = GameObject.Find(ParentObjName), obj;
        if (parentobj != null)
            obj = Instantiate(Pobj, pos, Quaternion.Euler(eular.x, eular.y, eular.z), parentobj.transform);
        else
            obj = Instantiate(Pobj, pos, Quaternion.Euler(eular.x, eular.y, eular.z));
        ReplicatiorBase replicatior = obj.GetComponent<ReplicatiorBase>();
        if (replicatior == null)
        {
            Debug.Log("CreatingAutonomousPrefab Request Refused! Attach Replicator To Prefab!");
            Destroy(obj);
            return;
        }

        RegistNewAutonomousObject(Owner, replicatior, PrefabName);
        ClientDataList.ForEach((c) =>
        {
            if (c.TcpSocket != Owner.TcpSocket)
                SendTcpPacket(c, encoding.GetBytes("NewRepObj," + PrefabName + "," + Serializer.Vector3ToString(pos) + "," + Serializer.Vector3ToString(eular) + "," + ParentObjName + "," + replicatior.Id));
            else
                SendTcpPacket(c, encoding.GetBytes("AutoObjAdded," + ObjName + "," + replicatior.Id));
        });
    }

    void ProcessRPC(string ObjName, string MethodName, string arg)
    {
        GameObject obj = GameObject.Find(ObjName);
        if (obj == null)
        {
            Debug.Log("Object couldnt find. RPC failed");
            return;
        }
        obj.SendMessage(MethodName, arg, SendMessageOptions.DontRequireReceiver);
    }

    void HandOutRPC(byte ClientId, string ObjName, string MethodName, string arg)
    {
        if (ClientId == 0)
            ProcessRPC(ObjName, MethodName, arg);
        else
        {
            SendTcpPacket(ClientDataList.Find((c) => c.NetworkId == ClientId), encoding.GetBytes("RPCOC," + ObjName + "," + MethodName + "," + arg));
        }
    }

    void MultiCastRPC(string ObjName, string MethodName, string arg)
    {
        ProcessRPC(ObjName, MethodName, arg);
        ClientDataList.ForEach((c) => SendTcpPacket(c, encoding.GetBytes("RPCOC," + ObjName + "," + MethodName + "," + arg)));
    }

    void Update()
    {
        if (ClientDataList.Count < 1)
            return;

        ClientDataList.ForEach((c) =>
        {
            if (c.TcpSocket.Available > 0)
            {
                c.TcpSocket.Client.Receive(buffer);
                Debug.Log("Tcp Received : " + encoding.GetString(buffer));
                DecompClientRequest(buffer, c);
                if (OnTcpPacketReceived != null)
                    OnTcpPacketReceived.Invoke(buffer, c);
            }

        });
        if (UdpSocket.Available > 0)
        {
            IPEndPoint endPoint = null;
            byte[] Udpbuffer = UdpSocket.Receive(ref endPoint);
            Debug.Log("Udp Received : " + encoding.GetString(Udpbuffer));
            ClientDataContainer client = ClientDataList.Find((c) => c.address == endPoint.Address);
            DecompClientAutonomousData(Udpbuffer, client);
            if (OnUdpPacketReceived != null)
                OnUdpPacketReceived.Invoke(Udpbuffer, client);
        }
        Replicate();
    }

    private void OnDestroy()
    {
        if (ClientDataList != null)
            ClientDataList.ForEach((s) => s.TcpSocket.Close());
        if (UdpSocket != null)
            UdpSocket.Close();
    }
}
