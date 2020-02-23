using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugForMac : MonoBehaviour
{
    [SerializeField] bool Check01 = false;

    [SerializeField] Button OsitaiButton ;



    [SerializeField, Space(15)] bool DoAllBool = false;
    [SerializeField,Header("If you have buttons you want to try,insert one to this column")]
     List<Button> ButtonGun = new List<Button>();


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Check01 && OsitaiButton!=null)
        {
            OsitaiButton.onClick.Invoke();

            Check01 = false;

        }

        if (DoAllBool && ButtonGun.Count>0)
        {
            foreach(Button OneButten in ButtonGun)
            {
                OneButten.onClick.Invoke();

            }
            DoAllBool = false;


        }









    }



}
