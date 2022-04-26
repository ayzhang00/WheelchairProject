using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class NPCRoaming : MonoBehaviour
{
    NavMeshAgent nm;
    Rigidbody rb;
    public Animator anim;
    public Transform Target;
    public Transform WaypointGroup;
    Transform[] WayPoints;
    public int cur_waypoint;
    public float speed, stop_distance;
    public float MaxPauseTimer;

    [SerializeField]
    private float cur_timer;
    // Start is called before the first frame update
    void Start()
    {
        nm = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;


        WayPoints = WaypointGroup.GetComponentsInChildren<Transform>();
        cur_waypoint = Random.Range(0, WayPoints.Length - 1);

        Target = WayPoints[cur_waypoint];
        cur_timer = Random.Range(0,MaxPauseTimer);
    }

    // Update is called once per frame
    void Update()
    {

        nm.acceleration = speed;
        nm.stoppingDistance = stop_distance;

        float distance = Vector3.Distance(transform.position, Target.position);

        if (distance > stop_distance && WayPoints.Length > 0)
        {
            anim.SetBool("Moving", true);
            anim.SetBool("Idle", false);
            Target = WayPoints[cur_waypoint];
        }
        else if(distance <= stop_distance && WayPoints.Length > 0)
        {
            if (cur_timer > 0)
            {
                cur_timer -= 0.01f;
                anim.SetBool("Moving", false);
                anim.SetBool("Idle", true);
            }
            if (cur_timer <= 0){
                    cur_waypoint=Random.Range(0,WayPoints.Length-1);
                    
                    Target = WayPoints[cur_waypoint];
                    cur_timer = Random.Range(0,MaxPauseTimer);
                }
        }

        nm.SetDestination(Target.position);
    }
}
