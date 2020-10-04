using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBundleCharacterData
    : MonoBehaviour
{
    public Sprite[] idleW;
    public Sprite[] idleE;
    public Sprite[] idleN;
    public Sprite[] idleS;

    public Sprite[] walkW;
    public Sprite[] walkE;
    public Sprite[] walkN;
    public Sprite[] walkS;

    public Sprite[] actionW;
    public Sprite[] actionE;
    public Sprite[] actionN;
    public Sprite[] actionS;

    public Sprite[] exhaustW;
    public Sprite[] exhaustE;
    public Sprite[] exhaustN;
    public Sprite[] exhaustS;

    public bool animAutoFlip;
    public bool animAutoFlipInvert;

    public float animationSpeedMultiplier;
}
