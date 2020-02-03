using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicLeap {
    public class Player01Be : MonoBehaviour
    {
        [SerializeField] float WalkingSpeed = 3;
        [SerializeField] GameObject Player01;
        [SerializeField] bool ThisIsPlayer01 = false;
        [SerializeField] MyInputCo4135 myInputCo4135;

        private Vector3 Player_pos; //プレイヤーのポジション

        GameObject Player;
        // Start is called before the first frame update
        void Start()
        {
            if (ThisIsPlayer01)
            {
                Player = gameObject;
                Debug.Log("PlayerはこのGameObjectを参照してます:Number2131");

            }
            else
            {
                Player = Player01;
            }

            Player_pos = GetComponent<Transform>().position;

        }

        // Update is called once per frame
        void Update()
        {
            DoingWalking();


        }

        void DoingWalking()
        {

            Player.transform.position += WalkingSpeed * myInputCo4135.updatePosition;


        }

        void DoingWalking2()
        {

            Player.transform.Translate(Vector3.right * WalkingSpeed * Time.deltaTime);


        }
    }
}
