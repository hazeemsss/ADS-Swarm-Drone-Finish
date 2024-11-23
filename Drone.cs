using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Drone : MonoBehaviour
{
    public int DroneId { get; private set; }
    public int Ammo { get; private set; }
    public int WeaponCapacity { get; private set; }
    public int Temperature { get; private set; }

    private static int idCounter = 0;
    private Flock agentFlock;
    private Collider2D agentCollider;

    // New property to track if the drone is under manual control
    public bool IsUnderControl { get; set; } = false;

    public Flock AgentFlock => agentFlock;
    public Collider2D AgentCollider => agentCollider;

    private void Start()
    {
        agentCollider = GetComponent<Collider2D>();
        RandomizeAttributes();
        StartCoroutine(RandomizeAttributesPeriodically());
    }

    private void Update()
    {
        Temperature = Random.Range(0, 100); // Simulate temperature change

        // If the drone is under manual control, allow the player to control the drone
        if (IsUnderControl)
        {
            HandleManualControl();
        }
        else
        {
            // Add any automatic behavior or movement when not controlled
            // For example, if the drone follows a flock, call the flocking method here
        }
    }

    private IEnumerator RandomizeAttributesPeriodically()
    {
        while (true)
        {
            RandomizeAttributes();
            yield return new WaitForSeconds(5f); // Adjust interval as needed
        }
    }

    private void RandomizeAttributes()
    {
        Ammo = Random.Range(1, 100);
        WeaponCapacity = Random.Range(1, 100);
    }

    // New method to handle manual control of the drone using WASD keys
    private void HandleManualControl()
    {
        float speed = 5f; // Movement speed for the manual control
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow)) // Move up
        {
            movement += Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow)) // Move down
        {
            movement += Vector2.down;
        }
        if (Input.GetKey(KeyCode.LeftArrow)) // Move left
        {
            movement += Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow)) // Move right
        {
            movement += Vector2.right;
        }

        // Move the drone based on the user input
        if (movement != Vector2.zero)
        {
            Move(movement.normalized * speed);
        }
    }

    // Method to move the drone
    public void Move(Vector2 velocity)
    {
        transform.up = velocity; // Update the drone's orientation to match its movement direction
        transform.position += (Vector3)velocity * Time.deltaTime; // Apply the movement
    }

    public void SelfDestruct()
    {
        gameObject.SetActive(false);
    }

    // Initialize the drone with a reference to the flock and a random ID
    public void Initialize(Flock flock)
    {
        Ammo = Random.Range(1, 100);
        DroneId = idCounter++;
        agentFlock = flock;
    }

    // Method to change the color of the drone
    public void ChangeColor(Color newColor)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = newColor;
        }
    }
}
