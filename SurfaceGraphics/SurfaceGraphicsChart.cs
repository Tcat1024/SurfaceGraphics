using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SurfaceGraphics
{
    public partial class SurfaceGraphicsChart : DevExpress.XtraEditors.XtraUserControl
    {
        public SurfaceGraphicsChart()
        {
            InitializeComponent();
            this.Margin = new Padding(5, 20, 10, 5);
        }       
        private Rectangle rectT;
        private Rectangle rectB;
        private Rectangle rectAxisX;
        private Rectangle rectAxisY;
        private double coilLength = 300;
        private double coilWidth = 1050;
        private List<CoilDefect> defects;
        private List<DefectInformation> _DefectInformations = new List<DefectInformation>();
        private Dictionary<int, Color> _ClassColor;

        private Font m_font = new Font("Times", 10);
        private Font Title_font = new Font("Times", 15);

        private SizeF sizeT;
        private SizeF sizeB;
        private PointF rotatePointT;
        private PointF rotatePointB;
        private string TitleT = "Top Side";
        private string TitleB = "Buttom Side";
        public double CoilLength
        {
            get { return coilLength; }
            set { coilLength = value; }
        }
        
        public double CoilWidth
        {
            get { return coilWidth; }
            set { coilWidth = value; }
        }

        public List<CoilDefect> CoilDefects
        {
            get { return defects; }
            set 
            { 
                defects = value;
                Refresh();
            }
        }
        public Dictionary<int, Color> ClassColor
        {
            get { return _ClassColor; }
            set
            {
                _ClassColor = value;
            }
        }
        public List<DefectInformation> DefectInformations
        {
            get
            {
                return this._DefectInformations;
            }
        }
        public void CalculateRect(Graphics graph)
        {
            rectAxisY.X =Margin.Left+25;
            rectAxisY.Y =Margin.Top;
            rectAxisY.Width = 40;
            rectAxisY.Height = this.Height - Margin.Bottom - Margin.Top-5-m_font.Height;

            rectAxisX.X = rectAxisY.Right;
            rectAxisX.Y = rectAxisY.Bottom;
            rectAxisX.Width = this.Width - rectAxisX.Left - Margin.Right;
            rectAxisX.Height = 10;

            rectT.X = rectAxisY.Right;
            rectT.Y = (int)((float)rectAxisY.Bottom - (float)rectAxisY.Height / 2);
            rectT.Width = rectAxisX.Width;
            rectT.Height = rectAxisY.Height/2;
            rectB.X = rectAxisY.Right;
            rectB.Y = rectAxisY.Top;
            rectB.Width = rectAxisX.Width;
            rectB.Height = rectAxisY.Height / 2;

            sizeT = graph.MeasureString(TitleT, Title_font);
            sizeB = graph.MeasureString(TitleB, Title_font);
            rotatePointT = new PointF(this.Margin.Left + 30, rectAxisY.Bottom - rectAxisY.Height / 4 - sizeT.Width / 2); //设定旋转的中心点  
            rotatePointB = new PointF(this.Margin.Left + 30, rectAxisY.Bottom - 3*rectAxisY.Height / 4 - sizeT.Width / 2);
        }

        private void DrawAxisX(Graphics graph)
        {
            Pen pen = new Pen(Color.Black);

            Brush br = new SolidBrush(Color.Black);

            double scale = rectAxisX.Width / (coilLength%50000==0?coilLength:(((int)(coilLength/50000)+1)*50000));

            int count = (int)(coilLength / 50000)+(coilLength%50000==0?0:1);
         

            graph.DrawLine(pen, rectAxisX.Left, rectAxisX.Top, rectAxisX.Left, rectAxisX.Top + 5);

            for (int i = 1; i < count+1; i++)
            {
                double v = i * 50000;

                int x = rectAxisX.Left + (int)(v * scale);
                int y = rectAxisX.Top;

                graph.DrawLine(pen, x, y, x, y + 5);

                graph.DrawString((v/1000).ToString(), m_font, br, x - 20, y + 5);
            }

            graph.DrawLine(pen, rectAxisX.Left, rectAxisX.Top, rectAxisX.Right, rectAxisX.Top);
           // graph.DrawLine(pen, rectAxisX.Left, (float)rectAxisX.Top - (float)rectAxisY.Height / 2, rectAxisX.Right, rectAxisX.Top - rectAxisY.Height / 2);
        }

        private void DrawAxisY(Graphics graph)
        {
            //BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            //BufferedGraphics buffer = currentContext.Allocate(this.CreateGraphics(), this.rectAxisY);

            //Graphics graph = buffer.Graphics;

            //Brush br = new SolidBrush(BackColor);

            //graph.FillRectangle(br, rectAxisY);

            StringFormat fmt = new StringFormat();

            //if (m_yAxises.Count > 1)
            //{
            //    fmt.FormatFlags = (StringFormatFlags.DirectionVertical);
            //    fmt.Alignment = (StringAlignment.Center);
            //}
            //else
            //{
            fmt.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            fmt.Alignment = (StringAlignment.Far);
            fmt.LineAlignment = (StringAlignment.Center);
            //}

            Pen pen = new Pen(Color.Black);
            SolidBrush solidBrush = new SolidBrush(Color.Black);

            //draw y axis line
            graph.DrawLine(pen, new PointF(rectAxisY.Right, rectAxisY.Top), new PointF(rectAxisY.Right, rectAxisY.Bottom));

            float x = rectAxisY.Right;

            double ticksize = CoilWidth / 20;

            double scale = rectAxisY.Height / (CoilWidth*2);

            int fontWidth = 0;
            int fontHeight = 0;
            float y = (float)rectAxisY.Bottom - (float)rectAxisY.Height / 2;
            float x2 = rectAxisY.Right;
            float x1 = rectAxisY.Right - 6;
            graph.DrawLine(pen, x1, y, rectAxisX.Right, y);
            for (int j = 1; j < 21; j++)
            {
                int v = (int)(j * ticksize);

                SizeF size = graph.MeasureString(v.ToString(), m_font, new PointF(0, 0), fmt);

                fontWidth = (int)Math.Max(fontWidth, size.Width);
                fontHeight = (int)Math.Max(fontHeight, size.Height);

                
                float y1 =y + (float)(v * scale);
                
                float y2 = y- (float)(v * scale);

                //draw y axis scale
                graph.DrawLine(pen, x1, y1, x2, y1);
                graph.DrawLine(pen, x1, y2, x2, y2);
                
                //if (j == 0)
                //    fmt.LineAlignment = (StringAlignment.Far);
                //else if (j + 1 == 20)
                //    fmt.LineAlignment = (StringAlignment.Near);
                //else
                //    fmt.LineAlignment = (StringAlignment.Center);
  
                graph.DrawString(v.ToString(), m_font, solidBrush, new RectangleF(x2 -fontWidth- 1, y1 - fontHeight+2, fontWidth + 1, fontHeight + 1), fmt);
                graph.DrawString(v.ToString(), m_font, solidBrush, new RectangleF(x2 -fontWidth- 1, y2, fontWidth + 1, fontHeight + 1), fmt);

            }
            //graph.Transform = null ;
         
            //buffer.Render(g);
            //buffer.Dispose();
        }
        private void DrawTitle(Graphics graph)
        {
            Matrix MatrixT = new Matrix();
            MatrixT.RotateAt(270, rotatePointT, MatrixOrder.Append); //旋转270度  
            Matrix MatrixB = new Matrix();
            MatrixB.RotateAt(270, rotatePointB, MatrixOrder.Append); //旋转270度  
            Matrix MatrixN = new Matrix();
            MatrixB.RotateAt(0, rotatePointB, MatrixOrder.Append); //旋转270度  
            graph.Transform = MatrixT;
            graph.DrawString(TitleT, Title_font, new SolidBrush(Color.Black), rotatePointT.X - sizeT.Width, rotatePointT.Y - sizeT.Height); //写字  
            graph.Transform = MatrixB;
            graph.DrawString(TitleB, Title_font, new SolidBrush(Color.Black), rotatePointB.X - sizeB.Width, rotatePointB.Y - sizeB.Height);
            graph.Transform = MatrixN;
        }
        private void DrawDefect(Graphics graph)
        {
            if (defects == null)
                return;
            this._DefectInformations.Clear();
            float scaleX = (float)(rectT.Width / (coilLength % 50000 == 0 ? coilLength : (((int)(coilLength / 50000) + 1) * 50000)));
            float scaleY = (float)(rectT.Height / coilWidth);

            for (int i = 0; i < defects.Count; i++)
            {
                if (defects[i].Side == 0)
                {
                    float x0 = rectT.Left + (float)(defects[i].PositionMD) * scaleX;
                    float y0 = rectT.Top + (float)(defects[i].PositionCD) * scaleY;
         
                    float width = (float)(defects[i].SizeMD) * scaleX;
                    float height = (float)(defects[i].SizeCD) * scaleY;
                    Pen pen;
                    if(ClassColor==null)
                        pen = new Pen(Color.Red);
                    else
                    {
                        var id = defects[i].Class;
                        List<DefectInformation> re = this.DefectInformations.FindAll((temp) => { return temp.DefectID == id; });
                        if (re.Count > 0)
                            re[0].NumTS++;
                        else
                        {
                            this.DefectInformations.Add(new DefectInformation() { DefectID = id, NumTS = 1 });
                        }
                        pen = new Pen(ClassColor[defects[i].Class]);
                    }
                    graph.DrawRectangle(pen, x0, y0, width, height);
                }
                else if(defects[i].Side == 1)
                {
                    float x0 = rectB.Left + (float)(defects[i].PositionMD) * scaleX;
                    float y0 = rectB.Bottom - (float)(defects[i].PositionRCD) * scaleY;

                    float width = (float)(defects[i].SizeMD) * scaleX;
                    float height = (float)(defects[i].SizeCD) * scaleY;
                    Pen pen;
                    if (ClassColor == null)
                        pen = new Pen(Color.Red);
                    else
                    {
                        var id = defects[i].Class;
                        List<DefectInformation> re = this.DefectInformations.FindAll((temp)=>{return temp.DefectID==id;});
                        if(re.Count>0)
                            re[0].NumBS++;
                        else
                        {
                            this.DefectInformations.Add(new DefectInformation() {DefectID = id,NumBS = 1});
                        }
                        pen = new Pen(ClassColor[id]);
                    }
                    graph.DrawRectangle(pen, x0, y0-height, width, height);
                }
            }
        }

        private void CoilDefectsControl_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush br = new SolidBrush(Color.White);

            g.FillRectangle(br, new Rectangle(Left, Top, Width, Height));

            CalculateRect(g);
            DrawAxisX(g);
            DrawAxisY(g);
            DrawTitle(g);
            DrawDefect(g);
            
        }

        private void SurfaceGraphicsChart_Resize(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
