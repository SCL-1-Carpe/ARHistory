using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberInput1712 : MonoBehaviour
{
    string MoziGun;
    [SerializeField] GameObject MainTextOBject;
    Text ShowingMozi;
    [SerializeField] Text PreIpID;
    [SerializeField] Text ChangeKyoka;

    int AllMoziNumber = 0;
   // [System.NonSerialized] public string yoOutcome = "";
    [SerializeField] bool InitializeMozi = true;

    [SerializeField] NetworkManager_Client NetworkManager_Client1;
    

    // Start is called before the first frame update
    void Start()
    {
        ShowingMozi = MainTextOBject.GetComponent<Text>();

        ChangeKyoka.text = "NoConnect";

        if (InitializeMozi)
        {
            ClealInput();
        }

        PreIpID.text = NetworkManager_Client1.TargetIP;
    }

    // Update is called once per frame
    void Update()
    {

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
        MoziGun = "";
        AllMoziNumber = 0;
        ShowingMozi.text = MoziGun;

    }



    public void DeleteLastMozi()
    {
        if (AllMoziNumber > 0)
        {
            MoziGun = MoziGun.Substring(0, AllMoziNumber - 1);

            AllMoziNumber -= 1;

            ShowingMozi.text = MoziGun;
        }
        else
        {
            Debug.Log("文字なし");
        }

    }

    /*
    public void ChangeT()
    {
        ChangeIPBool=true;

        ChangeKyoka.text = "kyoka";
    }

    public void ChangeF()
    {
        ChangeIPBool = false;

        ChangeKyoka.text = "dame";
    }*/

    public void IPChanged()
    {
        //ここにPreIpIDの値をtragetIPに参照させる。
        PreIpID.text = MoziGun;

        NetworkManager_Client1.TargetIP = PreIpID.text;

    }

    public void ChangeToShowingCOnnect()
    {
        ChangeKyoka.text = "Connect";

    }


}
