using UnityEngine;

public class HugoAura : Projectile
{
    [SerializeField]
    private GameObject fireRing;
    [SerializeField]
    private GameObject distMarker;

    private float nextBurn = 0.0f;

    private float spawnTime;

    private void Start()
    {
        this.spawnTime = Time.time + 0.8f;
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time < spawnTime) return;

        fireRing.transform.localScale += new Vector3(1, 0, 1) * this.speed * Time.deltaTime;

        float ringRadius = Vector3.Distance(this.transform.position, distMarker.transform.position);
        if (PlayerWithinReach() && Time.time > nextBurn)
        {
            PlayerController.TakeDamage(this.attack);
            nextBurn = Time.time + 0.25f;
        }
    }

    private bool PlayerWithinReach()
    {
        float radius = Vector3.Distance(this.transform.position, distMarker.transform.position);
        float playerDist = Vector3.Distance(this.transform.position, PlayerController.instance.transform.position);

        return Mathf.Abs(radius - playerDist) <= 0.1f
            || Vector3.Distance(this.transform.position, PlayerController.instance.transform.position) <= 10f;
    }
}
