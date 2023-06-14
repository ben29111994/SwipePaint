using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public Color defaultColor;
    public bool isFilled;

    private Renderer meshRenderer;
    private MaterialPropertyBlock propBlock;
    private Collider collider;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        isFilled = false;
        collider.enabled = true;
        SetColor(defaultColor);
    }

    private void Init()
    {
        collider = GetComponent<Collider>();

        propBlock = new MaterialPropertyBlock();

        if (meshRenderer == null)
            meshRenderer = GetComponent<Renderer>();
    }

    private void SetColor(Color inputColor)
    {
        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", inputColor);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    public void PaintColor(Color inputColor)
    {
        if(isFilled == false)
        {
            isFilled = true;
            GameManager.instance.paintAmount++;
            GameManager.instance.CheckComplete();
        }

        SetColor(inputColor);
    }
    
}
