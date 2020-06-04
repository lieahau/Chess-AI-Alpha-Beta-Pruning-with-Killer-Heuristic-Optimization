using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    private static GameplayManager _instance;
    public static GameplayManager Instance { get { return _instance; } }

    private string AIAlphaBetaName = "AB";
    private string AIKillerHeuristicName = "KH";
    
    private void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        System.GC.Collect();
    }

    /// <summary>Function yang dipanggil ketika pertandingan berakhir.</summary>
    /// <param name="whiteWin">Apakah putih menang?</param>
    /// <param name="blackWin">Apakah hitam menang?</param>
    public void GameOver(bool whiteWin, bool blackWin)
    {
        WriteReport(whiteWin, blackWin); // tulis data pada .csv file
        StartRandomMatch(); // lanjut ke pertandingan selanjutnya
    }

    /// <summary>Lanjut ke pertandingan selanjutnya bila memungkinkan.</summary>
    private void StartRandomMatch()
    {
        if(MatchmakingManager.Instance.FindRandomMatch())
            SceneManager.LoadScene("Gameplay");
        else
            SceneManager.LoadScene("Menu");
    }

    /// <summary>Tulis hasil pertandingan pada file .csv</summary>
    private void WriteReport(bool whiteWin, bool blackWin)
    {
        // write for white
        string color = "White";
        string versus = GetAIName(false) + " " + GameState.blackSearchDepth.ToString();
        string status = "";
        if(whiteWin && !blackWin)
            status = "Win";
        else if(!whiteWin && blackWin)
            status = "Lose";
        else
            status = "Draw";
        
        string notes = "-";
        if(status == "Draw")
        {
            if(GameState.drawVariance == GameState.DrawVariance.StaleMate)
                notes = "Stalemate";
            else if(GameState.drawVariance == GameState.DrawVariance.PerpetualCheck)
                notes = "Perpetual Check";
            else if(GameState.drawVariance == GameState.DrawVariance.ThreefoldRepetition)
                notes = "Threefold Repetition";
            else if(GameState.drawVariance == GameState.DrawVariance.NotEnoughMaterials)
                notes = "Not Enough Materials";
        }
        
        CSVManager.AppendToReport(
            true,
            new string[9] {
                color,
                versus,
                GameState.whiteAvgTime.ToString(),
                GameState.whiteAvgNodes.ToString(),
                GameState.whiteTotalTime.ToString(),
                GameState.whiteTotalNodes.ToString(),
                GameState.totalTurn.ToString(),
                status,
                notes
            }
        );


        // write for black
        color = "Black";
        versus = GetAIName(true) + " " + GameState.whiteSearchDepth.ToString();
        if(whiteWin && !blackWin)
            status = "Lose";
        else if(!whiteWin && blackWin)
            status = "Win";
        else
            status = "Draw";
        
        CSVManager.AppendToReport(
            false,
            new string[9] {
                color,
                versus,
                GameState.blackAvgTime.ToString(),
                GameState.blackAvgNodes.ToString(),
                GameState.blackTotalTime.ToString(),
                GameState.blackTotalNodes.ToString(),
                GameState.totalTurn.ToString(),
                status,
                notes
            }
        );
    }

    /// <summary>Mengembalikkan string nama AI.</summary>
    private string GetAIName(bool forWhite)
    {
        if(GameState.boardMode == GameState.BoardMode.AlphaBetaVSAlphaBeta)
        {
            return AIAlphaBetaName;
        }
        else if(GameState.boardMode == GameState.BoardMode.AlphaBetaVSKillerHeuristic)
        {
            if(forWhite)
                return AIAlphaBetaName;
            else
                return AIKillerHeuristicName;
        }
        else if(GameState.boardMode == GameState.BoardMode.KillerHeuristicVSAlphaBeta)
        {
            if(forWhite)
                return AIKillerHeuristicName;
            else
                return AIAlphaBetaName;
        }
        else if(GameState.boardMode == GameState.BoardMode.KillerHeuristicVSKillerHeuristic)
        {
            return AIKillerHeuristicName;
        }
        else
            return "-";
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
