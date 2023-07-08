using System.Data;
using System.Data.SQLite;
using System.IO;

namespace TasksTracker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            DB DB = new DB();

            string insertQuery = "INSERT INTO CDV (CODE, DESC) VALUES (@Code, @Description);";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Code", 46201);
            parameters.Add("@Description", "MEF magazzino automatico bobine pallet.");

            DB.ExecuteNonQuery( insertQuery, parameters);


            DataTable dt = DB.SelectDT("SELECT * FROM CDV");


        }
    }
}