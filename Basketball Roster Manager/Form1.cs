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
        private static string appDataFolder = "Carroll Sports Media";
        private static string appDataFile = "Rosters.sdf";
        private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static string connectionPath = Path.Combine(appDataPath, appDataFolder, appDataFile);
        public static string connectionString = "Data Source=" + connectionPath;

        private int intTimeoutSeconds = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            if (verifyDataPath(out errorMessage))
            {
                loadHalves();
                loadLeagues();
                tsCboLeague_SelectedIndexChanged(sender, e);
                tsCboHalf_SelectedIndexChanged(sender, e);
                setRosterFormChanges(false, true);
            }
            else
            {
                MessageBox.Show(errorMessage + "Closing application.", "Closing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private bool verifyDataPath(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (File.Exists(connectionPath))
            {
                return true;
            }
            else
            {
                if (MessageBox.Show("The rosters data file was not found.  Create a new data file to maintain data?", "Create new file?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (createNewDatabase(out errorMessage))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    } 
                }
                else
                {
                    errorMessage = "User opted to not create new database file.  ";
                    return false;
                }
            }
        }

        private bool createNewDatabase(out string errorMessage)
        {
            errorMessage = string.Empty;

            SqlCeEngine engine = new SqlCeEngine(connectionString);
            engine.CreateDatabase();

            string[] strSql = {"CREATE TABLE Leagues ( LeagueID int IDENTITY(1,1) PRIMARY KEY, LeagueName nvarchar(100) );",
"CREATE TABLE Players ( PlayerID int IDENTITY(1,1) PRIMARY KEY, TeamID int NOT NULL, GraduatingClass nvarchar(100), JerseyNumber nvarchar(3), Name nvarchar(100) );",
"CREATE TABLE Teams ( TeamID int IDENTITY(1,1) PRIMARY KEY, LeagueID int NOT NULL, TeamName nvarchar(100), Color nvarchar(100) );",
"INSERT INTO Leagues (LeagueName) VALUES ('Boys varsity');",
"INSERT INTO Leagues (LeagueName) VALUES ('Girls varsity');"};

            SqlCeConnection conn = new SqlCeConnection(connectionString);
            System.Data.SqlServerCe.SqlCeCommand cmd = new System.Data.SqlServerCe.SqlCeCommand();
            cmd.Connection = conn;

            try
            {
                conn.Open();
                foreach (string s in strSql)
                {
                    cmd.CommandText = s;
                    cmd.ExecuteNonQuery();
                }
                
                conn.Close();

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Exception thrown while populating new database: " + ex.Message + "  ";
                return false;
            }
        }

        public void loadHalves()
        {
            /// This is dynamic because the IDE only allows items in the Items collection to be strings, 
            /// and assigns the same string to the Name and Value properties. Doing it this way allows me to 
            /// have different values for those two properties.
            tsCboHalf.Items.Add(new ComboBoxItem("First half", "1"));
            tsCboHalf.Items.Add(new ComboBoxItem("Second half", "2"));
            tsCboHalf.SelectedIndex = 0;
        }

        public void loadLeagues()
        {
            loadLeagues(-1);
        }

        public void loadLeagues(int selectedItemID)
        {
            ToolStripComboBox comboToLoad = tsCboLeague;

            try
            {
                // Load teams from this league
                SqlCeConnection conn = new SqlCeConnection(connectionString);
                System.Data.SqlServerCe.SqlCeCommand cmd = new System.Data.SqlServerCe.SqlCeCommand("Select * from Leagues order by LeagueName", conn);
                conn.Open();

                SqlCeDataReader dr = cmd.ExecuteReader();

                comboToLoad.Items.Clear();

                while (dr.Read())
                {
                    comboToLoad.Items.Add(new ComboBoxItem(dr["LeagueName"], dr["LeagueID"]));
                }

                dr.Close();
                conn.Close();

                if (selectedItemID != -1)
                {
                    for (int i = 0; i < comboToLoad.Items.Count; i++)
                    {
                        ComboBoxItem cbi = (ComboBoxItem)comboToLoad.Items[i];
                        if (int.Parse(cbi.Value) == selectedItemID)
                        {
                            comboToLoad.SelectedIndex = i;
                            break;
                        }
                    }
                }

                comboToLoad.Items.Add(new ComboBoxItem("Add new", "0"));

                setRosterFormChanges(false, true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Unable to load leagues: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //public void loadLeagues(string selectedLeagueText)
        //{
        //    loadLeagues();
        //}

        private void tsCboLeague_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Make sure this is being fired b/c of someone clicking on the ToolStripCombo
            if (sender is ToolStripComboBox)
            {
                ToolStripComboBox cb = (ToolStripComboBox)sender;
                cb.Owner.Hide();

                if (cb.ComboBox.SelectedItem != null)
                {
                    ComboBoxItem cbi = (ComboBoxItem)cb.ComboBox.SelectedItem;

                    if (cbi.Value != "0")
                    {
                        int leagueID = 0;

                        if (int.TryParse(cbi.Value, out leagueID))
                        {
                            loadTeams(cboTeam1, leagueID);
                            loadTeams(cboTeam2, leagueID);
                        }
                        else
                        {
                            MessageBox.Show("Unble to parse league ID from combobox selected value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        // Let them add a new league
                        NewLeague n = new NewLeague(this, tsCboLeague);
                        n.Show();
                    }
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
                        ComboBoxItem cbi = (ComboBoxItem)comboToLoad.Items[i];
                        if (int.Parse(cbi.Value) == selectedTeamID)
                        {
                            comboToLoad.SelectedIndex = i;
                            break;
                        }
                    }
                }

                comboToLoad.Items.Add(new ComboBoxItem("Add new", "0"));

                setRosterFormChanges(false, true);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Unble to load teams from league: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            ComboBox cb = (ComboBox)sender;
            loadTeamMembers(cb, e);
        }

        private void cboTeam2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            loadTeamMembers(cb, e);
        }

        private void loadTeamMembers(ComboBox cb, EventArgs e)
        {
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

                setRosterFormChanges(true, false);
                changeHalf(false);
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

            changeHalf();
        }

        private void changeHalf(bool switchSides)
        {
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

            if (switchSides)
            {
                SwitchSides();
                changePossession();
            }
        }

        private void changeHalf()
        {
            changeHalf(true);
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
            ComboBox cboTeam;
            ComboBoxItem selectedTeam;

            if (saveButton.Name == "btnSaveHome")
            {
                cboTeam = cboTeam1;
                homeOrAway = "Home";
            }
            else
            {
                cboTeam = cboTeam2;
                homeOrAway = "Away";
            }

            selectedTeam = (ComboBoxItem)cboTeam.SelectedItem;
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
                        MessageBox.Show("Exception thrown while inserting player record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Issue 20: Prompt to re-sort the team upon save
            TextBox f1 = (TextBox)Controls.Find(homeOrAway + "FoulFirstTotal", true)[0];
            TextBox f2 = (TextBox)Controls.Find(homeOrAway + "FoulSecondTotal", true)[0];

            if ((addFoulTotals(f1, f2) == 0) || (MessageBox.Show("Changes saved.\r\n\r\nDo you want to re-sort the roster numerically?\r\n(Not recommended if you have recorded fouls.)", "Re-sort roster?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes))
            {
                loadTeamMembers(cboTeam, new EventArgs());
            }
        }

        private int addFoulTotals(TextBox txtFoulBox1, TextBox txtFoulBox2)
        {
            int intTotal = 0;
            int intFoulBox1 = 0;
            int intFoulBox2 = 0;

            if (!int.TryParse(txtFoulBox1.Text, out intFoulBox1))
            {
                intFoulBox1 = 0;
            }
            if (!int.TryParse(txtFoulBox2.Text, out intFoulBox2))
            {
                intFoulBox2 = 0;
            }

            intTotal = intFoulBox1 + intFoulBox2;
            return intTotal;
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
                    MessageBox.Show("Exception thrown while attempting to save color: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPossession_Click(object sender, EventArgs e)
        {
            changePossession();
        }

        private void changePossession()
        {
            if (btnPossession.Text == "←")
            {
                btnPossession.Text = "→";
                btnPossession.Image = (Image)Properties.Resources.ResourceManager.GetObject("right");
            }
            else
            {
                btnPossession.Text = "←";
                btnPossession.Image = (Image)Properties.Resources.ResourceManager.GetObject("left");
            }
        }

        private void setRosterFormChanges(bool enabled, bool clear)
        {
            foreach (string homeOrAway in new string[] { "Home", "Away" })
            {
                for (int i = 1; i <= 18; i++)
                {
                    TextBox txtNumber = (TextBox)Controls.Find(homeOrAway + "Number" + i, true)[0];
                    TextBox txtName = (TextBox)Controls.Find(homeOrAway + "Name" + i, true)[0];
                    TextBox txtFoulFirst = (TextBox)Controls.Find(homeOrAway + "FoulFirst" + i, true)[0];
                    TextBox txtFoulSecond = (TextBox)Controls.Find(homeOrAway + "FoulSecond" + i, true)[0];

                    txtNumber.Enabled = enabled;
                    txtName.Enabled = enabled;
                    txtFoulFirst.Enabled = enabled;
                    txtFoulSecond.Enabled = enabled;

                    if (clear)
                    {
                        txtNumber.Text = "";
                        txtName.Text = "";
                        txtFoulFirst.Text = "";
                        txtFoulSecond.Text = "";
                    }
                }
            }

        }

        private void btnTimeout30_Click(object sender, EventArgs e)
        {
            setTimer(30);
        }

        private void btnTimeout60_Click(object sender, EventArgs e)
        {
            setTimer(60);
        }

        private void setTimer(int seconds)
        {
            intTimeoutSeconds = seconds;
            txtTimeout.Text = "0:" + String.Format("{0:D2}", intTimeoutSeconds);
            timerTimeout.Start();
        }

        private void timerTimeout_Tick(object sender, EventArgs e)
        {
            intTimeoutSeconds--;
            txtTimeout.Text = "0:" + String.Format("{0:D2}", intTimeoutSeconds);

            if (intTimeoutSeconds == 0) { timerTimeout.Stop(); }
        }

        private void btnStopTimer_Click(object sender, EventArgs e)
        {
            timerTimeout.Stop();
            txtTimeout.Text = "0:00";
        }

        // ============  AUTO WIRED-UP EVENTS ============================================

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

        private void switchSidesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SwitchSides();
        }
    }
}
