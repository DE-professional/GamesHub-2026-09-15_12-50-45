using System.Collections.Generic;

public static class SudokuSolver
{
    public static int CountSolutions(int[,] grid, int limit = 2)
    {
        int[,] copy = (int[,])grid.Clone();
        int count = 0;
        Solve(copy, ref count, limit);
        return count;
    }

    private static bool Solve(int[,] grid, ref int count, int limit)
    {
        int row = -1, col = -1;
        if (!FindEmptyCell(grid, out row, out col))
        {
            count++;
            return count >= limit;
        }

        for (int num = 1; num <= 9; num++)
        {
            if (IsValidPlacement(grid, row, col, num))
            {
                grid[row, col] = num;

                if (Solve(grid, ref count, limit))
                {
                    grid[row, col] = 0;
                    return true;
                }

                grid[row, col] = 0;
            }
        }

        return false;
    }

    private static bool FindEmptyCell(int[,] grid, out int row, out int col)
    {
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                if (grid[r, c] == 0)
                {
                    row = r;
                    col = c;
                    return true;
                }
            }
        }
        row = -1;
        col = -1;
        return false;
    }

    public static bool IsValidPlacement(int[,] grid, int row, int col, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (i != col && grid[row, i] == value) return false;
            if (i != row && grid[i, col] == value) return false;
        }

        int boxRow = (row / 3) * 3;
        int boxCol = (col / 3) * 3;
        for (int r = boxRow; r < boxRow + 3; r++)
            for (int c = boxCol; c < boxCol + 3; c++)
                if ((r != row || c != col) && grid[r, c] == value)
                    return false;

        return true;
    }
}