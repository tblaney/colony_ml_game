using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.Barracuda;

public class VolleyballAgent : Agent
{
    Rigidbody agentRb;
    BehaviorParameters behaviorParameters;
    Team teamId;

    public NNModel model;

    // To get ball's location for observations
    GameObject ball;
    Rigidbody ballRb;
    VolleyballEnvController envController;

    // Controls jump behavior
    float jumpingTime;
    Vector3 jumpTargetPos;
    Vector3 jumpStartingPos;
    float agentRot;
    LayerMask maskTerrain;
    bool _playing = false;

    public void Setup(VolleyballEnvController controller, Team teamId, GameObject ball)
    {
        this.envController = controller;
        this.teamId = teamId;
        this.ball = ball;
        InitializeAgent();
        //SetModel("Volleyball", model, InferenceDevice.GPU);
        _playing = true;
    }
    void Awake()
    {
        Stop();
        _playing = false;
    }
    void Stop()
    {
        //SetModel("Volleyball", null, InferenceDevice.Default);
        _playing = false;

    }

    public void InitializeAgent()
    {
        maskTerrain = LayerMask.GetMask("Terrain");
        behaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        agentRb = GetComponent<Rigidbody>();
        ballRb = ball.GetComponent<Rigidbody>();
        // for symmetry between player side
        if (teamId == Team.Blue)
        {
            agentRot = -1;
        }
        else
        {
            agentRot = 1;
        }
    }
    void MoveTowards(Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
    {   
        var moveToPos = targetPos - rb.worldCenterOfMass;
        var velocityTarget = Time.fixedDeltaTime * targetVel * moveToPos;
        if (float.IsNaN(velocityTarget.x) == false)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocityTarget, maxVel);
        }
    }
    public bool CheckIfGrounded()
    {
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position + new Vector3(0f, 0.5f, 0f), -Vector3.up, out hit, 20f, maskTerrain);
        if (isHit)
        {
            float height = transform.position.y - hit.point.y;
            if (height < 1f)
                return true;
        }
        return false;
    }
    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("ball"))
        {
            envController.UpdateLastHitter(teamId);
        }
    }
    /// <summary>
    /// Starts the jump sequence
    /// </summary>
    public void Jump()
    {
        jumpingTime = 0.2f;
        jumpStartingPos = agentRb.position;
    }
    /// <summary>
    /// Resolves the agent movement
    /// </summary>
    public void MoveAgent(ActionSegment<int> act)
    {
        var grounded = CheckIfGrounded();
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var rotateDirAction = act[1];
        var dirToGoSideAction = act[2];
        var jumpAction = act[3];

        if (dirToGoForwardAction == 1)
            dirToGo = (grounded ? 1f : 0.5f) * transform.forward * 1f;
        else if (dirToGoForwardAction == 2)
            dirToGo = (grounded ? 1f : 0.5f) * transform.forward * 0.7f* -1f;

        if (rotateDirAction == 1)
            rotateDir = transform.up * -1f;
        else if (rotateDirAction == 2)
            rotateDir = transform.up * 1f;

        if (dirToGoSideAction == 1)
            dirToGo = (grounded ? 1f : 0.5f) * transform.right *0.7f*-1f;
        else if (dirToGoSideAction == 2)
            dirToGo = (grounded ? 1f : 0.5f) * transform.right*0.7f;

        if (jumpAction == 1)
            if (((jumpingTime <= 0f) && grounded))
            {
                Jump();
            }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        agentRb.AddForce(agentRot * dirToGo * 1.5f,
            ForceMode.VelocityChange);

        if (jumpingTime > 0f)
        {
            jumpTargetPos =
                new Vector3(agentRb.position.x,
                    jumpStartingPos.y + 2.75f,
                    agentRb.position.z) + agentRot*dirToGo;

            MoveTowards(jumpTargetPos, agentRb, 400f,
                10f);
        }

        if (!(jumpingTime > 0f) && !grounded)
        {
            agentRb.AddForce(
                Vector3.down * 150f, ForceMode.Acceleration);
        }

        if (jumpingTime > 0f)
        {
            jumpingTime -= Time.fixedDeltaTime;
        }
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (!_playing)
            return;
        MoveAgent(actionBuffers.DiscreteActions);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if (!_playing)
            return;
        // Agent rotation (1 float)
        sensor.AddObservation(this.transform.rotation.y);

        // Vector from agent to ball (direction to ball) (3 floats)
        Vector3 toBall = new Vector3((ballRb.transform.position.x - this.transform.position.x)*agentRot, 
        (ballRb.transform.position.y - this.transform.position.y),
        (ballRb.transform.position.z - this.transform.position.z)*agentRot);

        sensor.AddObservation(toBall.normalized);

        // Distance from the ball (1 float)
        sensor.AddObservation(toBall.magnitude);

        // Agent velocity (3 floats)
        sensor.AddObservation(agentRb.velocity);

        // Ball velocity (3 floats)
        sensor.AddObservation(ballRb.velocity.y);
        sensor.AddObservation(ballRb.velocity.z*agentRot);
        sensor.AddObservation(ballRb.velocity.x*agentRot);
    }
}
