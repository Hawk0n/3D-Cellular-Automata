using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

public class Cellular_Logic : MonoBehaviour
{
    GameBoard gameBoard;

    public void CalculateNextEvolution()
    {
        if(gameBoard == null)
            gameBoard = GetComponent<GameBoard>();
        
        int3 centerIndex = new int3(0,0,0);

        for (int i = 0; i < gameBoard.cellCount; i++)
        {
            // Find next centerIndex
            centerIndex = new int3(i / (gameBoard.boardDimensions.z * gameBoard.boardDimensions.y), 
                                    (i / gameBoard.boardDimensions.z) % gameBoard.boardDimensions.y,
                                    i % gameBoard.boardDimensions.z);

            // Find all surrounding indexes
            int3[] surroundingIndexes = 
            {
                // Bottom layer
                new int3(centerIndex.x+0, centerIndex.y-1, centerIndex.z+1), // 1 forward
                new int3(centerIndex.x+1, centerIndex.y-1, centerIndex.z+1), // 2 forward right
                new int3(centerIndex.x+1, centerIndex.y-1, centerIndex.z+0), // 3 right
                new int3(centerIndex.x+1, centerIndex.y-1, centerIndex.z-1), // 4 back right
                new int3(centerIndex.x-0, centerIndex.y-1, centerIndex.z-1), // 5 back
                new int3(centerIndex.x-1, centerIndex.y-1, centerIndex.z-1), // 6 back left
                new int3(centerIndex.x-1, centerIndex.y-1, centerIndex.z-0), // 7 left
                new int3(centerIndex.x-1, centerIndex.y-1, centerIndex.z+1), // 8 forward left
                new int3(centerIndex.x+0, centerIndex.y-1, centerIndex.z+0), // 9 underneath
                
                // Middle layer
                new int3(centerIndex.x+0, centerIndex.y+0, centerIndex.z+1), // 10 forward
                new int3(centerIndex.x+1, centerIndex.y+0, centerIndex.z+1), // 11 forward right
                new int3(centerIndex.x+1, centerIndex.y+0, centerIndex.z+0), // 12 right
                new int3(centerIndex.x+1, centerIndex.y+0, centerIndex.z-1), // 13 back right
                new int3(centerIndex.x-0, centerIndex.y+0, centerIndex.z-1), // 14 back
                new int3(centerIndex.x-1, centerIndex.y+0, centerIndex.z-1), // 15 back left
                new int3(centerIndex.x-1, centerIndex.y+0, centerIndex.z-0), // 16 left
                new int3(centerIndex.x-1, centerIndex.y+0, centerIndex.z+1), // 17 forward left

                // Top layer
                new int3(centerIndex.x+0, centerIndex.y+1, centerIndex.z+1), // 18 forward
                new int3(centerIndex.x+1, centerIndex.y+1, centerIndex.z+1), // 19 forward right
                new int3(centerIndex.x+1, centerIndex.y+1, centerIndex.z+0), // 20 right
                new int3(centerIndex.x+1, centerIndex.y+1, centerIndex.z-1), // 21 back right
                new int3(centerIndex.x-0, centerIndex.y+1, centerIndex.z-1), // 22 back
                new int3(centerIndex.x-1, centerIndex.y+1, centerIndex.z-1), // 23 back left
                new int3(centerIndex.x-1, centerIndex.y+1, centerIndex.z-0), // 24 left
                new int3(centerIndex.x-1, centerIndex.y+1, centerIndex.z+1), // 25 forward left
                new int3(centerIndex.x+0, centerIndex.y+1, centerIndex.z+0), // 26 underneath
            };

            int cellMap = 0, aliveNeighbourCount = 0;
            GameBoard.Cell cell = new GameBoard.Cell();
            for(int k = 0; k < surroundingIndexes.Length; k++)
            {
                if(gameBoard.currentCellArray.TryGetValue(surroundingIndexes[k], out cell))
                {
                    if(cell.isAlive)
                    {
                        aliveNeighbourCount++;
                        cellMap += (1 << k);
                    }
                }
            }

            var isAlive = CalculateNextCell(centerIndex, cellMap, aliveNeighbourCount);
            gameBoard.nextCellArray[centerIndex] = isAlive;
        }
    }

    GameBoard.Cell CalculateNextCell(int3 centerIndex, int cellMap, int aliveNeighbourCount)
    {
        GameBoard.Cell newCell = new GameBoard.Cell();
        newCell.isAlive = Rules(gameBoard.rule, centerIndex, cellMap, aliveNeighbourCount);
        return newCell;
    }

    bool Rules(int rule, int3 centerIndex, int cellMap, int aliveNeighbourCount)
    {
        switch (rule)
        {
            case 1:
                if(aliveNeighbourCount <= 7 && gameBoard.currentCellArray[centerIndex].isAlive) {
                    // Debug.Log("Died from underpopulation");
                    return false;
                }
                else if(aliveNeighbourCount >= 17 && gameBoard.currentCellArray[centerIndex].isAlive) {
                    // Debug.Log("Died from overpopulation");
                    return false;
                }
                else if(aliveNeighbourCount == 8 && !gameBoard.currentCellArray[centerIndex].isAlive) {
                    // Debug.Log("Born");
                    return true;
                } 
                else if(gameBoard.currentCellArray[centerIndex].isAlive) {
                    // Debug.Log("Survived");
                    return true;
                }
                return false;

            case 2:
                return false;

            default:
                Debug.LogError("Nonexistent Rule");
                return false;
        }
    }
}
