using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SurfaceGraphics
{
    public partial class SurfaceGraphicsControl : DevExpress.XtraEditors.XtraUserControl
    {
        private DevExpress.XtraCharts.PaletteEntry[] Colors;
        private DevExpress.XtraCharts.ChartControl colormaker = new DevExpress.XtraCharts.ChartControl();
        private List<DefectInformation> DefectInformations = new List<DefectInformation>();
        private CoilSurfaceDefect _CoilInformation;
        private List<CoilDefect> _CoilDefects;
        private List<CoilDefectClass> _DefectClass;
        private Dictionary<int, Color> ColorDictionary;
        private BindingSource DefectsBind = new BindingSource();
        private BindingSource InformationBind = new BindingSource();
        public CoilSurfaceDefect CoilInformation 
        {
            get
            {
                return this._CoilInformation;
            }
            set
            {
                this._CoilInformation = value;
            }
        }
        public List<CoilDefect> CoilDefects
        {
            get
            {
                return this._CoilDefects;
            }
            set
            {
                this._CoilDefects = value;
            }
        }
        public List<CoilDefectClass> DefectClass
        {
            get
            {
                return this._DefectClass;
            }
            set
            {
                this._DefectClass = value;
            }
        }
        public SurfaceGraphicsControl()
        {
            InitializeComponent();
        }
        public void Init()
        {
            if (CoilInformation == null)
                throw new Exception("CoilInformation不能为NULL");
            if (CoilDefects == null)
                throw new Exception("CoilDefects不能为NULL");
            if (DefectClass == null)
                throw new Exception("DefectClass不能为NULL");
            InitGridControlExpression();
            this.InitClassColorDictionary();
            this.surfaceGraphicsChart1.ClassColor = this.ColorDictionary;
            this.DefectsBind.DataSource = this.surfaceGraphicsChart1.DefectInformations;
            this.gridControl1.DataSource = this.DefectsBind;
            this.InformationBind.DataSource = this.CoilInformation;
            this.gridControl2.DataSource = this.InformationBind;
            this.surfaceGraphicsChart1.CoilDefects = this.CoilDefects;
            this.surfaceGraphicsChart1.CoilLength = this.CoilInformation.Length;
            this.surfaceGraphicsChart1.CoilWidth = this.CoilInformation.Width;
            this.surfaceGraphicsChart1.Refresh();
            this.gridControl1.Refresh();
            this.advBandedGridView1.UpdateSummary();
        }
        private void InitClassColorDictionary()
        {
            ColorDictionary = new Dictionary<int, Color>();
            var tempList = this.CoilDefects.Distinct(new CoilDefectClassCompare());
            this.Colors = this.colormaker.GetPaletteEntries(tempList.Count());
            int count = tempList.Count();
            for (int i = 0; i <count; i++)
            {
                ColorDictionary.Add(tempList.ElementAt(i).Class,Colors[i].Color);
            }
        }
        private void InitGridControlExpression()
        {
            this.advBandedGridView1.Columns[1].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.advBandedGridView1.Columns[1].DisplayFormat.FormatString = "0.00";
            this.advBandedGridView1.Columns[1].UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            this.advBandedGridView1.Columns[1].UnboundExpression = "100*ToDecimal([NumTS])/" + this.CoilDefects.Count;
            this.advBandedGridView1.Columns[2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.advBandedGridView1.Columns[2].DisplayFormat.FormatString = "0.00";
            this.advBandedGridView1.Columns[2].UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            this.advBandedGridView1.Columns[2].UnboundExpression = "100*ToDecimal([NumBS])/" + this.CoilDefects.Count;
            this.advBandedGridView1.Columns[3].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.advBandedGridView1.Columns[3].DisplayFormat.FormatString = "0.00";
            this.advBandedGridView1.Columns[3].UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            this.advBandedGridView1.Columns[3].UnboundExpression = "100*ToDecimal([NumTS]+[NumBS])/" + this.CoilDefects.Count;

            this.advBandedGridView1.Columns[6].UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.advBandedGridView1.Columns[6].UnboundExpression = "[NumTS]+[NumBS]";
            this.repositoryItemLookUpEdit1.DataSource = this.DefectClass;
            this.repositoryItemLookUpEdit1.DisplayMember = "Name";
            this.repositoryItemLookUpEdit1.ValueMember = "ClassId";
        }
        private void advBandedGridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if(e.Column==this.advBandedGridView1.Columns[0]&&ColorDictionary!=null)
            {
                e.Appearance.ForeColor = this.ColorDictionary[Convert.ToInt32(e.CellValue)];
            }
        }
        public void refresh()
        {
            this.advBandedGridView1.UpdateSummary();

            this.gridControl1.Refresh();
            this.gridControl2.Refresh();
        }

    }
    public class CoilDefectClassCompare:IEqualityComparer<CoilDefect>
    {

        public bool Equals(CoilDefect x, CoilDefect y)
        {
            return x.Class == y.Class;
        }

        public int GetHashCode(CoilDefect obj)
        {
            return obj.Class.GetHashCode();
        }
    }
}
