using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public string GameMode;
    public GameObject Cross;
    public GameObject Nought;
    public GameObject Line;
    public TextMeshProUGUI Instructions;
    public TextMeshProUGUI Player2Role;

    // 新增按钮引用
    public Button RestartButton;
    public Button MainMenuButton;

    //棋盘中的三种情况
    public enum Seed { EMPTY, CROSS, NOUGHT }
    private Seed currentTurn;

    public GameObject[] AllSpawns = new GameObject[9];
    public Seed[] BoardState = new Seed[9];

    private Vector2 positionStart, positionEnd;

    private bool isPlayerTurn;
    private int aiDepthLimit = 1;
    private void Awake()
    {
        GameObject persistentObj = GameObject.FindGameObjectWithTag("PersistantObj");
        isPlayerTurn = true;
        if (persistentObj != null)
        {
            GameMode = persistentObj.GetComponent<PersistanceScript>().GameMode;
            string AIDifficulty = persistentObj.GetComponent<PersistanceScript>().AIDifficulty;

            // 设置不同难度的深度限制
            if (AIDifficulty == "Easy")
            {
                aiDepthLimit = 1;
            }
            else if (AIDifficulty == "Medium")
            {
                aiDepthLimit = 2;
            }
            else if (AIDifficulty == "Hard")
            {
                aiDepthLimit = 9;
            }
        }
        Player2Role.text = GameMode == "PVE" ? "AI" : "Player 2";
        currentTurn = Seed.CROSS;
        Instructions.text = "Turn: Player 1";

        // 初始化棋盘
        for (int i = 0; i < 9; i++)
        {
            BoardState[i] = Seed.EMPTY;
        }

        // 隐藏重启和返回主菜单按钮
        RestartButton.gameObject.SetActive(false);
        MainMenuButton.gameObject.SetActive(false);

        RestartButton.onClick.AddListener(RestartGame);
        MainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void Spawn(GameObject EmptyCell, int id)
    {
        if (!isPlayerTurn) return;
        if (currentTurn == Seed.CROSS)
        {
            PlayerMove(EmptyCell, id, Seed.CROSS);

            // 检查是否平局或胜利，如果没有则自动调用 AI 回合
            if (currentTurn == Seed.NOUGHT && GameMode == "PVE" && !IsDraw() && !CheckWin(Seed.CROSS))
            {
                isPlayerTurn = false; // 禁用玩家输入
                StartCoroutine(DelayedAIMove(0.5f));
            }
        }
        else if (currentTurn == Seed.NOUGHT && GameMode == "PVP")
        {
            PlayerMove(EmptyCell, id, Seed.NOUGHT);
        }

        // 销毁当前单元格
        Destroy(EmptyCell);

        // 检查是否为平局
        if (IsDraw() && currentTurn != Seed.EMPTY)
        {
            EndGame("Draw"); 
        }
    }
    // 延迟 AI 回合的协程
    private IEnumerator DelayedAIMove(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(AIMove());
    }

    private void PlayerMove(GameObject cell, int id, Seed playerSeed)
    {
        GameObject piece = playerSeed == Seed.CROSS ? Cross : Nought;
        AllSpawns[id] = Instantiate(piece, cell.transform.position, Quaternion.identity);
        BoardState[id] = playerSeed;

        if (CheckWin(playerSeed))
        {
            EndGame(playerSeed == Seed.CROSS ? "Player 1 Win!" : $"{Player2Role.text} Win!");
        }
        else
        {
            currentTurn = playerSeed == Seed.CROSS ? Seed.NOUGHT : Seed.CROSS;
            Instructions.text = "Turn: " + (currentTurn == Seed.CROSS ? "Player 1" : Player2Role.text);
        }
    }

    private IEnumerator AIMove()
    {
        // 显示 AI 的回合信息
        Instructions.text = "Turn: " + Player2Role.text; 

        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < 9; i++)
        {
            if (BoardState[i] == Seed.EMPTY)
            {
                BoardState[i] = Seed.NOUGHT;
                int score = MiniMax(Seed.CROSS, BoardState, int.MinValue, int.MaxValue);
                BoardState[i] = Seed.EMPTY;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }
        //找到生成位置生成圈圈
        if (bestMove >= 0)
        {
            Vector3 spawnPosition = AllSpawns[bestMove] != null ? AllSpawns[bestMove].transform.position : Vector3.zero;
            AllSpawns[bestMove] = Instantiate(Nought, spawnPosition, Quaternion.identity);
            BoardState[bestMove] = Seed.NOUGHT;
        }

        if (CheckWin(Seed.NOUGHT))
        {
            EndGame($"{Player2Role.text} Win!");
        }
        else if (!IsDraw())
        {
            currentTurn = Seed.CROSS;
            Instructions.text = "Turn: Player 1"; 
            isPlayerTurn = true; 
        }
        else
        {
            currentTurn = Seed.EMPTY;
            Instructions.text = "Draw";
            // 游戏结束，禁用玩家输入
            isPlayerTurn = false;
        }

        yield break; 
    }
    private void EndGame(string resultMessage)
    {
        currentTurn = Seed.EMPTY;
        Instructions.text = resultMessage;

        // 胜利之后把链接的区域用图片表示Line
        if (resultMessage != "Draw")
        {
            float slope = CalculateLineRotation();
            Instantiate(Line, CalculateCenterPosition(), Quaternion.Euler(0, 0, slope));
        }

        // 显示按钮
        RestartButton.gameObject.SetActive(true);
        MainMenuButton.gameObject.SetActive(true);
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    private void ReturnToMainMenu()
    {
        // 销毁 PersistentObj，确保返回主菜单时重新创建并选择新的模式
        GameObject persistentObj = GameObject.FindGameObjectWithTag("PersistantObj");
        if (persistentObj != null)
        {
            Destroy(persistentObj);
        }

        SceneManager.LoadScene("MainMenu");
    }

    private bool CheckWin(Seed player)
    {
        int[,] winConditions = new int[8, 3]
        {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
            {0, 4, 8}, {2, 4, 6}
        };

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            if (BoardState[winConditions[i, 0]] == player &&
                BoardState[winConditions[i, 1]] == player &&
                BoardState[winConditions[i, 2]] == player)
            {
                positionStart = AllSpawns[winConditions[i, 0]].transform.position;
                positionEnd = AllSpawns[winConditions[i, 2]].transform.position;
                return true;
            }
        }
        return false;
    }

    private bool IsDraw()
    {
        foreach (var spot in BoardState)
        {
            if (spot == Seed.EMPTY) return false;
        }
        return !CheckWin(Seed.CROSS) && !CheckWin(Seed.NOUGHT);
    }

    private Vector2 CalculateCenterPosition()
    {
        return (positionStart + positionEnd) / 2;
    }

    //计算Line的旋转
    private float CalculateLineRotation()
    {
        if (positionStart.x == positionEnd.x) return 0f;
        if (positionStart.y == positionEnd.y) return 90f;
        return positionStart.x > 0 ? -45f : 45f;
    }

    private int MiniMax(Seed currentPlayer, Seed[] board, int alpha, int beta, int depth = 0)
    {
        // 如果达到了最大深度，返回平局分数 0
        if (depth >= aiDepthLimit) return 0;

        if (IsDraw()) return 0;
        if (CheckWin(Seed.NOUGHT)) return 1;
        if (CheckWin(Seed.CROSS)) return -1;

        int bestScore;
        // AI 的回合，最大化分数
        if (currentPlayer == Seed.NOUGHT) 
        {
            bestScore = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.NOUGHT;
                    int score = MiniMax(Seed.CROSS, board, alpha, beta, depth + 1);
                    board[i] = Seed.EMPTY;
                    bestScore = Mathf.Max(bestScore, score);
                    alpha = Mathf.Max(alpha, score);
                    // Alpha-Beta 剪枝
                    if (alpha >= beta) break; 
                }
            }
            return bestScore;
        }
        // 玩家回合，最小化分数
        else
        {
            bestScore = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == Seed.EMPTY)
                {
                    board[i] = Seed.CROSS;

                    int score = MiniMax(Seed.NOUGHT, board, alpha, beta, depth + 1);
                    board[i] = Seed.EMPTY;
                    bestScore = Mathf.Min(bestScore, score);
                    beta = Mathf.Min(beta, score);
                    if (alpha >= beta) break; 
                }
            }
            return bestScore;
        }
    }

}
