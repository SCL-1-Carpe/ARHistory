using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingAnimatior : MonoBehaviour
{
    public List<SamuraiAnimationTrigger> anims;
    // Start is called before the first frame update
    public void Play()
    {
        anims.ForEach((a) => a.StartAnimation());
    }
}
