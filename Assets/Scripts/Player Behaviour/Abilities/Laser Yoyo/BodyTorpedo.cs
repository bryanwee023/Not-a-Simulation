using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special", menuName = "Ability/Laser Yoyo/Special/Torpedo")]
public class BodyTorpedo : YoyoGrapple
{

    [Header("Power")]
    [SerializeField]
    private GameObject torpedo;

    protected override IEnumerator Grapple(Vector3 dest)
    {
        GameObject tempTorpedo = Instantiate(torpedo, player);
        tempTorpedo.GetComponent<Torpedo>().attack = (int)attributes["torpedoDamage"];
        yield return base.Grapple(dest);
        Destroy(tempTorpedo);
    }
}