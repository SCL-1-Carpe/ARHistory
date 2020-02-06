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
        [SerializeField] float MukiKando = 0.01f;
        [SerializeField] GameObject MainCamera;


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

            Vector3 diff = transform.position - Player_pos;


            if (diff.magnitude > MukiKando) //ベクトルの長さが0.01fより大きい場合にプレイヤーの向きを変える処理を入れる(0では入れないので）
            {
                transform.rotation = Quaternion.LookRotation(diff);  //ベクトルの情報をQuaternion.LookRotationに引き渡し回転量を取得しプレイヤーを回転させる

                Player_pos = transform.position; //プレイヤーの位置を更新
            }

           
        }

        void DoingWalking()
        {
            Vector3 AA = MainCamera.transform.rotation * myInputCo4135.updatePosition * Time.deltaTime * WalkingSpeed;
            AA.y = 0;
            Player.transform.position += AA;
        }

        void DoingWalking2()
        {

            Player.transform.Translate(Vector3.right * WalkingSpeed * Time.deltaTime);


        }
    }
}
