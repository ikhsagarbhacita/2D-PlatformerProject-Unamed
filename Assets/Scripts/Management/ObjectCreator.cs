using System;
using System.Collections;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    public static ObjectCreator Instance;

    [Header("Traps")]
    public GameObject arrowPrefab;
    public GameObject fallingPlatformPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // Prevents the ObjectCreator from being destroyed when loading a new scene

        // Singleton pattern implementation to ensure only one instance of ObjectCreator exists
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Initiates asynchronous object creation using Transform position for Trap_Arrow
    public void CreateObject(GameObject prefab, Transform target, float delay = 0, bool shoudBeDestroyed = false)
    {
        StartCoroutine(CreateObjectCourotine(prefab, target, delay, shoudBeDestroyed));
    }

    // Initiates asynchronous object creation using Vector3 position for Trap_FallingPlatform
    public void CreateObject(GameObject prefab, Vector3 position, float delay = 0, bool shouldBeDestroyed = false)
    {
        StartCoroutine(CreateObjectCourotine(prefab, position, delay, shouldBeDestroyed));
    }

    // 
    public GameObject CreateObjectAndReturn(GameObject prefab, Transform target, float delay = 0, bool shouldBeDestroyed = false)
    {
        GameObject newObject = Instantiate(prefab, target.position, Quaternion.identity);

        if (shouldBeDestroyed)
            Destroy(newObject, 15f);

        return newObject;
    }

    // Handles delayed instantiation of a target prefab at a specified Transform position
    private IEnumerator CreateObjectCourotine(GameObject prefab, Transform target, float delay, bool shouldBeDestroyed)
    {
        Vector3 newPosition = target.position;
        yield return new WaitForSeconds(delay);

        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);

        if (shouldBeDestroyed)
            Destroy(newObject, 15);
    }

    // Handles delayed instantiation of a target prefab at a specified Vector3 position
    private IEnumerator CreateObjectCourotine(GameObject prefab, Vector3 position, float delay, bool shouldBeDestroyed)
    {
        yield return new WaitForSeconds(delay);

        GameObject newObject = Instantiate(prefab, position, Quaternion.identity);

        if (shouldBeDestroyed)
            Destroy(newObject, 15);
    }
}