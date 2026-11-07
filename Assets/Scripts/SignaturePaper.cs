using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SignaturePaper : MonoBehaviour
{
    [Tooltip("Metros que debe recorrer la punta del boli dentro del papel para dar la firma por buena.")]
    [SerializeField] float requiredStrokeLength = 0.25f;
    [Tooltip("Opcional: objeto visual (p. ej. un 'tick' o sello) que se activa al firmar.")]
    [SerializeField] GameObject signedMarker;

    public bool IsSigned { get; private set; }
    public bool IsActive { get; private set; }
    public event Action Signed;

    float accumulated;
    Vector3 lastTipPos;
    bool hasLast;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    public void Activate()
    {
        IsActive = true;
        IsSigned = false;
        accumulated = 0f;
        hasLast = false;
        if (signedMarker != null) signedMarker.SetActive(false);
    }

    public void Deactivate()
    {
        IsActive = false;
        hasLast = false;
        if (signedMarker != null) signedMarker.SetActive(false);
    }

    void OnTriggerStay(Collider other)
    {
        if (!IsActive || IsSigned) return;

        var pen = other.GetComponentInParent<SignaturePen>();
        if (pen == null) return;

        Vector3 tip = pen.TipPosition;
        if (hasLast)
            accumulated += Vector3.Distance(tip, lastTipPos);

        lastTipPos = tip;
        hasLast = true;

        if (accumulated >= requiredStrokeLength)
            CompleteSignature();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<SignaturePen>() != null)
            hasLast = false;
    }

    [ContextMenu("Firmar ahora (debug)")]
    public void CompleteSignature()
    {
        if (!IsActive || IsSigned) return;
        IsSigned = true;
        if (signedMarker != null) signedMarker.SetActive(true);
        Signed?.Invoke();
    }
}
