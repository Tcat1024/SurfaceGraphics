using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SurfaceGraphics;
using System.Data.OleDb;

namespace TestForm
{
    public partial class Form1 : Form
    {
        
        DevExpress.XtraCharts.PaletteEntry[] Colors;
        DevExpress.XtraCharts.ChartControl colormaker = new DevExpress.XtraCharts.ChartControl();
        public Form1()
        {
            InitializeComponent();
            
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            List<CoilDefect> coilDefect;
            List<CoilSurfaceDefect> coilsurface;
            List<CoilDefectClass> coilDefectClass;
            PersistentService<CoilDefect> setup = new PersistentService<CoilDefect>();
            PersistentService<CoilSurfaceDefect> setup2 = new PersistentService<CoilSurfaceDefect>();
            PersistentService<CoilDefectClass> setup3 = new PersistentService<CoilDefectClass>();
            DataTable temp = new DataTable();
            using (OleDbConnection connection = new OleDbConnection("Provider=MSDAORA.1;Password=lyq;User ID=lyq;Data Source=lyq;Persist Security Info=True"))
            {
                connection.Open();
                string sql = string.Format("SELECT * FROM hrm_l2_coilsurfreport WHERE coil_id = '{0}'","4A08577200");
                coilsurface = setup2.Load(sql, connection);
               
                sql = "select classid,name from HRM_DEFECT_CLASSES t where status = 'U' order by classid";
                coilDefectClass = setup3.Load(sql, connection);

                sql = string.Format("SELECT * FROM hrm_l2_coildefects WHERE coil_id = '{0}' order by defect_id", "4A08577200");
                coilDefect = setup.Load(sql, connection);
            }
            this.surfaceGraphicsControl1.CoilInformation = coilsurface[0];
            this.surfaceGraphicsControl1.CoilDefects = coilDefect;
            this.surfaceGraphicsControl1.DefectClass = coilDefectClass;
            this.surfaceGraphicsControl1.Init();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.surfaceGraphicsControl1.refresh();
        }
    }
}
