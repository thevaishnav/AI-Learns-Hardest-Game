using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float startPoint;
    public float endPoint;
    public float speed;

    private Vector2 position;
    public Vector2 velocity { get; private set; }

    
    void Update()
    {
        position += velocity * Time.deltaTime;
        transform.position = position;
        if (position.x <= startPoint)
        {
            position.x = startPoint;
            velocity *= -1;
        }
        else if (position.x >= endPoint)
        {
            position.x = endPoint;
            velocity *= -1;
        }
    }

    public void SetStartPos(bool startLeft)
    {
        if (startLeft)
        {
            position = new Vector2(startPoint, transform.position.y);
            velocity = new Vector2(speed, 0);
        }
        else
        {
            position = new Vector2(endPoint, transform.position.y);
            velocity = new Vector2(-speed, 0);
        }
        transform.position = position;
    }
}
