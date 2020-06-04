using System;
using System.Collections.Generic;

/// <summary>The chess board, disini moves di generate, checked for legality, performed and reverted.</summary>
[System.Serializable]
public class Board
{
    /*
        0 = unoccupied square

        1 = white pawn
        2 = white rook
        3 = white knight
        4 = white bishop
        5 = white queen
        6 = white king

        -1 = black pawn
        -2 = black rook
        -3 = black knight
        -4 = black bishop
        -5 = black queen
        -6 = black king

        0  1  2  3  4  5  6  7
        8  9  10 11 12 13 14 15
        16 17 18 19 20 21 22 23
        24 25 26 27 28 29 30 31
        32 33 34 35 36 37 38 39 
        40 41 42 43 44 45 46 47
        48 49 50 51 52 53 54 55
        56 57 58 59 60 61 62 63
    */

    /// <summary>0-63 index representation bidak pada papan.</summary>
    public List<int> squares;
    public static List<int> InitialPiecePositions = new List<int>
    {
        -2, -3, -4, -5, -6, -4, -3, -2,
        -1, -1, -1, -1, -1, -1, -1, -1,
        0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,
        1,  1,  1,  1,  1,  1,  1,  1,
        2,  3,  4,  5,  6,  4,  3,  2
    };
    
    /// <summary>Nama cell berdasarkan index position.</summary>
    public static string[] SquareNames =
    {
        "a8","b8","c8","d8","e8","f8","g8","h8",
        "a7","b7","c7","d7","e7","f7","g7","h7",
        "a6","b6","c6","d6","e6","f6","g6","h6",
        "a5","b5","c5","d5","e5","f5","g5","h5",
        "a4","b4","c4","d4","e4","f4","g4","h4",
        "a3","b3","c3","d3","e3","f3","g3","h3",
        "a2","b2","c2","d2","e2","f2","g2","h2",
        "a1","b1","c1","d1","e1","f1","g1","h1"
    };
    private int maxCellIndex = 64;

    /// <summary>List semua moves yang di-perform secara berurutan.</summary>
    public List<MoveData> moves = new List<MoveData>();

    public bool whiteTurn = true;

    private bool whiteHasCastled = false;
    private bool blackHasCastled = false;
    private int whiteLeftRookMoves = 0, whiteRightRookMoves = 0;
    private int blackLeftRookMoves = 0, blackRightRookMoves = 0;
    private int whiteKingMoves = 0, blackKingMoves = 0;
    private int[] whiteRooksStart = {63, 56}, blackRooksStart = {7, 0}; // [0] for short castle, [1] for long castle
    private int whiteKingStart = 60, blackKingStart = 4;
    private int[] whiteShortCastleCrossSquares = {61, 62}, whiteLongCastleCrossSquares = {57, 58, 59};
    private int[] blackShortCastleCrossSquares = {5, 6}, blackLongCastleCrossSquares = {1, 2, 3};
    private int whiteKingShortCastleTo = 62, whiteKingLongCastleTo = 58;
    private int blackKingShortCastleTo = 6, blackKingLongCastleTo = 2;
    private int whiteRookShortCastleTo = 61, whiteRookLongCastleTo = 59;
    private int blackRookShortCastleTo = 5, blackRookLongCastleTo = 3;

    private int pawnDoubleMoveDistance = 16; // distance index
    private int enPassantSquare = 0, enPassantAttackSquare = 0, halfwaySquare = 32;
    private int whitePromotionBelow = 8;
    private int blackPromotionAbove = 55;

    /// <summary>
    /// Semua kemungkinan moves yang di generate MoveGenerator,
    /// disimpan dan diidentifikasikan menggunakan string: (piecetype)+(indexposition)
    /// </summary>
    private Dictionary<string, MoveSet> allPossibleMoves = null;

    /// <summary>Buat instance papan yang baru</summary>
    public Board(List<int> positions = null, Dictionary<string, MoveSet> allPMoves = null)
    {
        if(positions != null) 
            squares = new List<int>(positions);
        else
            squares = new List<int>(InitialPiecePositions);

        if(allPMoves != null)
            allPossibleMoves = new Dictionary<string, MoveSet>(allPMoves);
        else
            generateAllPosibleMoves();
    }

    /// <summary>Generate semua possible move setiap jenis bidak di setiap cell papan.</summary>
    private void generateAllPosibleMoves()
    {
        allPossibleMoves = new Dictionary<string, MoveSet>();
        List<int> tempMoves = null;
        MoveGenerator moveGenerator = new MoveGenerator();
        for(int pos = 0; pos < maxCellIndex; pos++)
        {
            // pawn tidak mungkin berada di row pertama dan terakhir, jadi bisa di ignore
            if(pos >= whitePromotionBelow && pos <= blackPromotionAbove)
            {
                allPossibleMoves["1" + pos.ToString()] = new MoveSet(moveGenerator.GeneratePawnMoves(pos, true), pos, PieceData.PieceType.BlackPawn);
                allPossibleMoves["-1" + pos.ToString()] = new MoveSet(moveGenerator.GeneratePawnMoves(pos, false), pos, PieceData.PieceType.WhitePawn);
            }

            tempMoves = moveGenerator.GenerateRookMoves(pos);
            allPossibleMoves["2" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.WhiteRook);
            allPossibleMoves["-2" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.BlackRook);

            tempMoves = moveGenerator.GenerateKnightMoves(pos);
            allPossibleMoves["3" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.WhiteKnight);
            allPossibleMoves["-3" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.BlackKnight);
            
            tempMoves = moveGenerator.GenerateBishopMoves(pos);
            allPossibleMoves["4" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.WhiteBishop);
            allPossibleMoves["-4" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.WhiteBishop);

            tempMoves = moveGenerator.GenerateQueenMoves(pos);
            allPossibleMoves["5" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.WhiteQueen);
            allPossibleMoves["-5" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.WhiteQueen);

            tempMoves = moveGenerator.GenerateKingMoves(pos);
            allPossibleMoves["6" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.WhiteKing);
            allPossibleMoves["-6" + pos.ToString()] = new MoveSet(new List<int>(tempMoves), pos, PieceData.PieceType.BlackKing);
        }
    }

    /// <summary>Evaluate papan saat ini berdasarkan material dan positional values dari kedua pihak.</summary>
    /// <param name="isFirstDepth">Apakah ini depth pertama dari root?</param>
    public int Evaluate(bool isFirstDepth = false)
    {
        int resultVal = 0;

        // untuk menentukan phase game (untuk evaluasi position king)
        int whitePieceVal = 0, blackPieceVal = 0;
        int whiteKingPos = 0, blackKingPos = 0;
        for(int i = 0; i < maxCellIndex; i++)
        {
            switch(squares[i])
            {
                case (int)PieceData.PieceType.None:
                    break;
                case (int)PieceData.PieceType.WhitePawn:
                    resultVal += ValueModifiers.Value_Pawn + ValueModifiers.Position_White_Pawn[i];
                    whitePieceVal += ValueModifiers.Value_Pawn;
                    break;
                case (int)PieceData.PieceType.WhiteRook:
                    resultVal += ValueModifiers.Value_Rook + ValueModifiers.Position_White_Rook[i];
                    whitePieceVal += ValueModifiers.Value_Rook;
                    break;
                case (int)PieceData.PieceType.WhiteKnight:
                    resultVal += ValueModifiers.Value_Knight + ValueModifiers.Position_White_Knight[i];
                    whitePieceVal += ValueModifiers.Value_Knight;
                    break;
                case (int)PieceData.PieceType.WhiteBishop:
                    resultVal += ValueModifiers.Value_Bishop + ValueModifiers.Position_White_Bishop[i];
                    whitePieceVal += ValueModifiers.Value_Bishop;
                    break;
                case (int)PieceData.PieceType.WhiteQueen:
                    resultVal += ValueModifiers.Value_Queen + ValueModifiers.Position_White_Queen[i];
                    whitePieceVal += ValueModifiers.Value_Queen;
                    break;
                case (int)PieceData.PieceType.WhiteKing:
                    whiteKingPos = i;
                    break;
                case (int)PieceData.PieceType.BlackPawn:
                    resultVal -= ValueModifiers.Value_Pawn - ValueModifiers.Position_Black_Pawn[i];
                    blackPieceVal += ValueModifiers.Value_Pawn;
                    break;
                case (int)PieceData.PieceType.BlackRook:
                    resultVal -= ValueModifiers.Value_Rook - ValueModifiers.Position_Black_Rook[i];
                    blackPieceVal += ValueModifiers.Value_Rook;
                    break;
                case (int)PieceData.PieceType.BlackKnight:
                    resultVal -= ValueModifiers.Value_Knight - ValueModifiers.Position_Black_Knight[i];
                    blackPieceVal += ValueModifiers.Value_Knight;
                    break;
                case (int)PieceData.PieceType.BlackBishop:
                    resultVal -= ValueModifiers.Value_Bishop - ValueModifiers.Position_Black_Bishop[i];
                    blackPieceVal += ValueModifiers.Value_Bishop;
                    break;
                case (int)PieceData.PieceType.BlackQueen:
                    resultVal -= ValueModifiers.Value_Queen - ValueModifiers.Position_Black_Queen[i];
                    blackPieceVal += ValueModifiers.Value_Queen;
                    break;
                case (int)PieceData.PieceType.BlackKing:
                    blackKingPos = i;
                    break;
                default:
                    continue;
            }
        }

        if(whitePieceVal > ValueModifiers.Value_End_Game_Phase)
            resultVal += ValueModifiers.Value_King + ValueModifiers.Position_White_King_Start_Game_Phase[whiteKingPos];
        else
            resultVal += ValueModifiers.Value_King + ValueModifiers.Position_White_King_End_Game_Phase[whiteKingPos];
        
        if(blackPieceVal > ValueModifiers.Value_End_Game_Phase)
            resultVal -= ValueModifiers.Value_King - ValueModifiers.Position_Black_King_Start_Game_Phase[blackKingPos];
        else
            resultVal -= ValueModifiers.Value_King - ValueModifiers.Position_Black_King_End_Game_Phase[blackKingPos];
        
        GameState.Status curState = currentState(whiteTurn);
        if(curState == GameState.Status.Checkmate){
            if(whiteTurn){ // putih kalah
                resultVal -= ValueModifiers.Value_King;
                if(isFirstDepth) resultVal = int.MinValue; // force AI hitam untuk langsung memilih move ini
            }
            else{ // hitam kalah
                resultVal += ValueModifiers.Value_King;
                if(isFirstDepth) resultVal = int.MaxValue; // force AI putih untuk langsung memilih move ini
            }
        }
        else if(curState == GameState.Status.Draw)
            resultVal = 0; // Stockfish reference: https://github.com/official-stockfish/Stockfish/blob/master/src/types.h (enum "Value")
        return resultVal;
    }

    #region moves
    /// <summary>Perform the provided move, capturing if necessary.</summary>
    /// <param name="move">Move to perform.</param>
    public void move(MoveData move)
    {
        moves.Add(move);
        if(move.to < 0 || move.to >= maxCellIndex) // illegal index for move.
            return;
        
        if(move.moveType == MoveData.MoveType.Castle) // perform castling move
        {
            squares[move.to] = squares[move.from]; // king piece
            squares[move.from] = 0;
            squares[move.castleRookTo] = squares[move.castleRookFrom]; // rook piece
            squares[move.castleRookFrom] = 0;

            if(whiteTurn) whiteHasCastled = true;
            else blackHasCastled = true;
        }
        else if(move.moveType == MoveData.MoveType.EnPassant) // perform EnPassant move
        {
            move.capturedType = squares[move.enPassantAttackSquare]; // piece yang diserang
            squares[move.enPassantAttackSquare] = 0;

            squares[move.to] = squares[move.from]; // pawn piece
            squares[move.from] = 0;
        }
        else // perform simple move
        {
            move.capturedType = squares[move.to]; // piece yang diserang, bernilai 0 bila tidak ada
            squares[move.to] = squares[move.from];
            squares[move.from] = 0;
        }

        // pawn auto-promote to queen
        if(move.to < whitePromotionBelow && squares[move.to] == (int)PieceData.PieceType.WhitePawn)
        {
            move.promoted = true;
            squares[move.to] = (int)PieceData.PieceType.WhiteQueen;
        }
        else if(move.to > blackPromotionAbove && squares[move.to] == (int)PieceData.PieceType.BlackPawn)
        {
            move.promoted = true;
            squares[move.to] = (int)PieceData.PieceType.BlackQueen;
        }

        // rules yang membuat tidak dapat castling
        if(squares[move.to] == (int)PieceData.PieceType.WhiteRook)
        {
            if(move.from == whiteRooksStart[0])
                whiteRightRookMoves++; // white short castle
            else if(move.from == whiteRooksStart[1])
                whiteLeftRookMoves++; // white long castle
        }
        else if(squares[move.to] == (int)PieceData.PieceType.BlackRook)
        {
            if(move.from == blackRooksStart[0])
                blackRightRookMoves++; // black short castle
            else if(move.from == blackRooksStart[1])
                blackLeftRookMoves++; // black long castle
        }
        else if(squares[move.to] == (int)PieceData.PieceType.WhiteKing)
            whiteKingMoves++;
        else if(squares[move.to] == (int)PieceData.PieceType.BlackKing)
            blackKingMoves++;
        
        // change turn
        whiteTurn = !whiteTurn;
    }
    
    /// <summary>Apakah warna ini sedang di check? atau checkmate? draw? atau normal state.</summary>
    /// <param name="asWhite">Warna giliran saat ini.</param>
    /// <param name="setDrawVariance">Apakah perlu set nilai DrawVariance?</param>
    /// <returns>Status papan saat ini.</returns>
    public GameState.Status currentState(bool asWhite, bool setDrawVariance = false)
    {
        GameState.Status cur = GameState.Status.Normal;
        if(setDrawVariance) GameState.drawVariance = GameState.DrawVariance.None;

        // periksa apakah checked
        foreach(MoveData enemyMove in findLegalMoves(!asWhite))
        {
            if(squares[enemyMove.to] == (int)PieceData.PieceType.BlackKing 
                || squares[enemyMove.to] == (int)PieceData.PieceType.WhiteKing)
            {
                cur = GameState.Status.Check;
                break;
            }
        }

        if(cur == GameState.Status.Check)
        {
            // periksa apakah warna ini kalah (checkmate)
            List<MoveData> responseMoves = findAllLegalMoves(asWhite);
            if(responseMoves.Count == 0) // jika tidak ada move maka checkmate, jika masih ada move, hanya check biasa.
                cur = GameState.Status.Checkmate;
            else if(moves.Count > 9) // periksa apakah draw karena check berulang kali (Perpetual Check)
            {
                if(moves[moves.Count - 1].pieceType == moves[moves.Count - 5].pieceType
                    && moves[moves.Count - 1].pieceType == moves[moves.Count - 9].pieceType
                    && moves[moves.Count - 2].pieceType == moves[moves.Count - 6].pieceType
                    && moves[moves.Count - 2].pieceType == moves[moves.Count - 10].pieceType)
                {
                    // bidak yang sama digerakkan dalam 10 langkah terakhir. cek apakah posisinya sama juga.
                    if(moves[moves.Count - 1].to == moves[moves.Count - 5].to
                        && moves[moves.Count - 1].to == moves[moves.Count - 9].to
                        && moves[moves.Count - 2].to == moves[moves.Count - 6].to
                        && moves[moves.Count - 2].to == moves[moves.Count - 10].to)
                    {
                        cur = GameState.Status.Draw;
                        if(setDrawVariance) 
                            GameState.drawVariance = GameState.DrawVariance.PerpetualCheck;
                    }
                }
            }
            return cur;
        }
        else
        {
            // periksa apakah draw.
            if(moves.Count > 9) // periksa apakah draw karena posisi yang sama (Threefold repetition, simplified rule)
            {
                if(moves[moves.Count - 1].pieceType == moves[moves.Count - 5].pieceType
                    && moves[moves.Count - 1].pieceType == moves[moves.Count - 9].pieceType
                    && moves[moves.Count - 2].pieceType == moves[moves.Count - 6].pieceType
                    && moves[moves.Count - 2].pieceType == moves[moves.Count - 10].pieceType)
                {
                    // bidak yang sama digerakkan dalam 10 langkah terakhir. cek apakah posisinya sama juga.
                    if(moves[moves.Count - 1].to == moves[moves.Count - 5].to
                        && moves[moves.Count - 1].to == moves[moves.Count - 9].to
                        && moves[moves.Count - 2].to == moves[moves.Count - 6].to
                        && moves[moves.Count - 2].to == moves[moves.Count - 10].to)
                    {
                        cur = GameState.Status.Draw;
                        if(setDrawVariance) 
                            GameState.drawVariance = GameState.DrawVariance.ThreefoldRepetition;
                        return cur;
                    }
                }
            }

            List<MoveData> responseMoves = findAllLegalMoves(asWhite);
            if(responseMoves.Count == 0) // tidak ada move dan sedang tidak check, maka draw (stalemate).
            {
                cur = GameState.Status.Draw;
                if(setDrawVariance)
                    GameState.drawVariance = GameState.DrawVariance.StaleMate;
            }
            else
            {
                bool whiteBishopWhiteSquare = false;
                bool whiteBishopBlackSquare = false;
                int whiteKnights = 0;

                bool blackBishopWhiteSquare = false;
                bool blackBishopBlackSquare = false;
                int blackKnights = 0;

                for(int i = 0; i < maxCellIndex; i++)
                {
                    // hitung semua kecuali kotak yang kosong dan raja.
                    if(squares[i] != (int)PieceData.PieceType.None && squares[i] != (int)PieceData.PieceType.WhiteKing 
                        && squares[i] != (int)PieceData.PieceType.BlackKing)
                    {
                        if(squares[i] == (int)PieceData.PieceType.WhiteKnight)
                            whiteKnights++;
                        else if(squares[i] == (int)PieceData.PieceType.WhiteBishop)
                        {
                            // baris ganjil
                            if((i >= 0 && i <= 7) || (i >= 16 && i <= 23) || (i >= 32 && i <= 39) || (i >= 48 && i <= 55))
                            {
                                if(i % 2 == 0) whiteBishopWhiteSquare = true;
                                else whiteBishopBlackSquare = true;
                            }
                            else // baris genap
                            {
                                if(i % 2 == 0) whiteBishopBlackSquare = true;
                                else whiteBishopWhiteSquare = true;
                            }
                        }
                        else if(squares[i] == (int)PieceData.PieceType.BlackKnight)
                            blackKnights++;
                        else if(squares[i] == (int)PieceData.PieceType.BlackBishop)
                        {
                            // baris ganjil
                            if((i >= 0 && i <= 7) || (i >= 16 && i <= 23) || (i >= 32 && i <= 39) || (i >= 48 && i <= 55))
                            {
                                if(i % 2 == 0) blackBishopWhiteSquare = true;
                                else blackBishopBlackSquare = true;
                            }
                            else // baris genap
                            {
                                if(i % 2 == 0) blackBishopBlackSquare = true;
                                else blackBishopWhiteSquare = true;
                            }
                        }
                        else // pawn, queen, atau rook masih ada, masih belum draw.
                        {
                            return cur;
                        }
                    }
                }

                // masih ada 2 kuda, belum draw.
                if(whiteKnights >= 2 || blackKnights >= 2)
                    return cur;
                // masih ada 2 bishop, belum draw.
                if((whiteBishopBlackSquare && whiteBishopWhiteSquare) 
                    || (blackBishopBlackSquare && blackBishopWhiteSquare))
                    return cur;
                // masih ada 1 knight dan 1 bishop, belum draw.
                if((whiteKnights == 1 && (whiteBishopBlackSquare || whiteBishopWhiteSquare)) ||
                    (blackKnights == 1 && (blackBishopBlackSquare || blackBishopWhiteSquare))
                )
                    return cur;
                
                // selain itu, maka draw karena material untuk checkmate tidak mencukupi.
                cur = GameState.Status.Draw;
                if(setDrawVariance)
                    GameState.drawVariance = GameState.DrawVariance.NotEnoughMaterials;
            }
            return cur;
        }
    }

    /// <summary>Apakah masih boleh melakukan short castling?</summary>
    /// <param name="asWhite">Warna yang ingin dicek.</param>
    /// <returns>True bila dapat melakukan short castling.</returns>
    public bool canShortCastling(bool asWhite)
    {
        bool shortCastling = false;
        if(asWhite)
        {
            if(whiteKingMoves == 0 && whiteRightRookMoves == 0 && !whiteHasCastled 
                && squares[whiteRooksStart[0]] == (int)PieceData.PieceType.WhiteRook)
                shortCastling = true;
            foreach(int pos in whiteShortCastleCrossSquares)
            {
                if(squares[pos] != (int)PieceData.PieceType.None)
                {
                    shortCastling = false;
                    break;
                }
            }
        }
        else
        {
            if(blackKingMoves == 0 && blackRightRookMoves == 0 && !blackHasCastled 
                && squares[blackRooksStart[0]] == (int)PieceData.PieceType.BlackRook)
                shortCastling = true;
            foreach(int pos in blackShortCastleCrossSquares)
            {
                if(squares[pos] != (int)PieceData.PieceType.None)
                {
                    shortCastling = false;
                    break;
                }
            }
        }
        return shortCastling;  
    }

    /// <summary>Apakah masih boleh melakukan long castling?</summary>
    /// <param name="asWhite">Warna yang ingin dicek.</param>
    /// <returns>True bila dapat melakukan long castling.</returns>
    public bool canLongCastling(bool asWhite)
    {
        bool longCastling = false;
        if(asWhite)
        {
            if(whiteKingMoves == 0 && whiteLeftRookMoves == 0 && !whiteHasCastled 
                && squares[whiteRooksStart[1]] == (int)PieceData.PieceType.WhiteRook)
                longCastling = true;
            foreach(int pos in whiteLongCastleCrossSquares)
            {
                if(squares[pos] != (int)PieceData.PieceType.None)
                {
                    longCastling = false;
                    break;
                }
            }
        }            
        else
        {
            if(blackKingMoves == 0 && blackLeftRookMoves == 0 && !blackHasCastled 
                && squares[blackRooksStart[1]] == (int)PieceData.PieceType.BlackRook)
                longCastling = true;
            foreach(int pos in blackLongCastleCrossSquares)
            {
                if(squares[pos] != (int)PieceData.PieceType.None)
                {
                    longCastling = false;
                    break;
                }
            }
        }
        return longCastling;
    }

    /// <summary>Cari semua kemungkinan move untuk suatu bidak pada suatu posisi.</summary>
    /// <param name="piece">Jenis bidak.</param>
    /// <param name="pos">Index posisi bidak.</param>
    /// <returns>MoveSet suatu bidak dari suatu posisi.</returns>
    private MoveSet findMoveSet(int piece, int pos)
    {
        string key = piece.ToString() + pos.ToString();
        return allPossibleMoves[key];
    }

    /// <summary>Returns all legal moves untuk parameter warna untuk papan saat ini.</summary>
    /// <param name="asWhite">Move sebagai putih?</param>
    /// <returns>Semua legal moves dari parameter warna yang diberikan.</returns>
    public List<MoveData> findAllLegalMoves(bool asWhite)
    {
        List<MoveData> legalMoves = findLegalMoves(asWhite);
        for(int i = legalMoves.Count - 1; i >= 0; i--)
            if(!kingVerifyLegality(legalMoves[i]))
                legalMoves.RemoveAt(i);

        Shuffle.ShuffleList<MoveData>(legalMoves);
        return legalMoves;
    }

    /// <summary>
    /// Returns all legal moves untuk parameter warna untuk papan saat ini.
    /// Tidak termasuk move king yang membuat king menjadi diserang.
    /// </summary>
    /// <param name="asWhite">Move sebagai putih?</param>
    /// <returns>Semua legal moves dari parameter warna yang diberikan.</returns>
    public List<MoveData> findLegalMoves(bool asWhite)
    {
        List<MoveData> ownMoves = new List<MoveData>();
        List<MoveData> enemyMoves = new List<MoveData>();

        bool isCheck = false;
        enPassantSquare = maxCellIndex;
        enPassantAttackSquare = maxCellIndex;
        if(moves.Count > 0)
        {
            MoveData lastMove = moves[moves.Count - 1];
            if(squares[lastMove.to] == (int)PieceData.PieceType.WhitePawn 
                || squares[lastMove.to] == (int)PieceData.PieceType.BlackPawn)
            {
                // ini bidak pawn
                if(Math.Abs(lastMove.from - lastMove.to) == pawnDoubleMoveDistance)
                {
                    // pawn melakukan double move pada move sebelumnya, jadi ada kesempatan enpassant.
                    int moveDistanceIdx = 8;
                    if(lastMove.from >= halfwaySquare)
                        enPassantSquare = lastMove.from - moveDistanceIdx;
                    else
                        enPassantSquare = lastMove.from + moveDistanceIdx;
                    enPassantAttackSquare = lastMove.to;
                }
            }
        }

        for(int i = 0; i < maxCellIndex; i++)
        {
            int piece = squares[i];
            if((piece > 0 && asWhite) || (piece < 0 && !asWhite))
                ownMoves.AddRange(removeIllegalMoves(findMoveSet(piece, i)));
            else if(piece != 0)
                enemyMoves.AddRange(removeIllegalMoves(findMoveSet(piece, i)));
        }

        // tes apakah dapat castling
        bool shortCastling = canShortCastling(asWhite);
        bool longCastling = canLongCastling(asWhite);
        List<MoveData> checkingMoves = new List<MoveData>();

        // cek berdasarkan enemy move
        foreach(MoveData enemyMove in enemyMoves)
        {
            if(squares[enemyMove.to] == (int)PieceData.PieceType.WhiteKing 
                || squares[enemyMove.to] == (int)PieceData.PieceType.BlackKing)
            {
                // ada enemy move yg sedang menargetkan King. jadi tidak dapat castle dan harus mengurus check ini.
                isCheck = true;
                shortCastling = false;
                longCastling = false;
                checkingMoves.Add(enemyMove);
            }
            else if(squares[enemyMove.to] != (int)PieceData.PieceType.None)
            {
                // cek apakah ada enemy move yang menargetkan non-king yang menutupi king,
                // sehingga bidak tersebut tidak dapat dipindahkan keluar area ray.
                List<int> ray = getFullRayMove(findMoveSet(squares[enemyMove.from], enemyMove.from), enemyMove.to);
                if(ray != null)
                {
                    int hitsAlongRay = 0;
                    int hitKingAt = -1;
                    int totalRay = ray.Count;
                    for(int i = 0; i < totalRay; i++)
                    {
                        if((squares[ray[i]] == (int)PieceData.PieceType.WhiteKing && squares[enemyMove.from] < 0)
                            || (squares[ray[i]] == (int)PieceData.PieceType.BlackKing && squares[enemyMove.from] > 0))
                        {
                            hitKingAt = i;
                            break;
                        }
                        else if((squares[ray[i]] > 0 && squares[enemyMove.from] < 0) 
                            || (squares[enemyMove.from] > 0 && squares[ray[i]] < 0))
                            hitsAlongRay++;
                    }
                    
                    // enemy move ini memiliki ray yang menyerang King dan hanya di block oleh one piece(hitsalongray == 1)
                    // jadi piece tersebut tidak boleh move keluar area dari ray, jadi hapus move yang illegal tersebut.
                    if(hitKingAt != -1 && hitsAlongRay == 1)
                    {
                        ray.RemoveRange(hitKingAt, ray.Count - hitKingAt);
                        for(int i = ownMoves.Count - 1; i >= 0; i--)
                        {
                            if(ray.Contains(ownMoves[i].from))
                            {
                                // ownMove ini adalah piece yang memblock ray musuh pada king.
                                if(!ray.Contains(ownMoves[i].to) && ownMoves[i].to != enemyMove.from)
                                {
                                    // ownMove ini keluar area ray dan tidak menuju bidak musuh yang menyerang juga.
                                    if(Math.Abs(squares[ownMoves[i].to]) != (int)PieceData.PieceType.WhiteKing)
                                    {
                                        // rare case untuk mencegah raja musuh bergerak pada
                                        // squares yang diserang own bidak yang sedang melakukan block
                                        ownMoves.RemoveAt(i); // move ini illegal, dihapus.
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // disable castling move jika enemy move menyerang any castling cross square.
            if(shortCastling)
            {
                if(asWhite)
                {
                    foreach(int pos in whiteShortCastleCrossSquares)
                    {
                        if(enemyMove.to == pos)
                        {
                            shortCastling = false;
                            break;
                        }
                    }
                }
                else if(!asWhite)
                {
                    foreach(int pos in blackShortCastleCrossSquares)
                    {
                        if(enemyMove.to == pos)
                        {
                            shortCastling = false;
                            break;
                        }
                    }
                }
            }

            if(longCastling)
            {
                if(asWhite)
                {
                    foreach(int pos in whiteLongCastleCrossSquares)
                    {
                        if(enemyMove.to == pos)
                        {
                            longCastling = false;
                            break;
                        }
                    }
                }
                else if(!asWhite)
                {
                    foreach(int pos in blackLongCastleCrossSquares)
                    {
                        if(enemyMove.to == pos)
                        {
                            longCastling = false;
                            break;
                        }
                    }
                }
            }
        }
        // end of enemymoves loop

        // cek bila ada own King move yang illegal (King yang dapat berjalan ke kotak yang sedang diserang enemy).
        for(int i = ownMoves.Count; i > 0; i--)
        {
            if(squares[ownMoves[i - 1].from] == (int)PieceData.PieceType.WhiteKing
                || squares[ownMoves[i - 1].from] == (int)PieceData.PieceType.BlackKing)
            {
                foreach(MoveData enemyMove in enemyMoves)
                {
                    if(enemyMove.to == ownMoves[i - 1].to 
                        && squares[enemyMove.from] != (int)PieceData.PieceType.WhitePawn 
                        && squares[enemyMove.to] != (int)PieceData.PieceType.BlackPawn)
                    {
                        ownMoves.RemoveAt(i - 1);
                        break;
                    }
                }
            }
        }

        // jika castling masih true, maka sudah dapat dimasukkan ke ownMoves.
        if(shortCastling)
        {
            if(asWhite)
                ownMoves.Add(new MoveData((int)PieceData.PieceType.WhiteKing,
                    whiteKingStart, whiteKingShortCastleTo, whiteRooksStart[0], whiteRookShortCastleTo));
            else if(!asWhite)
                ownMoves.Add(new MoveData((int)PieceData.PieceType.BlackKing, 
                    blackKingStart, blackKingShortCastleTo, blackRooksStart[0], blackRookShortCastleTo));
        }

        if(longCastling)
        {
            if(asWhite)
                ownMoves.Add(new MoveData((int)PieceData.PieceType.WhiteKing, 
                    whiteKingStart, whiteKingLongCastleTo, whiteRooksStart[1], whiteRookLongCastleTo));
            else if(!asWhite)
                ownMoves.Add(new MoveData((int)PieceData.PieceType.BlackKing, 
                    blackKingStart, blackKingLongCastleTo, blackRooksStart[1], blackRookLongCastleTo));
        }

        // jika sedang tidak check, maka semua ownMove saat ini bisa dinyatakan legal, boleh di return.
        if(!isCheck) return ownMoves;
        else
        {
            // ada setidaknya 1 enemy move yang saat ini check own King.

            // jika hanya ada 1 move yang Check ke King, maka bisa serang bidak tersebut atau block path ray nya
            if(checkingMoves.Count == 1)
            {
                List<int> legalMoveToSquares = getBlockAttackSquares(checkingMoves[0]);
                for(int i = ownMoves.Count; i > 0; i--)
                {
                    if(!legalMoveToSquares.Contains(ownMoves[i - 1].to)
                        && squares[ownMoves[i - 1].from] != (int)PieceData.PieceType.WhiteKing
                        && squares[ownMoves[i - 1].from] != (int)PieceData.PieceType.BlackKing)
                        ownMoves.RemoveAt(i - 1);
                }
            }
            else
            {
                // King diserang oleh lebih dari 1 enemy move, jadi bidak selain king tidak boleh bergerak.
                for(int i = ownMoves.Count; i > 0; i--)
                    if(squares[ownMoves[i - 1].from] != (int)PieceData.PieceType.WhiteKing 
                        && squares[ownMoves[i - 1].from] != (int)PieceData.PieceType.BlackKing)
                        ownMoves.RemoveAt(i - 1);
            }
        }

        return ownMoves;
    }

    /// <summary>Cari full ray (tidak terpotong oleh destination/includesIndex) di moveset.</summary>
    /// <param name="moveSet">Moveset yang ingin disearch.</param>
    /// <param name="includesIndex">Index yang dituju.</param>
    /// <returns>Full ray yang termasuk index position.</returns>
    private List<int> getFullRayMove(MoveSet moveSet, int includesIndex)
    {
        if(moveSet.type == PieceData.PieceType.WhitePawn || moveSet.type == PieceData.PieceType.BlackPawn
            || moveSet.type == PieceData.PieceType.WhiteKnight || moveSet.type == PieceData.PieceType.BlackKnight
            || moveSet.type == PieceData.PieceType.WhiteKing || moveSet.type == PieceData.PieceType.BlackKing)
            return null;
        
        int startOfRay = 0;
        int endOfRay = maxCellIndex;
        bool hitRay = false;
        int totalSet = moveSet.moves.Count;
        for(int i = 0; i < totalSet; i++)
        {
            if(moveSet.moves[i] == -1 && !hitRay)
                startOfRay = i+1;
            if(moveSet.moves[i] == -1 && hitRay)
            {
                endOfRay = i;
                break;
            }
            if(moveSet.moves[i] == includesIndex)
                hitRay = true;
        }
        if(!hitRay)
            return null;
        else
        {
            List<int> ray = new List<int>();
            for(int i = startOfRay; i < endOfRay; i++)
                if(moveSet.moves[i] > -1)
                    ray.Add(moveSet.moves[i]);
            
            return ray;
        }
    }

    /// <summary>Return semua posisi yang dapat block suatu move.</summary>
    /// <param name="forMove">Move yang harus di block.</param>
    /// <returns>posisi yang dapat block move-nya.</returns>
    private List<int> getBlockAttackSquares(MoveData forMove)
    {
        List<int> resultPos = new List<int>();
        resultPos.Add(forMove.from);
        int type = squares[forMove.from];
        if(type == (int)PieceData.PieceType.WhiteRook || type == (int)PieceData.PieceType.BlackRook
            || type == (int)PieceData.PieceType.WhiteBishop || type == (int)PieceData.PieceType.BlackBishop
            || type == (int)PieceData.PieceType.WhiteQueen || type == (int)PieceData.PieceType.BlackQueen)
        {
            MoveSet mset = findMoveSet(type, forMove.from);
            int totalMSet = mset.moves.Count;
            int startOfRay = 0, endOfRay = 0;
            for(int i = 0; i < totalMSet; i++)
            {
                if(mset.moves[i] == -1)
                    startOfRay = i + 1;
                else if(mset.moves[i] == forMove.to)
                {
                    endOfRay = i;
                    break;
                }
            }
            
            for(int i = startOfRay; i < endOfRay; i++)
                resultPos.Add(mset.moves[i]);
        }

        return resultPos;
    }

    /// <summary>
    /// Cek sebuah moveset,
    /// dan remove semua illegal moves pada moveset tersebut berdasarkan papan saat ini.
    /// </summary>
    /// <param name="moveSet">Moveset yang ingin dicek.</param>
    /// <returns>Semua legal moves.</returns>
    private List<MoveData> removeIllegalMoves(MoveSet moveSet)
    {
        List<MoveData> legalMoves = new List<MoveData>();
        bool asWhite = squares[moveSet.from] > 0 ? true : false;
        bool rayGoing = true;
        bool canAttack = true;
        bool canMove = true;
        bool isKing = false;
        if(moveSet.type == PieceData.PieceType.WhitePawn || moveSet.type == PieceData.PieceType.BlackPawn)
            canAttack = false;
        if(moveSet.type == PieceData.PieceType.WhiteKing || moveSet.type == PieceData.PieceType.BlackKing)
            isKing = true;
        int fromPos = moveSet.from, toPos;

        for(int i = 0; i < moveSet.moves.Count; i++)
        {
            toPos = moveSet.moves[i];

            // knight atau king kan selalu bisa berjalan kecuali tujuannya diisi oleh bidak sendiri.
            if(moveSet.type == PieceData.PieceType.WhiteKnight 
                || moveSet.type == PieceData.PieceType.BlackKnight || isKing)
                rayGoing = true;

            if(toPos == -1) // tanda akhir dari ray dari suatu arah yang sama
            {
                rayGoing = true;
                continue;
            }
            else if(toPos == -2) // tanda attack move pada move berkutnya (hanya untuk pawn, diagonal attack)
            {
                canAttack = true;
                canMove = false;
                continue;
            }

            if(rayGoing || !canMove)
            {
                // move sederhana, memindahkan bidak ke kotak yang kosong.
                if(squares[toPos] == (int)PieceData.PieceType.None && canMove)
                    legalMoves.Add(new MoveData(squares[fromPos], fromPos, toPos));

                // tujuan dari move ini adalah ke tempat bidak lawan.
                else if((squares[toPos] > 0 && !asWhite) || (squares[toPos] < 0 && asWhite))
                {
                    if(canAttack) // jika bisa menyerang, maka tambahkan ke legal moves
                        legalMoves.Add(new MoveData(squares[fromPos], fromPos, toPos));
                    rayGoing = false; // tidak bisa jalan lebih jauh dari ini.
                    continue;
                }
                // test apakah enpassant move itu memungkinkan.
                else if(squares[toPos] == (int)PieceData.PieceType.None && canAttack 
                        && !canMove && enPassantSquare == toPos)
                    legalMoves.Add(new MoveData(squares[fromPos], fromPos, toPos, enPassantAttackSquare));
                // tujuan dari move ini adalah ke tempat bidak sendiri.
                else if((squares[toPos] > 0 && asWhite) || (squares[toPos] < 0 && !asWhite))
                    rayGoing = false;
            }
        }

        return legalMoves;
    }

    /// <summary>Final verification, cek apakah King mencoba menyerang yang mengakibatkan King tersebut diserang.</summary>
    /// <param name="testMove">Move yang ingin dicek.</param>
    /// <returns>Apakah move King ini legal?</returns>
    public bool kingVerifyLegality(MoveData testMove)
    {
        if(squares[testMove.from] != (int)PieceData.PieceType.WhiteKing 
            && squares[testMove.from] != (int)PieceData.PieceType.BlackKing)
            return true;
        else
        {
            move(testMove);
            List<MoveData> nextLegalMoves = findLegalMoves(whiteTurn);
            foreach(MoveData mov in nextLegalMoves)
            {
                if(mov.to == testMove.to)
                {
                    revert();
                    return false;
                }
            }
            revert();
            return true;
        }
    }

    /// <summary>Revert move yang terakhir dijalankan.</summary>
    public void revert()
    {
        if(moves.Count > 0)
        {
            MoveData lastMove = moves[moves.Count - 1];
            whiteTurn = !whiteTurn; // revert turn

            // revert penempatan piece
            if(lastMove.moveType == MoveData.MoveType.Castle) // revert castling move
            {
                // revert the king
                squares[lastMove.from] = squares[lastMove.to];
                squares[lastMove.to] = 0;

                // revert the rook
                squares[lastMove.castleRookFrom] = squares[lastMove.castleRookTo];
                squares[lastMove.castleRookTo] = 0;

                // revert flags
                if(whiteTurn)
                {
                    if(squares[lastMove.castleRookFrom] == whiteRooksStart[0])
                        whiteRightRookMoves--;
                    else if(squares[lastMove.castleRookFrom] == whiteRooksStart[1])
                        whiteLeftRookMoves--;
                    whiteKingMoves--;
                    whiteHasCastled = false;
                }
                else
                {
                    if(squares[lastMove.castleRookFrom] == blackRooksStart[0])
                        blackRightRookMoves--;
                    else if(squares[lastMove.castleRookFrom] == blackRooksStart[1])
                        blackLeftRookMoves--;
                    blackKingMoves--;
                    blackHasCastled = false;
                }
            }
            else if(lastMove.moveType == MoveData.MoveType.EnPassant) // revert enpassant move
            {
                // revert pawn
                squares[lastMove.from] = squares[lastMove.to];
                squares[lastMove.to] = 0;

                // revert piece yang diserang
                squares[lastMove.enPassantAttackSquare] = lastMove.capturedType;
            }
            else // revert simple move
            {
                if(lastMove.promoted) // jika move ini mem-promote pawn
                {
                    if(lastMove.from < halfwaySquare)
                        squares[lastMove.from] = (int)PieceData.PieceType.WhitePawn;
                    else
                        squares[lastMove.from] = (int)PieceData.PieceType.BlackPawn;                        
                }
                else
                    squares[lastMove.from] = squares[lastMove.to];
                
                squares[lastMove.to] = lastMove.capturedType;

                // revert variables untuk castling.
                if(squares[lastMove.from] == (int)PieceData.PieceType.WhiteRook)
                {
                    if(lastMove.from == whiteRooksStart[0])
                        whiteRightRookMoves--;
                    else if(lastMove.from == whiteRooksStart[1])
                        whiteLeftRookMoves--;
                }
                else if(squares[lastMove.from] == (int)PieceData.PieceType.BlackRook)
                {
                    if(lastMove.from == blackRooksStart[0])
                        blackRightRookMoves--;
                    else if(lastMove.from == blackRooksStart[1])
                        blackLeftRookMoves--;
                }
                else if(squares[lastMove.from] == (int)PieceData.PieceType.WhiteKing)
                    whiteKingMoves--;
                else if(squares[lastMove.from] == (int)PieceData.PieceType.BlackKing)
                    blackKingMoves--;
            }
            moves.RemoveAt(moves.Count - 1);
        }
    }
    #endregion

    /// <summary>Membuat papan baru sebagai duplikasi dari instance ini.</summary>
    /// <returns>Duplikasi Board dari instance ini.</returns>
    public Board duplicate()
    {
        Board dup = new Board(squares, allPossibleMoves);
        dup.moves = new List<MoveData>();
        foreach(MoveData move in this.moves)
            dup.moves.Add(move.duplicate());

        dup.whiteTurn = this.whiteTurn;

        dup.whiteHasCastled = this.whiteHasCastled;
        dup.blackHasCastled = this.blackHasCastled;

        dup.whiteLeftRookMoves = this.whiteLeftRookMoves;
        dup.whiteRightRookMoves = this.whiteRightRookMoves;

        dup.blackLeftRookMoves = this.blackLeftRookMoves;
        dup.blackRightRookMoves = this.blackRightRookMoves;

        dup.whiteKingMoves = this.whiteKingMoves;
        dup.blackKingMoves = this.blackKingMoves;

        dup.enPassantSquare = this.enPassantSquare;
        dup.enPassantAttackSquare = this.enPassantAttackSquare;

        return dup;
    }
}
