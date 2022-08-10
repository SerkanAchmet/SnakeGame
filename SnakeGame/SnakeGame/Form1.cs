using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace SnakeGame
{
    public partial class Form1 : Form
    {

        private List<circle> Snake = new List<circle>();
        private circle food = new circle();
        int maxWidth;
        int maxHeight;

        int score;
        int hightScore;

        Random rand = new Random();


        bool goLeft, goRight, goDown, goUp;


        public Form1()
        {
            InitializeComponent();

            new Settings();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Settings.directions != "Down")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Settings.directions != "up")
            {
                goDown = true;
            }

           
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }


        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {
            Label caption = new Label();
            caption.Text = "I score: " + score + " and my Highscore is " + hightScore + " on the SnakeGame";
            caption.Font = new Font("Arial", 14, FontStyle.Bold);
            caption.ForeColor = Color.DarkTurquoise;
            caption.AutoSize = false;
            caption.Width  = picCanvas.Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(caption);
            
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "SnakeGame Snap";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(caption);

            }



        }

        private void GameTimer(object sender, EventArgs e)
        {
            //directions
            if(goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }
            //end of directions

            for (int i = Snake.Count -1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch(Settings.directions)
                    {
                        case "left":
                            Snake[i].x--;
                            break;
                        case "right":
                            Snake[i].x++;
                            break;
                        case "down":
                            Snake[i].y++;
                            break;
                        case "up":
                            Snake[i].y--;
                            break; 

                    }
                    if (Snake[i].x < 0)
                    {
                        Snake[i].x = maxWidth;
                    }
                    if (Snake[i].x > maxWidth)
                    {
                        Snake[i].x = 0;
                    }
                    if (Snake[i].y < 0)
                    {
                        Snake[i].y = maxHeight;
                    }
                    if (Snake[i].y > maxHeight)
                    {
                        Snake[i].y = 0;
                    }

                    if (Snake[i].x == food.x && Snake[i].y == food.y )
                    {
                        eatFood();
                    }

                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].x == Snake[j].x && Snake[i].y == Snake[j].y)
                        {
                            GameOver();
                        }
                    }
                }
                else
                {
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }

            }

            picCanvas.Invalidate(); 
    }

        private void UpdateGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColor;

            for (int i = 0; i< Snake.Count; i++ )
            {
                if (i == 0)
                {
                    snakeColor = Brushes.Red;
                }
                else
                {
                    snakeColor = Brushes.Yellow;
                }

                canvas.FillEllipse(snakeColor, new Rectangle
                    (
                    Snake[i].x * Settings.Width,
                    Snake[i].y * Settings.Height,
                    Settings.Width,Settings.Height
                    ));
            }
            canvas.FillEllipse(Brushes.Blue, new Rectangle
            (
            food.x * Settings.Width,
            food.y * Settings.Height,
            Settings.Width, Settings.Height
            ));
        }

        private void txtScore_Click(object sender, EventArgs e)
        {

        }

        private void RestartGame()
        {
            maxWidth = picCanvas.Width / Settings.Width - 1;
            maxHeight = picCanvas.Height / Settings.Height - 1;

            Snake.Clear();

            startButton.Enabled = false;

            snapButton.Enabled = false;
            score = 0;
            txtScore.Text = "Score: " + score;

            circle head = new circle { x = 10, y = 5 };
            Snake.Add(head);

            for(int i = 0; i < 10; i++)
            {
                circle body = new circle ();
                Snake.Add(body);
            }

            food = new circle { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight) };

            gameTimer.Start();

        }

        private void eatFood()
        {
            score += 1;

            txtScore.Text = "Score: " + score;

            circle body = new circle
            {
                x = Snake[Snake.Count - 1].x,
                y = Snake[Snake.Count - 1].y,
            };

            Snake.Add(body);

            food = new circle { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight) };
        }
        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            snapButton.Enabled = true;

            if (score > hightScore)
            {
                hightScore = score;

                txtHighScore.Text = "High Score: " + Environment.NewLine + hightScore;
                txtHighScore.ForeColor = Color.Red;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
        }

    }
}
