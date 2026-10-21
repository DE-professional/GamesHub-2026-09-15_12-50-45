using System.Collections.Generic;

public class SudokuCell
{
    public int row;
    public int col;
    public int value;
    public bool isSolved;
    public HashSet<int> notes = new HashSet<int>();

    public SudokuCell(int row, int col, int value, bool isSolved)
    {
        this.row = row;
        this.col = col;
        this.value = value;
        this.isSolved = isSolved;
    }

    public void ToggleNote(int number)
    {
        if (notes.Contains(number))
            notes.Remove(number);
        else
            notes.Add(number);
    }

    public void ClearNotes()
    {
        notes.Clear();
    }
}