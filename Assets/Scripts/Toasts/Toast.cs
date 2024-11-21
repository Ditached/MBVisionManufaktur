using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ToastType
{
    Info,
    Warning,
    Error
}

public class Toast : MonoBehaviour
{
    public TMP_Text label;
    
    public Color infoColor;
    public Color warningColor;
    public Color errorColor;
    public RawImage backgroundImage;

    public CanvasGroup canvasGroup;

    public void Init(string text, float lifetime, ToastType type = ToastType.Info)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.25f);
        
        switch (type)
        {
            case ToastType.Info:
                backgroundImage.color = infoColor;
                break;
            case ToastType.Warning:
                backgroundImage.color = warningColor;
                break;
            case ToastType.Error:
                backgroundImage.color = errorColor;
                break;
        }
        
        label.text = text;
        canvasGroup.DOFade(0f, 0.25f).SetDelay(lifetime).OnComplete(() => Destroy(gameObject));
    }

}
    
