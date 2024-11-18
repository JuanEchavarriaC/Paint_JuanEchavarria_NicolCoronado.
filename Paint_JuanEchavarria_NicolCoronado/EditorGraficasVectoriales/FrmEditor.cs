using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EditorGraficasVectoriales
{
    public partial class FrmEditor : Form
    {
        private Point puntoInicial;  
        private Point puntoFinal;    
        private bool dibujando = false; 
        private List<Shape> shapes = new List<Shape>();
        private Shape shapeSeleccionada = null;
        private bool seleccionando = false;
        private Rectangle rectSeleccion = Rectangle.Empty;

        public FrmEditor()
        {
            InitializeComponent();
            cmbTrazo.SelectedIndex = 0;
        }

        private void pnlDibujo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (btnSeleccionar.Checked)
                {
                   
                    seleccionando = true;
                    puntoInicial = e.Location;
                    rectSeleccion = new Rectangle(puntoInicial, new Size(0, 0));
                }
                else
                {
        
                    puntoInicial = e.Location;
                    dibujando = true;
                }
            }
        }

        private void pnlDibujo_MouseMove(object sender, MouseEventArgs e)
        {
            if (seleccionando)
            {
              
                puntoFinal = e.Location;
                rectSeleccion = new Rectangle(
                    Math.Min(puntoInicial.X, puntoFinal.X),
                    Math.Min(puntoInicial.Y, puntoFinal.Y),
                    Math.Abs(puntoInicial.X - puntoFinal.X),
                    Math.Abs(puntoInicial.Y - puntoFinal.Y));
                pnlDibujo.Invalidate();
            }
            else if (dibujando)
            {
               
                puntoFinal = e.Location;
                pnlDibujo.Invalidate();
            }
        }

        private void pnlDibujo_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (seleccionando)
                {
                   
                    seleccionando = false;
                    foreach (var shape in shapes)
                    {
                        shape.IsSelected = rectSeleccion.Contains(new Rectangle(shape.Start, new Size(shape.End.X - shape.Start.X, shape.End.Y - shape.Start.Y)));
                    }
                    pnlDibujo.Invalidate();
                }
                else if (dibujando)
                {
                  
                    dibujando = false;
                    puntoFinal = e.Location;
                    Shape shape = null;
                    switch (cmbTrazo.SelectedIndex)
                    {
                        case 0:
                            shape = new Line(puntoInicial, puntoFinal);
                            break;
                        case 1:
                            shape = new RectangleShape(puntoInicial, puntoFinal);
                            break;
                        case 2:
                            shape = new Ellipse(puntoInicial, puntoFinal);
                            break;
                    }
                    if (shape != null)
                    {
                        shapes.Add(shape);
                    }
                    pnlDibujo.Invalidate();
                }
            }
        }

        private void pnlDibujo_Paint(object sender, PaintEventArgs e)
        {
          
            foreach (var shape in shapes)
            {
                shape.Draw(e.Graphics);
            }
           
            if (dibujando)
            {
                using (Pen pen = new Pen(Color.White, 3))
                {
                    switch (cmbTrazo.SelectedIndex)
                    {
                        case 0:
                            e.Graphics.DrawLine(pen, puntoInicial, puntoFinal);
                            break;
                        case 1:
                            e.Graphics.DrawRectangle(pen, new RectangleF(puntoInicial,
                                new Size(puntoFinal.X - puntoInicial.X, puntoFinal.Y - puntoInicial.Y)));
                            break;
                        case 2:
                            e.Graphics.DrawEllipse(pen, new RectangleF(puntoInicial,
                                new Size(puntoFinal.X - puntoInicial.X, puntoFinal.Y - puntoInicial.Y)));
                            break;
                    }
                }
            }
          
            if (seleccionando)
            {
                using (Pen pen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(pen, rectSeleccion);
                }
            }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "PNG Files|*.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pnlDibujo.BackgroundImage = Image.FromFile(openFileDialog.FileName);
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Files|*.png";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(pnlDibujo.Width, pnlDibujo.Height);
                    pnlDibujo.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    bmp.Save(saveFileDialog.FileName);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            pnlDibujo.BackgroundImage = null;
            shapes.Clear(); 
            pnlDibujo.Invalidate();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            shapes.RemoveAll(shape => shape.IsSelected);
            pnlDibujo.Invalidate();
        }

        private void btnDibujar_Click(object sender, EventArgs e)
        {
            btnSeleccionar.Checked = false;
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            btnSeleccionar.Checked = true;
        }
    }
}
