using System;
using System.IO;
using UnityEngine;

public class CSVManager
{
    private static string reportRootFolderName = "Report";
    private static string reportFolderNameAlphaBeta = "Alpha Beta";
    private static string reportFolderNameKillerHeuristic = "Killer Heuristic";
    private static string reportFileNamePrefix = "Report ";
    private static string reportExtension = ".csv";
    private static string reportSeparator = ",";
    private static string[] reportHeaders = new string[9]{
        "Color",
        "Versus",
        "Average Time/Match (s)",
        "Average Tree's Node/Match",
        "Total Time/Match (s)",
        "Total Tree's Node per Match",
        "Total Turn",
        "Status",
        "Notes"
    };
    private static string timeStampHeader = "Time Stamp";

    #region Interactions
    /// <summary>Menambahkan konten data pada file csv.</summary>
    public static void AppendToReport(bool forWhite, string[] contents)
    {
        VerifyDirectory(forWhite);
        VerifyFile(forWhite);

        using(StreamWriter sw = File.AppendText(GetFilePath(forWhite)))
        {
            string finalString = GetTimeStamp();
            for(int i = 0; i < contents.Length; i++)
                finalString += reportSeparator + contents[i];
            sw.WriteLine(finalString);
        }
    }
    #endregion

    #region Operations
    
    private static void CreateReport(bool forWhite)
    {
        VerifyDirectory(forWhite);
        using(StreamWriter sw = File.CreateText(GetFilePath(forWhite)))
        {
            string finalString = timeStampHeader;
            for(int i = 0; i < reportHeaders.Length; i++)
                finalString += reportSeparator + reportHeaders[i];
            sw.WriteLine(finalString);
        }
    }

    private static void VerifyDirectory(bool forWhite)
    {
        string dirPath = GetDirectoryPath(forWhite);
        if(!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
    }

    private static void VerifyFile(bool forWhite)
    {
        string filePath = GetFilePath(forWhite);
        if(!File.Exists(filePath))
            CreateReport(forWhite);
    }
    #endregion

    #region Queries
    private static string GetDirectoryPath(bool forWhite)
    {
        return Path.Combine(Application.dataPath, reportRootFolderName, GetFolderName(forWhite));
    }

    private static string GetFilePath(bool forWhite)
    {
        return Path.Combine(GetDirectoryPath(forWhite), GetFileName(forWhite));
    }

    private static string GetFolderName(bool forWhite)
    {
        if(GameState.boardMode == GameState.BoardMode.AlphaBetaVSAlphaBeta)
        {
            return reportFolderNameAlphaBeta;
        }
        else if(GameState.boardMode == GameState.BoardMode.AlphaBetaVSKillerHeuristic)
        {
            if(forWhite)
                return reportFolderNameAlphaBeta;
            else
                return reportFolderNameKillerHeuristic;
        }
        else if(GameState.boardMode == GameState.BoardMode.KillerHeuristicVSAlphaBeta)
        {
            if(forWhite)
                return reportFolderNameKillerHeuristic;
            else
                return reportFolderNameAlphaBeta;
        }
        else if(GameState.boardMode == GameState.BoardMode.KillerHeuristicVSKillerHeuristic)
        {
            return reportFolderNameKillerHeuristic;
        }
        else
            return "Other Board Mode";
    }

    private static string GetFileName(bool forWhite)
    {
        if(forWhite)
            return reportFileNamePrefix + GetFolderName(forWhite) + " Depth " + GameState.whiteSearchDepth.ToString() + reportExtension;
        else
            return reportFileNamePrefix + GetFolderName(forWhite) + " Depth " + GameState.blackSearchDepth.ToString() + reportExtension;
    }

    private static string GetTimeStamp()
    {
        return DateTime.Now.ToString();
    }
    #endregion
}
