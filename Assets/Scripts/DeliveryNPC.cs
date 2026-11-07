using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DeliveryNPC : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Punto (hijo del NPC, delante del pecho) donde se coloca la caja mientras la lleva.")]
    [SerializeField] Transform carryPoint;
    [Tooltip("Opcional. Si lo dejas vacío no se usan animaciones.")]
    [SerializeField] Animator animator;

    [Header("Animator (opcional)")]
    [Tooltip("Parámetro float con la velocidad. Déjalo vacío si no tienes uno.")]
    [SerializeField] string speedParam = "Speed";
    [Tooltip("Parámetro bool para 'lleva caja'. Déjalo vacío si no tienes uno.")]
    [SerializeField] string carryingParam = "Carrying";

    [Header("Movimiento")]
    [SerializeField] float turnSpeed = 360f;
    [Tooltip("Segundos máximos para llegar a un destino antes de rendirse (evita bloqueos).")]
    [SerializeField] float walkTimeout = 30f;

    NavMeshAgent agent;

    public Transform CarryPoint => carryPoint;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (animator != null && !string.IsNullOrEmpty(speedParam))
            animator.SetFloat(speedParam, agent.velocity.magnitude);
    }

    public void SetCarrying(bool carrying)
    {
        if (animator != null && !string.IsNullOrEmpty(carryingParam))
            animator.SetBool(carryingParam, carrying);
    }

    public void WarpTo(Transform point)
    {
        agent.Warp(point.position);
        transform.rotation = point.rotation;
    }

    public IEnumerator WalkTo(Vector3 target)
    {
        agent.isStopped = false;
        agent.SetDestination(target);

        float t = 0f;
        while (t < walkTimeout)
        {
            t += Time.deltaTime;
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
                break;
            yield return null;
        }

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    public IEnumerator FaceTowards(Vector3 worldPoint)
    {
        Vector3 dir = worldPoint - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) yield break;

        Quaternion target = Quaternion.LookRotation(dir);
        while (Quaternion.Angle(transform.rotation, target) > 1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = target;
    }
}
