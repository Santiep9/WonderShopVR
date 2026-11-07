using UnityEngine;

public class SignaturePen : MonoBehaviour
{
    [SerializeField] Transform tip;

    public Vector3 TipPosition => tip != null ? tip.position : transform.position;
}
