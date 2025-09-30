using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI tutorialTXT;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tutorialTXT.gameObject.SetActive(true);
        }
    }

    

}
