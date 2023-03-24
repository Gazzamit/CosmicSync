using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UpdateHUD : MonoBehaviour
{
    private TextMeshProUGUI _velocityTMP;
    
    // Start is called before the first frame update
    void Start()
    {
        _velocityTMP = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _velocityTMP.text = "Velocity: " + SpaceshipControls._magnitude.ToString();
    }
}
