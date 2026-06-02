using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoneSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    [SerializeField] private CinemachineVirtualCamera secretCamera;
    [SerializeField] private float musicFadeTime = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Switch priority to make the secret camera the active one
            mainCamera.Priority = 0;
            secretCamera.Priority = 10;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMusicWithFade(AudioManager.Instance.secretAreaMusic,
                    musicFadeTime);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Switch priority back
            mainCamera.Priority = 10;
            secretCamera.Priority = 0;

            AudioManager.Instance.PlayMusicWithFade(AudioManager.Instance.mainMusic,
                    musicFadeTime);
        }
    }
}
