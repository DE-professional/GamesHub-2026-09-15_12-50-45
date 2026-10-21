using UnityEngine;

public class SudokuBoard : MonoBehaviour
{
    public int[,] answerKey;
    public int[,] startingPuzzle;
    public SudokuCell[,] cells;

    public void LoadPuzzle(int[,] answer, int[,] starting)
    {
        answerKey = answer;
        startingPuzzle = starting;
        InitBoard();
    }

    public void InitBoard()
    {
        cells = new SudokuCell[9, 9];
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                int val = startingPuzzle[r, c];
                bool given = val != 0;
                cells[r, c] = new SudokuCell(r, c, val, given);
            }
        }
    }

    public bool TrySetValue(int row, int col, int value)
    {
        SudokuCell cell = cells[row, col];
        if (cell.isSolved)
            return false;

        if (value != answerKey[row, col])
            return false;

        cell.value = value;
        cell.isSolved = true;
        return true;
    }

    public int GetSelectedValue(int row, int col)
    {
        return cells[row,col].value;
    }

    public bool IsBoardComplete()
    {
        for (int r = 0; r < 9; r++)
            for (int c = 0; c < 9; c++)
                if (!cells[r,c].isSolved)
                    return false;
        return true;
    }
}