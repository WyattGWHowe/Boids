using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBoidObject : MonoBehaviour
{
    public int index;
    public float antiClumpRadius = 5f;
    public float localRadius = 10f;
    public float speed = 10f;
    public float steeringSpeed = 100f;

    [SerializeField] private MeshRenderer _mr;
    [SerializeField] private BoidController _bc;

    public void Movement(List<OnBoidObject> listOfBoids, float t)
    {
        Vector3 direction = Vector3.zero;

        
        //These should be joined into a singular method.
        //Keeping seperate for now to make it easier to make adjustments.
        direction += SeperationDirection(listOfBoids).normalized * .5f;
        direction += CohesionDirection(listOfBoids).normalized * .16f;
        direction += AlignmentDirection(listOfBoids).normalized * .34f;
        direction += LeadershipDirection(listOfBoids);
        
       var targetPoint = _bc.targetPoint.transform.position;
       direction += (targetPoint - transform.position).normalized;
        //Object Avoidance
        direction = ObjectAvoidance(direction);

        
        
        if(direction != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction),steeringSpeed * t);

        transform.position += transform.TransformDirection(new Vector3(0, 0, speed) * t);
    }

    /// <summary>
    /// Makes sure the boids dont collide into eachother, keeps them marginally seperate.
    /// </summary>
    /// <param name="listOfBoids"></param>
    /// <returns></returns>
    private Vector3 SeperationDirection(List<OnBoidObject> listOfBoids)
    {
        Vector3 dir = Vector3.zero;
        int counter = 0;

        foreach (var boid in listOfBoids)
        {
            if(boid == this|| boid.index != this.index) continue; //Skip if its this boid

            float dis = Vector3.Distance(this.transform.position, boid.transform.position); 

            if (dis < antiClumpRadius) //Check distance is lower than clump range
            {
                counter++;
                dir += boid.transform.position - transform.position;
            }
        }
        
        //Calculate the average of the dir
        if (counter > 0) dir /= (float)counter;
        
        //Flip and normalize the direction
        dir = -dir.normalized;

        return dir;
    }

    /// <summary>
    /// Makes sure that each boid is following other boids in the same direction.
    /// </summary>
    /// <param name="listOfBoids"></param>
    /// <returns></returns>
    private Vector3 AlignmentDirection(List<OnBoidObject> listOfBoids)
    {
        Vector3 dir = Vector3.zero;
        int counter = 0;

        foreach (var boid in listOfBoids)
        {
            if(boid == this|| boid.index != this.index) continue; //Skip if its this boid

            float dis = Vector3.Distance(this.transform.position, boid.transform.position);

            if (dis < localRadius)
            {
                counter++;
                dir += boid.transform.forward;
            }
        }

        if (counter > 0) dir /= (float)counter;
        return dir;
    }
    
    /// <summary>
    /// Makes sure that the boids stay atleast a little clumped together.
    /// </summary>
    /// <param name="listOfBoids"></param>
    /// <returns></returns>
    private Vector3 CohesionDirection(List<OnBoidObject> listOfBoids)
    {
        Vector3 dir = Vector3.zero;
        int counter = 0;

        foreach (var boid in listOfBoids)
        {
            if(boid == this|| boid.index != this.index) continue; //Skip if its this boid

            float dis = Vector3.Distance(this.transform.position, boid.transform.position); 

            if (dis < antiClumpRadius) //Check distance is lower than clump range
            {
                counter++;
                dir += boid.transform.position - transform.position;
            }
        }
        
        //Calculate the average of the dir
        if (counter > 0) dir /= (float)counter;
        dir -= transform.position;
        //Flip and normalize the direction
        //dir = -dir.normalized;

        return dir;
    }

    
    /// <summary>
    /// Makes the boids follow a leader boid that will set the direction, makes sure that the boid at the back can still follow the leaders direction.
    /// </summary>
    /// <param name="listOfBoids"></param>
    /// <returns></returns>
    private Vector3 LeadershipDirection(List<OnBoidObject> listOfBoids)
    {
        float leaderAngle = 360;
        OnBoidObject leaderBoid = null;
        foreach (var boid in listOfBoids)
        {
            if(boid == this || boid.index != this.index) continue; //Skip if its this boid

            float dis = Vector3.Distance(this.transform.position, boid.transform.position); 

            if (dis < localRadius) //Check distance is lower than clump range
            {
                float angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                if (angle < leaderAngle && angle < 90f)
                {
                    leaderAngle = angle;
                    leaderBoid = boid;
                }
            }
        }

        if (leaderBoid != null)
        {
            return (leaderBoid.transform.position - transform.position).normalized * .5f;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Makes sure that the boids will avoid objects with the tag "Avoidance"
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Vector3 ObjectAvoidance(Vector3 direction)
    {
        RaycastHit hitInfo;
        Debug.DrawRay(transform.position, transform.forward * localRadius, Color.white);
        if (Physics.SphereCast(transform.position,1f, transform.forward, out hitInfo, localRadius, LayerMask.GetMask("Avoidance")))
            return -(hitInfo.point - transform.position).normalized;
        return direction;
    }

    public void SetColor(Color c)
    {
        _mr.material.color = c;
    }

    public void SetBoidController(BoidController bc)
    {
        _bc = bc;
    }
}
