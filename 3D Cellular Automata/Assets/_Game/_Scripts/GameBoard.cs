using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Random = UnityEngine.Random;

public class GameBoard : MonoBehaviour
{
    public int3 boardDimensions = new int3(10, 10, 10);
    public int rule = 1;
    public float cellSize = 1;
    public int cellCount;
    public float timeBetweenGenerations = 1f;
    [Range(0f, 1f)] public float deadToAliveRatio = 0.75f; 
    public GameObject cube;
    public Dictionary<int3, Cell> currentCellArray;
    public Dictionary<int3, Cell> nextCellArray;
    Cellular_Logic cellular_Logic;
 

    private void Start() 
    {
        cellular_Logic = GetComponent<Cellular_Logic>();

        cellCount = boardDimensions.x * boardDimensions.y * boardDimensions.z;
        currentCellArray = new Dictionary<int3, Cell>();
        nextCellArray = new Dictionary<int3, Cell>();
        for (int x = 0; x < boardDimensions.x; x++){
            for (int y = 0; y < boardDimensions.y; y++){
                for (int z = 0; z < boardDimensions.z; z++)
                {
                    Cell newCell = new Cell();
                    newCell.isAlive = (Random.Range(0.0f, 1.0f) > deadToAliveRatio) ? true : false;
                    currentCellArray[new int3(x,y,z)] = newCell;
                }
            }
        }

        StartCoroutine(AdvanceGenerations());
    }


    void Update()
    {
        
    }

    private IEnumerator AdvanceGenerations()
    {
        while (true)
        {
            cellular_Logic.CalculateNextEvolution();

            int3[] keyArray = new int3[currentCellArray.Count];
            currentCellArray.Keys.CopyTo(keyArray, 0);
            for(int i = 0; i < currentCellArray.Count; i++)
            {
                Destroy(currentCellArray[keyArray[i]].cellObject);
                currentCellArray[keyArray[i]] = nextCellArray[keyArray[i]];
                SpawnCubes(keyArray[i]);
            }

            yield return new WaitForSeconds(timeBetweenGenerations);
        }
    }

    private void SpawnCubes(int3 centerIndex)
    {
        if(currentCellArray[centerIndex].isAlive) // Cell is alive
        {
            Cell newCell = new Cell();
            newCell.cellObject = GameObject.Instantiate(cube);
            newCell.cellObject.transform.localScale = (float3)cellSize;
            newCell.cellObject.transform.position = (float3)centerIndex * cellSize;
            newCell.isAlive = currentCellArray[centerIndex].isAlive;

            currentCellArray[centerIndex] = newCell;
        }
        else // cell is dead
        {

        }
    }

    public struct Cell
    {
        public bool isAlive;
        public GameObject cellObject;
    }
}
