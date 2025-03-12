using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : IEquatable<Board>{

    public Turn[,] grid;
    public bool gameOver = false;
    public Turn winner; 

    private BigBoard bigBoard;
    private int index;
    public Turn MyTurn => bigBoard.turn;
    public int numMoves = 0;

    public Board(BigBoard bigBoard, int index){
        this.grid = new Turn[3,3];
        for (int i = 0; i < 3; i++){
            for (int j = 0; j < 3; j++){
                grid[i,j] = Turn.NONE;
            }
        }
        this.bigBoard = bigBoard;
        this.index = index;
        winner = Turn.NONE;
    }
    public Board(Turn[,] grid, BigBoard bigBoard, int index){
        this.grid = grid;
        winner = Turn.NONE;
        this.bigBoard = bigBoard;
        this.index = index;
    }

    public List<int> GetAvailableIndicies(){
        List<int> availableIndicies = new List<int>();
        if (gameOver) return availableIndicies;
        for (int i = 0; i < 3; i++){
            for (int j = 0; j < 3; j++){
                if (grid[i,j] == Turn.NONE){
                    availableIndicies.Add(i*3+j);
                }
            }
        }
        return availableIndicies;
    }

    public Turn[,] CopyGrid(){
        Turn[,] newGrid = new Turn[3,3];
        for (int i = 0; i < 3; i++){
            for (int j = 0; j < 3; j++){
                if (grid[i,j] == Turn.X){
                    newGrid[i,j] = Turn.X;
                }else if (grid[i,j] == Turn.O){
                    newGrid[i,j] = Turn.O;
                }else{
                    newGrid[i,j] = Turn.NONE;
                }
            }
        }
        return newGrid;
    }

    public virtual int MakeMove(int i, int j){
        if (gameOver) return 0;
        bool threeInARow = false;
        if (grid[i,j] == Turn.NONE){
            grid[i,j] = MyTurn;
            bigBoard.lastMoveI = i;
            bigBoard.lastMoveJ = j;
            numMoves++;
            if (ThreeInARow(MyTurn)){
                gameOver = true;
                winner = MyTurn;
                bigBoard.MakeMove(index / 3, index % 3);
                threeInARow = true;
            }else if (numMoves == 9){
                gameOver = true;
                winner = Turn.NONE;
            }
            bigBoard.turn = bigBoard.OppositeTurn();
            if (bigBoard.boards[i*3+j].gameOver){
                bigBoard.lastMoveI = -1;
                bigBoard.lastMoveJ = -1;
            }
            if (threeInARow) return 2;
            return 1;
        }else{
            return 0;
        }
    }

    public bool ThreeInARow(Turn turn){
        return (grid[0, 0] == turn && grid[0, 1] == turn && grid[0, 2] == turn) ||
            (grid[1, 0] == turn && grid[1, 1] == turn && grid[1, 2] == turn) ||
            (grid[2, 0] == turn && grid[2, 1] == turn && grid[2, 2] == turn) ||
            (grid[0, 0] == turn && grid[1, 0] == turn && grid[2, 0] == turn) ||
            (grid[0, 1] == turn && grid[1, 1] == turn && grid[2, 1] == turn) ||
            (grid[0, 2] == turn && grid[1, 2] == turn && grid[2, 2] == turn) ||
            (grid[0, 0] == turn && grid[1, 1] == turn && grid[2, 2] == turn) ||
            (grid[0, 2] == turn && grid[1, 1] == turn && grid[2, 0] == turn);
    }

    public int NumPossibleConnect3(Turn turn){
        Turn oppositeTurn = turn == Turn.X ? Turn.O : Turn.X;
        int numPossibleConnect3 = 0;
        for (int i = 0; i < 3; i++){
            bool possibleConnect3 = true;
            for (int j = 0; j < 3; j++){
                if (grid[i,j] == oppositeTurn){
                    possibleConnect3 = false;
                    break;
                }
            }
            if (possibleConnect3) numPossibleConnect3++;
        }
        for (int i = 0; i < 3; i++){
            bool possibleConnect3 = true;
            for (int j = 0; j < 3; j++){
                if (grid[i,j] == oppositeTurn){
                    possibleConnect3 = false;
                    break;
                }
            }
            if (possibleConnect3) numPossibleConnect3++;
        }
        if (grid[0,0] != oppositeTurn && grid[1,1] != oppositeTurn && grid[2,2] != oppositeTurn){
            numPossibleConnect3++;
        }
        if (grid[2,0] != oppositeTurn && grid[1,1] != oppositeTurn && grid[0,2] != oppositeTurn){
            numPossibleConnect3++;
        }
        return numPossibleConnect3;
    }
    public Vector2Int[] ThreeInARowWithLocations(Turn turn){
        int consecutive;
        Vector2Int[] winningTiles;
        for (int i = 0; i < 3; i++){
            consecutive = 0;
            for (int j = 0; j < 3; j++){
                if (this.grid[i,j] == turn){
                    consecutive++;
                    if (consecutive == 3){
                        winningTiles = new Vector2Int[3];
                        winningTiles[0] = new Vector2Int(i, j);
                        winningTiles[1] = new Vector2Int(i, j-1);
                        winningTiles[2] = new Vector2Int(i, j-2);
                        return winningTiles;
                    }
                    
                }else{
                    consecutive = 0;
                }
            }
        }
        for (int j = 0; j < 3; j++){
            consecutive = 0;
            for (int i = 0; i < 3; i++){
                if (this.grid[i, j] == turn){
                    consecutive++;
                    if (consecutive == 3){
                        winningTiles = new Vector2Int[3];
                        winningTiles[0] = new Vector2Int(i, j);
                        winningTiles[1] = new Vector2Int(i-1, j);
                        winningTiles[2] = new Vector2Int(i-2, j);
                        return winningTiles;
                    }
                }else{
                    consecutive = 0;
                }
            }
        }
        if (grid[0,0] == turn && grid[1,1] == turn && grid[2,2] == turn){
            winningTiles = new Vector2Int[3];
            winningTiles[0] = new Vector2Int(0, 0);
            winningTiles[1] = new Vector2Int(1, 1);
            winningTiles[2] = new Vector2Int(2, 2);
            return winningTiles;
        }
        if (grid[2,0] == turn && grid[1,1] == turn && grid[0,2] == turn){
            winningTiles = new Vector2Int[3];
            winningTiles[0] = new Vector2Int(2, 0);
            winningTiles[1] = new Vector2Int(1, 1);
            winningTiles[2] = new Vector2Int(0, 2);
            return winningTiles;
        } 
        return null;
    }

    public int MaxNumConnected(Turn turn){
        int maxConsecutive = 0;
        int consecutive = 0;
        for (int i = 0; i < 3; i++){
            consecutive = 0;
            for (int j = 0; j < 3; j++){
                if (this.grid[i,j] == turn){
                    consecutive++;
                    if (consecutive > maxConsecutive){
                        maxConsecutive = consecutive;
                    }
                }else{
                    consecutive = 0;
                }
            }
        }
        for (int j = 0; j < 3; j++){
            consecutive = 0;
            for (int i = 0; i < 3; i++){
                if (this.grid[i, j] == turn){
                    consecutive++;
                    if (consecutive > maxConsecutive){
                        maxConsecutive = consecutive;
                    }
                }else{
                    consecutive = 0;
                }
            }
        }
        consecutive = 0;
        int consecutive2 = 0;
        for (int i = 0; i < 3; i++){
            if (grid[i,i] == turn){
                consecutive++;
            }else{
                consecutive--;
            }
            if (consecutive > maxConsecutive){
                maxConsecutive = consecutive;
            }
            if (grid[2-i,i] == turn){
                consecutive2++;
            }else{
                consecutive2--;
            }
            if (consecutive2 > maxConsecutive){
                maxConsecutive = consecutive2;
            }
            
        }
        return maxConsecutive;
    }
    



    public bool Equals(Board other){   
        if (MyTurn != other.MyTurn) return false;
        if (gameOver != other.gameOver) return false;
        if (winner != other.winner) return false;
        if (numMoves != other.numMoves) return false;

        for (int i = 0; i < 3; i++){
            for (int j = 0; j < 3; j++){
                if (other.grid[i,j] != this.grid[i,j]) return false;
            }
        }

        return true;
    }
    public override int GetHashCode(){
        int hashCode = gameOver.GetHashCode() * winner.GetHashCode() * 24653 * MyTurn.GetHashCode();
        for (int i = 0; i < 3; i++){
            for (int j = 0; j < 3; j++){
                hashCode += (i+5) * grid[i,j].GetHashCode()*6 + (j+6) * grid[i,j].GetHashCode()*124;
            }
        }    
        return hashCode;
    }
}   
