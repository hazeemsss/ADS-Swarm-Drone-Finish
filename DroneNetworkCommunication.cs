using System.Collections.Generic;
using UnityEngine;

public class DroneNetworkCommunication
{
    private Dictionary<int, List<int>> adjacencyList;
    private Dictionary<int, Drone> drones;

    public DroneNetworkCommunication()
    {
        adjacencyList = new Dictionary<int, List<int>>();
        drones = new Dictionary<int, Drone>();
    }

    // Add a drone to the network
    public void AddDrone(Drone drone)
    {
        if (!drones.ContainsKey(drone.DroneId))
        {
            drones[drone.DroneId] = drone;
            adjacencyList[drone.DroneId] = new List<int>();
        }
    }

    // Connect two drones (undirected connection)
    public void ConnectDrones(int droneId1, int droneId2)
    {
        if (adjacencyList.ContainsKey(droneId1) && adjacencyList.ContainsKey(droneId2))
        {
            adjacencyList[droneId1].Add(droneId2);
            adjacencyList[droneId2].Add(droneId1);
        }
    }

    // Get a drone by ID
    public Drone GetDroneById(int droneId)
    {
        if (drones.ContainsKey(droneId))
            return drones[droneId];
        return null;
    }

    // Search a drone using BFS or DFS
    public Drone SearchDroneWithPositionBFS(int droneId)
    {
        if (!adjacencyList.ContainsKey(droneId))
            return null;

        Queue<int> queue = new Queue<int>();
        HashSet<int> visited = new HashSet<int>();

        queue.Enqueue(droneId);
        visited.Add(droneId);

        while (queue.Count > 0)
        {
            int currentDroneId = queue.Dequeue();

            if (currentDroneId == droneId)
                return GetDroneById(currentDroneId);

            foreach (int neighborId in adjacencyList[currentDroneId])
            {
                if (!visited.Contains(neighborId))
                {
                    visited.Add(neighborId);
                    queue.Enqueue(neighborId);
                }
            }
        }

        return null;
    }

    // Find the shortest path between two drones using BFS
    public List<int> FindShortestPath(int startId, int endId)
    {
        if (!adjacencyList.ContainsKey(startId) || !adjacencyList.ContainsKey(endId))
            return null;

        Queue<int> queue = new Queue<int>();
        Dictionary<int, int> previous = new Dictionary<int, int>();
        HashSet<int> visited = new HashSet<int>();

        queue.Enqueue(startId);
        visited.Add(startId);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            if (current == endId)
                return ReconstructPath(previous, startId, endId);

            foreach (int neighbor in adjacencyList[current])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    previous[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return null; // No path found
    }

    // Reconstruct the path from start to end
    private List<int> ReconstructPath(Dictionary<int, int> previous, int startId, int endId)
    {
        List<int> path = new List<int>();
        for (int current = endId; current != startId; current = previous[current])
        {
            path.Insert(0, current);
        }
        path.Insert(0, startId);
        return path;
    }

    // Remove a drone from the network
    public void RemoveDrone(int droneId)
    {
        if (drones.ContainsKey(droneId))
        {
            drones.Remove(droneId);
            adjacencyList.Remove(droneId);

            // Remove all references to the drone from other drone adjacency lists
            foreach (var entry in adjacencyList)
            {
                entry.Value.Remove(droneId);
            }
        }
    }
}