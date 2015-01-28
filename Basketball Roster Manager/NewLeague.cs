using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basketball_Roster_Manager
{
    public partial class NewLeague : Form
    {
        private Form1 form1;
        private ToolStripComboBox tsComboLeague;

        public NewLeague(Form1 _form1, ToolStripComboBox _tsComboLeague)
        {
            InitializeComponent();
            form1 = _form1;
            tsComboLeague = _tsComboLeague;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCeConnection conn = new SqlCeConnection(Form1.connectionString);
            System.Data.SqlServerCe.SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = conn;

            cmd.CommandText = String.Format("Insert into Leagues (LeagueName) values ('{0}');", textBox1.Text.Replace("'", "''"));
            conn.Open();
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT @@IDENTITY;";
            object scalarLeague = cmd.ExecuteScalar();
            int leagueID = int.Parse(scalarLeague.ToString());
            conn.Close();

            form1.loadLeagues(leagueID);

            this.Close();
        }
    }
}
