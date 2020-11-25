using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClassManagement;
using UnityEngine.SceneManagement;

public class MazeLoader : MonoBehaviour
{
    public int rowMaze = 0, colMaze = 0;
    public GameObject wall;
    public GameObject floor;
    public GameObject modelAI;
    public Player playerAI;
    public float wallSize = 4f;
    int[] arrOfInt01 = new int[]{66, 8, 6, 8, 6, 6, 8, 4, 4, 8, 6, 6, 0, 4, 4, 8, 8, 8, 6, 6, 2, 4, 2, 6, 6, 6, 6, 8, 4, 0, 66, 8, 8, 8, 6, 6, 8, 6, 2, 0, 8, 8, 0, 44, 6, 66, 0, 8, 6, 6, 6, 0, 44, 66, 0};
    int[] arrOfInt02 = new int[]{4, 4, 8, 6, 6, 0, 66, 8, 8, 8, 8, 6, 2, 2, 6, 8, 6, 2, 6, 2, 6, 8, 8, 4, 8, 4, 4, 8, 4, 4, 8, 6, 6, 6, 2, 6, 6, 2, 0, 44, 6, 6, 8, 8, 8, 6, 8, 4, 8, 8, 4, 0, 8, 8, 4, 22, 8, 0, 22, 0};

    private GameObject Ground;
    private GameObject Walls;

    public MazeCells[][] maze;
    public (int, int) startPoint;
    public (int, int) endPoint;
    public float speedOfAI = 10f;
    public List<(int, int)> arrOfPoint = new List<(int, int)>(){(0, 3), (1, 3), (1, 4), (2, 4), (2, 3), (4, 3), (5, 3), (5, 2), (5, 1), (6, 1), (6, 2), (6, 3)};
    public Dictionary<(int, int), (int, int)> BestRoad = new Dictionary<(int, int), (int, int)>();
    // Start is called before the first frame update
    void Start()
    {
        initializeMaze();
        
        MazeAlgorihm ma;
        if(SceneManager.GetActiveScene().name == "Level01"){
            startPoint = (0, 3);
            endPoint = (6, 3);
            ma = new HuntAndKillMazeAlgorithm(maze, arrOfInt01, startPoint, endPoint);
        }else{
            startPoint = (0, 3);
            endPoint = (6, 4);
            ma = new HuntAndKillMazeAlgorithm(maze, arrOfInt02, startPoint, endPoint);
        }
        playerAI = new Player(modelAI, maze, startPoint, endPoint);
        
        ma.createMaze();
        //BestRoad = ma.SearchBFS();
        BestRoad = ma.SearchDFS();
        // foreach (KeyValuePair<(int, int), (int, int)> item in BestRoad)
        // {   
        //     Debug.Log(item.Value + " - " + item.Key);
        // }    
        
        arrOfPoint = ma.findBestRoad(BestRoad);
    }
    // Update is called once per frame
    void Update()
    {
        playerAI.moveAI(ref arrOfPoint);
    }


    private void initializeMaze(){
        Ground = new GameObject();
        Ground.name = "Player-Yard";
        Walls = new GameObject();
        Walls.name = "Walls";

        maze = new MazeCells[rowMaze][];
        for(int i = 0; i < rowMaze; i++){
            maze[i] = new MazeCells[colMaze];
        }

        for(int i = 0; i < rowMaze; i++){
            for(int j = 0; j < colMaze; j++){
                maze[i][j] = new MazeCells();
                GameObject tempFloor = Instantiate(floor, new Vector3(j * wallSize, 0f, i * wallSize), Quaternion.identity) as GameObject;
                Vector3 temp = new Vector3(j * wallSize, 0.24f, i * wallSize);
                maze[i][j].Center = temp;
                tempFloor.layer = 8;
                tempFloor.transform.localScale = new Vector3(2f, 1f, 2f);
                maze[i][j].Floor = tempFloor;
                tempFloor.transform.parent = Ground.transform;

                if(j == 0){
                    GameObject tempWallLeft = Instantiate(wall, new Vector3(j * wallSize - wallSize/2, 0f, i * wallSize), Quaternion.Euler(0f, 90f, 0f)) as GameObject;
                    tempWallLeft.transform.localScale = new Vector3(2f, 1f, 1f);
                    maze[i][j].LeftWall = tempWallLeft;
                    maze[i][j].LeftWall.name = "LeftWall" + i + j;
                    tempWallLeft.transform.parent = Walls.transform;
                }

                GameObject tempWallRight = Instantiate(wall, new Vector3(j * wallSize + wallSize/2, 0f, i * wallSize), Quaternion.Euler(0f, 90f, 0f)) as GameObject;
                tempWallRight.transform.localScale = new Vector3(2f, 1f, 1f);
                maze[i][j].RightWall = tempWallRight;
                maze[i][j].RightWall.name = "RightWall" + i + j;
                tempWallRight.transform.parent = Walls.transform;

                if(i == 0){
                    GameObject tempWallBack = Instantiate(wall, new Vector3(j * wallSize, 0f, i * wallSize - wallSize/2), Quaternion.identity) as GameObject;
                    tempWallBack.transform.localScale = new Vector3(2f, 1f, 1f);
                    maze[i][j].BackWall = tempWallBack;
                    maze[i][j].BackWall.name = "BackWall" + i + j;
                    tempWallBack.transform.parent = Walls.transform;
                }

                GameObject tempWallForward = Instantiate(wall, new Vector3(j * wallSize, 0f, i * wallSize + wallSize/2), Quaternion.identity) as GameObject;
                tempWallForward.transform.localScale = new Vector3(2f, 1f, 1f);
                maze[i][j].ForwardWall = tempWallForward;
                maze[i][j].ForwardWall.name = "ForwardWall" + i + j;
                tempWallForward.transform.parent = Walls.transform;

                
            }
        }






        Ground.transform.parent = transform;
        Walls.transform.parent = transform;
    }
}
