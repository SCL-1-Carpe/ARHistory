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
        server.OnNewClientConnected += OnClientConnected;
        server.OnClientDisconnected += OnClientDisconnected;
        server.OnTcpMessageReceived += (data, client) =>
        {

        };
    }

    void OnClientConnected(ClientDataContainer client)
    {
        GameObject obj = Instantiate(ClientUIPrefab, ContentPanel.transform);
        ClientUIController uIController = obj.GetComponent<ClientUIController>();
        uIController.Initialize(server, client);
        clientStatuses.Add(client.address, uIController);
    }
    void OnClientDisconnected(ClientDataContainer client)
    {
        if (clientStatuses.TryGetValue(client.address, out ClientUIController uIController))
        {
            Destroy(uIController.gameObject);
            clientStatuses.Remove(client.address);
        }
    }
}
