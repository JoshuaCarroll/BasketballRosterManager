using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basketball_Roster_Manager
{
    public partial class NewTeam : Form
    {
        Form1 form1;
        ToolStripComboBox cboLeague;
        ComboBox cboTeam;

        public NewTeam(Form1 parent, ToolStripComboBox toolStripCombo, ComboBox comboBox)
        {
            InitializeComponent();
            form1 = parent;
            cboLeague = toolStripCombo;
            cboTeam = comboBox;
        }

        private void NewTeam_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'rostersDataSet.Leagues' table. You can move, or remove it, as needed.
            this.leaguesTableAdapter.Fill(this.rostersDataSet.Leagues);

            cmboLeague.SelectedValue = cboLeague.ComboBox.SelectedValue;
        }

        private void btnColorSelect_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                txtColor.Text = colorDialog1.Color.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCeConnection conn = new SqlCeConnection(Form1.connectionString);
            System.Data.SqlServerCe.SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = conn;

            int leagueID = int.Parse(cmboLeague.SelectedValue.ToString());

            if (leagueID == -1)
            {
                cmd.CommandText = String.Format("Insert into Leagues (LeagueName) values ('{0}');", cmboLeague.Text.Replace("'", "''"));
                conn.Open();
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT @@IDENTITY;";
                object scalarLeague = cmd.ExecuteScalar();
                leagueID = int.Parse(scalarLeague.ToString());
                conn.Close();

                form1.loadLeagues(cmboLeague.Text);
            }

            cmd.CommandText = String.Format("Insert into Teams (LeagueID, TeamName, Color) values ({0},'{1}','{2}');", leagueID, txtTeamName.Text.Replace("'", "''"), txtColor.Text.Replace("'", "''"));
            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            Debug.Print("Execute:\r\n" + cmd.CommandText + "\r\n" + rowsAffected.ToString() + " rows affected");
            cmd.CommandText = "SELECT @@IDENTITY;";
            object scalarTeam = cmd.ExecuteScalar();
            int teamID = int.Parse(scalarTeam.ToString());
            conn.Close();

            form1.loadTeams(cboTeam, leagueID, teamID);

            this.Close();
        }
    }
}
