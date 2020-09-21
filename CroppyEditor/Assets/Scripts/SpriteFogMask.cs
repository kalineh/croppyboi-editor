using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class SpriteFogMask
    : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private static List<SpriteFogMask> references;

    public static void NotifyFogOn()
    {
        foreach (var reference in references)
            reference.MaskInside();
    }

    public static void NotifyFogOff()
    {
        foreach (var reference in references)
            reference.MaskNone();
    }

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnEnable()
    {
        if (references == null)
            references = new List<SpriteFogMask>();

        references.Remove(this);
        references.Add(this);
    }

    public void OnDisable()
    {
        references.Remove(this);
    }

    public void OnDestroy()
    {
        references.Remove(this);
    }

    public void MaskNone()
    {
        spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
    }

    public void MaskInside()
    {
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
}
