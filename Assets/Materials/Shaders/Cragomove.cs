using UnityEngine;

public class Cragomove : MonoBehaviour
{
    public float Cargospeed = 5f;
    public Vector3 direction = Vector3.forward;

    void Update()
    {
        transform.Translate(direction * Cargospeed * Time.deltaTime);
    }
}
