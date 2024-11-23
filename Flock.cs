using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public Drone agentPrefab; // Drone prefab
    private Drone[] agents; // Array of all drones
    public FlockBehavior behavior; // Reference to behavior script

    [Range(10, 5000)] public int startingCount = 250;
    private const float AgentDensity = 0.08f;

    [Range(1f, 100f)] public float driveFactor = 10f;
    [Range(1f, 100f)] public float maxSpeed = 5f;
    [Range(1f, 10f)] public float neighborRadius = 1.5f;
    [Range(0f, 1f)] public float avoidanceRadiusMultiplier = 0.5f;

    private float squareMaxSpeed;
    private float squareNeighborRadius;
    private float squareAvoidanceRadius;

    public DroneNetworkCommunication lessThanOrEqualNetwork;
    public DroneNetworkCommunication greaterThanNetwork;

    private Color lessThanOrEqualColor = new Color(0.2f, 0.3f, 0.1f); // Dark Green
    private Color greaterThanColor = new Color(0.6f, 0.5f, 0.4f); // Tan/Brown

    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    void Start()
    {
        InitializeParameters();
        InitializeAgents();
        PartitionAndInitializeNetworks();
    }

    private void InitializeParameters()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
    }

    private void InitializeAgents()
    {
        agents = new Drone[startingCount];

        for (int i = 0; i < startingCount; i++)
        {
            Drone newAgent = Instantiate(
                agentPrefab,
                UnityEngine.Random.insideUnitCircle * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(0f, 360f)),
                transform
            );
            newAgent.name = $"Drone {i}";
            newAgent.Initialize(this);
            agents[i] = newAgent;
        }
    }

    private void PartitionAndInitializeNetworks()
    {
        lessThanOrEqualNetwork = new DroneNetworkCommunication();
        greaterThanNetwork = new DroneNetworkCommunication();

        if (agents.Length > 0)
        {
            var partitioned = PartitionDronesByAmmo();
            AddDronesToNetwork(partitioned.lessThanOrEqual, lessThanOrEqualNetwork, lessThanOrEqualColor);
            AddDronesToNetwork(partitioned.greaterThan, greaterThanNetwork, greaterThanColor);
        }
    }

    private void AddDronesToNetwork(Drone[] drones, DroneNetworkCommunication network, Color color)
    {
        foreach (Drone drone in drones)
        {
            if (drone != null)
            {
                drone.GetComponent<SpriteRenderer>().color = color;
                network.AddDrone(drone);
            }
        }
    }

    private (Drone[] lessThanOrEqual, Drone[] greaterThan) PartitionDronesByAmmo()
    {
        List<Drone> lessThanOrEqualList = new List<Drone>();
        List<Drone> greaterThanList = new List<Drone>();

        int pivotAmmo = agents[10].Ammo;

        foreach (Drone agent in agents)
        {
            if (agent.Ammo <= pivotAmmo)
                lessThanOrEqualList.Add(agent);
            else
                greaterThanList.Add(agent);
        }

        return (lessThanOrEqualList.ToArray(), greaterThanList.ToArray());
    }

    void Update()
    {
        foreach (Drone agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;

            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            agent.Move(move);
        }
    }

    
private List<Transform> GetNearbyObjects(Drone agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        foreach (Collider2D collider in contextColliders)
        {
            if (collider != agent.AgentCollider)
            {
                context.Add(collider.transform);
            }
        }
        return context;
    }

    public Drone FindDroneById(int droneId)
    {
        foreach (Drone drone in agents)
        {
            if (drone.DroneId == droneId) // Change 'ID' to 'DroneId'
            {
                return drone;
            }
        }

        Debug.LogWarning($"Drone with ID {droneId} not found.");
        return null;
    }

}