using UnityEngine;

public class MortyHit : Projectile
{
    [SerializeField]
    private GameObject explosion;
    [SerializeField]
    private float radius;

    private GameObject rune;

    private bool exploded = false;

    public void SetTargetRune(GameObject rune) { this.rune = rune; }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y > -3) this.transform.position += Vector3.down * Time.deltaTime * this.speed;
        else if (!exploded)
        {
            exploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        float dist = Vector3.Distance(PlayerState.player.position, this.transform.position);
        if (dist < radius)
            PlayerController.TakeDamage(attack);
        Instantiate(explosion, this.transform.position, Quaternion.identity);
        Destroy(this.rune);
        Destroy(this.gameObject, 1.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(this.transform.position.x, 0, this.transform.position.y), radius);
    }
}
