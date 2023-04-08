using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Blue = 0,
    Purple = 1,
    Default = 2
}

public enum Event
{
    HitPurpleGoal = 0,
    HitBlueGoal = 1,
    HitOutOfBounds = 2,
    HitIntoBlueArea = 3,
    HitIntoPurpleArea = 4
}

public class VolleyballEnvController : MonoBehaviour
{
    [Header("Inputs:")]
    List<Renderer> RenderersList = new List<Renderer>();
    public GameObject ball;
    Rigidbody ballRb;
    public GameObject blueGoal;
    public GameObject purpleGoal;
    public Material goalMaterial;
    public Material defaultMaterial;
    public GameObject toggleObject;
    Renderer blueGoalRenderer;
    Renderer purpleGoalRenderer;

    // cache
    Team lastHitter;
    private int resetTimer;
    int ballSpawnSide;
    VolleyballAgent blueAgent;
    VolleyballAgent purpleAgent;


    public void Initialize()
    {
        // Used to control agent & ball starting positions
        ballRb = ball.GetComponent<Rigidbody>();
        // Starting ball spawn side
        // -1 = spawn blue side, 1 = spawn purple side
        var spawnSideList = new List<int> { -1, 1 };
        ballSpawnSide = spawnSideList[Random.Range(0, 2)];

        // Render ground to visualise which agent scored
        blueGoalRenderer = blueGoal.GetComponent<Renderer>();
        purpleGoalRenderer = purpleGoal.GetComponent<Renderer>();
        RenderersList.Add(blueGoalRenderer);
        RenderersList.Add(purpleGoalRenderer);
        StopGame();
        //ResetScene();
    }
    public void StartGame(VolleyballAgent purple, VolleyballAgent blue)
    {
        blueAgent = blue;
        purpleAgent = purple;
        purpleAgent.Setup(this, Team.Purple, ball);
        blueAgent.Setup(this, Team.Blue, ball);
        ball.SetActive(true);
        toggleObject.gameObject.SetActive(true);
        ResetScene();
    }
    public void StopGame()
    {
        ball.SetActive(false);
        purpleAgent = null;
        blueAgent = null;
        resetTimer = 0;
        toggleObject.gameObject.SetActive(false);
        lastHitter = Team.Default; // reset last hitter
    }
    public void ResetScene()
    {
        resetTimer = 0;
        lastHitter = Team.Default; // reset last hitter
        // reset ball to starting conditions
        ResetBall();
    }
    public void UpdateLastHitter(Team team)
    {
        lastHitter = team;
    }
    public void ResolveEvent(Event triggerEvent)
    {
        switch (triggerEvent)
        {
            case Event.HitOutOfBounds:
                if (lastHitter == Team.Blue)
                {
                    // apply penalty to blue agent
                    // blueAgent.AddReward(-0.1f);
                    // purpleAgent.AddReward(0.1f);
                }
                else if (lastHitter == Team.Purple)
                {
                    // apply penalty to purple agent
                    // purpleAgent.AddReward(-0.1f);
                    // blueAgent.AddReward(0.1f);
                }
                // end episode
                blueAgent.EndEpisode();
                purpleAgent.EndEpisode();
                ResetScene();
                break;

            case Event.HitBlueGoal:
                // blue wins
                blueAgent.AddReward(1f);
                purpleAgent.AddReward(-1f);

                // turn floor blue
                StartCoroutine(GoalScoredSwapGroundMaterial(goalMaterial, RenderersList, .5f));
                // end episode
                blueAgent.EndEpisode();
                purpleAgent.EndEpisode();
                ResetScene();
                break;

            case Event.HitPurpleGoal:
                // purple wins
                purpleAgent.AddReward(1f);
                blueAgent.AddReward(-1f);

                // turn floor purple
                StartCoroutine(GoalScoredSwapGroundMaterial(goalMaterial, RenderersList, .5f));

                // end episode
                blueAgent.EndEpisode();
                purpleAgent.EndEpisode();
                ResetScene();
                break;

            case Event.HitIntoBlueArea:
                // if (lastHitter == Team.Purple)
                // {
                //     purpleAgent.AddReward(1);
                // }
                break;

            case Event.HitIntoPurpleArea:
                // if (lastHitter == Team.Blue)
                // {
                //     blueAgent.AddReward(1);
                // }
                break;
        }
    }

    IEnumerator GoalScoredSwapGroundMaterial(Material mat, List<Renderer> rendererList, float time)
    {
        foreach (var renderer in rendererList)
        {
            renderer.material = mat;
        }
        yield return new WaitForSeconds(time); // wait for 2 sec
        foreach (var renderer in rendererList)
        {
            renderer.material = defaultMaterial;
        }
    }
    void ResetBall()
    {
        var randomPosX = Random.Range(-2f, 2f);
        var randomPosZ = Random.Range(6f, 10f);
        var randomPosY = Random.Range(6f, 8f);

        // alternate ball spawn side
        // -1 = spawn blue side, 1 = spawn purple side
        ballSpawnSide = -1 * ballSpawnSide;

        if (ballSpawnSide == -1)
        {
            ball.transform.localPosition = new Vector3(randomPosX, randomPosY, randomPosZ);
        }
        else if (ballSpawnSide == 1)
        {
            ball.transform.localPosition = new Vector3(randomPosX, randomPosY, -1 * randomPosZ);
        }

        ballRb.angularVelocity = Vector3.zero;
        ballRb.velocity = Vector3.zero;
    }
}
