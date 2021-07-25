using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
//    public delegate void UnlockDelegate();
//    public delegate void UpgradeDelegate(float value);

//    private static Dictionary<string, UnlockDelegate> unlockLibrary = new Dictionary<string, UnlockDelegate>();
//    private static Dictionary<string, UpgradeDelegate> upgradeLibrary = new Dictionary<string, UpgradeDelegate>();

    protected Transform player;
    protected Animator animator;
    protected Transform mesh;

    public Dictionary<string, float> attributes = new Dictionary<string, float>();

    public virtual void Initialize()
    {
        this.player = PlayerState.player;
        this.animator = PlayerController.instance.animator;
        this.mesh = PlayerController.instance.mesh;
    }

    public abstract void Trigger();

    public virtual void Terminate() { }

    protected Vector3 GetAimDirection()
    {
        Vector3 temp;
        if (CameraRig.MouseToWorldPosition(out temp))
        {
            Vector3 dir = (temp - PlayerState.player.position).normalized;
            return Vector3.Scale(dir, new Vector3(1, 0, 1));
        }           
        else
        {
            Vector3 mousePos = Input.mousePosition;
            return Quaternion.Euler(0, 225, 0) * new Vector3(mousePos.x - Screen.width / 2, 0, mousePos.y - Screen.height / 2);
        }
    }

    protected virtual Collider[] Sweep(Vector2 dimensions)
    {
        return Physics.OverlapBox(
            player.position + PlayerState.player.forward * dimensions.y / 2 + 3 * Vector3.up,
            new Vector3(dimensions.x / 2, 3, dimensions.y / 2),
            player.rotation
        );
    }

    protected Collider[] RadialSweep(float radius)
    {
        return Physics.OverlapSphere(player.position + 3 * Vector3.up, radius);
    }

    protected void CastPower(GameObject power, Vector3 position, float damage)
    {
        if (power == null) return;

        Power powerClone = Instantiate(power, position, Quaternion.identity).GetComponent<Power>();
        powerClone.SetDamage((int)damage);
    }

    protected void LungeForward(float force, float duration)
    {
        PlayerController.instance.GetComponent<PushBody>().Push(player.rotation * Vector3.forward, force, duration);
    }

    protected void PushEnemy(Collider enemy, float force, float duration)
    {
        PushBody pb = enemy.GetComponent<PushBody>();
        if (pb != null) pb.Push(player.forward, force, duration);
    }

    public void PortAttributes(Ability old)
    {
        this.attributes = old.attributes;
    }

    public void SetAttribute(string attributeName, float value)
    {
        this.attributes[attributeName] = value;
    }

    //For debugging purposes
    public void PrintAttributes()
    {
        string log = "";
        foreach (KeyValuePair<string, float> pair in this.attributes)
            log += string.Format("[{0}, {1}], ", pair.Key, pair.Value);

        Debug.Log(log);
    }
}
