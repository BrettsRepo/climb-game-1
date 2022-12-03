using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fun1
{
    public partial class Form1 : Form
    {
        int MaxFormSize = 600;
        int GravitySpeed = 15;
        int PlayerSpeed = 7;
        int PlayerSize = 20;
        int HeartBeatInterval = 25;
        int JumpUntilHeight;
        bool GoLeft, GoRight, GoUp, GoDown, Climbing, TouchingWall, Grounded, Jumping;
        bool CanMove;
        Label LabelInfo = new Label();
        PictureBox Player = new PictureBox();
        int MaxClimbCount = 29;
        int ClimbCount = 0;
        int WallCount = 0;
        PictureBox[] Climb = new PictureBox[31];

        PictureBox[] Wall = new PictureBox[31];

        PictureBox[] Enemy = new PictureBox[31];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ResetGame();
        }

        public void GameOver()
        {
            Heartbeat.Stop();
//            Controls.Remove(Wall[]);

            // create GAME OVER label
            Label LabelGameOver = new Label();
            LabelGameOver.Text = "GAME OVER";
            LabelGameOver.Location = new Point(100, 100);
            LabelGameOver.Font = new Font("Arial", 28);
            LabelGameOver.AutoSize = true;
            Controls.Add(LabelGameOver);

        }

        public void ResetGame()
        {

            // set max form size
            MaximumSize = new Size(MaxFormSize, MaxFormSize);
            MinimumSize = new Size(MaxFormSize, MaxFormSize);

            // create debug label
            LabelInfo.Text = "DEBUG";
            LabelInfo.Location = new Point(300, 200);
            LabelInfo.Font = new Font("Arial", 8);
            LabelInfo.AutoSize = true;
            Controls.Add(LabelInfo);
            
            // create PLAYER object
            Player.Size = new Size(PlayerSize, PlayerSize);
            Player.Location = new Point(100, 100);
            Player.BackColor = Color.Transparent;
            Player.Image = Properties.Resources.chickenR1;
            Player.SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.Add(Player);

            // create ENEMY[1] object
            Enemy[1] = new PictureBox();
            Enemy[1].Location = new Point(200, 200);
            Enemy[1].BackColor = Color.Black;
            Enemy[1].SizeMode = PictureBoxSizeMode.StretchImage;
            Enemy[1].Size = new Size(20, 20);
            Controls.AddRange(Enemy);


            // create WALL[1] object
            WallCount++;
            Wall[WallCount] = new PictureBox();
            Wall[WallCount].Size = new Size(100, 40);
            Wall[WallCount].Location = new Point(200, 400);
            Wall[WallCount].BackColor = Color.Transparent;
            Wall[WallCount].Image = Properties.Resources.brick2;
            Wall[WallCount].SizeMode = PictureBoxSizeMode.StretchImage;
            Controls.AddRange(Wall);
            


            // create WALL[2-12]

            for (int LoopCounter = 0; LoopCounter <= 12; LoopCounter++)
            {
                WallCount++;
                Point point1 = new Point(LoopCounter * 50, 410 - LoopCounter * 0);
                Size size1 = new Size(50, 20);
                ConstructPlayField(0, point1, size1, WallCount);
            }


          


            // create climb objects
            for (int LoopCounter = 1; LoopCounter <= MaxClimbCount; LoopCounter++)
            {
                if (LoopCounter <= 15)
                {
                    Climb[LoopCounter] = new PictureBox()
                    {
                        Image = Properties.Resources.vine,
                        Size = new Size(40, 16),
                        Location = new Point(LoopCounter * 40 - 40, 0),
                        BackColor = Color.Transparent,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    Climb[LoopCounter].Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else
                {
                    Climb[LoopCounter] = new PictureBox()
                    {
                        Image = Properties.Resources.ladder,
                        Size = new Size(16, 40),
                        Location = new Point(0, (LoopCounter - 15) * 40 - 40 + Climb[1].Height),
                        BackColor = Color.Transparent,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                }
            }

            Controls.AddRange(Climb);
            LabelInfo.SendToBack();

            // set heartbeat speed
            Heartbeat.Interval = HeartBeatInterval;

            // start heartbeat
            Heartbeat.Start();

        }

        private void Gravity()
        {
            // check player position, induce gravity
            if (Player.Top + Player.Height + GravitySpeed <= Height - GravitySpeed - Player.Height && !Climbing && !GoUp && !Grounded && !Jumping) { Player.Top += GravitySpeed; }

            // climb launch bug workarund
            if (!Climbing) { GoDown = false; GoUp = false; }
        }
        private void PlayerMovement()
        {
            CanMove = true;

            // Player going LEFT
            if (GoLeft && Player.Left > 0)
            {
                for (int LoopCounter = 1; LoopCounter <= 2; LoopCounter++)
                {
                    if (TouchingWall && Math.Abs(Player.Left - Wall[LoopCounter].Right) <= PlayerSpeed)
                    {
                        CanMove = false;
                    }
                }
                if (CanMove) { Player.Left -= PlayerSpeed; }
            }
            // Player going RIGHT
            if (GoRight && Player.Right + Player.Width <= Width)
            {
                for (int LoopCounter = 1; LoopCounter <= 2; LoopCounter++)
                {
                    if (TouchingWall && Math.Abs(Player.Right - Wall[LoopCounter].Left) <= PlayerSpeed)
                    {
                        CanMove = false;
                    }
                }
                if (CanMove) { Player.Left += PlayerSpeed; }
            }
            if (GoUp && Player.Top > 0)
            {
                for (int LoopCounter = 1; LoopCounter <= 2; LoopCounter++)
                {
                    if (TouchingWall && Math.Abs(Player.Top - Wall[LoopCounter].Bottom) <= PlayerSpeed)
                    {
                        CanMove = false;
                    }
                }
                if (CanMove) { Player.Top -= PlayerSpeed; }
            }

            if (GoDown && Player.Bottom <= Height - Player.Height - PlayerSpeed)
            {
                for (int LoopCounter = 1; LoopCounter <= 2; LoopCounter++)
                {
                    if (TouchingWall && Math.Abs(Player.Bottom - Wall[LoopCounter].Top) <= PlayerSpeed)
                    {
                        CanMove = false;
                    }
                }
                if (CanMove) { Player.Top += PlayerSpeed; }
            }


        }
        private void LabelInfoUpdate()
        {
            if (GoLeft || GoRight || GoUp || GoDown)
            {
                LabelInfo.Text = "Top: " + (Player.Top).ToString() + " Left: " + (Player.Left).ToString() + "\r\nRight: " + Player.Right.ToString() + " Bottom: " +
                Player.Bottom.ToString() + "\r\nHeight: " + Player.Height.ToString() + " Width: " + Player.Width.ToString() + "\r\nSpeed: " +
                PlayerSpeed.ToString() + " Gravity: " + GravitySpeed.ToString() + "\r\nClimbing: " + Climbing.ToString() +
                " TouchingWall: " + TouchingWall.ToString() + "\r\nGrounded: " + Grounded.ToString() + " CanMove: " + CanMove.ToString();
            }
        }



        public void CollisionCheck_PlayerAndClimb()
        {
            Climbing = false;
            for (int LoopCounter = 1; LoopCounter <= MaxClimbCount; LoopCounter++)
            {
                if (Player.Bounds.IntersectsWith(Climb[LoopCounter].Bounds)) 
                {
                    Climbing = true;
                }
            }
        }
        public void CollisionCheck_PlayerAndWall()
        {
            Grounded = false;
            TouchingWall = false;
            for (int LoopCounter = 1; LoopCounter <= WallCount; LoopCounter++)
            {
                if (Player.Bounds.IntersectsWith(Wall[LoopCounter].Bounds))
                {
                    if (Math.Abs(Player.Bottom - Wall[LoopCounter].Top) <= PlayerSpeed)
                    {
                        Grounded = true;
                    }
                    else
                    {
                        TouchingWall = true;
                    }
                }
            }
        }
        private void Jump()
        {
            if (Jumping)
            {
                Player.Top -= PlayerSpeed;
            }
            if (Player.Top <= JumpUntilHeight)
            {
                Jumping = false;
            }
        }
        private void Heartbeat_Tick(object sender, EventArgs e)
        {

            if (Player.Bounds.IntersectsWith(Enemy[1].Bounds)) { GameOver(); }

            // gravity
            Gravity();

           

            // update the info label for debug help
            LabelInfoUpdate();

            // collision check with climb structures
            CollisionCheck_PlayerAndClimb();

            // collision check with walls
            CollisionCheck_PlayerAndWall();

            // player movement
            PlayerMovement();

            Jump();

        }

        private void ConstructPlayField(int type, Point point1, Size size1, int ArrayPosition)
        {
            // type, position, size, image, currentArrayCount
            // type list: 0=wall, 1=climb
            //int type = 0;
            //Point p1 = new Point(-10,550);
            //Size s1 = new Size(600, 40);
            //String ImageName = "log";
            int CurrentArrayCount = ArrayPosition;
            switch (type)
            {
                case 0:
                    // create a wall
                    Wall[CurrentArrayCount] = new PictureBox
                    {
                        Size = size1,
                        Location = point1,
                        Image = Properties.Resources.log,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    Controls.Add(Wall[CurrentArrayCount]);

                    break;
                case 1:
                    // create a climb

                    break;
            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && !GoRight) { GoLeft = true; }
            if (e.KeyCode == Keys.D && !GoLeft) { GoRight = true; }
            if (e.KeyCode == Keys.W && !GoDown && Climbing) { GoUp = true; }
            if (e.KeyCode == Keys.S && !GoUp && Climbing) { GoDown = true; }
            
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A) { GoLeft = false; }
            if (e.KeyCode == Keys.D) { GoRight = false; }
            if (e.KeyCode == Keys.W) { GoUp = false; }
            if (e.KeyCode == Keys.S) { GoDown = false; }
            
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Jump Keypress
            if (e.KeyChar == ' ' && !Jumping && (Grounded || Climbing))
            {
                JumpUntilHeight = Player.Top - PlayerSpeed * 11;
                Jumping = true;
            }
        }
    }
}
