using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QMazeSolver
{
    public enum CellState
    {
        BACKGROUND,
        OBSTACLE,
        START,
        FINISH,
        BONUS
    }

    public partial class Form1 : Form
    {
        const int nRows = 20;
        const int nColumns = 20;
        const int cellSize = 40;

        CellState[,] cellStates;

        bool startSet = false;
        bool finishSet = false;
        bool mazeSolved = false;

        TableLayoutPanel grid;

        double[][] Q = new double[nRows * nColumns][];
        double[][] R = new double[nRows * nColumns][];

        void WalkSolution()
        {
            int nStartCell = 0;
            int nFinishCell = 0;

            for (int i = 0; i < nRows; ++i)
            {
                for (int j = 0; j < nColumns; ++j)
                {
                    if (cellStates[i, j] == CellState.START)
                    {
                        nStartCell = i * nColumns + j;
                    }
                    else if (cellStates[i, j] == CellState.FINISH)
                    {
                        nFinishCell = i * nColumns + j;
                    }
                }
            }

            try
            {
                List<int> steps = QLearning.Walk(nStartCell, nFinishCell, Q, R);
                for (int s = 1; s < steps.Count; ++s)
                {
                    int k = steps[s];
                    int i = k / nColumns;
                    int j = k % nColumns;

                    Button button = (Button)grid.GetControlFromPosition(j, i);
                    button.BackColor = Color.Yellow;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Nu se poate ajunge la acea destinatie");
            }
        }

        private void SolveMaze()
        {
        int nCells = nRows * nColumns;
        for (int i = 0; i < nCells; ++i)
        {
            R[i] = new double[nCells];
            for (int j = 0; j < nCells; ++j)
            {
                R[i][j] = double.MinValue;
            }
        }

        int nFinishCell = 0;
        for (int i = 0; i < nRows; ++i)
        {
            for (int j = 0; j < nColumns; ++j)
            {
                int nCell = i * nColumns + j;

                if (cellStates[i, j] == CellState.FINISH)
                    nFinishCell = nCell;

                if (cellStates[i, j] == CellState.BACKGROUND
                    || cellStates[i, j] == CellState.START)
                {
                    List<Point> v = new List<Point>();
                    if (j < nColumns - 1)
                        v.Add(new Point(j + 1, i));
                    if (j > 0)
                        v.Add(new Point(j - 1, i));
                    if (i < nRows - 1)
                        v.Add(new Point(j, i + 1));
                    if (i > 0)
                        v.Add(new Point(j, i - 1));

                    for (int k = 0; k < v.Count; ++k)
                    {
                        int vCell = v[k].Y * nColumns + v[k].X;

                        switch (cellStates[v[k].Y, v[k].X])
                        {
                            case CellState.BACKGROUND:
                            case CellState.START:
                                R[nCell][vCell] = -0.01;
                                break;
                            case CellState.OBSTACLE:
                                R[nCell][vCell] = double.MinValue;
                                break;
                            case CellState.FINISH:
                                R[nCell][vCell] = 1000.0;
                                break;
                            case CellState.BONUS:
                                R[nCell][vCell] = 1.0;
                                break;
                        }
                    }
                }
            }
        }

        R[nFinishCell][nFinishCell] = 0.0;
            QLearning.Solve(R, Q, nFinishCell, 0.5, 0.5, 1000, 200000);

            WalkSolution();
        }

        private void ResetMaze()
        {
            for (int i = 0; i < nRows; ++i)
            {
                for (int j = 0; j < nColumns; ++j)
                {
                    Button button = (Button)grid.GetControlFromPosition(j, i);
                    button.BackColor = Color.White;
                    cellStates[i, j] = CellState.BACKGROUND;
                }
            }

            finishSet = false;
            mazeSolved = false;
            startSet = false;
        }

        void KeyCallback(object sender, KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.Space)
            {
                if (startSet && finishSet)
                {
                    if (!mazeSolved)
                    {
                        SolveMaze();
                        mazeSolved = true;
                    }
                    else
                    {
                        ResetMaze();
                        mazeSolved = false;
                    }
                }
                else
                {
                    MessageBox.Show("Start cell and end cell not set");
                }
            }
        }

        void ButtonClick(object sender, MouseEventArgs mouseEvent)
        {
            Button button = (Button)sender;

            int index = (int)button.Tag;
            int row = index / nColumns;
            int column = index % nColumns;

            if (mouseEvent.Button == MouseButtons.Left)
            {
                if (mazeSolved)
                    return;

                if (cellStates[row, column] == CellState.BACKGROUND)
                {
                    button.BackColor = Color.Black;
                    cellStates[row, column] = CellState.OBSTACLE;
                }
                else if (cellStates[row, column] != CellState.BACKGROUND)
                {
                    if (cellStates[row, column] == CellState.START)
                        startSet = false;

                    if (cellStates[row, column] == CellState.FINISH)
                        finishSet = false;

                    button.BackColor = Color.White;
                    cellStates[row, column] = CellState.BACKGROUND;
                }
            }
            else if (mouseEvent.Button == MouseButtons.Right)
            {
                if (cellStates[row, column] == CellState.OBSTACLE ||
                cellStates[row, column] == CellState.START ||
                cellStates[row, column] == CellState.FINISH)
                    return;

                if (!startSet)
                {
                    cellStates[row, column] = CellState.START;
                    button.BackColor = Color.Red;
                    startSet = true;
                }
                else
                {
                    if (finishSet)
                    {
                        for (int i = 0; i < nRows; ++i)
                        {
                            for (int j = 0; j < nColumns; ++j)
                            {
                                Button btn = (Button)grid.GetControlFromPosition(j, i);

                                if (cellStates[i, j] == CellState.START)
                                {
                                    cellStates[i, j] = CellState.BACKGROUND;
                                    btn.BackColor = Color.White;
                                }

                                if (btn.BackColor == Color.Yellow)
                                    btn.BackColor = Color.White;
                            }
                        }

                        cellStates[row, column] = CellState.START;
                        button.BackColor = Color.Red;

                        if (mazeSolved)
                            WalkSolution();
                    }
                    else
                    {
                        if (cellStates[row, column] != CellState.START)
                        {
                            cellStates[row, column] = CellState.FINISH;
                            button.BackColor = Color.Green;
                            finishSet = true;
                        }
                    }
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(nRows * cellSize, nColumns * cellSize);

            FormBorderStyle = FormBorderStyle.FixedSingle;

            KeyUp += KeyCallback;

            cellStates = new CellState[nRows, nColumns];

            grid = new TableLayoutPanel();
            grid.RowCount = nRows;
            grid.ColumnCount = nColumns;

            grid.BackColor = Color.Black;

            grid.Size = Size;

            Controls.Add(grid);

            for (int i = 0; i < nRows; ++i)
            {
                for (int j = 0; j < nColumns; ++j)
                {
                    cellStates[i, j] = CellState.BACKGROUND;

                    Button button = new Button();
                    button.BackColor = Color.White;
                    button.KeyDown += KeyCallback;
                    button.Text = (i * nColumns + j).ToString();

                    button.Width = cellSize;
                    button.Height = cellSize;
                    button.Margin = Padding.Empty;
                    button.Padding = Padding.Empty;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 1;
                    button.MouseDown += ButtonClick;
                    button.Tag = i * nColumns + j;

                    TableLayoutPanelCellPosition cellPos = new TableLayoutPanelCellPosition(j, i);
                    grid.SetCellPosition(button, cellPos);

                    grid.Controls.Add(button);
                }
            }

            int nCells = nRows * nColumns;
            for (int i = 0; i < nCells; ++i)
            {
                Q[i] = new double[nCells];
            }
        }
    }
}
