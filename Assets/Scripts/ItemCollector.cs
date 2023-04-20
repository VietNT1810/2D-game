using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int PineApples = 0;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private AudioSource collectSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PineApple"))
        {
            Destroy(collision.gameObject);
            PineApples++;
            Debug.Log("PineApples: " + PineApples);
            scoreText.text = $"Pine Apples:{PineApples}";
            collectSound.Play();
        }
    }
}
