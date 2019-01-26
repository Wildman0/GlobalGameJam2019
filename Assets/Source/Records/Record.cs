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

    void Update()
    {
        if (isPlaying)
        {
            src.volume = Mathf.Clamp(2 / (Vector3.Distance(player.transform.position, gameObject.transform.position)), 0, 1);

            if (src.volume < 0.1f)
                src.volume = 0.0f;
        }
    }
}
