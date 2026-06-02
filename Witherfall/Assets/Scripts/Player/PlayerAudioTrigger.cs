using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioTrigger : MonoBehaviour
{
    public void StepEvent()
    {
        AudioManager.Instance.PlayFootstep();
    }
}
