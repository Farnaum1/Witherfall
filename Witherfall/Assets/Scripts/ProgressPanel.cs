using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressPanel : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI progressNum;
    [SerializeField] RectTransform sliderHandle;

    private int progressValue = 0;

    private GameController gameController;

    private Vector2 sliderhandldePos;


    // Start is called before the first frame update
    void Start()
    {

        // Find the GameController in the scene
        gameController = FindObjectOfType<GameController>();


    }

    // Update is called once per frame
    void Update()
    {
        // Update the progress text if the value has changed
        progressValue = gameController.progressAmount;

        // Show the progress number percentage
        progressNum.text = progressValue.ToString() + "%";
    }

    private void LateUpdate()
    {
        sliderhandldePos = sliderHandle.position;

        gameObject.transform.position = new Vector2(gameObject.transform.position.x, sliderhandldePos.y);
    }

 
    


}
