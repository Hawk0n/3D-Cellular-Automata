using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Collections;

public class GameBoard : MonoBehaviour
{
    public int3 boardDimensions = new int3(10, 10, 10);
    public int cellSize = 1;
    public int cellCount;
    [HideInInspector] public NativeHashMap<int3, Cell> cellArray;

    private void Start() {
        cellCount = boardDimensions.x * boardDimensions.y * boardDimensions.z;
        cellArray = new NativeHashMap<int3, Cell>(cellCount, Allocator.Persistent);
        for (int x = 0; x < boardDimensions.x; x++){
            for (int y = 0; y < boardDimensions.y; y++){
                for (int z = 0; z < boardDimensions.z; z++){
                    cellArray[new int3(x,y,z)] = new Cell();

                    //Debug.DrawLine(Vector3.zero, (float3)new int3(x,y,z) * cellSize, Color.red, 5f);
                }
            }
        }
    }
    

    public struct Cell // Can contain data if i want that in the future
    {
        
    }
}
