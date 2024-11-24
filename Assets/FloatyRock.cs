using DefaultNamespace;
using Sirenix.OdinInspector;
using UnityEngine;

public class FloatyRock : MonoBehaviour
{
    [InlineEditor(InlineEditorModes.FullEditor)]
    public FloatingRocksConfig config;
    
    private Vector3 startPosition;
    private float randomOffset;  // Makes each rock float differently
    
    void Start()
    {
        startPosition = transform.position;
        // Random offset so not all rocks move in sync
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        // Calculate floating movement
        float timeValue = Time.time * config.floatSpeed + randomOffset;
        
        // Create different frequencies for each axis to make movement more interesting
        float xOffset = Mathf.Sin(timeValue * 0.9f) * config.maxMovement;
        float yOffset = Mathf.Sin(timeValue * 1.1f) * config.maxMovement;
        float zOffset = Mathf.Sin(timeValue * 0.7f) * config.maxMovement;
        
        // Apply the floating movement
        Vector3 newPosition = startPosition + new Vector3(xOffset, yOffset, zOffset);
        transform.position = newPosition;
        
        // Add a gentle rotation
        transform.Rotate(Vector3.up * config.rotationSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right * (config.rotationSpeed * 0.6f) * Time.deltaTime, Space.World);
    }
}