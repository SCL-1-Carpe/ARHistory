using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class ServerCommandSystem : MonoBehaviour
{
    [SerializeField]
    NetworkManager_Server server;
    [SerializeField]
    GameObject ClientUIPrefab, ContentPanel;
    Dictionary<IPAddress, ClientUIController> clientStatuses;
    // Start is called before the first frame update
    void Start()
    {
        server.OnNewClientConnected += (c) =>
        {
            GameObject obj = Instantiate(ClientUIPrefab, ContentPanel.transform);
            ClientUIController uIController = obj.GetComponent<ClientUIController>();
            uIController.Initialize(server, c);
            clientStatuses.Add(c.address, uIController);
        };
        server.OnClientDisconnected += (c) =>
        {
            if (clientStatuses.TryGetValue(c.address, out ClientUIController uIController))
            {
                Destroy(uIController.gameObject);
                clientStatuses.Remove(c.address);
            }
        };
        server.OnTcpMessageReceived += (data,client) =>
        {

        };
    }
}
