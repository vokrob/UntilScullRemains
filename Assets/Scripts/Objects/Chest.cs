using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Настройки сундука")]
    [SerializeField] private GameObject lootPrefab;

    private Animator animator;
    private Collider2D chestCollider;
    private bool isOpen = false;
    private bool playerInRange = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        chestCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other is CapsuleCollider2D && other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other is CapsuleCollider2D && other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void OpenChest()
    {
        isOpen = true;
        animator.SetBool("isOpen", true);

        if (lootPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);
            Instantiate(lootPrefab, spawnPos, Quaternion.identity);
        }

        chestCollider.enabled = false;
    }
}