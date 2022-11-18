using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidController : MonoBehaviour
{
    public GameObject boidPrefab;

    [Range(0, 300)] public int amountOfBoids;

    [SerializeField] private List<OnBoidObject> _listOfBoids;
    public List<Color> colors;

    public GameObject targetPoint;
    
    // Start is called before the first frame update
    // Create a new list and spawn in the correct amount of boids
    void Start()
    {
        _listOfBoids = new List<OnBoidObject>();

        for (int i = 0; i < amountOfBoids; i++)
        {
            SpawnBoid();
        }
    }

    //Spawn boid and add the "OnBoidObject" script into the _listOfBoids list.
    private void SpawnBoid()
    {
        GameObject newBoid = Instantiate(boidPrefab, Random.insideUnitSphere * Random.Range(-10f,10f), Quaternion.identity);
        newBoid.GetComponent<OnBoidObject>().index = Random.Range(0, 3);
        newBoid.GetComponent<OnBoidObject>().SetBoidController(this);
        newBoid.GetComponent<OnBoidObject>().SetColor(colors[newBoid.GetComponent<OnBoidObject>().index]);
        _listOfBoids.Add(newBoid.GetComponent<OnBoidObject>());
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var boid in _listOfBoids)
        {
            boid.Movement(_listOfBoids, Time.deltaTime);
        }
    }
}
