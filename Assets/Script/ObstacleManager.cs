using UnityEngine;

public class ObstacleManager : MonoBehaviour {
    public static ObstacleManager instance { get; private set; }

    public GameObject obstaclepPrefab;
    public int obstacleCount;
    public float yOffset;

    public Obstacle[] obstacles { get; private set; }
    private Matrix frameInputs;

    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        float temp = (obstacleCount - 1) * yOffset / 2;
        obstacles = new Obstacle[obstacleCount];

        for (int i=0; i < obstacleCount; i++)
        {
            Obstacle obstacle =  Instantiate(obstaclepPrefab, new Vector3(0, yOffset * i - temp, 0), Quaternion.identity, transform).GetComponent<Obstacle>();
            obstacle.SetStartPos(i % 2 == 0);
            obstacles[i] = obstacle;
        }
        frameInputs = new Matrix(obstacleCount + 4, 1);
        CalcInput();
    }

    private void Update()
    {
        CalcInput();
    }

    private void CalcInput()
    {
        if (obstacles[0].velocity.x < 0)
        {
            frameInputs[2, 0] = obstacles[0].transform.position.x;
            frameInputs[3, 0] = obstacles[1].transform.position.x;
        } else
        {
            frameInputs[2, 0] = obstacles[1].transform.position.x;
            frameInputs[3, 0] = obstacles[0].transform.position.x;
        }

        for (int i = 0; i <= obstacleCount; i += 2)
        {
            frameInputs[i + 4, 0] = obstacles[i].velocity.x;
        }
    }

    public Matrix GetInput(Vector3 position)
    { 
        frameInputs[0, 0] = position.x;
        frameInputs[1, 0] = position.y;
        return frameInputs.NormalizedCopy();
    }
}
