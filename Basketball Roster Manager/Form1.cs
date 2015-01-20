using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basketball_Roster_Manager
{
    public partial class Form1 : Form
    {
        private static string appDataFolder = "Carroll Media";
        private static string appDataFile = "Rosters.sdf";
        private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Basketball_Roster_Manager.Properties.Settings.RostersConnectionString"].ConnectionString;
        //public static string connectionString = Path.Combine(appDataPath, appDataFolder, appDataFile);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //if (verifyDataPath(connectionString))
            //{
                loadHalves();
                loadLeagues();
                tsCboLeague_SelectedIndexChanged(sender, e);
                tsCboHalf_SelectedIndexChanged(sender, e);
            //}
            //else
            //{
            //    MessageBox.Show("Data file not found.  Closing application.", "Closing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    this.Close();
            //}
        }

        private bool verifyDataPath(string connStr)
        {
            if (File.Exists(connStr))
            {
                return true;
            }
            else
            {
                if (MessageBox.Show("The rosters data file was not found.  Create a new data file to maintain data?", "Create new file?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    // Create new data folder
                    if (!Directory.Exists(Path.Combine(appDataPath, appDataFolder))) Directory.CreateDirectory(Path.Combine(appDataPath, appDataFolder));

                    // Create new data file
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    using (Stream input = assembly.GetManifestResourceStream("EmptyRosters"))
                    using (Stream output = File.Create(connStr))
                    {
                        input.CopyTo(output);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void loadHalves()
        {
            tsCboHalf.Items.Add(new ComboBoxItem("First half", "1"));
            tsCboHalf.Items.Add(new ComboBoxItem("Second half", "2"));
            tsCboHalf.SelectedIndex = 0;
        }

        public void loadLeagues()
        {
            tsCboLeague.ComboBox.BindingContext = this.BindingContext;
            tsCboLeague.ComboBox.DisplayMember = "LeagueName";
            tsCboLeague.ComboBox.ValueMember = "LeagueID";
            tsCboLeague.ComboBox.DataSource = leaguesBindingSource;

            this.leaguesTableAdapter.Fill(this.rostersDataSet.Leagues);
        }

        public void loadLeagues(string selectedLeagueText)
        {
            loadLeagues();
            tsCboLeague.SelectedText = selectedLeagueText;
        }

        private void tsCboLeague_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripComboBox)
            {

                Debug.Print("Sender is: ");

                ToolStripComboBox cb = (ToolStripComboBox)sender;
                cb.Owner.Hide();

                if (cb.ComboBox.SelectedValue != null)
                {
                    int leagueID = (int)cb.ComboBox.SelectedValue;

                    loadTeams(cboTeam1, leagueID);
                    loadTeams(cboTeam2, leagueID);
                }
            }
        }

        public void loadTeams(ComboBox comboToLoad, int leagueID)
        {
            loadTeams(comboToLoad, leagueID, -1);
        }

        public void loadTeams(ComboBox comboToLoad, int leagueID, int selectedTeamID)
        {
            try
            {
                // Load teams from this league
                SqlCeConnection conn = new SqlCeConnection(connectionString);
                System.Data.SqlServerCe.SqlCeCommand cmd = new System.Data.SqlServerCe.SqlCeCommand("Select * from Teams where LeagueID = " + leagueID + " order by TeamName", conn);
                conn.Open();

                SqlCeDataReader dr = cmd.ExecuteReader();

                comboToLoad.Items.Clear();

                while (dr.Read())
                {
                    comboToLoad.Items.Add(new ComboBoxItem(dr["TeamName"], dr["TeamID"]));
                }

                dr.Close();
                conn.Close();

                if (selectedTeamID != -1)
                {
                    for (int i = 0; i < comboToLoad.Items.Count; i++)
                    {
                        ComboBoxItem cbi = (ComboBoxItem)comboToLoad.Items[0];
                        if (int.Parse(cbi.Value) == selectedTeamID)
                        {
                            comboToLoad.SelectedIndex = i;
                            break;
                        }
                    }
                }

                comboToLoad.Items.Add(new ComboBoxItem("Add new", "0"));

            }
            catch (System.Exception ex)
            {
                Debug.Print("Unable to load teams from league: " + ex.Message);
            }

        }

        private void tsMenuItemSwitchSides_Click(object sender, EventArgs e)
        {
            SwitchSides();
        }

        private void FoulTextbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TextBox t = (TextBox)sender;
            int fouls = 0;

            try
            {
                fouls = int.Parse(t.Text);
            }
            catch { }

            fouls++;
            t.Text = fouls.ToString();

            label1.Focus();
        }

        private void FoulTextBox_KeyPress(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            string lineNumber = string.Empty;
            string homeOrAway = string.Empty;
            string firstOrSecond = string.Empty;

            if (t.Name.StartsWith("Home"))
            {
                homeOrAway = "Home";
            }
            else
            {
                homeOrAway = "Away";
            }

            if (t.Name.StartsWith(homeOrAway + "FoulFirst"))
            {
                firstOrSecond = "First";
            }
            else
            {
                firstOrSecond = "Second";
            }

            lineNumber = t.Name.Substring((homeOrAway + "Foul" + firstOrSecond).Length);

            CheckBox ck = (CheckBox)Controls.Find(homeOrAway + "Entered" + lineNumber, true)[0];
            ck.Checked = true;

            int fouls1 = 0;
            int fouls2 = 0;
            int foulsTot = 0;

            try
            {
                TextBox FoulFirst = (TextBox)Controls.Find(homeOrAway + "FoulFirst" + lineNumber, true)[0];
                TextBox FoulSecond = (TextBox)Controls.Find(homeOrAway + "FoulSecond" + lineNumber, true)[0];
                TextBox FoulTotal = (TextBox)Controls.Find(homeOrAway + "FoulTotal" + lineNumber, true)[0];

                int.TryParse(FoulFirst.Text, out fouls1);
                int.TryParse(FoulSecond.Text, out fouls2);

                foulsTot = fouls1 + fouls2;

                TextBox txtNumber = (TextBox)Controls.Find(homeOrAway + "Number" + lineNumber, true)[0];
                TextBox txtName = (TextBox)Controls.Find(homeOrAway + "Name" + lineNumber, true)[0];

                if (foulsTot < 4)
                {
                    txtNumber.BackColor = Color.White;
                    txtName.BackColor = Color.White;
                    FoulTotal.BackColor = SystemColors.Control;

                    txtNumber.ForeColor = Color.Black;
                    txtName.ForeColor = Color.Black;
                    FoulTotal.ForeColor = Color.Black;
                }
                else if (foulsTot == 4)
                {
                    txtNumber.BackColor = Color.White;
                    txtName.BackColor = Color.White;
                    FoulTotal.BackColor = Color.Yellow;

                    txtNumber.ForeColor = Color.Black;
                    txtName.ForeColor = Color.Black;
                    FoulTotal.ForeColor = Color.Black;
                }
                else if (foulsTot >= 5)
                {
                    txtNumber.BackColor = SystemColors.Control;
                    txtName.BackColor = SystemColors.Control;
                    FoulTotal.BackColor = SystemColors.Control;

                    txtNumber.ForeColor = Color.Gray;
                    txtName.ForeColor = Color.Gray;
                    FoulTotal.ForeColor = Color.Gray;

                    foulsTot = 5;
                }

                FoulTotal.Text = foulsTot.ToString();
            }
            catch { }

            setHalfTotals(homeOrAway, firstOrSecond);
        }

        private void setHalfTotals(string homeOrAway, string firstOrSecond)
        {
            int intTotal = 0;

            for (int i = 1; i <= 18; i++)
            {
                TextBox t = (TextBox)Controls.Find(homeOrAway + "Foul" + firstOrSecond + i, true)[0];
                int f = 0;
                int.TryParse(t.Text, out f);

                intTotal += f;
            }

            TextBox total = (TextBox)Controls.Find(homeOrAway + "Foul" + firstOrSecond + "Total", true)[0];

            if (intTotal <= 10)
            {
                total.Text = intTotal.ToString();
                total.BackColor = SystemColors.Control;

                if (intTotal == 10)
                {
                    total.BackColor = Color.Orange;
                }
                else if (intTotal >= 7)
                {
                    total.BackColor = Color.Yellow;
                }
            }
            else
            {
                total.Text = "10";
            }
        }

        private void cboTeam1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadTeamMembers(sender, e);
        }

        private void cboTeam2_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadTeamMembers(sender, e);
        }

        private void loadTeamMembers(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            string selectedValue = ((ComboBoxItem)cb.SelectedItem).Value;
            SqlCeConnection conn = new SqlCeConnection(connectionString);
            System.Data.SqlServerCe.SqlCeCommand cmd;

            if (selectedValue != "0")
            {
                string homeOrAway = "Home";
                if (cb.Name == "cboTeam2") { homeOrAway = "Away"; }

                GroupBox g = new GroupBox();
                if (homeOrAway == "Home")
                {
                    g = groupHome;
                }
                else
                {
                    g = groupVisitor;
                }

                if ((homeOrAway == "Home") && (homeTeamWhiteToolStripMenuItem.Checked))
                {
                    g.BackColor = Color.White;
                }
                else
                {
                    #region Get team color from database
                    // Get team color from database
                    cmd = new System.Data.SqlServerCe.SqlCeCommand("Select Color from Teams where TeamID = " + selectedValue, conn);
                    conn.Open();
                    object scalarResult = cmd.ExecuteScalar();
                    conn.Close();

                    int argbColor = 0;

                    if (int.TryParse(scalarResult.ToString(), out argbColor))
                    {
                        g.BackColor = Color.FromArgb(argbColor);
                    }
                    else
                    {
                        g.BackColor = SystemColors.Control;
                    }
                    #endregion
                }

                // Get players from database
                conn = new SqlCeConnection(connectionString);
                cmd = new System.Data.SqlServerCe.SqlCeCommand("Select * from Players where TeamID = " + selectedValue + " order by JerseyNumber", conn);

                conn.Open();

                SqlCeDataReader dr = cmd.ExecuteReader();

                for (int i = 1; i <= 18; i++)
                {
                    TextBox jerseyNumber = (TextBox)Controls.Find(homeOrAway + "Number" + i, true)[0];
                    TextBox playerName = (TextBox)Controls.Find(homeOrAway + "Name" + i, true)[0];

                    if (dr.Read())
                    {
                        jerseyNumber.Text = dr["JerseyNumber"].ToString();
                        playerName.Text = dr["Name"].ToString();
                    }
                    else
                    {
                        jerseyNumber.Text = "";
                        playerName.Text = "";
                    }
                }

                dr.Close();
                conn.Close();
            }
            else
            {
                // Let them add a new team
                NewTeam n = new NewTeam(this, tsCboLeague, cb);
                n.Show();
            }
        }

        private void tsCboHalf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripComboBox)
            {
                ToolStripComboBox cb = (ToolStripComboBox)sender;
                cb.Owner.Hide();
            }

            ComboBoxItem cbi = (ComboBoxItem)tsCboHalf.SelectedItem;
            if (cbi.Value == "1")
            {
                for (int i = 1; i <= 18; i++)
                {
                    TextBox home1 = (TextBox)Controls.Find("HomeFoulFirst" + i, true)[0];
                    TextBox away1 = (TextBox)Controls.Find("AwayFoulFirst" + i, true)[0];
                    TextBox home2 = (TextBox)Controls.Find("HomeFoulSecond" + i, true)[0];
                    TextBox away2 = (TextBox)Controls.Find("AwayFoulSecond" + i, true)[0];

                    home1.Enabled = true;
                    away1.Enabled = true;
                    home2.Enabled = false;
                    away2.Enabled = false;
                }

                // Set the total box color
                HomeFoulSecondTotal.BackColor = SystemColors.Control;
                AwayFoulSecondTotal.BackColor = SystemColors.Control;
                setHalfTotals("Home", "First");
                setHalfTotals("Away", "First");

                // Disable the total counts for the other half
                HomeFoulFirstTotal.Enabled = true;
                AwayFoulFirstTotal.Enabled = true;
                HomeFoulSecondTotal.Enabled = false;
                AwayFoulSecondTotal.Enabled = false;

            }
            else if (cbi.Value == "2")
            {
                for (int i = 1; i <= 18; i++)
                {
                    TextBox home1 = (TextBox)Controls.Find("HomeFoulFirst" + i, true)[0];
                    TextBox away1 = (TextBox)Controls.Find("AwayFoulFirst" + i, true)[0];
                    TextBox home2 = (TextBox)Controls.Find("HomeFoulSecond" + i, true)[0];
                    TextBox away2 = (TextBox)Controls.Find("AwayFoulSecond" + i, true)[0];

                    home1.Enabled = false;
                    away1.Enabled = false;
                    home2.Enabled = true;
                    away2.Enabled = true;
                }

                // Set the total box color
                HomeFoulFirstTotal.BackColor = SystemColors.Control;
                AwayFoulFirstTotal.BackColor = SystemColors.Control;
                setHalfTotals("Home", "Second");
                setHalfTotals("Away", "Second");

                HomeFoulFirstTotal.Enabled = false;
                AwayFoulFirstTotal.Enabled = false;
                HomeFoulSecondTotal.Enabled = true;
                AwayFoulSecondTotal.Enabled = true;
            }

            SwitchSides();
            btnPossession_Click(sender, e);
        }

        private void switchSidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchSides();
        }

        private void SwitchSides()
        {
            if (groupHome.Location.X == 12)
            {
                groupHome.Location = new Point(this.Width - groupHome.Width - 28, 33);
                groupVisitor.Location = new Point(12, 33);

                groupHome.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
                groupVisitor.Anchor = (AnchorStyles.Left | AnchorStyles.Top);
            }
            else
            {
                groupHome.Location = new Point(12, 33);
                groupVisitor.Location = new Point(this.Width - groupVisitor.Width - 28, 33);

                groupHome.Anchor = (AnchorStyles.Left | AnchorStyles.Top);
                groupVisitor.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
            }
        }

        private void MarkDirty(object sender, KeyPressEventArgs e)
        {
            TextBox t = (TextBox)sender;
            t.BackColor = Color.LightCyan;

            if (t.Name.Contains("Home"))
            {
                btnSaveHome.Visible = true;
            }
            else
            {
                btnSaveAway.Visible = true;
            }
        }

        private void SavePlayers(object sender, EventArgs e)
        {
            SqlCeConnection conn = new SqlCeConnection(connectionString);
            System.Data.SqlServerCe.SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = conn;

            Button saveButton = (Button)sender;
            string homeOrAway = string.Empty;
            ComboBoxItem selectedTeam;

            if (saveButton.Name == "btnSaveHome")
            {
                selectedTeam = (ComboBoxItem)cboTeam1.SelectedItem;
                homeOrAway = "Home";
            }
            else
            {
                selectedTeam = (ComboBoxItem)cboTeam2.SelectedItem;
                homeOrAway = "Away";
            }

            string teamID = selectedTeam.Value;

            conn.Open();

            //Remove all players first
            cmd.CommandText = String.Format("Delete from Players where TeamID = '{0}'", teamID);
            cmd.ExecuteNonQuery();

            //HomeNumber1 - 18, HomeName1 - 18
            for (int i = 1; i <= 18; i++)
            {
                TextBox txtNumber = (TextBox)Controls.Find(homeOrAway + "Number" + i, true)[0];
                TextBox txtName = (TextBox)Controls.Find(homeOrAway + "Name" + i, true)[0];

                if ((txtNumber.Text.Trim() != string.Empty) || (txtName.Text.Trim() != string.Empty))
                {
                    cmd.CommandText = String.Format("INSERT INTO Players (TeamID,JerseyNumber,Name) VALUES ('{0}','{1}','{2}');", teamID, txtNumber.Text.Replace("'", "''"), txtName.Text.Replace("'", "''"));
                    try
                    {
                        cmd.ExecuteNonQuery();

                        txtName.BackColor = Color.White;
                        txtNumber.BackColor = Color.White;
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Exception thrown while inserting player record: " + ex.Message);
                    }
                    
                }
                else
                {
                    txtName.BackColor = Color.White;
                    txtNumber.BackColor = Color.White;
                } 
            }

            conn.Close();

            saveButton.Visible = false;
        }

        private void resetFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset all fouls and entered players?", "Reset form?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                for (int i = 1; i <= 18; i++)
                {
                    TextBox home1 = (TextBox)Controls.Find("HomeFoulFirst" + i, true)[0];
                    TextBox away1 = (TextBox)Controls.Find("AwayFoulFirst" + i, true)[0];
                    TextBox home2 = (TextBox)Controls.Find("HomeFoulSecond" + i, true)[0];
                    TextBox away2 = (TextBox)Controls.Find("AwayFoulSecond" + i, true)[0];
                    CheckBox homeIn = (CheckBox)Controls.Find("HomeEntered" + i, true)[0];
                    CheckBox awayIn = (CheckBox)Controls.Find("AwayEntered" + i, true)[0];

                    home1.Text = "";
                    away1.Text = "";
                    home2.Text = "";
                    away2.Text = "";
                    homeIn.Checked = false;
                    awayIn.Checked = false;
                }
            }
        }

        private void setTeamColorOLD(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ToolStripMenuItem t = (ToolStripMenuItem)sender;
                Color c = colorDialog1.Color;
                GroupBox g = new GroupBox();
                string strTeamId = string.Empty;
                ComboBoxItem cbi = new ComboBoxItem();

                if (t.Name == "setHomeColorToolStripMenuItem")
                {
                    g = groupHome;
                    cbi = (ComboBoxItem)cboTeam1.SelectedItem;
                }
                else
                {
                    g = groupVisitor;
                    cbi = (ComboBoxItem)cboTeam2.SelectedItem;
                }

                g.BackColor = c;

                try
                {
                    strTeamId = cbi.Value;
                    ///TODO: Save to database (Update Teams set color = {c} where TeamID = {strTeamId}
                }
                catch (Exception ex)
                {
                    Debug.Print("Exception thrown while attempting to save color: " + ex.Message);
                }
            }
        }

        private void setTeamColor(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                ToolStripMenuItem t = (ToolStripMenuItem)sender;
                Color color = colorDialog1.Color;
                GroupBox g = new GroupBox();
                string strTeamId = string.Empty;
                ComboBoxItem cbi = new ComboBoxItem();

                if ((t.Name == "setHomeColorToolStripMenuItem") || (t.Name == "setHomeColorToolStripMenuItem1"))
                {
                    g = groupHome;
                    cbi = (ComboBoxItem)cboTeam1.SelectedItem;
                }
                else
                {
                    g = groupVisitor;
                    cbi = (ComboBoxItem)cboTeam2.SelectedItem;
                }
                g.BackColor = color;

                try
                {
                    strTeamId = cbi.Value;
                    //Save to database (Update Teams set color = {c} where TeamID = {strTeamId}

                    SqlCeConnection conn = new SqlCeConnection(connectionString);
                    SqlCeCommand cmd = new SqlCeCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = String.Format("UPDATE Teams SET Color = '{0}' WHERE TeamID = '{1}';", color.ToArgb().ToString(), strTeamId);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    cmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.Print("Exception thrown while attempting to save color: " + ex.Message);
                }
            }
        }

        private void homeTeamWhiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = (ToolStripMenuItem)sender;
            i.Checked = !i.Checked;
        }

        private void homeTeamWhiteToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            bool homeTeamInWhite = homeTeamWhiteToolStripMenuItem.Checked;

            GroupBox g = groupHome;

            if (homeTeamInWhite)
            {
                g.BackColor = Color.White;
            }
            else
            {
                #region Get team color from database

                string selectedValue = ((ComboBoxItem)cboTeam1.SelectedItem).Value;

                // Get team color from database
                SqlCeConnection conn = new SqlCeConnection(connectionString);
                System.Data.SqlServerCe.SqlCeCommand cmd = new System.Data.SqlServerCe.SqlCeCommand("Select Color from Teams where TeamID = " + selectedValue, conn);
                conn.Open();
                object scalarResult = cmd.ExecuteScalar();
                conn.Close();

                int argbColor = 0;

                if (int.TryParse(scalarResult.ToString(), out argbColor))
                {
                    g.BackColor = Color.FromArgb(argbColor);
                }
                else
                {
                    g.BackColor = SystemColors.Control;
                }
                #endregion
            }
        }

        private void btnPossession_Click(object sender, EventArgs e)
        {
            if (btnPossession.Text == "←")
            {
                btnPossession.Text = "→";
            }
            else
            {
                btnPossession.Text = "←";
            }
        }

        private void changeHalfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsCboHalf.SelectedIndex == 0)
            {
                tsCboHalf.SelectedIndex = 1;
            }
            else
            {
                tsCboHalf.SelectedIndex = 0;
            }
        }
    }
}
