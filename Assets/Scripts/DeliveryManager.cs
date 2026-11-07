using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryManager : MonoBehaviour
{
    [Header("Escena")]
    [SerializeField] DeliveryNPC npc;
    [Tooltip("Punto justo en la puerta (o fuera) donde aparece y desaparece el repartidor.")]
    [SerializeField] Transform doorPoint;
    [Tooltip("Donde se queda de pie el repartidor frente al mostrador.")]
    [SerializeField] Transform counterStandPoint;
    [Tooltip("Donde queda la caja encima del mostrador.")]
    [SerializeField] Transform counterDropPoint;
    [SerializeField] SignaturePaper paper;

    [Header("Paquetes")]
    [Tooltip("Prefabs de caja. Cada entrega escoge uno al azar.")]
    [SerializeField] GameObject[] boxPrefabs;
    [SerializeField] float delayBetweenDeliveries = 10f;
    [SerializeField] bool startAutomatically = true;

    [Header("Diálogo")]
    [SerializeField] string[] arrivalLines = { "Paquete para usted. Firme aquí, por favor." };
    [SerializeField] string[] reminderLines = { "Solo necesito su firma en el papel." };
    [SerializeField] string[] goodbyeLines = { "Gracias, que tenga buen día." };
    [Tooltip("Segundos sin firmar antes de que el repartidor repita el aviso.")]
    [SerializeField] float reminderInterval = 15f;
    [Tooltip("Conecta aquí tu texto en pantalla, subtítulos o audio. Recibe cada frase.")]
    public UnityEvent<string> onNpcSpeak;

    [Header("Eventos")]
    public UnityEvent<GameObject> onBoxDelivered;
    public UnityEvent onDeliveryFinished;

    GameObject currentBox;

    void Start()
    {
        if (npc != null) npc.gameObject.SetActive(false);
        if (startAutomatically) StartCoroutine(DeliveryLoop());
    }

    public void BeginDeliveries()
    {
        StopAllCoroutines();
        StartCoroutine(DeliveryLoop());
    }

    IEnumerator DeliveryLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenDeliveries);
            yield return DoDelivery();
        }
    }

    IEnumerator DoDelivery()
    {
        npc.gameObject.SetActive(true);
        npc.WarpTo(doorPoint);
        SpawnCarriedBox();
        npc.SetCarrying(true);

        yield return npc.WalkTo(counterStandPoint.position);
        yield return npc.FaceTowards(counterDropPoint.position);

        PlaceBoxOnCounter();
        npc.SetCarrying(false);
        yield return new WaitForSeconds(0.5f);

        Say(arrivalLines);
        paper.Activate();

        float timer = 0f;
        while (!paper.IsSigned)
        {
            timer += Time.deltaTime;
            if (timer >= reminderInterval)
            {
                timer = 0f;
                Say(reminderLines);
            }
            yield return null;
        }

        Say(goodbyeLines);
        yield return new WaitForSeconds(1f);
        paper.Deactivate();

        yield return npc.WalkTo(doorPoint.position);
        npc.gameObject.SetActive(false);

        onDeliveryFinished?.Invoke();
    }

    void SpawnCarriedBox()
    {
        var prefab = boxPrefabs[Random.Range(0, boxPrefabs.Length)];
        currentBox = Instantiate(prefab, npc.CarryPoint);
        currentBox.transform.localPosition = Vector3.zero;
        currentBox.transform.localRotation = Quaternion.identity;

        SetBoxPhysics(currentBox, carried: true);
    }

    void PlaceBoxOnCounter()
    {
        currentBox.transform.SetParent(null);
        currentBox.transform.SetPositionAndRotation(counterDropPoint.position, counterDropPoint.rotation);
        SetBoxPhysics(currentBox, carried: false);
        onBoxDelivered?.Invoke(currentBox);
        currentBox = null;
    }

    static void SetBoxPhysics(GameObject box, bool carried)
    {
        foreach (var col in box.GetComponentsInChildren<Collider>())
            col.enabled = !carried;

        var rb = box.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = carried;
    }

    void Say(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;
        onNpcSpeak?.Invoke(lines[Random.Range(0, lines.Length)]);
    }
}
