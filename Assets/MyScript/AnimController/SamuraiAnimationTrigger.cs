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
    [SerializeField]
    bool LaunchOnStart;
    [SerializeField]
    Color red, blue;
    // Start is called before the first frame update
    void Start()
    {
        red_Inst = Instantiate(samurai_Red,transform);
        red_Inst.transform.position = transform.position + offset;
        red_Inst.transform.localEulerAngles = new Vector3(0, 180, 0);
        red_Inst.GetComponentInChildren<MeshRenderer>().material.color = red;
        blue_Inst = Instantiate(samurai_Blue,transform);
        blue_Inst.transform.position = transform.position - offset;
        blue_Inst.GetComponentInChildren<MeshRenderer>().material.color = blue;
        red_Inst.SetActive(false);
        blue_Inst.SetActive(false);
        if(LaunchOnStart)
        StartAnimation();
    }

    // Update is called once per frame
    public void StartAnimation()
    {
        red_Inst.SetActive(true);
        blue_Inst.SetActive(true);
    }
}
