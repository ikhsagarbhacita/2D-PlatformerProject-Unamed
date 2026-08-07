using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private Transform[] wayPoint;
    Vector3[] wayPointPosition;

    public int wayPointIndex = 1;
    public int moveDirection = 1;
    private bool canMove = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Caches all waypoint positions and places the object at the starting waypoint
        UpdateWayPointsInfo();
        transform.position = wayPointPosition[0];
    }

    private void UpdateWayPointsInfo()
    {
        // 
        List<Trap_SawWayPoint> wayPointList = new List<Trap_SawWayPoint>(GetComponentsInChildren<Trap_SawWayPoint>());

        // 
        if (wayPointList.Count != wayPoint.Length)
        {
            wayPoint = new Transform[wayPointList.Count];

            // 
            for (int i = 0; i < wayPointList.Count; i++)
            {
                wayPoint[i] = wayPointList[i].transform;
            }
        }

        wayPointPosition = new Vector3[wayPoint.Length]; // Initializes the array and stores the world positions of all waypoints

        // Iterates through all waypoints to cache their world positions into Vector3 (X, Y, Z)
        for (int i = 0; i < wayPoint.Length; i++)
        {
            wayPointPosition[i] = wayPoint[i].position;
        }
    }

    private void Update()
    {
        anim.SetBool("active", canMove);

        if (canMove == false)
            return;

        transform.position = Vector2.MoveTowards(transform.position, wayPointPosition[wayPointIndex], moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPointPosition[wayPointIndex]) < 0.1f)
        {
            /* // Increments the target waypoint index
            wayPointIndex++;

            // Checks if the end of the waypoint array has been reached
            if (wayPointIndex >= wayPoint.Length)
            {
                // Resets the index to the beginning and starts the movement cooldown
                wayPointIndex = 0;
                StartCoroutine(StopMovement(cooldown));
            } */

            // Reverses movement direction when reaching either end of the waypoint array
            if (wayPointIndex == wayPointPosition.Length - 1 || wayPointIndex == 0)
            {
                moveDirection = moveDirection * -1;
                StartCoroutine(StopMovement(cooldown));
            }

            wayPointIndex = wayPointIndex + moveDirection; // Advances the waypoint index forward or backward based on current direction
        }
    }

    private IEnumerator StopMovement(float delay)
    {
        canMove = false;
        yield return new WaitForSeconds(delay);
        canMove = true;
        sr.flipX = !sr.flipX;
    }
}
