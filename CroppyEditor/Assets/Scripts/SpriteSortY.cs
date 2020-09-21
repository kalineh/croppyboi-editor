using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class SpriteSortY
    : MonoBehaviour
{
    public static float SpriteSortScale = 8.0f;

    private SpriteRenderer spriteRenderer;
    private IEnumerator funcUpdate;

#if UNITY_EDITOR
    public void OnValidate()
    {
        var editorSpriteRenderer = GetComponent<SpriteRenderer>();

        editorSpriteRenderer.sortingOrder = (int)(transform.position.y * -SpriteSortScale);
    }
#endif

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnBecameVisible()
    {
        funcUpdate = DoUpdate();
        StartCoroutine(funcUpdate);
    }

    public void OnBecameInvisible()
    {
        StopCoroutine(funcUpdate);
        funcUpdate = null;
    }

    public IEnumerator DoUpdate()
    {
        while (true)
        {
            spriteRenderer.sortingOrder = (int)(transform.position.y * -SpriteSortScale);
            yield return null;
        }
    }
}
