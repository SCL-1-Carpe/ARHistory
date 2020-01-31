using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player01Be : MonoBehaviour
{
    [SerializeField] float WalkingSpeed=3;
    [SerializeField] GameObject Player01;
    [SerializeField] bool ThisIsPlayer01=false;
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
        if (Input.GetKey(KeyCode.W))
        {
            DoingWalking();
        }

        if (Input.GetKey(KeyCode.D))
        {
            DoingWalking2();
        }

        Vector3 diff = transform.position - Player_pos; //プレイヤーがどの方向に進んでいるかがわかるように、初期位置と現在地の座標差分を取得

        if (diff.magnitude > 0.01f) //ベクトルの長さが0.01fより大きい場合にプレイヤーの向きを変える処理を入れる(0では入れないので）
        {
            transform.rotation = Quaternion.LookRotation(diff);  //ベクトルの情報をQuaternion.LookRotationに引き渡し回転量を取得しプレイヤーを回転させる
        }

        Player_pos = transform.position; //プレイヤーの位置を更新


    }

    void DoingWalking()
    {

        Player.transform.Translate(Vector3.forward * WalkingSpeed * Time.deltaTime);


    }

    void DoingWalking2()
    {

        Player.transform.Translate(Vector3.right * WalkingSpeed * Time.deltaTime);


    }
}
