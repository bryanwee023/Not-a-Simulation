using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HugoBomb : EnemyController
{
    private static List<HugoBomb> inMap = new List<HugoBomb>();

    public static void KillAll()
    {
        List<HugoBomb> copy = new List<HugoBomb>(inMap);
        foreach (HugoBomb hb in copy)
            hb.Die();
    }

    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private GameObject skull;

    private void Awake()
    {
        this.stats = this.GetComponent<EnemyStats>();
        this.hbManager = this.GetComponentInChildren<HealthbarManager>();

        this.ball.SetActive(true);
        this.skull.SetActive(false);
    }

    private void Start()
    {
        this.target = PlayerState.player;

        inMap.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!activated && this.transform.position.y > 2)
        {
            this.transform.position -= 2 * this.speed * Time.deltaTime * Vector3.up;
            if (this.transform.position.y <= 2)
                Invoke("Activate", 3f);
        }
        else if (activated)
        {
            if (PlayerWithinReach())
            {
                PlayerController.TakeDamage(this.stats.attack);
                Die();
            } else if (PlayerWithinReach(3 * this.attackRadius))
            {
                this.FaceTarget(20);
                this.transform.position += this.speed * Time.deltaTime * this.transform.forward;
            } else
            {
                this.FaceTarget();
                this.transform.position += this.speed * Time.deltaTime * this.transform.forward;
            }
        }
    }

    private void Activate()
    {
        this.activated = true;
        this.ball.SetActive(false);
        this.skull.SetActive(true);

        Vector3 position = this.transform.position;
        position.y = 2;
        this.transform.position = position;
    }

    public override void Die()
    {
        Instantiate(this.explosion, this.transform.position, this.transform.rotation);
        Destroy(this.gameObject);

        inMap.Remove(this);
    }

    public override void TakeDamage(int damage, bool stagger=true)
    {
        if (this.stats.TakeDamage(damage) <= 0)
        {
            this.Die();
        }
    }

    public override void TakeDamage(int damage, Color color, bool stagger = true)
    {
        this.TakeDamage(damage, stagger);
    }

}
