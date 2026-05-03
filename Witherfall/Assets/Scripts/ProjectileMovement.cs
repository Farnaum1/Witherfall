using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float lifetime = 3f;
    [SerializeField] LayerMask destroyLayers;

    [Header("Spin")]
    [SerializeField] float spinDegreesPerSecond = 720f;

    [Header("ParticleFx")]
    public ParticleSystem soulCrushFX;

    public Vector2 direction;

    void OnEnable()
    {
        PlayerMovement.OnShoot += ReceiveShot;
    }

    void OnDisable()
    {
        PlayerMovement.OnShoot -= ReceiveShot;
    }

    public void ReceiveShot(ProjectileMovement proj ,Vector2 dir)
    {
        if (proj != this) return;

        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        //Functionally this is the same movement as normal translate, but it removes any dependency on the object’s transform axes.
        //So my spin wont be messed up anymore.
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        Spin();
    }

    private void Spin()
    {
        if (direction.x > 0f)
        {
            transform.Rotate(Vector3.back, spinDegreesPerSecond * Time.deltaTime, Space.Self);
        }
           
        else
        {
            transform.Rotate(Vector3.forward, spinDegreesPerSecond * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        // Check if the other's layer is one of the layers in destroyLayers
        if ((destroyLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            Instantiate(soulCrushFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (other.CompareTag("Enemy") && enemy != null)
        {
            enemy.Die();
            Destroy(gameObject);

        }
    }


}
