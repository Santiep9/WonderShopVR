using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [Tooltip("Objeto que se activa/desactiva (normalmente el propio Canvas o su panel).")]
    [SerializeField] GameObject bubbleRoot;
    [SerializeField] float secondsVisible = 4f;
    [Tooltip("Si está vacío usa Camera.main (la cámara del XR Origin).")]
    [SerializeField] Transform lookAtTarget;

    Coroutine hideRoutine;

    void Awake()
    {
        if (bubbleRoot != null) bubbleRoot.SetActive(false);
    }

    public void Show(string text)
    {
        label.text = text;
        bubbleRoot.SetActive(true);

        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfterDelay(text.Length));
    }

    IEnumerator HideAfterDelay(int length)
    {
        yield return new WaitForSeconds(Mathf.Max(secondsVisible, length * 0.06f));
        bubbleRoot.SetActive(false);
    }

    public void Hide()
    {
        if (bubbleRoot != null) bubbleRoot.SetActive(false);
    }

    void LateUpdate()
    {
        Transform target = lookAtTarget != null ? lookAtTarget : (Camera.main != null ? Camera.main.transform : null);
        if (target == null) return;

        Vector3 dir = transform.position - target.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}
