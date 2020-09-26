using UnityEngine;

public class FarmZoneProxy
    : MonoBehaviour
{
    public ClaimBoardProxy claimBoardProxy;
    public ShippingBoxProxy shippingBoxProxy;
    //public FarmGateProxy farmGateProxy;

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (GetComponent<Collider2D>() == null)
            Debug.LogErrorFormat(gameObject, "FarmZone: missing Collider2D");
        if (claimBoardProxy == null)
            Debug.LogWarningFormat(gameObject, "FarmZone: maybe missing claim board?");
        if (shippingBoxProxy == null)
            Debug.LogWarningFormat(gameObject, "FarmZone: maybe missing shipping box?");
    }
#endif
}
