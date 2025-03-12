
using System.Collections.Generic;
using UnityEngine;
public class Node {

    readonly int depth;
    int score;
    BigBoard bigBoard;
    Node[] children;
    Turn computerTurn;
    

    public Node(int depth, BigBoard bigBoard, Turn computerTurn){
        this.depth = depth;
        this.bigBoard = bigBoard;
        this.computerTurn = computerTurn;
    }

    public Vector2Int GetComputerTurn(int depth){
        Extend(1);
        GameManager.instance.predeterminedBoardScores = new Dictionary<BigBoard, int>();
        int bestScore = MiniMax(depth, int.MinValue, int.MaxValue, true);
        if (GameManager.instance.developMode){
            Debug.Log("Best Score: " + bestScore);
            Debug.Log("bestScore: " + bestScore);
            Debug.Log("Score of current board: " + this.bigBoard.Score(this.computerTurn));

            for (int j = 6; j >= 0; j-= 3){
                for (int k = 2; k >= 0; k--){
                    Debug.Log("" + children[0].children[0].bigBoard.boards[j].grid[k,0] + children[0].children[0].bigBoard.boards[j].grid[k,1] + children[0].children[0].bigBoard.boards[j].grid[k,2] + " " + children[0].children[0].bigBoard.boards[j+1].grid[k,0] + children[0].children[0].bigBoard.boards[j+1].grid[k,1] + children[0].children[0].bigBoard.boards[j+1].grid[k,2] + " " + children[0].children[0].bigBoard.boards[j+2].grid[k,0] + children[0].children[0].bigBoard.boards[j+2].grid[k,1] + children[0].children[0].bigBoard.boards[j+2].grid[k,2]);
                }
            }

            Debug.Log("^ Game Over: " + children[0].children[0].bigBoard.gameOver);
            Debug.Log("^ Score: " + children[0].children[0].score);
            Debug.Log("Big board Grid: ");
            for (int k = 2; k >= 0; k--){
                Debug.Log("" + children[0].children[0].bigBoard.grid[k,0] + children[0].children[0].bigBoard.grid[k,1] + children[0].children[0].bigBoard.grid[k,2]);
            }

            for (int j = 6; j >= 0; j-= 3){
                for (int k = 2; k >= 0; k--){
                    Debug.Log("" + children[0].children[3].bigBoard.boards[j].grid[k,0] + children[0].children[3].bigBoard.boards[j].grid[k,1] + children[0].children[3].bigBoard.boards[j].grid[k,2] + " " + children[0].children[3].bigBoard.boards[j+1].grid[k,0] + children[0].children[3].bigBoard.boards[j+1].grid[k,1] + children[0].children[3].bigBoard.boards[j+1].grid[k,2] + " " + children[0].children[3].bigBoard.boards[j+2].grid[k,0] + children[0].children[3].bigBoard.boards[j+2].grid[k,1] + children[0].children[3].bigBoard.boards[j+2].grid[k,2]);
                }
            }
            Debug.Log("Score of first childs third childs first child: " + children[0].children[3].children[0].score);
            Debug.Log("Real Score: " + children[0].children[3].children[0].bigBoard.Score(computerTurn));
            Debug.Log("Game Over: " + children[0].children[3].children[0].bigBoard.boards[0].gameOver);
            Debug.Log("Turn " + children[0].children[3].children[0].bigBoard.boards[0].MyTurn);
            for (int j = 6; j >= 0; j-= 3){
                for (int k = 2; k >= 0; k--){
                    Debug.Log("" + children[0].children[3].children[0].bigBoard.boards[j].grid[k,0] + children[0].children[3].children[0].bigBoard.boards[j].grid[k,1] + children[0].children[3].children[0].bigBoard.boards[j].grid[k,2] + " " + children[0].children[3].children[0].bigBoard.boards[j+1].grid[k,0] + children[0].children[3].children[0].bigBoard.boards[j+1].grid[k,1] + children[0].children[3].children[0].bigBoard.boards[j+1].grid[k,2] + " " + children[0].children[3].children[0].bigBoard.boards[j+2].grid[k,0] + children[0].children[3].children[0].bigBoard.boards[j+2].grid[k,1] + children[0].children[3].children[0].bigBoard.boards[j+2].grid[k,2]);
                }
            }
        }
        
        for (int i = 0; i < children.Length; i++){
            if (children[i].score == bestScore){
                for (int j = 0; j < children[i].bigBoard.boards.Length; j++){
                    for (int k = 0; k < 3; k++){
                        for (int l = 0; l < 3; l++){
                            if (children[i].bigBoard.boards[j].grid[k,l] != bigBoard.boards[j].grid[k,l]){
                                return new Vector2Int(j, k * 3 + l);
                            }
                        }
                    }
                }
            }
        }
        return Vector2Int.zero;

    }

    void Extend(int depth){
        if (depth == 0 || bigBoard.gameOver){
            return;
        } 
        List<BigBoard> futureBoards = bigBoard.GetFutureBigBoards();
        children = new Node[futureBoards.Count];
        for (int i = 0; i < futureBoards.Count; i++){
            BigBoard childBigBoard = futureBoards[i];
            Node child = new Node(this.depth+1, childBigBoard, computerTurn);
            children[i] = child;
            child.Extend(depth-1);
        }
        
    }
    
    int MiniMax(int depth, int alpha, int beta, bool maximizingPlayer){
        if (GameManager.instance.predeterminedBoardScores.ContainsKey(bigBoard)){
            score = GameManager.instance.predeterminedBoardScores[bigBoard];
            if (score > 0) score -= this.depth;
            if (score < 0) score += this.depth;
            return score;
        }
        List<BigBoard> futureBoards = bigBoard.GetFutureBigBoards();
        if (children != null){
            if (children.Length == 0){
                int score = bigBoard.Score(computerTurn);
                GameManager.instance.predeterminedBoardScores.Add(bigBoard, score);
                this.score = score;
                return score;
            }
            
            int value;
            if (maximizingPlayer){
                value = int.MinValue;
                for (int i = 0; i < children.Length; i++){
                    value = Mathf.Max(value, children[i].MiniMax(depth - 1, alpha, beta, false));
                    alpha = Mathf.Max(alpha, value);
                    if (value >= beta){
                        break;
                    }
                }
                score = value;
                return value;
            }else{
                value = int.MaxValue;
                for (int i = 0; i < futureBoards.Count; i++){
                    Node childNode = new Node(this.depth+1, futureBoards[i], computerTurn);
                    value = Mathf.Min(value, childNode.MiniMax(depth - 1, alpha, beta, true));
                    beta = Mathf.Min(beta, value);
                    if (value <= alpha){
                        break;
                    }
                }
                score = value;
                return value;
            }
        }else{
            if (depth == 0 || futureBoards.Count == 0){
                int score = bigBoard.Score(computerTurn);
                GameManager.instance.predeterminedBoardScores.Add(bigBoard, score);
                this.score = score;
                return score;
            }

            int value;
            if (maximizingPlayer){
                value = int.MinValue;
                for (int i = 0; i < futureBoards.Count; i++){
                    Node childNode = new Node(this.depth+1, futureBoards[i], computerTurn);
                    value = Mathf.Max(value, childNode.MiniMax(depth - 1, alpha, beta, false));
                    alpha = Mathf.Max(alpha, value);
                    if (value >= beta){
                        break;
                    }
                }
                score = value;
                return value;
            }else{
                value = int.MaxValue;
                for (int i = 0; i < futureBoards.Count; i++){
                    Node childNode = new Node(this.depth+1, futureBoards[i], computerTurn);
                    value = Mathf.Min(value, childNode.MiniMax(depth - 1, alpha, beta, true));
                    beta = Mathf.Min(beta, value);
                    if (value <= alpha){
                        break;
                    }
                }
                score = value;
                return value;
            }
        }
    }

}


    // Debug.Log("bestScore: " + bestScore);
    //     Debug.Log("Score of current board: " + this.bigBoard.Score(this.computerTurn));
    //     for (int j = 6; j >= 0; j-= 3){
    //         for (int k = 2; k >= 0; k--){
    //             Debug.Log("" + children[0].bigBoard.boards[j].grid[k,0] + children[0].bigBoard.boards[j].grid[k,1] + children[0].bigBoard.boards[j].grid[k,2] + " " + children[0].bigBoard.boards[j+1].grid[k,0] + children[0].bigBoard.boards[j+1].grid[k,1] + children[0].bigBoard.boards[j+1].grid[k,2] + " " + children[0].bigBoard.boards[j+2].grid[k,0] + children[0].bigBoard.boards[j+2].grid[k,1] + children[0].bigBoard.boards[j+2].grid[k,2]);
    //         }
    //     }
    //     Debug.Log("Score of first childs third child: " + children[0].children[3].score);

    //     for (int j = 6; j >= 0; j-= 3){
    //         for (int k = 2; k >= 0; k--){
    //             Debug.Log("" + children[0].children[3].bigBoard.boards[j].grid[k,0] + children[0].children[3].bigBoard.boards[j].grid[k,1] + children[0].children[3].bigBoard.boards[j].grid[k,2] + " " + children[0].children[3].bigBoard.boards[j+1].grid[k,0] + children[0].children[3].bigBoard.boards[j+1].grid[k,1] + children[0].children[3].bigBoard.boards[j+1].grid[k,2] + " " + children[0].children[3].bigBoard.boards[j+2].grid[k,0] + children[0].children[3].bigBoard.boards[j+2].grid[k,1] + children[0].children[3].bigBoard.boards[j+2].grid[k,2]);
    //         }
    //     }
    //     Debug.Log("Score of first childs third childs first child: " + children[0].children[3].children[0].score);
    //     Debug.Log("Real Score: " + children[0].children[3].children[0].bigBoard.Score(computerTurn));
    //     Debug.Log("Game Over: " + children[0].children[3].children[0].bigBoard.boards[0].gameOver);
    //     Debug.Log("Turn " + children[0].children[3].children[0].bigBoard.boards[0].MyTurn);
    //     for (int j = 6; j >= 0; j-= 3){
    //         for (int k = 2; k >= 0; k--){
    //             Debug.Log("" + children[0].children[3].children[0].bigBoard.boards[j].grid[k,0] + children[0].children[3].children[0].bigBoard.boards[j].grid[k,1] + children[0].children[3].children[0].bigBoard.boards[j].grid[k,2] + " " + children[0].children[3].children[0].bigBoard.boards[j+1].grid[k,0] + children[0].children[3].children[0].bigBoard.boards[j+1].grid[k,1] + children[0].children[3].children[0].bigBoard.boards[j+1].grid[k,2] + " " + children[0].children[3].children[0].bigBoard.boards[j+2].grid[k,0] + children[0].children[3].children[0].bigBoard.boards[j+2].grid[k,1] + children[0].children[3].children[0].bigBoard.boards[j+2].grid[k,2]);
    //         }
    //     }