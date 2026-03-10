using UnityEngine;

public class OrbitAxe : MonoBehaviour
{
    private Transform player;
    private float angle;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private GameObject hitEffectPrefab; 
    private float damage;

    private float currentYRotation;

    public void Init(Transform player, float damage)
    {
        this.player = player;
        this.damage = damage;
        angle = 0f;
        currentYRotation = 0f;
    }

    private void Update()
    {
        angle += Time.deltaTime * speed;
        transform.position = player.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

        currentYRotation += rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(90f, currentYRotation, 0f);

        if (angle >= Mathf.PI * 2f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<EnemyStateAbstract>().takeDamage(damage);

            SoundManager.SendEvent(SoundType.SFX_AxeAttack1);

            if (hitEffectPrefab != null)
            {
                Vector3 hitPos = other.ClosestPoint(transform.position); // 가장 가까운 접점 찾기
                Instantiate(hitEffectPrefab, hitPos, Quaternion.identity);
            }
        }
    }
}