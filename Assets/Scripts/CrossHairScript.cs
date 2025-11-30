using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrossHairScript : MonoBehaviour
{

    RectTransform rectTransform;

    void OnEnable()
    {
        Cursor.visible = false;
    }
    void OnDisable()
    {
        Cursor.visible = true;
    }

    void Start()
    {
        this.rectTransform = GetComponent<RectTransform>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        rectTransform.position = Mouse.current.position.ReadValue();
    }

}
