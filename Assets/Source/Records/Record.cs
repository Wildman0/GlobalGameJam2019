using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record : MonoBehaviour
{
    GameObject player;
    bool isPlaying;
    [SerializeField] AudioSource src;

    private void Start()
    {
        src = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void PlayRecord()
    {
        src.Play();
        isPlaying = true;
    }

    public void StopRecord()
    {
        src.Stop();
        isPlaying = false;
    }
}
