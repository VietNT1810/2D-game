using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    private GameObject currentOneWayPlatform;
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private float DisableCollisionDuration = .35f;
    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentOneWayPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        CompositeCollider2D platformCollider = currentOneWayPlatform.GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        yield return new WaitForSeconds(DisableCollisionDuration);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
