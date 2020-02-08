using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiAnimationTrigger : MonoBehaviour
{
    [SerializeField]
    GameObject samurai_Red,samurai_Blue;
    GameObject red_Inst, blue_Inst;
    [SerializeField]
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        red_Inst = Instantiate(samurai_Red,transform);
        red_Inst.transform.position = transform.position + offset;
        blue_Inst = Instantiate(samurai_Blue,transform);
        blue_Inst.transform.position = transform.position - offset;
        red_Inst.SetActive(false);
        blue_Inst.SetActive(false);
    }

    // Update is called once per frame
    public void StartAnimation()
    {

    }
}
