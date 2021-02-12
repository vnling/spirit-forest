using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotspot : MonoBehaviour
{
    private GameObject player;
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main.transform.parent.gameObject;
    }

    public void OnPointerClick() 
    {
        //move from one hotspot to another
        float distance = Vector3.Distance(transform.position, player.transform.position);
        LeanTween.move(player, transform.position, distance/12f);
        //play audio
        StartCoroutine("move");
    }

    IEnumerator move() {
        yield return new WaitForSeconds(1.7f);
        audioSource.PlayOneShot(audioClip);
        
    }
}
