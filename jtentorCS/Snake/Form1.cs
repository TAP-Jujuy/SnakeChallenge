using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/*
    Original from Muhammad Zain Ali
    https://code.msdn.microsoft.com/windowsapps/MySnakegame-0ced8739
    http://social.msdn.microsoft.com/Profile/muhammad_zain_ali
    Apache License Version 2.0, January 2004
    http://www.apache.org/licenses/ 
*/

namespace Snake
{
    public partial class Form1 : Form
    {
        private Random random = new Random();

        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        private bool inPause = false;

        private Queue<Direction> pathData = null;

        #region Some Useful Properties

        /// <summary>
        /// Maxium X position
        /// </summary>
        private int maxColPos 
        {
            get
            {
                return pbCanvas.Size.Width / Settings.Width;
            }
        }
        /// <summary>
        /// Maxium Y position
        /// </summary>
        private int maxRowPos
        {
            get
            {
                return pbCanvas.Size.Height / Settings.Height;
            }
        }

        
        /// <summary>
        /// Now going left
        /// </summary>
        private bool nowGoLeft
        {
            get
            {
                return Settings.direction == Direction.Left;
            }
        }
        /// <summary>
        /// Now going right
        /// </summary>
        private bool nowGoRight
        {
            get
            {
                return Settings.direction == Direction.Right;
            }
        }
        /// <summary>
        /// Now going up
        /// </summary>
        private bool nowGoUp
        {
            get
            {
                return Settings.direction == Direction.Up;
            }
        }
        /// <summary>
        /// Now going down
        /// </summary>
        private bool nowGoDown
        {
            get
            {
                return Settings.direction == Direction.Down;
            }
        }

        #endregion

        public Form1()
        {
            InitializeComponent();

            //Set settings to default
            new Settings();

            //Set game speed and start timer
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            //Start New game
            StartGame();
        }

        private void StartGame()
        {
            lblGameOver.Visible = false;

            //Set settings to default
            new Settings();

            //available = new bool[maxXPos, maxYPos];

            //Create new player object
            Snake.Clear();

            GeneratePath();

            Circle head = new Circle();
            //head.X = 5;
            //head.Y = 5;
            head.X = 0;
            head.Y = 0;

            Snake.Add(head);

            GenerateFood();

        }

        //Place random food object
        private void GenerateFood()
        {
            lblScore.Text = "Score: " + Settings.Score.ToString() +
                "\nRows: " + maxRowPos.ToString() + " Cols: " + maxColPos.ToString();

            lblScore.Visible = true;

            if (Settings.Score >= (maxRowPos * maxColPos))
            {
                Die(true);
                //pbCanvas.Update();
            }
            else
            {
                do
                {
                    food.X = random.Next(0, maxColPos);
                    food.Y = random.Next(0, maxRowPos);
                } while (isSnakeCollision(food.X, food.Y, 0));
            }
        }


        private void UpdateScreen(object sender, EventArgs e)
        {
            //Check for Game Over
            if (Settings.GameOver)
            {
                //Check if Enter is pressed
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Escape))
                {
                    Die();
                    return;
                }
                if (Input.KeyPressed(Keys.Space))
                {
                    inPause = !inPause;
                }
                else
                {
                    if (!inPause)
                    {

                        //NextDirection();
                        //SelectPosition();

                        BruteForceScanning();

                        MovePlayer();
                    }
                }

            }
            // mark the control to be displayed again
            pbCanvas.Invalidate();
        }



        private void NextDirection()
        {
            Circle head = Snake[0];

            bool canGoLeft, canGoRight, canGoUp, canGoDown;

            canGoLeft = (head.X > 0) && !isSnakeCollision(head.X - 1, head.Y);
            canGoRight = (head.X < maxColPos - 1) && !isSnakeCollision(head.X + 1, head.Y);
            canGoUp = (head.Y > 0) && !isSnakeCollision(head.X, head.Y - 1);
            canGoDown = (head.Y < maxRowPos - 1) && !isSnakeCollision(head.X, head.Y + 1);

            // shortest path
            if (head.X < food.X)
            {
                if (nowGoLeft)
                {
                    // buscar arriba o abajo
                    if (head.Y > food.Y)
                    {
                        if (canGoUp)
                        {
                            Settings.direction = Direction.Up;
                        }
                        return;
                    }
                    if (head.Y < food.Y)
                    {
                        if (canGoDown)
                        {
                            Settings.direction = Direction.Down;
                        }
                        return;
                    }
                    // no deberia estar aquí
                    return;
                }
                if (canGoRight)
                {
                    Settings.direction = Direction.Right;
                }
                return;
            }

            if (head.X > food.X)
            {
                if (nowGoRight)
                {
                    // buscar arriba o abajo
                    if (head.Y > food.Y)
                    {
                        if (canGoUp)
                        {
                            Settings.direction = Direction.Up;
                        }
                        return;
                    }
                    if (head.Y < food.Y)
                    {
                        if (canGoDown)
                        {
                            Settings.direction = Direction.Down;
                        }
                        return;
                    }
                    // no deberia estar aquí
                    return;
                }
                if (canGoLeft)
                {
                    Settings.direction = Direction.Left;
                }
                return;
            }

            // head.X == food.X

            if (head.Y < food.Y)
            {
                if (nowGoUp)
                {
                    // buscar izquierda y derecha
                    if (head.X > food.X)
                    {
                        if (canGoLeft)
                        {
                            Settings.direction = Direction.Left;
                        }
                        return;
                    }
                    if (head.X < food.X)
                    {
                        if (canGoRight)
                        {
                            Settings.direction = Direction.Right;
                        }
                        return;
                    }
                    // no deberia estar aquí
                    return;
                }
                if (canGoDown)
                {
                    Settings.direction = Direction.Down;
                }
                return;
            }

            if (head.Y > food.Y)
            {
                if (nowGoDown)
                {
                    // buscar izquierda y derecha
                    if (head.X > food.X)
                    {
                        if (canGoLeft)
                        {
                            Settings.direction = Direction.Left;
                        }
                        return;
                    }
                    if (head.X < food.X)
                    {
                        if (canGoRight)
                        {
                            Settings.direction = Direction.Right;
                        }
                        return;
                    }
                    // no deberia estar aquí
                    return;
                }
                if (canGoUp)
                {
                    Settings.direction = Direction.Up;
                }
                return;
            }

        }


        private void SelectPosition()
        {

            bool canGoLeft, canGoRight, canGoUp, canGoDown;
            canGoLeft = (Snake[0].X > 0) && !isSnakeCollision(Snake[0].X - 1, Snake[0].Y);
            canGoRight = (Snake[0].X < maxColPos - 1) && !isSnakeCollision(Snake[0].X + 1, Snake[0].Y);
            canGoUp = (Snake[0].Y > 0) && !isSnakeCollision(Snake[0].X, Snake[0].Y - 1);
            canGoDown = (Snake[0].Y < maxRowPos - 1) && !isSnakeCollision(Snake[0].X, Snake[0].Y + 1);

            if (nowGoLeft && !canGoLeft)
            {
                if (canGoUp) { 
                    Settings.direction = Direction.Up;
                }
                else
                {
                    if (canGoDown)
                    {
                        Settings.direction = Direction.Down;
                    }
                }
                return;
            }

            if (nowGoUp && !canGoUp)
            {
                if (canGoRight)
                {
                    Settings.direction = Direction.Right;
                }
                else
                {
                    if (canGoLeft)
                    {
                        Settings.direction = Direction.Left;
                    }
                }
                return;
            }

            if (nowGoRight && !canGoRight)
            {
                if (canGoDown)
                {
                    Settings.direction = Direction.Down;
                }
                else
                {
                    if (canGoUp)
                    {
                        Settings.direction = Direction.Up;
                    }
                }
                return;
            }

            if (nowGoDown && !canGoDown)
            {
                if (canGoLeft)
                {
                    Settings.direction = Direction.Left;
                }
                else
                {
                    if (canGoRight)
                    {
                        Settings.direction = Direction.Right;
                    }
                }
                return;
            }



        }


        private void GeneratePath()
        {
            //pathData = new Direction[maxRowPos * maxColPos];
            pathData = new Queue<Direction>(maxRowPos * maxColPos);

            if (((maxRowPos % 2) == 0) && ((maxColPos % 2) == 0))
            {
                // number of pair rows & pair cols

                //pathData[pathIndex(0, 0)] = Direction.Right;
                pathData.Enqueue(Direction.Right);
                for (int i = 0; i < maxRowPos; i += 2)
                {
                    for (int j = 1; j < maxColPos - 1; ++j)
                    {
                        //pathData[pathIndex(i, j)] = Direction.Right;
                        pathData.Enqueue(Direction.Right);
                    }
                    //pathData[pathIndex(i, maxColPos - 1)] = Direction.Down;
                    pathData.Enqueue(Direction.Down);

                    for (int j = maxColPos - 1; j > 1; --j)
                    {
                        //pathData[pathIndex(i + 1, j)] = Direction.Left;
                        pathData.Enqueue(Direction.Left);
                    }

                    if ((i + 1) < (maxRowPos - 1))
                    {
                        //pathData[pathIndex(i + 1, 1)] = Direction.Down;
                        pathData.Enqueue(Direction.Down);
                    }
                }

                //pathData[pathIndex(maxRowPos - 1, 1)] = Direction.Left;
                pathData.Enqueue(Direction.Left);

                for (int i = maxRowPos - 1; i > 0; --i)
                {
                    //pathData[pathIndex(i, 0)] = Direction.Up;
                    pathData.Enqueue(Direction.Up);
                }
            }

        }

        private void BruteForceScanning()
        {
            Settings.direction = pathData.Dequeue();
            pathData.Enqueue(Settings.direction);
        }

        private void GoHome()
        {

        }

        private void manualSelectPosition()
        {
            if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                Settings.direction = Direction.Right;
            else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                Settings.direction = Direction.Left;
            else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                Settings.direction = Direction.Up;
            else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                Settings.direction = Direction.Down;
        }

        private bool isSnakeCollision(int X, int Y, int start = 1)
        {

            for (int j = start; j < Snake.Count; j++)
            {
                if ((X == Snake[j].X) && (Y == Snake[j].Y))
                {
                    return true;
                }
            }
            return false;
        }

        private void MovePlayer()
        {
            for(int i = Snake.Count -1; i >= 0; i--)
            {
                //Move head
                if(i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[0].X++;
                            break;
                        case Direction.Left:
                            Snake[0].X--;
                            break;
                        case Direction.Up:
                            Snake[0].Y--;
                            break;
                        case Direction.Down:
                            Snake[0].Y++;
                            break;
                    }

                    //Detect collission with game borders.
                    if (Snake[0].X < 0 || Snake[0].Y < 0 || Snake[0].X >= maxColPos || Snake[0].Y >= maxRowPos)
                    {
                        Die();
                    }

                    //Detect collission with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if(Snake[0].X == Snake[j].X && Snake[0].Y == Snake[j].Y )
                        {
                            Die();
                        }
                    }

                    //Detect collision with food piece
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();
                    }
                }
                else
                {
                    //Move body
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }
        private void Eat()
        {
            //Add circle to body
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };
            Snake.Add(circle);

            //Update Score
            Settings.Score += Settings.Points;

            GenerateFood();
        }

        private void Die(bool win = false)
        {
            Settings.GameOver = true;
            Settings.Win = win;
        }

        private void ShowGameBoard(Graphics canvas)
        {

            //Brush brush = Brushes.White;
            //for (int i = 0; i < maxRowPos; ++i)
            //{
            //    for (int j = 0; j < maxColPos; ++j)
            //    {
            //        switch (pathData[pathIndex(i, j)])
            //        {
            //            case Direction.Right: brush = Brushes.Black; break;
            //            case Direction.Down: brush = Brushes.Gray; break;
            //            case Direction.Left: brush = Brushes.White; break;
            //            case Direction.Up: brush = Brushes.Yellow; break;
            //        }

            //        canvas.FillEllipse(brush,
            //            new Rectangle(j * Settings.Width,
            //                            i * Settings.Height,
            //                            Settings.Width,
            //                            Settings.Height));
            //    }
            //}

            //Draw Food
            canvas.FillEllipse(Brushes.Red,
                new Rectangle(food.X * Settings.Width,
                                food.Y * Settings.Height,
                                Settings.Width,
                                Settings.Height));

            //Draw snake
            for (int i = 0; i < Snake.Count; i++)
            {
                //Draw snake
                canvas.FillEllipse((i == 0) ? Brushes.Purple : Brushes.Blue,
                    new Rectangle(Snake[i].X * Settings.Width,
                                    Snake[i].Y * Settings.Height,
                                    Settings.Width,
                                    Settings.Height));
            }

        }



        #region Windows Form Methods

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            ShowGameBoard(e.Graphics);
            if (Settings.GameOver)
            {
                if (!Settings.Win) { 
                    lblGameOver.Text = "Game over !!! \nFinal Score: " + Settings.Score + "\nPress Enter For New Game";
                }
                else
                {
                    lblGameOver.Text = "G A N A S T E !!! \nPuntaje final: " + Settings.Score + "\nPress Enter For New Game";
                }
                lblGameOver.Visible = true;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            StartGame();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("My Name Is Muhammad Zain Ali,I Am Doing Software Engineering If You need Help Then Contact me\nhttp://social.msdn.microsoft.com/Profile/muhammad_zain_ali\nhttps://www.facebook.com/xain.ch1067", "About Me", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

    }
}
