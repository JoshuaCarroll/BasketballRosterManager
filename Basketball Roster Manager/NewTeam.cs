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
        private Form1 form1;
        private ToolStripComboBox cboLeague;
        private ComboBox cboTeam;
        private string strSelectedLeagueID = string.Empty;

        public NewTeam(Form1 parent, ToolStripComboBox toolStripCombo, ComboBox comboBox)
        {
            InitializeComponent();
            form1 = parent;
            cboLeague = toolStripCombo;
            cboTeam = comboBox;

            try
            {
                ComboBoxItem cbiSelectedLeagueItem = (ComboBoxItem)cboLeague.SelectedItem;
                strSelectedLeagueID = cbiSelectedLeagueItem.Value;
            }
            catch
            {
                // Can't pre-set the league, no big deal
            }

        }

        private void NewTeam_Load(object sender, EventArgs e)
        {
            // Load leagues
            SqlCeConnection conn = new SqlCeConnection(Form1.connectionString);
            System.Data.SqlServerCe.SqlCeCommand cmd = new System.Data.SqlServerCe.SqlCeCommand("Select * from Leagues order by LeagueName", conn);
            conn.Open();

            SqlCeDataReader dr = cmd.ExecuteReader();

            cmboLeague.Items.Clear();

            while (dr.Read())
            {
                cmboLeague.Items.Add(new ComboBoxItem(dr["LeagueName"], dr["LeagueID"]));
            }

            dr.Close();
            conn.Close();

            if (strSelectedLeagueID != string.Empty)
            {
                for (int i = 0; i < cmboLeague.Items.Count; i++)
                {
                    ComboBoxItem cbi = (ComboBoxItem)cmboLeague.Items[i];
                    if (cbi.Value == strSelectedLeagueID)
                    {
                        cmboLeague.SelectedIndex = i;
                        break;
                    }
                }
            }
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

            ComboBoxItem cbiLeague = (ComboBoxItem)cmboLeague.SelectedItem;
            int leagueID = -1;

            if (cbiLeague != null)
            {
                leagueID = int.Parse(cbiLeague.Value);
            }

            if ((cbiLeague == null) || (leagueID == -1))
            {
                cmd.CommandText = String.Format("Insert into Leagues (LeagueName) values ('{0}');", cmboLeague.Text.Replace("'", "''"));
                conn.Open();
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT @@IDENTITY;";
                object scalarLeague = cmd.ExecuteScalar();
                leagueID = int.Parse(scalarLeague.ToString());
                conn.Close();

                form1.loadLeagues(leagueID);
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
