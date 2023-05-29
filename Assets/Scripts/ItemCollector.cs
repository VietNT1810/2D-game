using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int PineApples = 0;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private AudioSource collectSound;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PineApple"))
        {
            Destroy(collision.gameObject);
            PineApples++;
            Debug.Log("PineApples: " + PineApples);
            scoreText.text = $"Pine Apples:{PineApples}";
            //collectSound.Play();
            audioManager.PlaySFX(audioManager.fruit);
        }
    }
}
