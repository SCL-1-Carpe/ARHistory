using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIController : MonoBehaviour
{
    public Text Nametext;
    public ClientDataContainer clientDataContainer;
    // Start is called before the first frame update
    public void Initialize(NetworkManager_Server server, ClientDataContainer clientData)
    {
        clientDataContainer = clientData;
        Nametext.text = clientData.address + " : Id " + clientData.NetworkId;
    }

    public void Ban()
    {

    }
}
