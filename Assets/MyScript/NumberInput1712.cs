using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberInput1712 : MonoBehaviour
{
    string MoziGun;
    [SerializeField] GameObject TextObjectA;
    Text ShowingMozi;
   [SerializeField] Text PreIpID;
   [SerializeField] Text ChangeKyoka;

    int AllMoziNumber = 0;
    [System.NonSerialized] public string yoOutcome="";
    [SerializeField] bool InitializeMozi = true;

    [SerializeField] NetworkManager_Client NetworkManager_Client1;
    [SerializeField] bool ChangeIPBool = false;



    // Start is called before the first frame update
    void Start()
    {
        ShowingMozi = TextObjectA.GetComponent<Text>();

        ChangeKyoka.text = "dame";

        if (InitializeMozi)
        {
            ClealInput();
        }
    }

    // Update is called once per frame
    void Update()
    {

     PreIpID.text = NetworkManager_Client1.TargetIP;

    }

    public void Input0()
    {
        InputX("0");
    }

    public void Input1()
    {
        InputX("1");
    }

    public void Input2()
    {
        InputX("2");
    }

    public void Input3()
    {
        InputX("3");
    }
    public void Input4()
    {
        InputX("4");
    }

    public void Input5()
    {
        InputX("5");
    }
    public void Input6()
    {
        InputX("6");
    }

    public void Input7()
    {
        InputX("7");
    }

    public void Input8()
    {
        InputX("8");
    }

    public void Input9()
    {
        InputX("9");
    }

    public void InputDotto()
    {
        InputX(".");
    }

    void InputX(string NumberA)
    {
        MoziGun += NumberA;

        AllMoziNumber += 1;

        ShowingMozi.text = MoziGun;
    }

    public void ClealInput()
    {
        MoziGun="";
        AllMoziNumber = 0;
        ShowingMozi.text = MoziGun;

    }

    public void DeleteLastMozi()
    {
        if (AllMoziNumber > 0)
        {
            MoziGun = MoziGun.Substring(0, AllMoziNumber-1);

            AllMoziNumber -= 1;

            ShowingMozi.text = MoziGun;
        }
        else
        {
            Debug.Log("文字なし");
        }
      
    }

    public void ChangeT()
    {
        ChangeIPBool=true;

        ChangeKyoka.text = "kyoka";
    }

    public void ChangeF()
    {
        ChangeIPBool = false;

        ChangeKyoka.text = "dame";
    }

    public void SendMozi()
    {
        if (ChangeIPBool)
        {
            NetworkManager_Client1.TargetIP = MoziGun;

            ClealInput();
        }
        else
        {
            Debug.Log("許可なしでIP変更しテイル");
        }
       


    }

}
