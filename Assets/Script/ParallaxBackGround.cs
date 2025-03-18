using UnityEngine;

public class ParallaxBackGround : MonoBehaviour
{

    private GameObject cam;

    [SerializeField] private float parallaxeffect;

    private float xPosition;
	private float length;
	void Start()
	{
		cam = GameObject.Find("Main Camera");

		length = GetComponent<SpriteRenderer>().bounds.size.x;

		xPosition = transform.position.x;
	}

	// Update is called once per frame
	void Update()
    {
        float distanceToMove = cam.transform.position.x * parallaxeffect;
		float distanceMoved = cam.transform.position.x * (1 - parallaxeffect);

		transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

		if(distanceMoved > xPosition + length)
		{
			xPosition = xPosition + length;
		}
		else
		{
			xPosition = xPosition - length;
		}
    }
}
