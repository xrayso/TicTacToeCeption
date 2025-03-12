using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{

    BigBoard bigBoard;
    public static GameManager instance;
    [SerializeField]  MiniBoardManager[] boardManagers;
    [SerializeField]  GameObject spriteX;
    [SerializeField]  GameObject spriteO;

    [SerializeField] bool againstComputer, computerVsComputer;
    [SerializeField] Turn computerTurn;
    [SerializeField] int depth;

    [SerializeField] public bool developMode {get; private set;}
    Node computerBot;
    public Dictionary<BigBoard, int> predeterminedBoardScores;


    void Awake(){
        instance = this;
        bigBoard = new BigBoard(Turn.X);
        predeterminedBoardScores = new Dictionary<BigBoard, int>();
    }

    void Start(){
        UpdateBoardColours();
        for (int i = 0; i < boardManagers.Length; i++){
            boardManagers[i].board = bigBoard.boards[i];
        }
    }


    

    public void MoveOnMiniBoard(int boardI, int boardJ, int i, int j){
        
        int boardIndex = boardI * 3 + boardJ;
        int moveResult = boardManagers[boardIndex].MakeMove(i, j);

        if (moveResult > 0){
            if (moveResult == 2){ // mini board won
                if (bigBoard.gameOver){
                    Vector2Int[] winningTiles = bigBoard.ThreeInARowWithLocations(bigBoard.OppositeTurn());
                    for (int index = 0; index < winningTiles.Length; index++){
                        Vector2Int tileIndex = winningTiles[index];
                        MiniBoardManager miniBoardManager = boardManagers[tileIndex.x*3+tileIndex.y].GetComponent<MiniBoardManager>();
                        // miniBoardManager.gameOverSprite.color = Color.green;
                        miniBoardManager.gameOverGameObject.GetComponent<Animation>().Play();
                    }
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            
            }
            
            Vector3 screenPosition = new Vector3(boardJ * (Screen.width/3f) + j * (Screen.width/9f) + Screen.width/18f, boardI * (Screen.height/3f) + i * (Screen.height/9f)-2f + Screen.height/18f, 1);

            Vector3 spritePos = Camera.main.ScreenToWorldPoint(screenPosition);
            if (bigBoard.OppositeTurn() == Turn.X){
                Instantiate(spriteX, spritePos, Quaternion.identity);
            }else{
                Instantiate(spriteO, spritePos, Quaternion.identity);
                bigBoard.turn = Turn.X;
            }
            UpdateBoardColours();
        }
    }

    void Update(){
        if (bigBoard.gameOver){
            return;
        }
        if (Input.GetMouseButtonDown(0) && (computerTurn != bigBoard.turn || !againstComputer) && !computerVsComputer){
            if (againstComputer && computerTurn == bigBoard.turn && !computerVsComputer) return;
            int boardI = (int) Input.mousePosition.y / (Screen.height/3);
            int boardJ = (int) Input.mousePosition.x / (Screen.width/3);
            
            if  ((boardI != bigBoard.lastMoveI || boardJ != bigBoard.lastMoveJ) && bigBoard.lastMoveI != -1) return;

            int i = (int) (Input.mousePosition.y / (Screen.height/9)) % 3;
            int j = (int) (Input.mousePosition.x / (Screen.width/9)) % 3;


            MoveOnMiniBoard(boardI, boardJ, i, j);
        }
        else if (againstComputer && computerTurn == bigBoard.turn || computerVsComputer){
            MakeComputerMove();
        }
    
        if (developMode){
            if (Input.GetKeyDown(KeyCode.Space)){
                Debug.Log("Real Board Grid: ");
                for (int k = 2; k >= 0; k--){
                    Debug.Log("" + bigBoard.grid[k,0] + bigBoard.grid[k,1] + bigBoard.grid[k,2]);
                }
            }
        }
        
    }


    void MakeComputerMove(){
        computerBot = new Node(0, bigBoard, bigBoard.turn);
        Vector2Int indicies = computerBot.GetComputerTurn(depth);
        int computerBoardIndex = indicies.x;
        int computerInBoardIndex = indicies.y;
        
        int computerBoardI = computerBoardIndex / 3;
        int computerBoardJ = computerBoardIndex % 3;

        int computerInBoardI = computerInBoardIndex / 3;
        int computerInBoardJ = computerInBoardIndex % 3;

        MoveOnMiniBoard(computerBoardI, computerBoardJ, computerInBoardI, computerInBoardJ);
    }


    void UpdateBoardColours(){
        if (bigBoard.lastMoveI != -1){
            for (int index = 0; index < boardManagers.Length; index++){
                boardManagers[index].gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }                
            boardManagers[bigBoard.lastMoveI*3+bigBoard.lastMoveJ].gameObject.GetComponent<SpriteRenderer>().color = bigBoard.turn == Turn.X ? Color.red : Color.cyan;
        }else{
            for (int index = 0; index < boardManagers.Length; index++){
                if (bigBoard.boards[index].gameOver){
                    boardManagers[index].gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    continue;
                }
                boardManagers[index].gameObject.GetComponent<SpriteRenderer>().color = bigBoard.turn == Turn.X ? Color.red : Color.cyan;
            }
        }
    }
}

