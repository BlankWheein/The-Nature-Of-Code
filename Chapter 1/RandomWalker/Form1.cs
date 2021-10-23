using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomWalker
{
    public partial class form : Form
    {
        public int cols;
        public int rows;
        Random rnd = new();
        public int Resolution = 1;
        Walker[] walkers;
        BackgroundWorker bgWorker = new();
        Bitmap drawingSurface;
        Graphics? GFX;
        Cell[,] Cells;
        Cell[,] OldCells;
        public form()
        {
            InitializeComponent();
            cols = Size.Width / Resolution;
            rows = Size.Height / Resolution;
            canvas.Size = Size;
            walkers = new Walker[1000];
            for (int i = 0; i < walkers.Length; i++)
            {
                walkers[i] = new(cols / 2, rows / 2, cols, rows);
            }
            Cells = new Cell[cols, rows];
            OldCells = new Cell[cols, rows];
            for (int i = 0; i < cols; i++)
                for (int j = 0; j < rows; j++)
                {
                    Cells[i, j] = new(i, j);
                    OldCells[i, j] = new(i, j);
                }
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += DoWorkHandler;
            drawingSurface = new Bitmap(Size.Width, Size.Height);
            GFX = Graphics.FromImage(drawingSurface);
            bgWorker.RunWorkerAsync();
            button1.Text = "Stop";
        }

        private void DoWorkHandler(object? sender, DoWorkEventArgs e)
        {
            while (true)
            {
                for (int i = 0; i < cols; i++)
                    for (int j = 0; j < rows; j++)
                    {
                        //Cells[i, j].State = 0;
                    }
                for (int i = 0; i < 1000; i++)
                {
                    for (int j = 0; j < walkers.Length; j++)
                    {
                        walkers[j].Walk8();
                        walkers[j].draw(Cells);
                    }
                }
                
                
                draw(sender, e);

                if (bgWorker.CancellationPending)
                {
                    break;
                }
            }
        }

        private void draw(object? sender, DoWorkEventArgs? e)
        {
            Bitmap _drawingSurface = new Bitmap(Size.Width, Size.Height);
            GFX = Graphics.FromImage(_drawingSurface);
            //GFX.FillRectangle(Brushes.White, 0, 0, Size.Width, Size.Height);
            for (int i = 0; i < cols; i++)
                for (int j = 0; j < rows; j++)
                {
                    if (Cells[i, j].State != OldCells[i, j].State)
                        GFX.FillRectangle(Cells[i, j].GetColor(), i * Resolution, j * Resolution, Resolution, Resolution);
                }
            canvas.Image = _drawingSurface;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bgWorker.IsBusy) { bgWorker.CancelAsync(); button1.Text = "Start"; return; }
            bgWorker.RunWorkerAsync();
            button1.Text = "Stop";
        }

        private void form_Load(object sender, EventArgs e){}
    }

    public class Cell
    {
        public int State;
        public int i, j;
        public Cell(int i, int j)
        {
            this.j = j;
            this.i = i;
            State = 0;
        }
        public Cell(int i, int j, int State) : this(i, j)
        {
            this.State = State;
        }
        public Brush GetColor()
        {
            if (State == 0)
            {
                return Brushes.White;
            }
            else
            {
                return Brushes.Black;
            }
        }
    }
    public class Walker
    {
        Random rnd = new Random();
        int x;
        int y;
        int Height;
        int Width;
        public Walker(int x, int y, int Width, int Height)
        {
            this.x = x;
            this.y = y;
            this.Width = Width;
            this.Height = Height;
        }
        public void Walk8()
        {
            y += rnd.Next(-1, 2);
            x += rnd.Next(-1, 2);
            Constrain();
        }
        private void Constrain()
        {
            if (y < 0)
                y = 0;
            else if (y >= Height)
                y = Height-1;
            if (x < 0)
                x = 0;
            else if (x >= Width)
                x = Width-1;
        }
        public void Walk4()
        {
            int flip1 = rnd.Next(0, 2);
            int flip2 = rnd.Next(0, 2);
            int state = flip1 == flip2 ? flip1 == 0 ? 1 : 4 : flip1 == 1 ? 3 : 2;
            switch (state)
            {
                case 1:
                    y -= 1;
                    break;
                case 2:
                    x += 1;
                    break;
                case 3:
                    y += 1;
                    break;
                case 4:
                    x += -1;
                    break;
            }
            Constrain();
        }
        public void draw(Cell[,] cells)
        {
            cells[x, y].State = 1;
        }
    }
}