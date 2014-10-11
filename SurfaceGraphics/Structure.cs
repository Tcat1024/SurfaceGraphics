using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Reflection;
using System.Data;

namespace SurfaceGraphics
{
    public class ColorBase
    {
        public DataTable getClass()
        {
            DataTable result = new DataTable();
            return result;
        }
    }
    [Serializable]
    public class CoilDefect
    {
        [Persistent("COIL_ID")]
        public string CoilId;
        [Persistent("DEFECT_ID")]
        public string DefectId;
        [Persistent("CLASS")]
        public int Class;
        [Persistent("GRADE")]
        public int Grade;
        [Persistent("PERIODID")]
        public int PeriodId;
        [Persistent("PERIOD_LENGTH")]
        public double PeriodLength;
        [Persistent("POSITION_CD")]
        public double PositionCD;
        [Persistent("POSITION_RCD")]
        public double PositionRCD;
        [Persistent("POSITION_MD")]
        public double PositionMD;
        [Persistent("SIDE")]
        public int Side;
        [Persistent("SIZE_CD")]
        public double SizeCD;
        [Persistent("SIZE_MD")]
        public double SizeMD;
        [Persistent("CAMERA_NO")]
        public int CameraNo;
        [Persistent("DEFECT_NO")]
        public int DefectNo;
        [Persistent("MERGEDTO")]
        public int MergedTo;
        [Persistent("CONFIDENCE")]
        public int Confidence;
        [Persistent("ROIX0")]
        public int RoiX0;
        [Persistent("ROIX1")]
        public int RoiX1;
        [Persistent("ROIY0")]
        public int RoiY0;
        [Persistent("ROIY1")]
        public int RoiY1;
        [Persistent("ORIGINAL_CLASS")]
        public int OriginalClass;
        [Persistent("PP_ID")]
        public int PPId;
        [Persistent("POST_CL")]
        public int PostCL;
        [Persistent("MERGERPP")]
        public int MergerPP;
        [Persistent("ONLINE_CPP")]
        public int OlineCPP;
        [Persistent("OFFLINE_CPP")]
        public int OfflineCPP;
        [Persistent("CL_PROD_CLASS")]
        public int CLProdClass;
        [Persistent("CL_TEST_CLASS")]
        public int CLTestClass;
        [Persistent("ABS_POS_CD")]
        public double AbsPosCD;
    }
    public class PersistentService<T> where T : new()
    {
        class ColumnInfo
        {
            private bool field;
            private Type dataType;

            public Type DataType
            {
                get { return dataType; }
                set { dataType = value; }
            }
            public bool Field
            {
                get { return field; }
                set { field = value; }
            }
            private string name;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            private string mapTo;

            public string MapTo
            {
                get { return mapTo; }
                set { mapTo = value; }
            }

            public ColumnInfo(bool field, string name, Type dataType)
            {
                this.field = field;
                this.name = name;
                this.mapTo = name;
                this.dataType = dataType;
            }

            public ColumnInfo(bool field, string name, string mapTo, Type dataType)
            {
                this.field = field;
                this.name = name;
                this.mapTo = mapTo;
                this.dataType = dataType;
            }
        }

        static List<ColumnInfo> columns = new List<ColumnInfo>();

        static PersistentService()
        {
            GetMemberInfo();
        }

        public List<T> Load(string sql, OleDbConnection connection)
        {
            List<T> result = new List<T>();

            OleDbCommand command = new OleDbCommand(sql, connection);

            using (OleDbDataReader reader = command.ExecuteReader())
            {
                Type type = typeof(T);

                while (reader.Read())
                {
                    T obj = new T();
                    result.Add(obj);

                    foreach (ColumnInfo column in columns)
                    {
                        Object value = System.Convert.ChangeType(reader[column.MapTo], column.DataType);

                        if (column.Field)
                        {
                            type.InvokeMember(column.Name,
                                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField,
                                null,
                                obj,
                                new object[] { value });
                        }
                        else
                        {
                            type.InvokeMember(column.Name,
                                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                                    null,
                                    obj,
                                    new object[] { value });
                        }
                    }
                }
            }

            return result;
        }

        public static void GetMemberInfo()
        {
            Type type = typeof(T);
            PersistentAttribute persistAttr;

            //Querying Class-Property (only public)Attributes  
            foreach (PropertyInfo prop in type.GetProperties())
            {
                bool found = false;

                foreach (Attribute attr in prop.GetCustomAttributes(true))
                {
                    persistAttr = attr as PersistentAttribute;
                    if (null != persistAttr)
                    {
                        found = true;
                        columns.Add(new ColumnInfo(false, prop.Name, persistAttr.MapTo, prop.PropertyType));
                    }
                }

                if (found == false)
                {
                    columns.Add(new ColumnInfo(false, prop.Name, prop.PropertyType));
                }
            }

            //Querying Class-Field (only public) Attributes
            foreach (FieldInfo field in type.GetFields())
            {
                bool found = false;

                foreach (Attribute attr in field.GetCustomAttributes(true))
                {
                    persistAttr = attr as PersistentAttribute;
                    if (null != persistAttr)
                    {
                        found = true;
                        columns.Add(new ColumnInfo(true, field.Name, persistAttr.MapTo, field.FieldType));
                    }
                }

                if (found == false)
                {
                    columns.Add(new ColumnInfo(true, field.Name, field.FieldType));
                }
            }
        }
    }
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class PersistentAttribute : System.Attribute
    {
        public static readonly Type AttributeType;
        private string mapTo;

        static PersistentAttribute()
        {
            AttributeType = typeof(PersistentAttribute);
        }

        PersistentAttribute()
        {
        }

        public PersistentAttribute(string mapTo)
        {
            this.mapTo = mapTo;
        }

        public string MapTo
        {
            get { return mapTo; }
            set { mapTo = value; }
        }
    }
    [Serializable]
    public class CoilSurfaceDefect
    {
        [Persistent("COIL_ID")]
        public string CoilId { get; set; }
        [Persistent("START_TIME")]
        public DateTime StartTime { get; set; }
        [Persistent("STOP_TIME")]
        public DateTime StopTime { get; set; }
        [Persistent("PARAMSET")]
        public int ParamSet { get; set; }
        [Persistent("GRADE")]
        public int Grade { get; set; }
        [Persistent("LENGTH")]
        public double Length { get; set; }
        [Persistent("WIDTH")]
        public double Width { get; set; }
        [Persistent("THICKNESS")]
        public double Thickness { get; set; }
        [Persistent("WEIGHT")]
        public double Weight { get; set; }
        [Persistent("CHARGE")]
        public string Charge { get; set; }
        [Persistent("MATERIALID")]
        public int MaterialId { get; set; }
        [Persistent("STATUS")]
        public string Status { get; set; }
        [Persistent("DESCRIPTION")]
        public string Description { get; set; }
        [Persistent("LAST_DEFECT_ID")]
        public int LastDefectId { get; set; }
        [Persistent("TARGET_QUALITY")]
        public int TargetQuality { get; set; }
        [Persistent("PDI_RECV_TIME")]
        public DateTime PDIReceiveTime { get; set; }
        [Persistent("SLENGTH")]
        public double SlabLength { get; set; }
        [Persistent("DEFECT_COUNT")]
        public int DefectCount { get; set; }
        [Persistent("SURFACE_CODE")]
        public string SurfaceCode { get; set; }
        [Persistent("SCARFING")]
        public string Scarfing { get; set; }
    }
    [Serializable]
    public class CoilDefectClass
    {
        [Persistent("CLASSID")]
        public int ClassId{get;set;}
        [Persistent("NAME")]
        public string Name { get; set; }

    }
    public class DefectInformation
    {
        //public string DefectName { get; set; }
        public int DefectID { get; set; }
        //public double ShareTS { get; set; }
        //public double ShareBS { get; set; }
        //public double ShareSU { get; set; }
        public int NumTS { get; set; }
        public int NumBS { get; set; }
        //public int NumSU { get; set; }
        public System.Drawing.Color color { get; set; }
    }
}
