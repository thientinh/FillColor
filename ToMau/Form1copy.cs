using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace ToMau
{
    public partial class Form1 : Form
    {
        Color colorFill = Color.Red;
        public Bitmap bgBitmap;
        public Bitmap colorBitmap;
        
        public Form1()
        {
            InitializeComponent();
        }
     
       

        private void Form1_Load(object sender, EventArgs e)
        {
            bgBitmap = new Bitmap(pictureBox1.Image);
            colorBitmap = new Bitmap(pictureBox2.Image);
            colorFill = Color.Red;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!IsEqualColor(bgBitmap.GetPixel(e.X, e.Y), Color.Black) && !IsEqualColor(bgBitmap.GetPixel(e.X, e.Y), colorFill))
            {
                Color colorStart = bgBitmap.GetPixel(e.X, e.Y);

                Context context = Context.getInstance();
                
                context.img = pictureBox1.Image;
                context.colorFill = colorFill;
                context.colorStart = colorStart;
                context.colorBoundary = Color.Black;

                context.AlgorithmFill(e.X, e.Y);

                pictureBox1.Image = context.bitmap;
                context.SwitchStrategy();
            }

        }

        private bool IsEqualColor(Color _color1, Color _color2)
        {
            if (_color1.A == _color2.A && _color1.B == _color2.B && _color1.G == _color2.G && _color1.R == _color2.R)
                return true;
            else
                return false;
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            colorFill = colorBitmap.GetPixel(e.X, e.Y);
        }

       
    }
}
