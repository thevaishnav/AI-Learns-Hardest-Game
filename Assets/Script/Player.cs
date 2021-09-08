using UnityEngine;
using System.Collections.Generic;


public class Player : MonoBehaviour
{
    private static Dictionary<int, Vector3> moveIntToVel = new Dictionary<int, Vector3>
    {
        { 0, new Vector3( 1f, 0f) }, { 1, new Vector3( 0.7071f,  0.7071f) }, { 2, new Vector3(0f,  1f) }, { 3, new Vector3(-0.7071f,  0.7071f) },
        { 4, new Vector3(-1f, 0f) }, { 5, new Vector3(-0.7071f, -0.7071f) }, { 6, new Vector3(0f, -1f) }, { 7, new Vector3( 0.7071f, -0.7071f) }
    };

    public float movementSpeed = 0;
    public Network brain;
    public float fitness;
    public bool isAlive { get; private set; }
    List<string> passedThroughChecks;
    public int rank;
    private SpriteRenderer sr;

    private void Start()
    {
        isAlive = true;
        fitness = 0;
        passedThroughChecks = new List<string>();
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0.5f);
    }

    void Update()
    {
        if (isAlive) { MakeMove(); }
    }

    public void MakeMove() {
        Matrix inps = ObstacleManager.instance.GetInput(transform.position);
        Vector3 vel = moveIntToVel[brain.Predict(inps).GetMaxLoc()[0]];
        transform.position = transform.position + vel * movementSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        string tag = collision.gameObject.tag;
        string objectName = collision.gameObject.name;
        if (tag == "CheckPoint" && !passedThroughChecks.Contains(objectName))  { 
            fitness += 50;
            Nature.instance.TrySetScore(fitness);
            passedThroughChecks.Add(objectName);  }
        if (tag == "NegCheck" && !passedThroughChecks.Contains(objectName)){
            fitness -= 25;
            passedThroughChecks.Add(objectName);
        }
        else if (tag == "Obstacle") { Nature.instance.Kill(this); gameObject.SetActive(false); isAlive = false; }
    }

    public float evaluateFitness()
    {
        Nature.instance.TrySetScore(fitness);
        return fitness;
    }

    public void SetColor(Color color)
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        sr.color = color;
    }

    public Color GetColor() { return sr.color; }
}