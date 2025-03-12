using System.Collections.Generic;
using UnityEngine;

public class MiniBoardManager : MonoBehaviour{

    public Board board;

    [SerializeField] bool isBigBoard;

    [SerializeField] GameObject xGameObject, oGameObject;
    [HideInInspector] public GameObject gameOverGameObject;
    [HideInInspector] public SpriteRenderer gameOverSprite;

    [HideInInspector] public bool gameOver => board.gameOver;


    public int MakeMove(int i, int j){
        int result = board.MakeMove(i, j);
        if (result == 2){
            DisplayPlayerWon(board.MyTurn == Turn.X ? Turn.O : Turn.X);
        }
        return result;
    }
    

    void DisplayPlayerWon(Turn turn){
        if (isBigBoard){
            return;
        }
        
        if (turn == Turn.X){
            gameOverGameObject = Instantiate(xGameObject, transform.position, Quaternion.identity);
        }else{
            gameOverGameObject = Instantiate(oGameObject, transform.position, Quaternion.identity);
        }
        gameOverSprite = gameOverGameObject.GetComponentInChildren<SpriteRenderer>();;
        gameOverSprite.transform.localScale = Vector3.one * 4f;
        gameOverSprite.sortingOrder = 2;
        

    }

}
