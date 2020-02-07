using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMakingPlayer01 : MonoBehaviour
{
    [SerializeField] NetworkManager_Client client;
    [SerializeField] GameObject A;
    [SerializeField] GameObject UI;
    [SerializeField] Color[]Mats;
    [SerializeField] ReplicatiorBase ReRe;


    //[SerializeField] GameObject MakingBasyo;

    
    ReplicatiorBase AutonomousObj;

    // Start is called before the first frame update
    void Start()
    {
        A.SetActive(false);
        client.OnNewRepObjectAdded += NewObjCreated;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void MakingPre01()
    {
       // Instantiate(A, MakingBasyo.transform.position , MakingBasyo.transform.rotation);

        A.SetActive(true);
        UI.SetActive(false);

        client.RequestCreatingNewAutonomousObject(AutonomousObj, A.name, AutonomousObj.transform.position, AutonomousObj.transform.eulerAngles, AutonomousObj.transform.parent != null ? AutonomousObj.transform.parent.name : "");

        A.GetComponent<Material>().color = Mats[ReRe.LocalHostNetId];
    }

    void NewObjCreated(ReplicatiorBase replicatior)
    {
        replicatior.gameObject.GetComponent<Material>().color = Mats[replicatior.LocalHostNetId];
    }

    private void OnDestroy()
    {
        client.OnNewRepObjectAdded -= NewObjCreated;
    }

}
