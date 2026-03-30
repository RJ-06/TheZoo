using System.Collections.Generic;
using UnityEngine;

public class BoidBehavior : MonoBehaviour
{
    public float speed = 1f;
    public Vector3 vel;
    public HashSet<BoidBehavior> detectedBoids = new HashSet<BoidBehavior>();

    public float minSpeed;
    public float maxSpeed;
    public float separationRange;
    public float groundYPos;
    public float topYPos;

    public float separationFactor;
    public float matchingFactor;
    public float centeringFactor;
    public float turnFactor;

    private Rigidbody rb;

    [SerializeField] Transform leftMargin;
    [SerializeField] Transform rightMargin;
    [SerializeField] Transform frontMargin;
    [SerializeField] Transform backMargin;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        vel = new Vector3(Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)).normalized * minSpeed;
    }

    private void FixedUpdate()
    {
        // Calculate all forces
        Vector3 acceleration = Vector3.zero;
        acceleration += SeparationBehavior();
        acceleration += AlignmentBehavior();
        acceleration += CohesionBehavior();
        acceleration += EdgeBehavior();

        // Apply acceleration to velocity smoothly over time
        vel += acceleration * Time.fixedDeltaTime;

        // Clamp the velocity to ensure they don't stop or go too fast
        float currentSpeed = vel.magnitude;
        if (currentSpeed < minSpeed)
        {
            vel = vel.normalized * minSpeed;
        }
        else if (currentSpeed > maxSpeed)
        {
            vel = vel.normalized * maxSpeed;
        }

        // Apply the smoothed velocity to the Rigidbody
        rb.linearVelocity = vel * speed;

        // Smoothly rotate the boid to face its movement direction
        if (vel.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(vel.normalized);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BoidBehavior otherBoid = other.GetComponent<BoidBehavior>();
        if (otherBoid != null && otherBoid != this)
        {
            detectedBoids.Add(otherBoid);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BoidBehavior otherBoid = other.GetComponent<BoidBehavior>();
        if (otherBoid != null)
        {
            detectedBoids.Remove(otherBoid);
        }
    }

    public Vector3 SeparationBehavior()
    {
        Vector3 force = Vector3.zero;
        foreach (BoidBehavior b in detectedBoids)
        {
            if (b == null) continue;

            float distance = Vector3.Distance(b.transform.position, transform.position);
            if (distance < separationRange && distance > 0)
            {
                // The closer they are, the stronger the separation force
                force += (transform.position - b.transform.position).normalized / distance;
            }
        }
        return force * separationFactor;
    }

    public Vector3 AlignmentBehavior()
    {
        Vector3 averageVelocity = Vector3.zero;
        int nBoidCount = 0;

        foreach (BoidBehavior b in detectedBoids)
        {
            if (b == null) continue;

            if (Vector3.Distance(b.transform.position, transform.position) >= separationRange)
            {
                averageVelocity += b.rb.linearVelocity;
                nBoidCount++;
            }
        }

        if (nBoidCount > 0)
        {
            averageVelocity /= nBoidCount;
            // Steering force = Desired Velocity - Current Velocity
            Vector3 steer = averageVelocity - vel;
            return steer * matchingFactor;
        }
        return Vector3.zero;
    }

    public Vector3 CohesionBehavior()
    {
        Vector3 centerOfMass = Vector3.zero;
        int nBoidCount = 0;

        foreach (BoidBehavior b in detectedBoids)
        {
            if (b == null) continue;

            if (Vector3.Distance(b.transform.position, transform.position) >= separationRange)
            {
                centerOfMass += b.transform.position;
                nBoidCount++;
            }
        }

        if (nBoidCount > 0)
        {
            centerOfMass /= nBoidCount;
            // Desired direction towards center of mass
            Vector3 desiredDirection = centerOfMass - transform.position;
            return desiredDirection * centeringFactor;
        }
        return Vector3.zero;
    }

    public Vector3 EdgeBehavior()
    {
        Vector3 force = Vector3.zero;

        if (transform.position.x < leftMargin.position.x) force.x += turnFactor;
        if (transform.position.x > rightMargin.position.x) force.x -= turnFactor;

        if (transform.position.z > frontMargin.position.z) force.z -= turnFactor;
        if (transform.position.z < backMargin.position.z) force.z += turnFactor;

        if (transform.position.y < groundYPos) force.y += turnFactor;
        if (transform.position.y > topYPos) force.y -= turnFactor;

        return force;
    }
}