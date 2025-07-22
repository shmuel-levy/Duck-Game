using UnityEngine;

public class BulletSelfDestruct : MonoBehaviour
{
    public float lifeTime = 2f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
} 