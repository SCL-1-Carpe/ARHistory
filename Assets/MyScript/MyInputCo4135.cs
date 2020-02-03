using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    [RequireComponent(typeof(ControllerConnectionHandler))]
    public class MyInputCo4135 : MonoBehaviour
    {
        [SerializeField,Tooltip("the object you want to move")] GameObject PlayerCube;
        [SerializeField,Tooltip("Sensitivity:Effective Range(0-1)")] float TrigerSensitivity = 0.5f;

        //the available 
        [System.NonSerialized] public Vector3 updatePosition;
        [System.NonSerialized] public bool TrigerPush = false;
        
        private ControllerConnectionHandler _controllerConnectionHandler = null;



        // Start is called before the first frame update
        void Start()
        {
            _controllerConnectionHandler = GetComponent<ControllerConnectionHandler>();

            if (TrigerSensitivity<0||TrigerSensitivity>1)
            {
                TrigerSensitivity = 0.5f;
                Debug.LogError("<color=red>TrigerSensitivity is not available</color>");
            }
        }


        // Update is called once per frame
        void Update()
        {
            UpdateTouchpadIndicater();
        }

        void UpdateTouchpadIndicater()
        {

            if (!_controllerConnectionHandler.IsControllerValid())
            {
                Debug.Log("Can't detect controllerConnectionHandler.IsControllerValid()");
                return;
            }

            MLInputController controller = _controllerConnectionHandler.ConnectedController;

            updatePosition = new Vector3(controller.Touch1PosAndForce.x,0.0f,controller.Touch1PosAndForce.y);


            if (controller.TriggerValue>TrigerSensitivity)
            {
                TrigerPush = true;
                Debug.Log("<color=blue>色確認</color>");

            }
            else
            {
                TrigerPush = false;
            }


        }

    }
}
