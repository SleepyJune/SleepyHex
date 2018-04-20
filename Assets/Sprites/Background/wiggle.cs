using UnityEngine;
using System.Collections;

public class Wiggle : MonoBehaviour {

    public float speed;

	Vector3 randomToPos() {
        Vector3 random = Vector3.zero;

        random.x = Random.Range(-1.0f, +1.0f);
        random.y = Random.Range(-1.0f, +1.0f);
        random.z = Random.Range(-1.0f, +1.0f);

        return random;
	}
    
	void Start () {
		randomToPos();
	}

	void Update () {
        transform.position += randomToPos() * speed * Time.deltaTime;
	}
}