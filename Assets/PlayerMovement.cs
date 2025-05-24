using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    //boom define variables
    public float moveSpeed = 10f;
    public float jumpForce = 5f;
    public float deathCounter = 0f;

    //playerPrefab, just used for clone
    public GameObject playerPrefab;      
    public Transform spawnPoint;

    private Rigidbody2D rb;

    private bool isGrounded = false;
    private int groundedSources = 0;

    private List<Vector2> positionHistory = new List<Vector2>();
    private float recordInterval = 0.05f;
    private float lastRecordTime = 0f;

    private static GameObject activeClone = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("I CANT JUMP SOMETIMES WHY?");
            Debug.Log(isGrounded);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (Time.time - lastRecordTime > recordInterval)
        {
            positionHistory.Add(transform.position);
            lastRecordTime = Time.time;
        }

        if (rb.position.y < -100)
        {
            StartCoroutine(DieAndRespawn(false)); // No clone on fall death
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            StartCoroutine(DieAndRespawn(true));
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                groundedSources++;
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        groundedSources--;
        if (groundedSources <= 0)
        {
            groundedSources = 0;
            isGrounded = false;
        }
    }


    IEnumerator DieAndRespawn(bool spawnClone)
    {
        yield return new WaitForSeconds(0.1f);

        if (spawnClone && positionHistory.Count > 0)
        {
            if (activeClone != null)
            {
                Destroy(activeClone);
            }
            //create clone, Quaternion.identity just means no rotation bascially
            activeClone = Instantiate(playerPrefab, transform.position, Quaternion.identity);
            Destroy(activeClone.GetComponent<PlayerMovement>());
            activeClone.tag = "Clone";
            activeClone.GetComponent<SpriteRenderer>().color = Color.green;
            activeClone.AddComponent<ReverseClone>().SetPath(new List<Vector2>(positionHistory));
        }

        // go back to spawn
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint.position;

        deathCounter++;
        positionHistory.Clear();

        Debug.Log("Player died" + (spawnClone ? " and clone spawned." : ""));
    }
}
