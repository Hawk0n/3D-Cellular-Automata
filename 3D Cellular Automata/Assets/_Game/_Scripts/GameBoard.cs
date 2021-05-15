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
    public ComputeShader logicShader;
    public int3 boardDimensions = new int3(10, 10, 10);
    public int rule = 1;
    public float cellSize = 1;
    public int cellCount;
    public float timeBetweenGenerations = 1f;
    [Range(0f, 1f)] public float deadToAliveRatio = 0.75f; 
    public GameObject cube;
    public Dictionary<int3, Cell> cellDictionary;
    int[] aliveArray;
    Cellular_Logic cellular_Logic;
 
    ComputeBuffer nextCellBuffer, currentCellBuffer;
    private void Start() 
    {
        cellular_Logic = GetComponent<Cellular_Logic>();

        cellCount = boardDimensions.x * boardDimensions.y * boardDimensions.z;
        cellDictionary = new Dictionary<int3, Cell>();
        aliveArray = new int[cellCount];
        int i = 0;
        for (int z = 0; z < boardDimensions.z; z++){
            for (int y = 0; y < boardDimensions.y; y++){
                for (int x = 0; x < boardDimensions.x; x++)
                {
                    Cell newCell = new Cell();
                    newCell.isAlive = (Random.Range(0.0f, 1.0f) > deadToAliveRatio) ? true : false;
                    cellDictionary[new int3(x,y,z)] = newCell;
                    aliveArray[i] = newCell.isAlive ? 1 : 0;
                    i++;
                }
            }
        }

        StartCoroutine(AdvanceGenerations());
    }

    private IEnumerator AdvanceGenerations()
    {
        int3[] keyArray = new int3[cellDictionary.Count];
        cellDictionary.Keys.CopyTo(keyArray, 0);

        while (true)
        {
            CalculateNextGeneration();

            for(int i = 0; i < cellDictionary.Count; i++)
            {
                Destroy(cellDictionary[keyArray[i]].cellObject);

                var k = cellDictionary[keyArray[i]];
                k.isAlive = (aliveArray[i] == 1) ? true : false;
                cellDictionary[keyArray[i]] = k;

                SpawnCubes(keyArray[i]);
            }

            yield return new WaitForSeconds(timeBetweenGenerations);
        }
    }

    private void CalculateNextGeneration()
    {
        nextCellBuffer = new ComputeBuffer(cellCount, sizeof(int));
        currentCellBuffer = new ComputeBuffer(cellCount, sizeof(int));
        currentCellBuffer.SetData(aliveArray);

        logicShader.SetBuffer(0, "currentCellBuffer", currentCellBuffer);
        logicShader.SetBuffer(0, "nextCellBuffer", nextCellBuffer);
        logicShader.SetInt("ruleToUse", rule);
        logicShader.SetVector("boardDimensions", new float4(boardDimensions, 0));
        
        logicShader.Dispatch(0, Mathf.CeilToInt(boardDimensions.x / 8f), Mathf.CeilToInt(boardDimensions.y / 8f), Mathf.CeilToInt(boardDimensions.z / 8f));

        nextCellBuffer.GetData(aliveArray);

        currentCellBuffer.Release();
        nextCellBuffer.Release();
    }

    private void SpawnCubes(int3 centerIndex)
    {
        if(cellDictionary[centerIndex].isAlive) // Cell is alive
        {
            Cell newCell = new Cell();
            newCell.cellObject = GameObject.Instantiate(cube);
            newCell.cellObject.transform.parent = transform;
            newCell.cellObject.transform.localScale = (float3)cellSize;
            newCell.cellObject.transform.position = (float3)centerIndex * cellSize;
            newCell.isAlive = cellDictionary[centerIndex].isAlive;

            cellDictionary[centerIndex] = newCell;

            // Debug.Log("Alive");
        }
        else // cell is dead
        {
            // Debug.Log("Dead");
        }
    }

    public struct Cell
    {
        public bool isAlive;
        public GameObject cellObject;
    }
}
