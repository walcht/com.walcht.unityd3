using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    float m_cached_x_scale;
    void OnEnable()
    {
        m_cached_x_scale = transform.localScale.x;
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    void Update()
    {
        gameObject.transform.LookAt(Camera.main.transform);
    }
    void OnDisable()
    {
        transform.localScale = new Vector3(
            m_cached_x_scale,
            transform.localScale.y,
            transform.localScale.z
        );
    }
}