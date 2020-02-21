using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugForMac : MonoBehaviour
{
    //[SerializeField]
    // Start is called before the first frame update
    [SerializeField] bool Check01 = false;


    [SerializeField] Button OsitaiButton;

    [SerializeField] List<Button> Test = new List<Button>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Check01)
        {
            //ここにデバック用の操作を付け加える。


            OsitaiButton.onClick.Invoke();


            Check01 = false;

        }


    }



}
