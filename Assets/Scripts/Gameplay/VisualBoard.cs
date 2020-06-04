using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualBoard : MonoBehaviour
{
    /// <summary>true = giliran putih, false = giliran hitam.</summary>
    private bool whiteTurnToMove = true;

    /// <summary>Representasi papan.</summary>
    private Board abstractBoard = new Board();
    /// <summary>GameObject root dari semua kotak.</summary>
    public GameObject squareRoot;
    /// <summary>Representasi setiap kotak.</summary>
    private Square[] squares;
    /// <summary>GameObject root dari semua pieces.</summary>
    public GameObject piecesRoot;
    /// <summary>Representasi setiap piece.</summary>
    private Piece[] pieces;
    /// <summary>Lokasi bidak yang telah mati.</summary>
    public Vector2 whiteDeadPiecesLocation, blackDeadPiecesLocation;
    /// <summary>Jumlah bidak yang mati.</summary>
    private int totalDeadWhitePieces = 0, totalDeadBlackPieces = 0;
    /// <summary>AI instance.</summary>
    private Engine engine;
    /// <summary>Penanda apakah move telah selesai dijalankan atau belum.</summary>
    private bool doneMove = true, newGame = true;

    public Text moveLog, title, whiteSearchDepth, blackSearchDepth;
    public Text whiteAvgTime, whiteTotalTime, whiteAvgNodes, whiteTotalNodes;
    public Text blackAvgTime, blackTotalTime, blackAvgNodes, blackTotalNodes;
    public ScrollRect moveLogScrollRect;

    private void Start(){
        squares = squareRoot.GetComponentsInChildren<Square>();
        pieces = piecesRoot.GetComponentsInChildren<Piece>();
        engine = new Engine();
        moveLog.text = "";
        if(GameState.boardMode == GameState.BoardMode.AlphaBetaVSAlphaBeta)
            title.text = "Alpha Beta VS Alpha Beta";
        else if(GameState.boardMode == GameState.BoardMode.AlphaBetaVSKillerHeuristic)
            title.text = "Alpha Beta VS Killer Heuristic";
        else if(GameState.boardMode == GameState.BoardMode.KillerHeuristicVSAlphaBeta)
            title.text = "Killer Heuristic VS Alpha Beta";
        else if(GameState.boardMode == GameState.BoardMode.KillerHeuristicVSKillerHeuristic)
            title.text = "Killer Heuristic VS Killer Heuristic";
        whiteSearchDepth.text = "(Search Depth: " + GameState.whiteSearchDepth.ToString() + ")";
        blackSearchDepth.text = "(Search Depth: " + GameState.blackSearchDepth.ToString() + ")";
        GameState.RestartValue();
        StartCoroutine(NewGame());
    }

    private void Update(){
        if(doneMove && GameState.boardStatus != GameState.Status.Draw && 
            GameState.boardStatus != GameState.Status.Checkmate && !newGame)
        {
            doneMove = false;
            StartCoroutine(MakeEngineMove(abstractBoard.duplicate()));
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            GameplayManager.Instance.BackToMenu(); // kembali ke scene menu
        }
    }

    private IEnumerator NewGame()
    {
        yield return new WaitForSeconds(1f);
        newGame = false;
    }

    /// <summary>Meminta AI untuk generate move.</summary>
    private IEnumerator MakeEngineMove(Board board)
    {
        if(whiteTurnToMove)
            GameState.totalTurn++;

        GameState.stopwatch.Restart();

        MoveData move = null;
        if(GameState.boardMode == GameState.BoardMode.AlphaBetaVSAlphaBeta)
        {
            if(whiteTurnToMove)
                move = engine.makeMoveAlphaBeta(board, whiteTurnToMove, GameState.whiteSearchDepth);
            else
                move = engine.makeMoveAlphaBeta(board, whiteTurnToMove, GameState.blackSearchDepth);
        }
        else if(GameState.boardMode == GameState.BoardMode.AlphaBetaVSKillerHeuristic)
        {
            if(whiteTurnToMove)
                move = engine.makeMoveAlphaBeta(board, whiteTurnToMove, GameState.whiteSearchDepth);
            else
                move = engine.makeMoveKillerHeuristic(board, whiteTurnToMove, GameState.blackSearchDepth);
        }
        else if(GameState.boardMode == GameState.BoardMode.KillerHeuristicVSAlphaBeta)
        {
            if(whiteTurnToMove)
                move = engine.makeMoveKillerHeuristic(board, whiteTurnToMove, GameState.whiteSearchDepth);
            else
                move = engine.makeMoveAlphaBeta(board, whiteTurnToMove, GameState.blackSearchDepth);
        }
        else if(GameState.boardMode == GameState.BoardMode.KillerHeuristicVSKillerHeuristic)
        {
            if(whiteTurnToMove)
                move = engine.makeMoveKillerHeuristic(board, whiteTurnToMove, GameState.whiteSearchDepth);
            else
                move = engine.makeMoveKillerHeuristic(board, whiteTurnToMove, GameState.blackSearchDepth);
        }

        GameState.stopwatch.Stop();
        PerformMove(move);
        UpdateStatus();
        writeLog(move);
        whiteTurnToMove = abstractBoard.whiteTurn;
        doneMove = true;
        yield return null;
    }

    /// <summary>Melakukan move pada abstract board dan visualisasikan pada visual board.</summary>
    private void PerformMove(MoveData move)
    {
        abstractBoard.move(move);

        // visualisasikan piece yang diserang, bila ada
        Piece attackedPiece = null;
        if(move.moveType == MoveData.MoveType.Simple)
            attackedPiece = getPiece(Board.SquareNames[move.to]);
        else if(move.moveType == MoveData.MoveType.EnPassant)
            attackedPiece = getPiece(Board.SquareNames[move.enPassantAttackSquare]);
        attackPiece(attackedPiece);

        // visualisasikan bidak yang digerakkan
        Piece movedPiece = getPiece(Board.SquareNames[move.from]);
        if(move.promoted) // move ini mempromosikan pawn, otomatis ubah jadi queen
        {
            if(abstractBoard.squares[move.to] > 0) // putih
                movedPiece.SetType(PieceData.PieceType.WhiteQueen);
            else
                movedPiece.SetType(PieceData.PieceType.BlackQueen);
        }
        else if(move.moveType == MoveData.MoveType.Castle) // castling move
        {
            Piece rookPiece = getPiece(Board.SquareNames[move.castleRookFrom]);
            rookPiece.moveToSquare(getSquare(Board.SquareNames[move.castleRookTo]));
        }

        Square toSquare = getSquare(Board.SquareNames[move.to]);
        movedPiece.moveToSquare(toSquare);

        // ubah warna kotak
        foreach(Square square in squares)
            square.changeColor(square.startColor);
        toSquare.changeColor(toSquare.recentMoveColor);

        GameState.boardStatus = abstractBoard.currentState(!whiteTurnToMove, true);
    }

    /// <summary>Mendapatkan square berdasarkan squareName.</summary>
    /// <param name="squareName">Nama unik kotak.</summary>
    /// <returns>Piece object.</returns>
    private Square getSquare(string squareName)
    {
        foreach(Square square in squares)
            if(square.uniqueName == squareName)
                return square;
        return null;
    }

    /// <summary>Mendapatkan Piece berdasarkan squareName.</summary>
    /// <param name="squareName">Nama unik kotak.</summary>
    /// <returns>Piece object.</returns>
    private Piece getPiece(string squareName)
    {
        foreach(Piece piece in pieces)
            if(piece.square != null && piece.square.uniqueName == squareName)
                return piece;
        return null;
    }

    /// <summary>Menyerang suatu bidak dan menempatkan bidak tersebut pada dead location.</summary>
    /// <param name="attackedPiece">Bidak yang diserang.</param>
    private void attackPiece(Piece attackedPiece)
    {
        if(attackedPiece == null)
            return;

        Vector3 deadLocation;
        if(attackedPiece.isWhite)
        {
            deadLocation = whiteDeadPiecesLocation + new Vector2(totalDeadWhitePieces % 4, -(totalDeadWhitePieces / 4));
            totalDeadWhitePieces++;
        }
        else
        {
            deadLocation = blackDeadPiecesLocation + new Vector2(totalDeadBlackPieces % 4, totalDeadBlackPieces / 4);
            totalDeadBlackPieces++;
        }
        attackedPiece.SetDeadPiece(deadLocation);
    }

    private void writeLog(MoveData move)
    {
        string currentText = moveLog.text;
        string newLog = getNotation(abstractBoard.duplicate(), move);

        if(GameState.boardStatus == GameState.Status.Checkmate)
        {
            if(whiteTurnToMove)
                newLog += "\n1-0";
            else
                newLog += "\n0-1";
        }
        else if(GameState.boardStatus == GameState.Status.Draw)
            newLog += "\n1/2-1/2";

        if(whiteTurnToMove) // tulis pada baris baru
        {
            if(GameState.totalTurn > 1)
                currentText += "\n";
            currentText += GameState.totalTurn.ToString() + ". ";

            if(GameState.boardStatus != GameState.Status.Checkmate && 
                GameState.boardStatus != GameState.Status.Draw)
                newLog += " ";
        }

        currentText += newLog;
        moveLog.text = currentText;
        moveLogScrollRect.verticalNormalizedPosition = 0f;
    }

    public string getNotation(Board disambiguationBoard, MoveData move)
    {
        string str = "";
        int type = Mathf.Abs(move.pieceType);
        if(type == (int)PieceData.PieceType.WhitePawn && move.capturedType != (int)PieceData.PieceType.None)
            str += Board.SquareNames[move.from].Substring(0, 1);
        else if(type == (int)PieceData.PieceType.WhiteRook)
            str += "R";
        else if(type == (int)PieceData.PieceType.WhiteKnight)
            str += "N";
        else if(type == (int)PieceData.PieceType.WhiteBishop)
            str += "B";
        else if(type == (int)PieceData.PieceType.WhiteQueen)
            str += "Q";
        else if(type == (int)PieceData.PieceType.WhiteKing && move.moveType != MoveData.MoveType.Castle)
            str += "K";
        
        // periksa apakah ada move yang ambigu (contoh: 2 kuda yang dapat melangkah ke tempat yang sama)
        if(type != (int)PieceData.PieceType.WhitePawn && type != (int)PieceData.PieceType.WhiteKing)
        {
            List<MoveData> otherMoves = disambiguationBoard.findLegalMoves(disambiguationBoard.whiteTurn);
            List<MoveData> ambiguousMoves = new List<MoveData>();
            foreach(MoveData otherMove in otherMoves)
                if(otherMove.to == move.to && otherMove.from != move.from && otherMove.pieceType == move.pieceType)
                    ambiguousMoves.Add(otherMove);
            
            if(ambiguousMoves.Count > 0)
            {
                bool fileMatch = false; // kolom sama (a-h)
                bool rankMatch = false; // baris sama (1-8)
                foreach(var ambiguousMove in ambiguousMoves)
                {
                    if(Board.SquareNames[ambiguousMove.from].Substring(0, 1) == Board.SquareNames[move.from].Substring(0, 1))
                        fileMatch = true;
                    if(Board.SquareNames[ambiguousMove.from].Substring(1, 1) == Board.SquareNames[move.from].Substring(1, 1))
                        rankMatch = true;
                }

                if(!fileMatch)
                    str += Board.SquareNames[move.from].Substring(0, 1);
                else if(fileMatch && !rankMatch)
                    str += Board.SquareNames[move.from].Substring(1, 1);
                else if(fileMatch && rankMatch)
                    str += Board.SquareNames[move.from];
            }
        }

        // menyerang piece lawan
        if(move.capturedType != (int)PieceData.PieceType.None)
            str += "x";
        
        if(move.moveType == MoveData.MoveType.Castle) // castling move
        {
            if(move.to == 2 || move.to == 58) // long castle
                str += "O-O-O";
            else // short castle
                str += "O-O";
        }
        else
        {
            str += Board.SquareNames[move.to];
            if(move.moveType == MoveData.MoveType.EnPassant)
                str += "e.p.";
        }
        
        if(move.promoted)
            str += "=Q";

        if(GameState.boardStatus == GameState.Status.Check)
            str += "+";
        else if(GameState.boardStatus == GameState.Status.Checkmate)
            str += "#";

        return str;
    }

    private void UpdateStatus()
    {
        GameState.UpdateTimeValue(whiteTurnToMove);
        
        if(whiteTurnToMove)
        {
            // update text
            whiteTotalTime.text = GameState.whiteTotalTime.ToString();
            whiteAvgTime.text = GameState.whiteAvgTime.ToString();
            
            whiteTotalNodes.text = GameState.whiteTotalNodes.ToString();
            whiteAvgNodes.text = GameState.whiteAvgNodes.ToString();
        }
        else
        {
            // update text
            blackTotalTime.text = GameState.blackTotalTime.ToString();
            blackAvgTime.text = GameState.blackAvgTime.ToString();

            blackTotalNodes.text = GameState.blackTotalNodes.ToString();
            blackAvgNodes.text = GameState.blackAvgNodes.ToString();
        }

        if(GameState.boardStatus == GameState.Status.Checkmate)
        {
            if(whiteTurnToMove)
                GameplayManager.Instance.GameOver(true, false);
            else
                GameplayManager.Instance.GameOver(false, true);
        }
        else if(GameState.boardStatus == GameState.Status.Draw)
            GameplayManager.Instance.GameOver(false, false);
    }
}
