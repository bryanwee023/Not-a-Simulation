using UnityEngine;

public class SpideyAnimator : EnemyAnimator
{
    [SerializeField]
    private ParticleSystem lights;


    public void Pounce()
    {
        this.animator.SetTrigger("Pounce");
    }

    public void LightUp(float duration)
    {
        lights.Play();
        Invoke("LightDown", duration);
    }

    private void LightDown()
    {
        lights.Stop();
    }
}
