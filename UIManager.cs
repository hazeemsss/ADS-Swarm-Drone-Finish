using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_InputField inputText;
    public TMP_InputField targetText; // Input for the target drone ID
    public Button findDrone;
    public Button destroyDrone;
    public Button controlDrone;
    public Button releaseControl;
    public Button findPath; // New button for finding shortest path

    public TMP_Text position;

    public Flock flock; // Reference to Flock class

    private DroneNetworkCommunication currentNetwork;
    private Drone controlledDrone;

    void Start()
    {
        findDrone.onClick.AddListener(FindDroneButton);
        destroyDrone.onClick.AddListener(DestroyDroneButton);
        controlDrone.onClick.AddListener(ControlDroneButton);
        releaseControl.onClick.AddListener(ReleaseControlButton);
        findPath.onClick.AddListener(FindPathButton); // New pathfinding button
    }

    // Find a drone using BFS or DFS and display its position
    public void FindDroneButton()
    {
        if (int.TryParse(inputText.text, out int ID))
        {
            var resultBFS = flock.lessThanOrEqualNetwork.SearchDroneWithPositionBFS(ID);

            if (resultBFS != null)
            {
                currentNetwork = flock.lessThanOrEqualNetwork;
                resultBFS.ChangeColor(new Color(1f, 0.647f, 0f)); // Change color to orange
                position.text = $"[BFS] Drone ID: {resultBFS.DroneId}\nPosition: {resultBFS.transform.position}";
            }
            else
            {
                var resultDFS = flock.greaterThanNetwork.SearchDroneWithPositionBFS(ID);
                if (resultDFS != null)
                {
                    currentNetwork = flock.greaterThanNetwork;
                    resultDFS.ChangeColor(new Color(1f, 0.647f, 0f)); // Change color to orange
                    position.text = $"[DFS] Drone ID: {resultDFS.DroneId}\nPosition: {resultDFS.transform.position}";
                }
                else
                {
                    position.text = "Drone not found.";
                    currentNetwork = null;
                }
            }
        }
        else
        {
            position.text = "Invalid Input.";
            currentNetwork = null;
        }
    }

    // Destroy a drone from the network
    public void DestroyDroneButton()
    {
        if (int.TryParse(inputText.text, out int ID))
        {
            flock.lessThanOrEqualNetwork.RemoveDrone(ID);
            flock.greaterThanNetwork.RemoveDrone(ID);

            var drone = flock.FindDroneById(ID);
            if (drone != null)
            {
                drone.ChangeColor(Color.black);
                drone.gameObject.SetActive(false);
            }

            position.text = $"Drone ID: {ID} destroyed.";
        }
        else
        {
            position.text = "Invalid Drone ID.";
        }
    }

    // Control the selected drone
    public void ControlDroneButton()
    {
        if (currentNetwork != null && int.TryParse(inputText.text, out int ID))
        {
            controlledDrone = currentNetwork.SearchDroneWithPositionBFS(ID);
            if (controlledDrone != null)
            {
                controlledDrone.IsUnderControl = true;
                controlledDrone.transform.localScale = new Vector3(2, 2, 2); // Example: increase size
                var renderer = controlledDrone.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red; // Change color to red
                }

                position.text = $"Drone ID: {ID} is now under control.";
            }
            else
            {
                position.text = "Drone not found.";
            }
        }
        else
        {
            position.text = "Invalid Drone ID or no drone found.";
        }
    }

    // Release control of the drone
    public void ReleaseControlButton()
    {
        if (controlledDrone != null)
        {
            controlledDrone.IsUnderControl = false;
            controlledDrone.transform.localScale = Vector3.one; // Reset size
            var renderer = controlledDrone.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white; // Reset color
            }

            position.text = $"Drone ID: {controlledDrone.DroneId} control released.";
            controlledDrone = null;
        }
        else
        {
            position.text = "No drone is currently under control.";
        }
    }

    // Find the shortest path between two drones
    public void FindPathButton()
    {
        if (int.TryParse(inputText.text, out int startID) && int.TryParse(targetText.text, out int targetID))
        {
            DroneNetworkCommunication network = flock.lessThanOrEqualNetwork.GetDroneById(startID) != null
                ? flock.lessThanOrEqualNetwork
                : flock.greaterThanNetwork;

            var path = network.FindShortestPath(startID, targetID);

            if (path != null && path.Count > 0)
            {
                string pathString = string.Join(" -> ", path);
                position.text = $"Shortest Path: {pathString}";

                // Highlight the path (optional visualization)
                foreach (int id in path)
                {
                    var drone = network.GetDroneById(id);
                    if (drone != null)
                    {
                        drone.ChangeColor(Color.green);
                    }
                }
            }
            else
            {
                position.text = "No path found between drones.";
            }
        }
        else
        {
            position.text = "Invalid Input for pathfinding.";
        }
    }
}
