using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
public class SpriteSortY_MenuHelper
{
    [MenuItem("Custom/Sort Sprites")]
    public static void MenuSortAll()
    {
        foreach (var obj in GameObject.FindObjectsOfType<SpriteSortY>())
        {
            Undo.RecordObject(obj, "Sort");
            obj.OnValidate();
            EditorUtility.SetDirty(obj);
        }

        foreach (var obj in GameObject.FindObjectsOfType<SpriteSortY_Offset>())
        {
            Undo.RecordObject(obj, "Sort Offset");
            obj.OnValidate();
            EditorUtility.SetDirty(obj);
        }

        foreach (var obj in GameObject.FindObjectsOfType<SpriteSortY_Static>())
        {
            Undo.RecordObject(obj, "Sort Static");
            obj.OnValidate();
            EditorUtility.SetDirty(obj);
        }
    }
}
#endif


public class SpriteSortY_Static
    : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

#if UNITY_EDITOR
    public void OnValidate()
    {
        var editorSpriteRenderer = GetComponent<SpriteRenderer>();

        editorSpriteRenderer.sortingOrder = (int)(transform.position.y * -SpriteSortY.SpriteSortScale);
    }
#endif

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = (int)(transform.position.y * -SpriteSortY.SpriteSortScale);
    }
}
