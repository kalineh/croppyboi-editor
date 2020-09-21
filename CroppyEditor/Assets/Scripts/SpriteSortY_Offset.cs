using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class SpriteSortY_Offset
    : MonoBehaviour
{
    public float offset;

    private SpriteRenderer spriteRenderer;
    private IEnumerator funcUpdate;

#if UNITY_EDITOR
    public void OnValidate()
    {
        var editorSpriteRenderer = GetComponent<SpriteRenderer>();

        editorSpriteRenderer.sortingOrder = (int)(transform.position.y * -SpriteSortY.SpriteSortScale) + (int)(offset * SpriteSortY.SpriteSortScale);
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
            spriteRenderer.sortingOrder = (int)(transform.position.y * -SpriteSortY.SpriteSortScale) + (int)(offset * SpriteSortY.SpriteSortScale);
            yield return null;
        }
    }

    public void OnDrawGizmosSelected()
    {
        var pos = transform.position;
        var sprite = GetComponent<SpriteRenderer>();
        var center = sprite.bounds.center;
        var ofs = center - Vector3.up * offset;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, center);
        Gizmos.DrawWireSphere(pos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, 0.1f);
        Gizmos.DrawLine(pos, center);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ofs, 0.1f);
        Gizmos.DrawLine(center, ofs);
    }
}
