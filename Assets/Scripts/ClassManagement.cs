using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClassManagement
{
    [System.Serializable]
    public class Player{
        private Vector3 _lookRotation;
        private Vector3 _targetPosition;
        private bool _moving = false;
        public GameObject player;
        public MazeCells[][] map;
        public (int x, int y) _currPos, _aimPos;

        public Player(GameObject _player){
            player = _player;
            _lookRotation = Vector3.zero;
            _targetPosition = new Vector3(12f, 0.5f, 0f);
        }

        public Player(GameObject _player, MazeCells[][] _map, (int, int) _startPoint, (int, int) _endPoint){
            player = _player;
            _lookRotation = Vector3.zero;
            _targetPosition = new Vector3(12f, 0.5f, 0f);
            map = _map;
            _currPos = _startPoint;
            _aimPos = _endPoint;
        }

        public (int x, int y) CurrPos{
            get{
                return _currPos;
            }
            set{
                _currPos = value;
            }
        }

        public (int x, int y) AimPos{
            get{
                return _aimPos;
            }
            set{
                _aimPos = value;
            }
        }

        public Vector3 LookRotation{
            get{
                return _lookRotation;
            }
            set{
                _lookRotation = value;
            }
        }

        public Vector3 TargetPosition{
            get{
                return _targetPosition;
            }
            set{
                _targetPosition = value;
            }
        }

        public bool Moving{
            get{
                return _moving;
            }
            set{
                _moving = value;
            }
        }

        public void move(){
            if(player.transform.position == TargetPosition){
                Moving = false;
            }
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(LookRotation), 5 * Time.deltaTime);
            player.transform.position = Vector3.MoveTowards(player.transform.position, TargetPosition, 10 * Time.deltaTime);
        }

        public void lookAtTargetPosition(){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 1000) && hit.collider.tag == "Floor"){
                TargetPosition = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
                //this.transform.LookAt(targetPosition);
                LookRotation = new Vector3(TargetPosition.x - player.transform.position.x, player.transform.position.y, TargetPosition.z - player.transform.position.z);
                Moving = true;
            }
        }

        public void movePosition(Vector3 _aimPos){
            LookRotation = new Vector3(_aimPos.x - player.transform.position.x, player.transform.position.y, _aimPos.z - player.transform.position.z);

            if(player.transform.position == _aimPos){
                Moving = false;
            }
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(LookRotation), 5 * Time.deltaTime);
            player.transform.position = Vector3.MoveTowards(player.transform.position, _aimPos, 10 * Time.deltaTime);
        }

        public void moveAI(ref List<(int x, int y)> _arr){
            // if(player.transform.position != map[_arr[0].x][_arr[0].y].Center){
            //     movePosition(map[_arr[0].x][_arr[0].y].Center);
            // }

            // if(player.transform.position == map[_arr[0].x][_arr[0].y].Center && _arr.Count > 1){
            //     _arr.RemoveAt(0);
            // }

            if(player.transform.position != map[_arr[_arr.Count - 1].x][_arr[_arr.Count - 1].y].Center){
                movePosition(map[_arr[_arr.Count - 1].x][_arr[_arr.Count - 1].y].Center);
            }

            if(player.transform.position == map[_arr[_arr.Count - 1].x][_arr[_arr.Count - 1].y].Center && _arr.Count > 1){
                _arr.RemoveAt(_arr.Count - 1);
            }
        }
    }

    [System.Serializable]
    public class PlayerScore{
        private int _score;
        private int _totalScore;

        public int Score{
            get{
                return _score;
            }
            set{
                _score = value;
            }
        }

        public int TotalScore{
            get{
                return _totalScore;
            }
            set{
                _totalScore = value;
            }
        }

        public PlayerScore(int _maxScore){
            Score = _maxScore;
            TotalScore = 0;
        }
    }

    [System.Serializable]
    public class GameHandler{
        private float _timeOut;
        private bool _isWon;
        private bool _isLost;
        private float _maxTime;
        private int _maxScore;
        private PlayerScore player;
        private PlayerScore playerAI;
        private bool _isScorePlayer = false;
        private bool _isScorePlayerAI = false;
        //private static int _totalScore = 0;

        public static int TotalScorePlayer = 0;
        public static int TotalScoreAI = 0;

        public bool IsScorePlayer{
            get{
                return _isScorePlayer;
            }
            set{
                _isScorePlayer = value;
            }
        }

        public bool IsScorePlayerAI{
            get{
                return _isScorePlayerAI;
            }
            set{
                _isScorePlayerAI = value;
            }
        }

        public float MaxTime{
            get{
                return _maxTime;
            }
            set{
                _maxTime = value;
            }
        }

        public float TimeOut{
            get{
                return _timeOut;
            }
            set{
                _timeOut = value;
            }
        }

        public bool IsWon{
            get{
                return _isWon;
            }
            set{
                _isWon = value;
            }
        }

        public int MaxScore{
            get{
                return _maxScore;
            }
            set{
                _maxScore = value;
            }
        }

        public bool IsLost{
            get{
                return _isLost;
            }
            set{
                _isLost = value;
            }
        }

        public GameHandler(int _lv){
            if(_lv == 1){
                MaxTime = 180;
                MaxScore = 1000;
            }else if(_lv == 2){
                MaxTime = 360;
                MaxScore = 2000;
            }else{
                MaxTime = 10000;
            }

            TimeOut = MaxTime;
            IsWon = false;
            IsLost = false;
            player = new PlayerScore(MaxScore);
            playerAI = new PlayerScore(MaxScore);
        }

        public void timeOut(){
            TimeOut -= 1 * Time.deltaTime;
        }

        public void LosingChecker(){
            if(TimeOut <= 0){
                IsLost = true;
            }
        }

        public void UpdateCurrScore(){
            player.Score = (int)(TimeOut * MaxScore / MaxTime);
            playerAI.Score = (int)(TimeOut * MaxScore / MaxTime);
        }

        public void UpdateCurrScorePlayer(int _score){
            player.Score = _score;
        }

        public void UpdateCurrScorePlayerAI(int _score){
            playerAI.Score = _score;
        }

        public int getScorePlayer(){
            return player.Score;
        }

        public int getScorePlayerAI(){
            return playerAI.Score;
        }

        public int getScore(){
            return (int)(TimeOut * MaxScore / MaxTime);
        }
    }

    [System.Serializable]
    public class MazeCells{
        private bool _isVisited = false;
        private GameObject _forwardWall, _backWall, _leftWall, _rightWall, _floor;
        private Vector3 _center;

        public bool IsVisited{
            get{
                return _isVisited;
            }
            set{
                _isVisited = value;
            }
        }

        public Vector3 Center{
            get{
                return _center;
            }
            set{
                _center = value;
            }
        }

        public GameObject Floor{
            get{
                return _floor;
            }
            set{
                _floor = value;
            }
        }

        public GameObject ForwardWall{
            get{
                return _forwardWall;
            }
            set{
                _forwardWall = value;
            }
        }

        public GameObject BackWall{
            get{
                return _backWall;
            }
            set{
                _backWall = value;
            }
        }

        public GameObject LeftWall{
            get{
                return _leftWall;
            }
            set{
                _leftWall = value;
            }
        }

        public GameObject RightWall{
            get{
                return _rightWall;
            }
            set{
                _rightWall = value;
            }
        }
    }

    public abstract class MazeAlgorihm{
        protected MazeCells[][] myMaze;
        protected int numOfRow, numOfCol;
        protected int[] arrOfInt;
        protected (int x, int y) StartPoint;
        protected (int x, int y) EndPoint;

        protected MazeAlgorihm(MazeCells[][] _maze, int[] _arrOfInt, (int, int) _startPoint, (int, int) _endPoint) : base(){
            myMaze = _maze;
            numOfRow = _maze.Length;
            numOfCol = _maze.Length;
            arrOfInt = _arrOfInt;
            StartPoint = _startPoint;
            EndPoint = _endPoint;
        }

        public abstract void createMaze();
        public abstract Dictionary<(int, int), (int, int)> SearchBFS();
        public abstract List<(int, int)> findBestRoad(Dictionary<(int, int), (int, int)> _dic);
        public abstract Dictionary<(int, int), (int, int)> SearchDFS();
    }

    public class HuntAndKillMazeAlgorithm : MazeAlgorihm{
        int temp = 0;

        private int _currRow = 0;
        private int _currCol = 3;
        private bool _courseCompleted = false;

        public int CurrRow{
            get{
                return _currRow;
            }
            set{
                _currRow = value;
            }
        }

        public int CurrCol{
            get{
                return _currCol;
            }
            set{
                _currCol = value;
            }
        }

        public bool CourseCompleted{
            get{
                return _courseCompleted;
            }
            set{
                _courseCompleted = value;
            }
        }

        public HuntAndKillMazeAlgorithm(MazeCells[][] myMaze, int[] arrOfInt, (int x, int y) StartPoint, (int x, int y) EndPoint) : base(myMaze, arrOfInt, StartPoint, EndPoint){
            //NOTHING HERE
        }

        public override void createMaze(){
            _currRow = StartPoint.x;
            _currCol = StartPoint.y;
            DestroyWall(myMaze[StartPoint.x][StartPoint.y].BackWall);
            HuntAndKill();
            DestroyWall(myMaze[EndPoint.x][EndPoint.y].ForwardWall);
        }

        private void HuntAndKill(){
            myMaze[CurrRow][CurrCol].IsVisited = true;
            while(!CourseCompleted){
                Kill();
                Hunt();
            }
        }

        private void Kill(){
            while(true){
                //Debug.Log("IM IN " + CurrRow + " - " + CurrCol);
                int _direction = arrOfInt[temp];
                myMaze[CurrRow][CurrCol].IsVisited = true;
                if(temp < arrOfInt.Length - 1){
                    temp++;
                }
                //Debug.Log("DI CHUYEN SANG " + _direction);

                if(_direction == 22 && CellIsAvailable(CurrRow - 1, CurrCol)){
                    DestroyWall(myMaze[CurrRow][CurrCol].BackWall);
                    DestroyWall(myMaze[CurrRow - 1][CurrCol].ForwardWall);
                    myMaze[CurrRow][CurrCol].BackWall = null;
                    myMaze[CurrRow - 1][CurrCol].ForwardWall = null;
                    myMaze[CurrRow][CurrCol].IsVisited = false;
                    continue;
                }
                if(_direction == 88 && CellIsAvailable(CurrRow + 1, CurrCol)){
                    DestroyWall(myMaze[CurrRow][CurrCol].ForwardWall);
                    DestroyWall(myMaze[CurrRow + 1][CurrCol].BackWall);
                    myMaze[CurrRow][CurrCol].ForwardWall = null;
                    myMaze[CurrRow + 1][CurrCol].BackWall = null;
                    myMaze[CurrRow][CurrCol].IsVisited = false;
                    continue;
                }
                //BREAK BACK WALL WITHOUT CHANGING COORDINATE
                if(_direction == 44 && CellIsAvailable(CurrRow, CurrCol - 1)){
                    DestroyWall(myMaze[CurrRow][CurrCol].LeftWall);
                    DestroyWall(myMaze[CurrRow][CurrCol - 1].RightWall);
                    myMaze[CurrRow][CurrCol].LeftWall = null; 
                    myMaze[CurrRow][CurrCol - 1].RightWall = null;
                    myMaze[CurrRow][CurrCol].IsVisited = false;
                    continue;
                }
                //BREAK FORWARD WALL WITHOUT CHANGING COORDINATE
                if(_direction == 66 && CellIsAvailable(CurrRow, CurrCol + 1)){
                    DestroyWall(myMaze[CurrRow][CurrCol].RightWall);
                    DestroyWall(myMaze[CurrRow][CurrCol + 1].LeftWall);
                    myMaze[CurrRow][CurrCol].RightWall = null;
                    myMaze[CurrRow][CurrCol + 1].LeftWall = null;
                    myMaze[CurrRow][CurrCol].IsVisited = false;
                    continue;
                }

                if(_direction == 8 && CellIsAvailable(CurrRow + 1, CurrCol)){
                    if(CellIsVisited(CurrRow + 1, CurrCol)){
                        //Debug.Log("PREPARE TO DELETE FORWARD WALL!");
                        DestroyWall(myMaze[CurrRow][CurrCol].ForwardWall);
                        DestroyWall(myMaze[CurrRow + 1][CurrCol].BackWall);
                        myMaze[CurrRow][CurrCol].ForwardWall = null;
                        myMaze[CurrRow + 1][CurrCol].BackWall = null;
                        break;
                    }else{
                        DestroyWall(myMaze[CurrRow][CurrCol].ForwardWall);
                        DestroyWall(myMaze[CurrRow + 1][CurrCol].BackWall);
                        myMaze[CurrRow][CurrCol].ForwardWall = null;
                        myMaze[CurrRow + 1][CurrCol].BackWall = null;
                        //Debug.Log("IM GOING TO : " + CurrRow + " : " + CurrCol);
                        CurrRow++;
                    }
                    
                }
                else if(_direction == 2 && CellIsAvailable(CurrRow - 1, CurrCol)){
                    if(CellIsVisited(CurrRow - 1, CurrCol)){
                        //Debug.Log("PREPARE TO DELETE BACK WALL!");
                        DestroyWall(myMaze[CurrRow][CurrCol].BackWall);
                        DestroyWall(myMaze[CurrRow - 1][CurrCol].ForwardWall);
                        myMaze[CurrRow][CurrCol].BackWall = null;
                        myMaze[CurrRow - 1][CurrCol].ForwardWall = null;
                        break;
                    }else{
                        DestroyWall(myMaze[CurrRow][CurrCol].BackWall);
                        DestroyWall(myMaze[CurrRow - 1][CurrCol].ForwardWall);
                        myMaze[CurrRow][CurrCol].BackWall = null;
                        myMaze[CurrRow - 1][CurrCol].ForwardWall = null;
                        //Debug.Log("IM GOING TO : " + CurrRow + " : " + CurrCol);
                        CurrRow--;
                    }
                    
                }
                else if(_direction == 4 && CellIsAvailable(CurrRow, CurrCol - 1)){
                    if(CellIsVisited(CurrRow, CurrCol - 1)){
                        //Debug.Log("PREPARE TO DELETE LEFT WALL!");
                        DestroyWall(myMaze[CurrRow][CurrCol].LeftWall);
                        DestroyWall(myMaze[CurrRow][CurrCol - 1].RightWall);
                        myMaze[CurrRow][CurrCol].LeftWall = null;
                        myMaze[CurrRow][CurrCol - 1].RightWall = null;
                        break;
                    }else{
                        DestroyWall(myMaze[CurrRow][CurrCol].LeftWall);
                        DestroyWall(myMaze[CurrRow][CurrCol - 1].RightWall);
                        myMaze[CurrRow][CurrCol].LeftWall = null;
                        myMaze[CurrRow][CurrCol - 1].RightWall = null;
                        //Debug.Log("IM GOING TO : " + CurrRow + " : " + CurrCol);
                        CurrCol--;  
                    }
                    
                }
                else if(_direction == 6 && CellIsAvailable(CurrRow, CurrCol + 1)){
                    if(CellIsVisited(CurrRow, CurrCol + 1)){
                        //Debug.Log("PREPARE TO DELETE RIGHT WALL!");
                        DestroyWall(myMaze[CurrRow][CurrCol].RightWall);
                        DestroyWall(myMaze[CurrRow][CurrCol + 1].LeftWall);
                        myMaze[CurrRow][CurrCol].RightWall = null;
                        myMaze[CurrRow][CurrCol + 1].LeftWall = null;
                        break;
                    }else{
                        DestroyWall(myMaze[CurrRow][CurrCol].RightWall);
                        DestroyWall(myMaze[CurrRow][CurrCol + 1].LeftWall);
                        myMaze[CurrRow][CurrCol].RightWall = null;
                        myMaze[CurrRow][CurrCol + 1].LeftWall = null;
                        //Debug.Log("IM GOING TO : " + CurrRow + " : " + CurrCol);
                        CurrCol++;
                    }
                    
                }
                else{
                    break;
                }
            }
        }

        private void Hunt(){
            CourseCompleted = true;
            
            for(int i = 0; i < myMaze.Length; i++){
                for(int j = 0; j < myMaze[0].Length; j++){
                    if(!myMaze[i][j].IsVisited && CellIsHaveAnyNeighboursVisited(i, j)){
                        CourseCompleted = false;
                        CurrRow = i;
                        CurrCol = j;
                        myMaze[i][j].IsVisited = true;
                        return;
                    }
                }
            }
        }

        private void DestroyWall(GameObject _wall){
            if(_wall != null){
                GameObject.Destroy(_wall);
            }
        }

        private bool RouteStillAvailable(int _row, int _col){
            int i = 0;

            if(_row > 0 && !myMaze[_row - 1][_col].IsVisited){
                i++;
            }
            if(_row < myMaze.Length - 1 && !myMaze[_row + 1][_col].IsVisited){
                i++;
            }
            if(_col > 0 && !myMaze[_row][_col - 1].IsVisited){
                i++;
            }
            if(_col < myMaze.Length - 1 && !myMaze[_row][_col + 1].IsVisited){
                i++;
            }

            return i > 0;
        }

        private bool CellIsAvailable(int _row, int _col){
            if(_row < myMaze.Length && _col < myMaze[0].Length && _row >= 0 && _col >= 0){
                return true;
            }else return false;
        }

        private bool CellIsVisited(int _row, int _col){
            return myMaze[_row][_col].IsVisited;
        }

        private bool CellIsHaveAnyNeighboursVisited(int _row, int _col){
            int iCheck = 0;
            if(CellIsAvailable(_row + 1, _col) && CellIsVisited(_row + 1, _col)){
                iCheck++;
            }
            if(CellIsAvailable(_row - 1, _col) && CellIsVisited(_row - 1, _col)){
                iCheck++;
            }
            if(CellIsAvailable(_row, _col + 1) && CellIsVisited(_row, _col + 1)){
                iCheck++;
            }
            if(CellIsAvailable(_row, _col - 1) && CellIsVisited(_row, _col - 1)){
                iCheck++;
            }

            return iCheck > 0;
        }

        public override Dictionary<(int, int), (int, int)> SearchBFS(){
            List<(int x, int y)> frontierList = new List<(int x, int y)>(){StartPoint};

            Dictionary<(int x1, int y1), (int x2, int y2)> bestRoadDict = new Dictionary<(int x1, int y1), (int x2, int y2)>();
            bestRoadDict.Add(StartPoint, StartPoint);

            List<(int x, int y)> visitedList = new List<(int x, int y)>();
            visitedList.Add(StartPoint);
            while(frontierList.Count > 0){
                (int x, int y) currPoint = frontierList[0];
                frontierList.RemoveAt(0);

                //DOWN
                if(currPoint.x - 1 >= 0 && !visitedList.Contains((currPoint.x - 1, currPoint.y)) && myMaze[currPoint.x - 1][currPoint.y].ForwardWall == null && myMaze[currPoint.x][currPoint.y].BackWall == null){
                    (int x, int y) downCell = (currPoint.x - 1, currPoint.y);
                    frontierList.Add(downCell);
                    bestRoadDict[downCell] = currPoint;
                    visitedList.Add(downCell);
                }

                //UP
                if(currPoint.x + 1 < myMaze.Length && !visitedList.Contains((currPoint.x + 1, currPoint.y)) && myMaze[currPoint.x + 1][currPoint.y].BackWall == null && myMaze[currPoint.x][currPoint.y].ForwardWall == null){
                    (int x, int y) upCell = (currPoint.x + 1, currPoint.y);
                    frontierList.Add(upCell);
                    bestRoadDict[upCell] = currPoint;
                    visitedList.Add(upCell);
                }

                //RIGHT
                if(currPoint.y + 1 < myMaze.Length && !visitedList.Contains((currPoint.x, currPoint.y + 1)) && myMaze[currPoint.x][currPoint.y + 1].LeftWall == null && myMaze[currPoint.x][currPoint.y].RightWall == null){
                    (int x, int y) rightCell = (currPoint.x, currPoint.y + 1);
                    frontierList.Add(rightCell);
                    bestRoadDict[rightCell] = currPoint;
                    visitedList.Add(rightCell);
                }

                //LEFT
                if(currPoint.y - 1 >= 0 && !visitedList.Contains((currPoint.x, currPoint.y - 1)) && myMaze[currPoint.x][currPoint.y - 1].RightWall == null && myMaze[currPoint.x][currPoint.y].LeftWall == null){
                    (int x, int y) leftCell = (currPoint.x, currPoint.y - 1);
                    frontierList.Add(leftCell);
                    bestRoadDict[leftCell] = currPoint;
                    visitedList.Add(leftCell);
                }
            }
            return bestRoadDict;
        }

        public override Dictionary<(int, int), (int, int)> SearchDFS(){
            List<(int x, int y)> frontierList = new List<(int x, int y)>(){StartPoint};

            Dictionary<(int x1, int y1), (int x2, int y2)> bestRoadDict = new Dictionary<(int x1, int y1), (int x2, int y2)>();
            bestRoadDict.Add(StartPoint, StartPoint);

            List<(int x, int y)> visitedList = new List<(int x, int y)>();
            visitedList.Add(StartPoint);
            while(frontierList.Count > 0){
                (int x, int y) currPoint = frontierList[frontierList.Count - 1];
                frontierList.RemoveAt(frontierList.Count - 1);
                //DOWN
                if(currPoint.x - 1 >= 0 && !visitedList.Contains((currPoint.x - 1, currPoint.y)) && myMaze[currPoint.x - 1][currPoint.y].ForwardWall == null && myMaze[currPoint.x][currPoint.y].BackWall == null){
                    (int x, int y) downCell = (currPoint.x - 1, currPoint.y);
                    frontierList.Add(downCell);
                    bestRoadDict[downCell] = currPoint;
                    visitedList.Add(downCell);
                }

                //UP
                if(currPoint.x + 1 < myMaze.Length && !visitedList.Contains((currPoint.x + 1, currPoint.y)) && myMaze[currPoint.x + 1][currPoint.y].BackWall == null && myMaze[currPoint.x][currPoint.y].ForwardWall == null){
                    (int x, int y) upCell = (currPoint.x + 1, currPoint.y);
                    frontierList.Add(upCell);
                    bestRoadDict[upCell] = currPoint;
                    visitedList.Add(upCell);
                }

                //RIGHT
                if(currPoint.y + 1 < myMaze.Length && !visitedList.Contains((currPoint.x, currPoint.y + 1)) && myMaze[currPoint.x][currPoint.y + 1].LeftWall == null && myMaze[currPoint.x][currPoint.y].RightWall == null){
                    (int x, int y) rightCell = (currPoint.x, currPoint.y + 1);
                    frontierList.Add(rightCell);
                    bestRoadDict[rightCell] = currPoint;
                    visitedList.Add(rightCell);
                }

                //LEFT
                if(currPoint.y - 1 >= 0 && !visitedList.Contains((currPoint.x, currPoint.y - 1)) && myMaze[currPoint.x][currPoint.y - 1].RightWall == null && myMaze[currPoint.x][currPoint.y].LeftWall == null){
                    (int x, int y) leftCell = (currPoint.x, currPoint.y - 1);
                    frontierList.Add(leftCell);
                    bestRoadDict[leftCell] = currPoint;
                    visitedList.Add(leftCell);
                }
            }
            // foreach (var item in visitedList)
            // {
            //     Debug.Log(item);
            // }

            return bestRoadDict;
        }

        public override List<(int, int)> findBestRoad(Dictionary<(int, int), (int, int)> _dic){
            List<(int, int)> result = new List<(int, int)>(); 
            result.Add(EndPoint);
            (int, int) temp = EndPoint;
            while(_dic[temp] != StartPoint){
                result.Add(_dic[temp]);
                temp = _dic[temp];
            }
            result.Add(StartPoint);
            return result;
        }
    }

}
