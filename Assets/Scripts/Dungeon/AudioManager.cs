using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null) { Debug.Log("More than one instance of Audio Manager found!"); }
        else instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    #endregion

    public AudioSource skillButton;
    public AudioSource hit;
    public AudioSource BGM;
    public AudioSource ambientNoise;
    public AudioSource spawn;
    public AudioSource consumeHealth;
    public AudioSource grapplingHit;
    public AudioSource grapplingReel;
}
