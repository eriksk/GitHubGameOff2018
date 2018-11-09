using UnityEngine;

public class RecordIcon : MonoBehaviour
{
    public LayerMask Ground;
    public TextMesh Text;
    public MeshRenderer Icon;
    public string PreText;
    public float Distance = 0;

    void Start()
    {
        SetDistance(Vector3.zero, 0f);
    }

    public void SetDistance(Vector3 startPosition, float distance)
    {
        if(distance < Distance) return;

        var position = startPosition + Vector3.forward * distance;
        position.y = 300f;

        RaycastHit hit;
        if(Physics.Raycast(position, Vector3.down, out hit, 600f, Ground, QueryTriggerInteraction.Ignore))
        {
            position.y = hit.point.y;
        }

        transform.position = position;

        Text.text = PreText + "\n" + (int)distance + "m";
        Distance = distance;

        Icon.enabled = true;
        Text.gameObject.SetActive(true);

        if(Distance < 1f)
        {
            Icon.enabled = false;
            Text.gameObject.SetActive(false);
        }
    }
}