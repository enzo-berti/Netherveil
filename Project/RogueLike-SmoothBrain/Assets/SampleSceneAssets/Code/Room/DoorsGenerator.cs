using System;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState : byte
{
    NONE = 0,
    OPEN,
    CLOSE
}

public class DoorsGenerator : MonoBehaviour
{
    public static int RandGenerator { get; private set; } = 0;
    DoorState[] doorsState;

    [SerializeField, Range(1, int.MaxValue)] private int minDoors = 1;
    [SerializeField] private int maxDoors = 4;

    public int NbAvailableDoors
    {
        get
        {
            int availableDoors = 0;
            foreach (var state in doorsState)
            {
                if (state == DoorState.NONE)
                {
                    availableDoors++;
                }
            }

            return availableDoors;
        }
    }

    public List<GameObject> AvailableDoors
    {
        get
        {
            List<GameObject> result = new List<GameObject>();

            for (int i = 0; i < doorsState.Length; i++)
            {
                if (doorsState[i] == DoorState.NONE)
                {
                    result.Add(transform.GetChild(i).gameObject);
                }
            }

            return result;
        }
    }

    private void OnValidate()
    {
        minDoors = Mathf.Clamp(minDoors, minDoors, maxDoors);
        maxDoors = Mathf.Clamp(maxDoors, minDoors, maxDoors);
    }

    private void Awake()
    {
        doorsState = new DoorState[transform.childCount];
    }

    /// <summary>
    /// TODO : J'aime pas le fait qu'on renvoie une list de GO alors que la fonction n'est pas un "get"
    /// surtout que si la fonction est call deux fois elle renvera pas du tout la même chose
    /// </summary>
    public List<GameObject> GenerateDoors(GenerationParameters generationParameters)
    {
        List<GameObject> result = new List<GameObject>();

        // TODO : ADD RANDOM VALUE BETWEEN (x, y) 

        // get number of doors that can spawn depending on the number of rooms available by genParams
        int numDoor = NbAvailableDoors - Mathf.Clamp(NbAvailableDoors - generationParameters.NbRoom, 0, int.MaxValue);

        // add doors with state.NONE in a random order
        while (numDoor > 0)
        {
            int index = (GameManager.Instance.seed.Range(0, doorsState.Length) + DoorsGenerator.RandGenerator) % doorsState.Length;
            DoorsGenerator.RandGenerator++;

            if (doorsState[index] == DoorState.NONE)
            {
                result.Add(gameObject.transform.GetChild(index).gameObject);
                doorsState[index] = DoorState.OPEN;
                numDoor--;
            }
        }

        // close the other doors
        for (int i = 0; i < doorsState.Length; i++)
        {
            doorsState[i] = doorsState[i] == DoorState.NONE ? DoorState.CLOSE : doorsState[i];
        }

        return result;
    }

    public GameObject GetRandomAvailableDoor()
    {
        var availableDoors = AvailableDoors;
        int index = (GameManager.Instance.seed.Range(0, availableDoors.Count) + DoorsGenerator.RandGenerator) % availableDoors.Count;

        return availableDoors[index];
    }

    public void SetDoorState(DoorState state, int index)
    {
        
    }

    public void SetDoorState(DoorState state, GameObject childObject)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (childObject == transform.GetChild(i).gameObject)
            {
                doorsState[i] = state;
                return;
            }
        }

        Debug.LogWarning("Try to set a door state with the wrong GameObject in " + gameObject, childObject);
    }
}
