using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StSt
{
    enum TypeOfMetal : int { Cu = 2, Al = 3, Fe = 4, Ni = 5 }
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //создание пустого bitmap
        public Bitmap bitmap = new Bitmap(Convert.ToInt32(560), Convert.ToInt32(500), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        
        private void Form1_Load(object sender, EventArgs e)
        {
            cBMaterials.Items.Add("Cu");
            cBMaterials.Items.Add("Al");
            cBMaterials.Items.Add("Fe");
            cBMaterials.Items.Add("Ni");

           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
       
       

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                TypeOfMetal m;
                switch (cBMaterials.SelectedIndex)
                {
                    case 0:
                        m = TypeOfMetal.Cu;
                        break;
                    case 1:
                        m = TypeOfMetal.Al;
                        break;
                    case 2:
                        m = TypeOfMetal.Fe;
                        break;
                    default:
                        m = TypeOfMetal.Ni;
                        break;
                }

                AbstractFigure dump;
                double diametr = double.Parse(textBox1.Text);
                double height = double.Parse(textBox2.Text);

                if (radioCylinder.Checked)
                    dump = new Cylinder(height, diametr, m);

                else if (radioParrall.Checked)
                    dump = new Parrallelepiped(height, diametr, m);

                else if (radioTreug.Checked)
                    dump = new TreugParral(height, diametr, m);

                else
                    dump = new Konus(height, diametr, m);

                dump.Calc();
                tBHeigReturn.Text = dump.deformHeight.ToString();
                tBOsnReturn.Text = dump.deformOsnov.ToString();

                //для сохранения картинки необходима привязка к Bitmap
                pictureBox1.Image = bitmap;
                dump.Draw(pictureBox1, bitmap, 1);
                //Draw(pictureBox1, 1);
            }
            catch
            {
                MessageBox.Show("Ошибка вводимых значений");
            }
        }

        private void Draw(PictureBox pic,  Bitmap  bitmap,double scale)
        {
            Graphics myGraph = Graphics.FromImage(bitmap);
            myGraph.Clear(Color.White);
            Pen pen = new Pen(Color.Black);

            int origHeight = 200;
            int origOsnov = 100;
            int deformHeight = 100; 
            int deformOsnov = 150;

            int height = Convert.ToInt32(origHeight * scale);
            int osnov = Convert.ToInt32(origOsnov * scale);
            int defHeight = Convert.ToInt32(deformHeight * scale);
            int defOsnov = Convert.ToInt32(deformOsnov * scale);

            int y_Start = (pic.Height / 2) - (height / 2);
            int x_Start = (pic.Width / 2);
            int def_x_Start = (pic.Width / 2);

            //orig

            Point B = new Point(x_Start, y_Start);
            Point A = new Point(B.X - osnov, B.Y - osnov/3);
            Point C = new Point(B.X + osnov, B.Y-osnov/3);
            
            Point D = new Point(C.X, C.Y + height);
            Point E = new Point(B.X, B.Y + height);
            Point F = new Point(A.X, A.Y + height);

            myGraph.DrawLine(pen, A, C);
            myGraph.DrawLine(pen, A, B);
            myGraph.DrawLine(pen, C, B);

            myGraph.DrawLine(pen, C, D);
            myGraph.DrawLine(pen, B, E);
            myGraph.DrawLine(pen, A, F);
            myGraph.DrawLine(pen, E, D);
            myGraph.DrawLine(pen, E, F);

            //deform
            Point E_d = new Point(def_x_Start, E.Y + deformOsnov/8);
            Point F_d = new Point(E_d.X - defOsnov, E_d.Y -defOsnov/3);
            Point D_d = new Point(E_d.X + defOsnov, E_d.Y -defOsnov/3);

            Point A_d = new Point(F_d.X, F_d.Y - defHeight);
            Point B_d = new Point(E_d.X, E_d.Y - defHeight);
            Point C_d = new Point(D_d.X, D_d.Y - defHeight);

            myGraph.DrawLine(pen, E_d, F_d);
            myGraph.DrawLine(pen, E_d, D_d);

            myGraph.DrawLine(pen, D_d, C_d);
            myGraph.DrawLine(pen, E_d, B_d);
            myGraph.DrawLine(pen, F_d, A_d);

            myGraph.DrawLine(pen, A_d, B_d);
            myGraph.DrawLine(pen, A_d, C_d);
            myGraph.DrawLine(pen, B_d, C_d);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            saveFileDialog1.Filter = "Png images (*.png)|*.png| All files(*.*)|*.*";
            ImageFormat format = ImageFormat.Png;
            bitmap.Save(saveFileDialog1.FileName, format);
            MessageBox.Show("Файл сохранен");
        }
    }

    abstract class AbstractFigure
    {
        public TypeOfMetal material;
        public double origHeight, origOsnov, deformHeight, deformOsnov;
        
        public AbstractFigure()
        {
            origHeight = 1;
            origOsnov = 1;
            deformHeight = 1;
            deformOsnov = 1;
        }

        public AbstractFigure(double h, double osn, TypeOfMetal m)
        {
            origHeight = h;
            origOsnov = osn;
            deformHeight = h;
            deformOsnov = osn;
            material = m;
        }

        public abstract void Calc();
        public abstract void Draw(PictureBox pic,  Bitmap  bitmap,double scale);
    }

    class Cylinder : AbstractFigure
    {
        public Cylinder() : base() { }
        public Cylinder(double h, double osn, TypeOfMetal m) : base(h, osn, m) { }
        public override void Calc()
        {
            this.deformHeight = Math.Round(this.origHeight - this.origHeight / (double)material, 3);
            this.deformOsnov = Math.Round(2 * Math.Sqrt(this.origHeight * Math.Pow((this.origOsnov * 0.5), 2) / this.deformHeight), 3);
        }

        public override void Draw(PictureBox pic,  Bitmap  bitmap,double scale)
        {
            Graphics myGraph = Graphics.FromImage(bitmap);
            myGraph.Clear(Color.White);
            Pen pen = new Pen(Color.Black);

            int height = Convert.ToInt32(origHeight * scale);
            int osnov = Convert.ToInt32(origOsnov * scale);
            int defHeight = Convert.ToInt32(deformHeight * scale);
            int defOsnov = Convert.ToInt32(deformOsnov * scale);

            int y_Start = (pic.Height / 2) - (height / 2);
            int x_Start = (pic.Width / 2) - (osnov / 2);
            int def_x_Start = (pic.Width / 2) - (defOsnov / 2);

            //orig
            Point A = new Point(x_Start, y_Start + osnov / 6);
            Point C = new Point(A.X, A.Y + height);
            Point B = new Point(A.X + osnov, A.Y);
            Point D = new Point(B.X, B.Y + height);

            myGraph.DrawEllipse(pen, x_Start, y_Start, osnov, osnov / 3);
            myGraph.DrawLine(pen, A, C);
            myGraph.DrawLine(pen, B, D);
            myGraph.DrawEllipse(pen, C.X, C.Y - osnov / 6, osnov, osnov / 3);

            //deform
            pen = new Pen(Color.Brown);
            Point C_d = new Point(def_x_Start, C.Y);
            Point A_d = new Point(C_d.X, C_d.Y - defHeight);
            Point D_d = new Point(C_d.X + defOsnov, C_d.Y);
            Point B_d = new Point(D_d.X, D_d.Y - defHeight);

            myGraph.DrawEllipse(pen, C_d.X, A_d.Y - defOsnov / 6, defOsnov, defOsnov / 3);
            myGraph.DrawLine(pen, A_d, C_d);
            myGraph.DrawLine(pen, B_d, D_d);
            myGraph.DrawEllipse(pen, C_d.X, C_d.Y - defOsnov / 6, defOsnov, defOsnov / 3);

            if ((height < 110 & osnov < 300) | (osnov < 110 & height < 300))
                Draw(pic,bitmap, scale + 0.1);

            List<Point> points = new List<Point>() { C, A, D, B, C_d, A_d, D_d, B_d };
            foreach (Point pro_point in points)
            {
                if (pro_point.X <= 0 | pro_point.Y <= 0 | pro_point.X >= pic.Width | pro_point.Y >= pic.Height - defOsnov / 6)
                {
                    Draw(pic,bitmap, scale - 0.01);
                    break;
                }
            }
        }
    }
    class Parrallelepiped : AbstractFigure
{
        public Parrallelepiped() : base() { }
        public Parrallelepiped(double h, double osn, TypeOfMetal m) : base(h, osn, m) { }
        public override void Calc()
        {
            deformHeight = Math.Round(origHeight - (origHeight / (double)material), 3);
            this.deformOsnov = Math.Round(Math.Sqrt(this.origHeight * Math.Pow(this.origOsnov, 2) / this.deformHeight), 3);
        }

        //для сохранения картинки необходима привязка к Bitmap
        public override void Draw(PictureBox pic,  Bitmap  bitmap,double scale)
        {
            Graphics myGraph = Graphics.FromImage(bitmap);
            myGraph.Clear(Color.White);
            Pen pen = new Pen(Color.Black, 2);

            int height = Convert.ToInt32(origHeight * scale);
            int osnov = Convert.ToInt32(origOsnov * scale);

            int defHeight = Convert.ToInt32(deformHeight * scale);
            int defOsnov = Convert.ToInt32(deformOsnov * scale);
            int deformXStart = (pic.Width / 2) - (defOsnov / 2);


            int y_Start = (pic.Height / 2) - (height / 2);
            int x_Start = (pic.Width / 2) - (osnov / 2);

            //OrigCube
            Point C = new Point(x_Start, y_Start + height / 4);
            Point A = new Point(C.X + osnov / 3, y_Start);
            Point B = new Point(A.X + osnov, y_Start);
            Point D = new Point(B.X - osnov / 3, B.Y + height / 4);

            Point B2 = new Point(B.X, B.Y + height);
            Point D2 = new Point(D.X, D.Y + height);
            Point C2 = new Point(C.X, C.Y + height);

            myGraph.DrawLine(pen, A, B);
            myGraph.DrawLine(pen, B, D);
            myGraph.DrawLine(pen, D, C);
            myGraph.DrawLine(pen, A, C);

            myGraph.DrawLine(pen, B, B2);
            myGraph.DrawLine(pen, D, D2);
            myGraph.DrawLine(pen, C, C2);

            myGraph.DrawLine(pen, C2, D2);
            myGraph.DrawLine(pen, D2, B2);

            //DeformCube
            Point C2_d = new Point(deformXStart, C2.Y + defHeight / 8);
            Point C_d = new Point(C2_d.X, C2_d.Y - defHeight);
            Point D2_d = new Point(C2_d.X + defOsnov, C2_d.Y);
            Point D_d = new Point(D2_d.X, D2_d.Y - defHeight);

            Point B2_d = new Point(D2_d.X + osnov / 3, D2_d.Y - height / 4);
            Point B_d = new Point(B2_d.X, B2_d.Y - defHeight);
            Point A_d = new Point(C_d.X + osnov / 3, C_d.Y - height / 4);

            pen = new Pen(Color.Brown, 2);
            myGraph.DrawLine(pen, C2_d, C_d);
            myGraph.DrawLine(pen, C2_d, D2_d);
            myGraph.DrawLine(pen, D2_d, D_d);
            myGraph.DrawLine(pen, D_d, C_d);

            myGraph.DrawLine(pen, D_d, B_d);
            myGraph.DrawLine(pen, D2_d, B2_d);
            myGraph.DrawLine(pen, B_d, B2_d);

            myGraph.DrawLine(pen, C_d, A_d);
            myGraph.DrawLine(pen, A_d, B_d);

            if ((height < 110 & osnov < 120) | (osnov < 110 & height < 120))
                Draw(pic,bitmap, scale + 0.1);

            List<Point> points = new List<Point>() { C, A, D, B, C2, D2, B2, C2_d, C_d, A_d, D_d, D2_d, B_d, B2_d };
            foreach (Point pro_point in points)
            {
                if (pro_point.X <= 0 | pro_point.Y <= 0 | pro_point.X >= pic.Width | pro_point.Y >= pic.Height)
                {
                    Draw(pic,bitmap, scale - 0.001);
                    break;
                }
            }
        }
    }

    class Konus : AbstractFigure
    {
        public Konus() : base() { }
        public Konus(double h, double osn, TypeOfMetal m) : base(h, osn, m) { }
        public override void Calc()
        {
            deformHeight = Math.Round(origHeight - ((origHeight) / ((double)material)), 3);
            this.deformOsnov = Math.Round(Math.Sqrt(this.origHeight * Math.Pow(this.origOsnov, 2) / this.deformHeight), 3);
        }

        public override void Draw(PictureBox pic,  Bitmap  bitmap,double scale)
        {
            Graphics myGraph = Graphics.FromImage(pic.Image);
            myGraph.Clear(Color.White);
            Pen pen = new Pen(Color.Black);

            int height = Convert.ToInt32(origHeight * scale);
            int osnov = Convert.ToInt32(origOsnov * scale);
            int defHeight = Convert.ToInt32(deformHeight * scale);
            int defOsnov = Convert.ToInt32(deformOsnov * scale);

            int y_Start = (pic.Height / 2) - (height / 2);
            int x_Start = (pic.Width / 2);
            int def_x_Start = x_Start - (defOsnov / 2);

            //orig
            Point A = new Point(x_Start, y_Start);
            Point B = new Point(x_Start - osnov / 2, y_Start + height);
            Point C = new Point(x_Start + osnov / 2, y_Start + height);

            myGraph.DrawLine(pen, A, C);
            myGraph.DrawLine(pen, A, B);
            myGraph.DrawEllipse(pen, B.X, B.Y - osnov / 6, osnov, osnov / 3);

            //deform
            Point A_d = new Point(def_x_Start, B.Y);
            Point B_d = new Point(A_d.X + defOsnov / 6, A_d.Y - defHeight);
            Point C_d = new Point(A_d.X + defOsnov, A_d.Y);
            Point D_d = new Point(C_d.X - defOsnov / 6, C_d.Y - defHeight);

            pen = new Pen(Color.Brown, 2);
            myGraph.DrawLine(pen, A_d, B_d);
            myGraph.DrawLine(pen, C_d, D_d);
            myGraph.DrawEllipse(pen, A_d.X, A_d.Y - defOsnov / 6, defOsnov, defOsnov / 3);
            int defOsnov2 = Convert.ToInt32(defOsnov / 1.5);
            myGraph.DrawEllipse(pen, B_d.X, B_d.Y - defOsnov2 / 6, defOsnov2, defOsnov2 / 3);

            if ((height < 110 & osnov < 110) | (osnov < 110 & height < 110))
                Draw(pic,bitmap, scale + 0.01);

            List<Point> points = new List<Point>() { C, A, B, C_d, A_d, D_d, B_d };
            foreach (Point pro_point in points)
            {
                if (pro_point.X <= 0 | pro_point.Y <= 0 | pro_point.X >= pic.Width | pro_point.Y >= pic.Height)
                {
                    Draw(pic,bitmap, scale - 0.001);
                    break;
                }
            }
        }
    }

    class TreugParral : AbstractFigure
    {
        public TreugParral() : base() { }
        public TreugParral(double h, double osn, TypeOfMetal m) : base(h, osn, m) { }
        public override void Calc()
        {
            deformHeight = Math.Round(origHeight - (origHeight / (double)material), 3);
            this.deformOsnov = Math.Round(Math.Sqrt(this.origHeight * Math.Pow(this.origOsnov, 2) / this.deformHeight), 3);
        }

        public override void Draw(PictureBox pic,  Bitmap  bitmap,double scale)
        {
            Graphics myGraph = Graphics.FromImage(bitmap);
            myGraph.Clear(Color.White);
            Pen pen = new Pen(Color.Black);

            int height = Convert.ToInt32(origHeight * scale);
            int osnov = Convert.ToInt32(origOsnov * scale);
            int defHeight = Convert.ToInt32(deformHeight * scale);
            int defOsnov = Convert.ToInt32(deformOsnov * scale);

            int y_Start = (pic.Height / 2) - (height / 2);
            int x_Start = (pic.Width / 2);
            int def_x_Start = (pic.Width / 2);

            //orig

            Point B = new Point(x_Start, y_Start);
            Point A = new Point(B.X - osnov, B.Y - osnov / 3);
            Point C = new Point(B.X + osnov, B.Y - osnov / 3);

            Point D = new Point(C.X, C.Y + height);
            Point E = new Point(B.X, B.Y + height);
            Point F = new Point(A.X, A.Y + height);

            myGraph.DrawLine(pen, A, C);
            myGraph.DrawLine(pen, A, B);
            myGraph.DrawLine(pen, C, B);

            myGraph.DrawLine(pen, C, D);
            myGraph.DrawLine(pen, B, E);
            myGraph.DrawLine(pen, A, F);
            myGraph.DrawLine(pen, E, D);
            myGraph.DrawLine(pen, E, F);

            //deform
            Point E_d = new Point(def_x_Start, E.Y + defOsnov/8);
            Point F_d = new Point(E_d.X - defOsnov, E_d.Y - defOsnov / 3);
            Point D_d = new Point(E_d.X + defOsnov, E_d.Y - defOsnov / 3);

            Point A_d = new Point(F_d.X, F_d.Y - defHeight);
            Point B_d = new Point(E_d.X, E_d.Y - defHeight);
            Point C_d = new Point(D_d.X, D_d.Y - defHeight);

            pen = new Pen(Color.Brown, 2);
            myGraph.DrawLine(pen, E_d, F_d);
            myGraph.DrawLine(pen, E_d, D_d);

            myGraph.DrawLine(pen, D_d, C_d);
            myGraph.DrawLine(pen, E_d, B_d);
            myGraph.DrawLine(pen, F_d, A_d);

            myGraph.DrawLine(pen, A_d, B_d);
            myGraph.DrawLine(pen, A_d, C_d);
            myGraph.DrawLine(pen, B_d, C_d);

            if ((height < 110 & osnov < 120) | (osnov < 110 & height < 120))
                Draw(pic,bitmap, scale + 0.1);

            List<Point> points = new List<Point>() { C, A, D, B, E,F, C_d, A_d, D_d, E_d, B_d, F_d };
            foreach (Point pro_point in points)
            {
                if (pro_point.X <= 0 | pro_point.Y <= 0 | pro_point.X >= pic.Width | pro_point.Y >= pic.Height)
                {
                    Draw(pic,bitmap, scale - 0.001);
                    break;
                }
            }
        }
    }
}
