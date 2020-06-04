using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    private static MatchmakingManager _instance;
    public static MatchmakingManager Instance { get { return _instance; } }

    [HideInInspector] public int AB2MaxMatch = 0;
    [HideInInspector] public int AB3MaxMatch = 0;
    [HideInInspector] public int AB4MaxMatch = 0;
    [HideInInspector] public int AB5MaxMatch = 0;
    [HideInInspector] public int KH2MaxMatch = 0;
    [HideInInspector] public int KH3MaxMatch = 0;
    [HideInInspector] public int KH4MaxMatch = 0;
    [HideInInspector] public int KH5MaxMatch = 0;
    private int whiteAI = 0, whiteDepth = 0, blackAI = 0, blackDepth = 0;

    private void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public bool FindRandomMatch()
    {
        // randomize
        if(IsMatchStillPossible())
        {
            do{
                whiteAI = Random.Range(0, 2); // 0 = Alpha Beta, 1 = Killer Heuristic
                whiteDepth = Random.Range(2, 6);
            }while(!isAICanMatch(whiteAI, whiteDepth));
            reduceAIMaxMatch(true);
            do{
                blackAI = Random.Range(0, 2); // 0 = Alpha Beta, 1 = Killer Heuristic
                blackDepth = Random.Range(2, 6);
            }while(!isAICanMatch(blackAI, blackDepth));
            reduceAIMaxMatch(false);
        }
        else
            return false;

        // set board mode and depth
        if(whiteAI == 0 && blackAI == 0)
            GameState.boardMode = GameState.BoardMode.AlphaBetaVSAlphaBeta;
        else if(whiteAI == 0 && blackAI == 1)
            GameState.boardMode = GameState.BoardMode.AlphaBetaVSKillerHeuristic;
        else if(whiteAI == 1 && blackAI == 0)
            GameState.boardMode = GameState.BoardMode.KillerHeuristicVSAlphaBeta;
        else if(whiteAI == 1 && blackAI == 1)
            GameState.boardMode = GameState.BoardMode.KillerHeuristicVSKillerHeuristic;

        GameState.whiteSearchDepth = whiteDepth;
        GameState.blackSearchDepth = blackDepth;
        return true;
    }

    private bool IsMatchStillPossible()
    {
        int matchLeft = 0;
        matchLeft += AB2MaxMatch + AB3MaxMatch + AB4MaxMatch + AB5MaxMatch;
        matchLeft += KH2MaxMatch + KH3MaxMatch + KH4MaxMatch + KH5MaxMatch;
        if(AB2MaxMatch < 0) matchLeft -= AB2MaxMatch;
        if(AB3MaxMatch < 0) matchLeft -= AB3MaxMatch;
        if(AB4MaxMatch < 0) matchLeft -= AB4MaxMatch;
        if(AB5MaxMatch < 0) matchLeft -= AB5MaxMatch;
        if(KH2MaxMatch < 0) matchLeft -= KH2MaxMatch;
        if(KH3MaxMatch < 0) matchLeft -= KH3MaxMatch;
        if(KH4MaxMatch < 0) matchLeft -= KH4MaxMatch;
        if(KH5MaxMatch < 0) matchLeft -= KH5MaxMatch;
        return matchLeft >= 2;
    }

    private bool isAICanMatch(int AI, int depth)
    {
        if(AI == 0) // Alpha Beta
        {
            if(depth == 2) return AB2MaxMatch > 0;
            else if(depth == 3) return AB3MaxMatch > 0;
            else if(depth == 4) return AB4MaxMatch > 0;
            else if(depth == 5) return AB5MaxMatch > 0;
            else return false;
        }
        else if(AI == 1) // Killer Heuristic
        {
            if(depth == 2) return KH2MaxMatch > 0;
            else if(depth == 3) return KH3MaxMatch > 0;
            else if(depth == 4) return KH4MaxMatch > 0;
            else if(depth == 5) return KH5MaxMatch > 0;
            else return false;
        }
        else return false;
    }
    
    public void reduceAIMaxMatch(bool isWhite)
    {
        if(isWhite)
        {
            if(whiteAI == 0) // Alpha Beta
            {
                if(whiteDepth == 2) AB2MaxMatch--;
                else if(whiteDepth == 3) AB3MaxMatch--;
                else if(whiteDepth == 4) AB4MaxMatch--;
                else if(whiteDepth == 5) AB5MaxMatch--;
            }
            else if(whiteAI == 1) // Killer Heuristic
            {
                if(whiteDepth == 2) KH2MaxMatch--;
                else if(whiteDepth == 3) KH3MaxMatch--;
                else if(whiteDepth == 4) KH4MaxMatch--;
                else if(whiteDepth == 5) KH5MaxMatch--;
            }   
        }
        else
        {
            if(blackAI == 0) // Alpha Beta
            {
                if(blackDepth == 2) AB2MaxMatch--;
                else if(blackDepth == 3) AB3MaxMatch--;
                else if(blackDepth == 4) AB4MaxMatch--;
                else if(blackDepth == 5) AB5MaxMatch--;
            }
            else if(blackAI == 1) // Killer Heuristic
            {
                if(blackDepth == 2) KH2MaxMatch--;
                else if(blackDepth == 3) KH3MaxMatch--;
                else if(blackDepth == 4) KH4MaxMatch--;
                else if(blackDepth == 5) KH5MaxMatch--;
            }
        }
    }
}
