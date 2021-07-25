using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spread Shot", menuName = "Ability/Laser Yoyo/Attack/Spread")]
public class SpreadShot : YoyoStrike
{
    public override void Initialize()
    {
        base.Initialize();

        GameObject parent = new GameObject("Spread Indicator");
        parent.transform.SetParent(player);
        
        for (int i = -1; i <= 1; i++)
        {
            GameObject indicator = Instantiate(this.aimIndicator, player);
            indicator.transform.SetParent(parent.transform);
            indicator.transform.localScale = new Vector3(0.75f, 1, this.range / 20);
            indicator.transform.Rotate(0, i * 40, 0);
            indicator.GetComponentInChildren<Animator>().speed = 1 / attributes["chargeTime"];
        }

        Destroy(this.tempIndicator.gameObject);
        this.tempIndicator = parent;
        this.tempIndicator.SetActive(false);
    }

    protected override Collider[] Sweep(Vector2 dimensions)
    {
        List<Collider> retList = new List<Collider>();

        for (int i = -1; i <= 1; i++)
        {
            Ray ray  = new Ray(player.position + Vector3.up, Quaternion.Euler(0, i * 40, 0) * player.transform.forward);
            foreach (RaycastHit hit in Physics.SphereCastAll(ray, 3, dimensions.y))
            {
                retList.Add(hit.collider);
            }
        }

        return retList.ToArray();
        /*
        HashSet<Collider> retSet = new HashSet<Collider>();

        for (int i = -1; i <= 1; i++)
        {
            foreach (Collider c in Physics.OverlapBox(
                player.position + PlayerState.player.forward * dimensions.y / 2 + 3 * Vector3.up,
                new Vector3(0.75f * dimensions.x / 2, 3, dimensions.y / 2),
                Quaternion.Euler(0, i* 40, 0) * player.rotation
            )) retSet.Add(c);
      }

        Collider[] ret = new Collider[retSet.Count];
        retSet.CopyTo(ret);
        
        return ret;
        */
    }

    protected override void PlayShootVFX(float normalizedRange)
    {
        this.animator.SetTrigger("Shoot");
        this.tempCharge.SetActive(false);
        this.tempIndicator.SetActive(false);

        for (int i = -1; i <= 1; i++)
        {
            Quaternion rot = Quaternion.Euler(0, i * 40, 0) * player.rotation;
            GameObject strike = Instantiate(this.strikeVFX, player.position, rot);
            strike.transform.localScale = Vector3.Scale(strike.transform.localScale, new Vector3(1, 1, normalizedRange));    //pre-built values
        }
    }
}
