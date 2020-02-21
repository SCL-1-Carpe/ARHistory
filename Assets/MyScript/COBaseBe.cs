using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicLeap{

    public class COBaseBe : MonoBehaviour
    {
        //[SerializeField] ImageTrackingExample A;
        //[SerializeField] GameObject ParentM;

        [System.NonSerialized] public bool HinannKanryou=true;

        // Start is called before the first frame update
        void Start()
        {
           // Debug.Log("テストフラグ１３１");


        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("テストフラグ５４２３１");


            /*
            if (A.MyCOBaseBool)
            {
                gameObject.transform.parent = ParentM.transform;

                HinannKanryou = false;

            }
            else
            {
                gameObject.transform.parent = null;

                HinannKanryou = true;

            }
            */

           

        }
        

    }
}
