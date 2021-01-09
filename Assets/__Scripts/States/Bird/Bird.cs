using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BirdStates {
    Rest,
    Flying,
    Chasing,
    Eating,
}

public class Bird : MonoBehaviour
{
    [SerializeField] private Transform[] nodeSpots;
    [SerializeField] private Transform[] bees;
    [SerializeField] private Transform nest;
    [SerializeField] private Transform hive;
    [SerializeField] private float moveSpeed = 1.0f;
    
    private SpriteRenderer sprite;
    private GameObject targetBee;
    private BirdStates state;
    private Rigidbody2D rb;
    private float energyAmt = 500f;
    private float maxEnergy = 500f;
    private float energy;
    private int beesAmt;
    private int randomSpot;


    void Start()
    {
        state = BirdStates.Flying;
        sprite = GetComponent<SpriteRenderer>();
        randomSpot = Random.Range(0, nodeSpots.Length);
    }

    void Update() 
    {
        switch (state) {
            case BirdStates.Flying:
                FlyingState();
                if(CheckBees()) {
                    state = BirdStates.Chasing;
                }
                CheckEnergy();
                break;

            case BirdStates.Chasing:
                ChasingState();
                CheckEnergy();
                break;

            case BirdStates.Rest:
                RestState();
                CheckEnergy();
                break;

            case BirdStates.Eating:
                EatingState();
                CheckEnergy();
                break;
        }
    }

    // RESTING STATE     
    void RestState() {
        sprite.color = new Color (0, 0, 1);
        transform.position = Vector2.MoveTowards(transform.position, nest.position, moveSpeed * Time.deltaTime);

        if(Vector2.Distance(transform.position, nest.position) < 0.2f) {
            energyAmt += 0.3f;

            if(energyAmt >= 500f) {
                state = BirdStates.Flying;
            }
        }
    }
    // FLYING STATE 
    void FlyingState() {
        sprite.color = new Color (1, 0, 1);
        transform.position = Vector2.MoveTowards(transform.position, nodeSpots[randomSpot].position, moveSpeed * Time.deltaTime);
        energy = 0.4f;
        energyAmt -= energy;

        if(Vector2.Distance(transform.position, nodeSpots[randomSpot].position) < 0.2f) {
            randomSpot = Random.Range(0, nodeSpots.Length);
        }

        // Check for energy
        if(!HasEnergy()) {
            state = BirdStates.Rest;
        }
    }

    // CHASING STATE 
    void ChasingState() {
        sprite.color = new Color (1, 0, 0);

        // Move towards bee
        transform.position = Vector2.MoveTowards(transform.position, targetBee.transform.position, moveSpeed * Time.deltaTime);
        energy = 0.8f;
        energyAmt -= energy;

        // Check distance between bees
        if(Vector2.Distance(transform.position, targetBee.transform.position) <= 0.2f) {
            state = BirdStates.Eating;
        }
        

        // Check for energy
        if(!HasEnergy()) {
            state = BirdStates.Rest;
        }

        if(CheckHive()) {
            state = BirdStates.Flying;
        }
    }
    
    // EATING STATE 
    void EatingState() {
        sprite.color = new Color (0, 1, 0);
        Destroy(targetBee);
        energyAmt = maxEnergy;

        state = BirdStates.Flying;

        if(!HasEnergy()) {
            state = BirdStates.Rest;
        }
    }

    // Check whether the bird has energy
    public bool HasEnergy() {
        if(energyAmt > 0) 
            return true;
        else
            return false;
    }

    // Check whether there are birds nearby
    public bool CheckBees() {
        for(int i = 0; i < bees.Length; i++) {
            if(Vector2.Distance(transform.position, bees[i].position) <= 1f) {
                targetBee = bees[i].gameObject;
                return true;
            }
        }
        return false;
    }

    public bool CheckHive() {
        if(Vector2.Distance(transform.position, hive.position) <= 1f) {
            return true;
        }
        return false;
    }

    // Check how much energy bees have
    public bool CheckEnergy() {
        if(energyAmt <= 300f) {
            sprite.color = new Color (0, 1, 1);
            return true;
        }
        return false;
    }
}
