using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Cellular_Logic : MonoBehaviour
{
    GameBoard gameBoard;

    private void Start() {
        gameBoard = GetComponent<GameBoard>();
    }

    void Update()
    {
        int3 centerIndex = new int3(0,0,0);

        for (int i = 0; i < gameBoard.cellCount; i++)
        {
            // Find next centerIndex
            if(centerIndex.x == gameBoard.boardDimensions.x)
            {
                centerIndex.x = 0;
                centerIndex.y++;
            } else if(centerIndex.y == gameBoard.boardDimensions.y)
            {
                centerIndex.x = 0;
                centerIndex.y = 0;
                centerIndex.z++;
            } else
                centerIndex.x++;

            // Find all surrounding indexes
            int3[] surroundingIndexes = 
            {
                // Bottom layer
                new int3(centerIndex.x+0, centerIndex.y-1, centerIndex.z+0),
                new int3(centerIndex.x+1, centerIndex.y-1, centerIndex.z+0),
                new int3(centerIndex.x+0, centerIndex.y-1, centerIndex.z+1),
                new int3(centerIndex.x+1, centerIndex.y-1, centerIndex.z+1),
                new int3(centerIndex.x-1, centerIndex.y-1, centerIndex.z-0),
                new int3(centerIndex.x-0, centerIndex.y-1, centerIndex.z-1),
                new int3(centerIndex.x-1, centerIndex.y-1, centerIndex.z-1),

                // Middle layer
                new int3(centerIndex.x+1, centerIndex.y-0, centerIndex.z+0),
                new int3(centerIndex.x+0, centerIndex.y-0, centerIndex.z+1),
                new int3(centerIndex.x+1, centerIndex.y-0, centerIndex.z+1),
                new int3(centerIndex.x-1, centerIndex.y-0, centerIndex.z-0),
                new int3(centerIndex.x-0, centerIndex.y-0, centerIndex.z-1),
                new int3(centerIndex.x-1, centerIndex.y-0, centerIndex.z-1),

                // Top layer
                new int3(centerIndex.x+0, centerIndex.y+1, centerIndex.z+0),
                new int3(centerIndex.x+1, centerIndex.y+1, centerIndex.z+0),
                new int3(centerIndex.x+0, centerIndex.y+1, centerIndex.z+1),
                new int3(centerIndex.x+1, centerIndex.y+1, centerIndex.z+1),
                new int3(centerIndex.x-1, centerIndex.y+1, centerIndex.z-0),
                new int3(centerIndex.x-0, centerIndex.y+1, centerIndex.z-1),
                new int3(centerIndex.x-1, centerIndex.y+1, centerIndex.z-1),
            };

            int cellMap = 0;
            for(int k = 0; k < surroundingIndexes.Length; k++)
            {
                if(gameBoard.cellArray.TryGetValue(surroundingIndexes(k), out GameBoard.Cell cell))
                {
                    cellMap = 1 << k;
                }
            }

            Debug.Log(cellMap);
        }
    }
}
