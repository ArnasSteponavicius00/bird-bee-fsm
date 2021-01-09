using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeeStates {
    Rest,
    Search,
    Flee,
    Gather,
    Dance
}

public class Bee : MonoBehaviour
{
    [SerializeField] private Transform[] foodSpots;
    [SerializeField] private Transform[] birds;
    [SerializeField] private Transform hive;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float waitTime;
    [SerializeField] private float startWaitTime = 5.0f;
    [SerializeField] private float beeMaxNectar = 10f;

    private SpriteRenderer sprite;
    private BeeStates state;
    private Rigidbody2D rb;
    private int randomSpot;
    private float energyAmt = 300f;
    private float energy;
    private float nectarGathered;
    private float nectarUnloaded = 0.2f;

    void Start()
    {
        state = BeeStates.Search;
        sprite = GetComponent<SpriteRenderer>();
        randomSpot = Random.Range(0, foodSpots.Length);
    }

    void Update() 
    {
        switch (state) {
            case BeeStates.Search:
                SearchState();
                break;

            case BeeStates.Gather:
                GatherState();
                break;
            
            case BeeStates.Flee:
                FleeState();
                break;

            case BeeStates.Rest:
                RestState();
                break;
        }

    }

    // RESTING STATE   
    void RestState() {
        sprite.color = new Color (1, 0, 1);
        transform.position = Vector2.MoveTowards(transform.position, hive.position, moveSpeed * Time.deltaTime);

        if(Vector2.Distance(transform.position, hive.position) < 0.2f) {
            energyAmt += 0.3f;
            nectarGathered -= nectarUnloaded;

            if(nectarGathered <= 0f) {
                if(energyAmt >= 500f) {
                    state = BeeStates.Search;
                }
            }
        }
    }

    // SEARCHING STATE
    void SearchState() {
        sprite.color = new Color (0, 1, 0);
        // Move the bee toward a random nectar spot
        transform.position = Vector2.MoveTowards(transform.position, foodSpots[randomSpot].position, moveSpeed * Time.deltaTime);
        energy = 0.1f;
        energyAmt -= energy;

        // Check the distance between the bee and the nectar, if within range
        // Change state to Gather nectar
        if(Vector2.Distance(transform.position, foodSpots[randomSpot].position) < 0.2f) {
            state = BeeStates.Gather;
        }

        // Keep looking for Birds
        if(CheckBirds()) {
            state = BeeStates.Flee;
        }
        

        if(!HasEnergy()) {
            state = BeeStates.Rest;
        }
    }

    // FLEEING STATE
    void FleeState() {
        sprite.color = new Color (1, 0, 0);

        transform.position = Vector2.MoveTowards(transform.position, hive.position, moveSpeed * Time.deltaTime);
        energy = 0.8f;
        energyAmt -= energy;

        // Check for energy
        if(!HasEnergy()) {
            state = BeeStates.Rest;
        }
    }

    // GATHERING STATE
    void GatherState() {
        sprite.color = new Color (1, 1, 1);
        nectarGathered += 0.1f;

        if(nectarGathered >= beeMaxNectar) {
            state = BeeStates.Rest;
        } else {
            state = BeeStates.Search;
        }

        // Keep looking for Birds
        if(CheckBirds()) {
            state = BeeStates.Flee;
        }

        if(!HasEnergy()) {
            state = BeeStates.Rest;
        }
    }

    // Check whether the bees have Energy
    public bool HasEnergy() {
        if(energyAmt > 0) 
            return true;
        else
            return false;
    }

    // Check whether there are birds nearby
    public bool CheckBirds() {
        for(int i = 0; i < birds.Length; i++) {
            if(Vector2.Distance(transform.position, birds[i].position) <= 1f) {
                return true;
            }
        }
        return false;
    }

    // Check how much energy bees have
    public bool CheckEnergy() {
        if(energyAmt <= 170f) {
            sprite.color = new Color (1, 1, 0);
            return true;
        }
        return false;
    }
}
