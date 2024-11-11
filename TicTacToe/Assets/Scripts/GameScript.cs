using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public string GameMode;
    public GameObject Cross;
    public GameObject Nought;
    public GameObject Line;
    public TextMeshProUGUI Instrcutions;
    public TextMeshProUGUI Player2Role;

    public enum Seed {EMPTY, CROSS, NOUGHT };

    Seed Turn;

    public GameObject[] AllSpawns = new GameObject[9];
    public Seed[] Player = new Seed[9];

    Vector2 Position1, Position2;


    private void Awake()
    {
        GameObject PersistantObj = GameObject.FindGameObjectWithTag("PersistantObj") as GameObject;
        GameMode = PersistantObj.GetComponent<PersistanceScript>().GameMode;
        Destroy(PersistantObj);

        if (GameMode == "PVE") Player2Role.text = "AI";
        else Player2Role.text = "Player 2";

        //Set Turn as Player 1 with the Cross
        Turn = Seed.CROSS;

        Instrcutions.text = "Turn: Player 1";
        for (int i = 0; i < 9; i++)
        {
            Player[i] = Seed.EMPTY;
        }
    }

    public void Spawn(GameObject EmptyCell, int id)
    {
        if(Turn == Seed.CROSS)
        {

            AllSpawns[id] = Instantiate(Cross,EmptyCell.transform.position,Quaternion.identity);
            Player[id] = Turn; 

            if(Win(Turn))
            {
                Turn = Seed.EMPTY;
                Instrcutions.text = "Player 1 Win!";
                float Slope = CalculateRotation();
                Instantiate(Line,CalculateCenter(),Quaternion.Euler(0,0,Slope));
            }
            else
            {
                Turn = Seed.NOUGHT;
                Instrcutions.text = "Turn: " + Player2Role.text;
            }
            

           
        }
        else if (Turn == Seed.NOUGHT && GameMode == "PVP")
        {

            AllSpawns[id] = Instantiate(Nought, EmptyCell.transform.position, Quaternion.identity);
            Player[id] = Turn;
            if (Win(Turn))
            {
                Turn = Seed.EMPTY;
                Instrcutions.text = Player2Role.text + " Win!";
                float Slope = CalculateRotation();
                Instantiate(Line, CalculateCenter(), Quaternion.Euler(0, 0, Slope));

            }
            else
            {
                Turn = Seed.CROSS;
                Instrcutions.text = "Turn: Player 1";
            }
        }
        if (Turn == Seed.NOUGHT && GameMode == "PVE")
        {
            Instrcutions.text = "Turn: " + Player2Role.text; // 显示 AI 的回合信息
                int BestScore = -1;
                int BestPosition = -1;
                int CurrentScore;
                for(int i = 0; i < 9; i++)
                {
                    if (Player[i] == Seed.EMPTY)
                    {
                        Player[i] = Seed.NOUGHT;
                        CurrentScore = MiniMax(Seed.CROSS,Player, int.MinValue, int.MaxValue);
                        Player[i] = Seed.EMPTY;

                        if(BestScore < CurrentScore)
                        {
                            BestScore = CurrentScore;
                            BestPosition = i;
                        }
                    }
                }
                if(BestScore > -1)
                {
                    AllSpawns[BestPosition] = Instantiate(Nought, AllSpawns[BestPosition].transform.position, Quaternion.identity);
                    Player[BestPosition] = Turn;
                }
               
            if (Win(Turn))
            {
                Turn = Seed.EMPTY;
                Instrcutions.text = Player2Role.text + " Win!";
                float Slope = CalculateRotation();
                Instantiate(Line, CalculateCenter(), Quaternion.Euler(0, 0, Slope));

            }
            else
            {
                Turn = Seed.CROSS;
                Instrcutions.text = "Turn: Player 1";
            }
        }
      

        if (IsDraw())
        {
            Turn = Seed.EMPTY;
            Instrcutions.text = "Draw";
        }

        Destroy(EmptyCell);
    }

    bool IsAnyEmpty()
    {
        bool bEmpty = false;

        for(int  i = 0; i < 9; i++)
        {
            if(Player[i] == Seed.EMPTY)
            {
                bEmpty = true;
                break;
            }
               
        }
        return bEmpty;
    }

    bool Win(Seed CurrentPlayer)
    {
        bool bIsWon = false;

        int[,] AllConditions = new int[8, 3]{ {0,1,2 }, { 3, 4, 5 }, { 6, 7, 8 },
                                              {0,3,6 }, { 1, 4, 7 }, { 2, 5, 8 },
                                              {0,4,8 }, { 2, 4, 6 }};

        for (int i = 0; i < 8; i++)
        {
            if(Player[AllConditions[i,0]] ==CurrentPlayer&&
                Player[AllConditions[i, 1]] == CurrentPlayer &&
                Player[AllConditions[i, 2]] == CurrentPlayer)
            {
                bIsWon = true; 
                Position1 = AllSpawns[AllConditions[i, 0]].transform.position;
                Position2 = AllSpawns[AllConditions[i, 2]].transform.position;
                break;
            }
        }
        return bIsWon;
    }

    bool IsDraw()
    {
        bool bPlayer1Win = Win(Seed.CROSS);
        bool bPlayer2Win = Win(Seed.NOUGHT);
        bool bEmptySpace = IsAnyEmpty();

        bool bIsDraw = false;

        if (bPlayer1Win == false && bPlayer2Win == false&& bEmptySpace == false) 
        { 
            bIsDraw = true; 
        }
        return bIsDraw;

    }

    Vector2 CalculateCenter()
    {
        float x = (Position1.x + Position2.x) / 2;
        float y = (Position1.y + Position2.y) / 2;

        return new Vector2(x, y);
    }
    float CalculateRotation()
    {
        float slope = 0.0f;

        if(Position1.x == Position2.x)
        {
            slope = 0.0f;
        }
        else if(Position1.y == Position2.y) 
        {
                slope = 90.0f;
        }
        else if(Position1.x > 0.0f)
        {
            slope = -45.0f;
        }
        else
        slope = 45.0f;
        return slope;

    }

    int MiniMax(Seed CurrentPlayer, Seed[] Board, int Alpha, int Beta)
    {
        if (IsDraw()) return 0;
        if (Win(Seed.NOUGHT)) return 1;
        if (Win(Seed.CROSS)) return -1;

        int Score;

        if (CurrentPlayer == Seed.NOUGHT)
        {
            for (int i = 0; i < 9; i++)
            {
                if (Board[i] == Seed.EMPTY)
                {
                    Board[i] = Seed.NOUGHT;
                    Score = MiniMax(Seed.CROSS, Board, Alpha, Beta);
                    Board[i] = Seed.EMPTY;

                    if(Score > Alpha)
                    {
                        Alpha = Score;
                    }
                    if (Alpha > Beta)
                        break;
                }
            }
            return Alpha;
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                if (Board[i] == Seed.EMPTY)
                {
                    Board[i] = Seed.CROSS;
                    Score = MiniMax(Seed.NOUGHT, Board, Alpha, Beta);
                    Board[i] = Seed.EMPTY;

                    if (Score < Beta)
                    {
                        Beta = Score;
                    }
                    if (Alpha > Beta)
                        break;
                }
            }
            return Beta;
        }
    }
            
        


    }
