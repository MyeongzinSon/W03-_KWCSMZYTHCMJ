using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StageResultIndicator : MonoBehaviour
{
    [SerializeField] float tweeningDuration = 0.25f;
    SpriteRenderer spriteRenderer;
    Color defaultColor;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = Color.white;
        defaultColor.a = 0;
        spriteRenderer.color = defaultColor;
    }
    public void ShowResult(bool isCleared)
    {
        DOTween.Kill(spriteRenderer);

        var startColor = isCleared ? Color.green : Color.red;
        var endColor = new Color(startColor.r, startColor.g, startColor.b, 0.5f);
        spriteRenderer.color = startColor;
        spriteRenderer.DOColor(endColor, tweeningDuration);
    }
    public void ClearResult()
    {
        DOTween.Kill(spriteRenderer);
        spriteRenderer.color = defaultColor;
    }
}
