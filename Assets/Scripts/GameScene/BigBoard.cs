using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigBoard : Board, IEquatable<BigBoard>{
    public int lastMoveI;
    public int lastMoveJ;
    public Board[] boards;

    public Turn turn;

    public BigBoard(Board[] boards, int lastMoveI, int lastMoveJ, Turn turn) : base(null, -1){
        this.lastMoveI = lastMoveI;
        this.lastMoveJ = lastMoveJ;
        this.boards = boards;
        this.turn = turn;
    }
    public BigBoard(Turn turn) : base(null, -1){
        boards = new Board[9];
        for (int i = 0; i < 9; i++){
            boards[i] = new Board(this, i);
        }
        lastMoveI = -1;
        lastMoveJ = -1;
        this.turn = turn;
    }
    
    public List<BigBoard> GetFutureBigBoards(){
        List<BigBoard> futureBigBoards = new List<BigBoard>();
        if (lastMoveI == -1 || lastMoveJ == -1){
            for (int i = 0; i < boards.Length; i++){
                List<int> allAvailableIndiciesOnMiniBoard = boards[i].GetAvailableIndicies();
                for (int j = 0; j < allAvailableIndiciesOnMiniBoard.Count; j++){
                    int miniBoardI = allAvailableIndiciesOnMiniBoard[j] / 3;
                    int miniBoardJ = allAvailableIndiciesOnMiniBoard[j] % 3;
                    BigBoard bigBoardCopy = CopyBoard();
                    bigBoardCopy.boards[i].MakeMove(miniBoardI, miniBoardJ);
                    futureBigBoards.Add(bigBoardCopy);
                }
            } 
        }else{
            List<int> allAvailableIndiciesOnMiniBoard = boards[lastMoveI*3+lastMoveJ].GetAvailableIndicies();
            for (int j = 0; j < allAvailableIndiciesOnMiniBoard.Count; j++){
                int miniBoardI = allAvailableIndiciesOnMiniBoard[j] / 3;
                int miniBoardJ = allAvailableIndiciesOnMiniBoard[j] % 3;
                BigBoard bigBoardCopy = CopyBoard();
                bigBoardCopy.boards[lastMoveI*3+lastMoveJ].MakeMove(miniBoardI, miniBoardJ);
                futureBigBoards.Add(bigBoardCopy);
            }
        }
        return futureBigBoards;
    }

    BigBoard CopyBoard(){
        Board[] boardCopies = new Board[boards.Length];
        BigBoard bigBoardCopy = new BigBoard(boardCopies, lastMoveI, lastMoveJ, turn);
        for (int i = 0; i < boards.Length; i++){
            bigBoardCopy.boards[i] = new Board(boards[i].CopyGrid(), bigBoardCopy, i)
            {
                winner = boards[i].winner,
                gameOver = boards[i].gameOver,
                numMoves = boards[i].numMoves,
            };
        }
        bigBoardCopy.grid = CopyGrid();
        bigBoardCopy.numMoves = numMoves;
        return bigBoardCopy;
    }
    public int Score(Turn computerTurn){
        Turn playerTurn = computerTurn == Turn.X ? Turn.O : Turn.X;
        int score = 0;
        
        if (winner == computerTurn) return int.MaxValue-100;
        if (winner == playerTurn) return int.MinValue+100;
        if (winner == Turn.NONE && gameOver) return 0;

        score += ((int) Mathf.Pow(MaxNumConnected(computerTurn), 5) - (int)Mathf.Pow(MaxNumConnected(playerTurn), 5)) * 25;
        score += (int)(Mathf.Pow(NumPossibleConnect3(computerTurn), 4) - Mathf.Pow(NumPossibleConnect3(playerTurn), 4));

        for (int i = 0; i < boards.Length; i++){
            if (boards[i].winner == computerTurn){
                score += 1000;
                continue;
            }
            if (boards[i].winner == playerTurn){
                score -= 1000;
                continue;
            }
            int computerInARow = boards[i].MaxNumConnected(computerTurn);
            int playerInARow = boards[i].MaxNumConnected(playerTurn); 
            score += ((int)Mathf.Pow(computerInARow, 3) - (int)Mathf.Pow(playerInARow, 3)) * 20;
            score += (int)(Mathf.Pow(boards[i].NumPossibleConnect3(computerTurn), 3) - Mathf.Pow(boards[i].NumPossibleConnect3(playerTurn), 3));
        }
        
        return score;
    }
    public override int MakeMove(int i, int j){
        if (gameOver) return 0;
        if (grid[i,j] == Turn.NONE){
            grid[i,j] = turn;
            numMoves++;
            if (ThreeInARow(turn)){
                gameOver = true;
                winner = turn;
                return 2;
            }else if (numMoves == 9){
                gameOver = true;
                winner = Turn.NONE;
            }
            return 1;
        }else{
            return 0;
        }
    }
    public Turn OppositeTurn(){
        return turn switch
        {
            Turn.X => Turn.O,
            Turn.O => Turn.X,
            _ => Turn.NONE,
        };
    }

    public bool Equals(BigBoard other){

        if (turn != other.turn) return false;
        if (gameOver != other.gameOver) return false;
        if (winner != other.winner) return false;
        if (numMoves != other.numMoves) return false;
        if (lastMoveI != other.lastMoveI) return false;
        if (lastMoveJ != other.lastMoveJ) return false;
        

        for (int i = 0; i < 9; i++){
            if (!boards[i].Equals(other.boards[i])) return false;
            if (grid[i/3, i%3] != other.grid[i/3, i%3]) return false; 
        }
        return true;
    }

    public override int GetHashCode(){
        int hashCode = gameOver.GetHashCode() * winner.GetHashCode() * 34 * turn.GetHashCode();
        for (int i = 0; i < 9; i++){
          hashCode += (i+5) * grid[i/3,i%3].GetHashCode()*6 + (i%3+6) * grid[i/3,i%3].GetHashCode()*124 + boards[i].GetHashCode();
        }    
        return hashCode;
    }
}





// public static bool operator ==(BigBoard my, BigBoard other){

//     if (my.turn != other.turn) return false;
//     if (my.gameOver != other.gameOver) return false;
//     if (my.winner != other.winner) return false;
//     if (my.numMoves != other.numMoves) return false;
//     if (my.lastMoveI != other.lastMoveI) return false;
//     if (my.lastMoveJ != other.lastMoveJ) return false;


//     for (int i = 0; i < 9; i++){
//         if (!my.boards[i].Equals(other.boards[i])) return false;
//     }
//     return true;
// }
// public static bool operator !=(BigBoard my, BigBoard other){
//     return !(my == other);
// }
