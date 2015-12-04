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
        public static string dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.CompanyName, Application.ProductName);
        public static string dataFile = "Rosters.sdf";
        public static string dataPath = Path.Combine(dataFolder, dataFile);
        public static string connectionString = "Data Source=" + dataPath + ";Persist Security Info=False;";

        private int mostRecentHomeTeamID = -1;
        private int mostRecentAwayTeamID = -1;

        private Tips tips = new Tips();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (verifyDataPath())
            {
                loadHalves();
                loadLeagues();
                tsCboLeague_SelectedIndexChanged(sender, e);
                tsCboHalf_SelectedIndexChanged(sender, e);
            }   
            else
            {
                MessageBox.Show("Data file not found.  Closing application.", "Closing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void foulTextBox_enter(object sender, EventArgs e)
        {
            if (tips.showFoulCountTip)
            {
                TextBox textBox = (TextBox)sender;
                tips.toolTip.Show("Tip: Double-click this field to add a foul", textBox, textBox.Width, -12, 3000);
            }
        }

        private void foulTextBox_leave(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            tips.toolTip.Hide(textBox);
        }

        private bool verifyDataPath()
        {
            if (File.Exists(dataPath))
            {
                return true;
            }
            else
            {
                if (MessageBox.Show("The rosters data file was not found.  Create a new data file to maintain data?", "Create new file?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    // Verify folder exists
                    if (!Directory.Exists(dataFolder))
                    {
                        Directory.CreateDirectory(dataFolder);
                    }

                    // Create new data file
                    bool allIsWell = true;
                    Exception exception = new Exception();

                    try
                    {
                        SqlCeEngine en = new SqlCeEngine(connectionString);
                        en.CreateDatabase();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        allIsWell = false;
                    }

                    if (allIsWell)
                    {
                        string[] sqlCreate = {"Create table Leagues (LeagueID int IDENTITY(1,1) PRIMARY KEY, LeagueName nvarchar(100));",
                            "Create table Teams (TeamID int IDENTITY(1,1) PRIMARY KEY, LeagueID int, TeamName nvarchar(100), Color nvarchar(100));",
                            "Create table Players (PlayerID int IDENTITY(1,1) PRIMARY KEY, TeamID int, GraduatingClass nvarchar(100), JerseyNumber nvarchar(3), Name nvarchar(100));",
                            "INSERT INTO Leagues (LeagueName) VALUES ('NCAA - men');",
                            "INSERT INTO Leagues (LeagueName) VALUES ('NCAA - women');",
                            "INSERT INTO Leagues (LeagueName) VALUES ('Varsity - boys');",
                            "INSERT INTO Leagues (LeagueName) VALUES ('Varsity - girls');"
                        };

                        SqlCeConnection conn = new SqlCeConnection(connectionString);
                        System.Data.SqlServerCe.SqlCeCommand cmd = new SqlCeCommand();
                        cmd.Connection = conn;
                        conn.Open();

                        for (int i = 0; i < sqlCreate.Length; i++)
                        {
                            cmd.CommandText = sqlCreate[i];
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                allIsWell = false;
                                exception = ex;
                            }
                        }
                        conn.Close();
                    }

                    if (allIsWell)
                    {
                        return true;
                    }
                    else
                    {
                        if (File.Exists(dataPath))
                        {
                            File.Delete(dataPath);
                        }

                        MessageBox.Show("The following error occurred creating the database:\r\n\r\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }


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
            // Load leagues
            SqlCeConnection conn = new SqlCeConnection(connectionString);
            System.Data.SqlServerCe.SqlCeCommand cmd = new System.Data.SqlServerCe.SqlCeCommand("Select * from Leagues order by LeagueName", conn);
            conn.Open();

            SqlCeDataReader dr = cmd.ExecuteReader();

            tsCboLeague.Items.Clear();

            while (dr.Read())
            {
                tsCboLeague.Items.Add(new ComboBoxItem(dr["LeagueName"], dr["LeagueID"]));
            }

            dr.Close();
            conn.Close();

            tsCboLeague.Items.Add(new ComboBoxItem("Add new", "0"));
        }

        public void loadLeagues(int selectedLeagueID)
        {
            loadLeagues();

            if (selectedLeagueID != -1)
            {
                for (int i = 0; i < tsCboLeague.Items.Count; i++)
                {
                    ComboBoxItem cbi = (ComboBoxItem)tsCboLeague.Items[i];
                    if (int.Parse(cbi.Value) == selectedLeagueID)
                    {
                        tsCboLeague.SelectedIndex = i;

                        if (selectedLeagueID > 0)
                        {
                            loadTeams(cboTeam1, selectedLeagueID);
                            loadTeams(cboTeam2, selectedLeagueID);
                        }
                        break;
                    }
                }
            }
        }

        private void tsCboLeague_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ToolStripComboBox)
            {
                ToolStripComboBox cb = (ToolStripComboBox)sender;
                cb.Owner.Hide();
                
                string selectedValue = ((ComboBoxItem)cb.SelectedItem).Value;

                if (selectedValue != "0")
                {
                    int leagueID = int.Parse(selectedValue);

                    loadTeams(cboTeam1, leagueID);
                    loadTeams(cboTeam2, leagueID);
                }
                else
                {
                    // Open new league dialog
                    NewLeague n = new NewLeague(this, tsCboLeague);
                    n.Show();
                }
            }
        }

        public void loadTeams(ComboBox comboToLoad, int leagueID)
        {
            loadTeams(comboToLoad, leagueID, -1);
        }

        public void loadTeams(ComboBox comboToLoad, int leagueID, int selectedTeamID)
        {
            ///TODO: If fouls or entered players have been recorded, confirm league change first

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
                    setComboBoxValue(comboToLoad, selectedTeamID);
                }
                else
                {
                    //Make sure all the player and foul fields are empty
                    resetAllForm();
                }

                comboToLoad.Items.Add(new ComboBoxItem("Add new", "0"));

            }
            catch (System.Exception ex)
            {
                Debug.Print("Unable to load teams from league: " + ex.Message);
            }

        }

        private void setComboBoxValue(ComboBox comboBox, int value)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                ComboBoxItem cbi = (ComboBoxItem)comboBox.Items[i];
                if (int.Parse(cbi.Value) == value)
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void tsMenuItemSwitchSides_Click(object sender, EventArgs e)
        {
            SwitchSides();
        }

        private void FoulTextbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tips.showFoulCountTip = false;

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
            //cboTeam_SelectedIndexChanging(sender, e);
            loadTeamMembers(sender, e);
        }

        private void cboTeam2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cboTeam_SelectedIndexChanging(sender, e);
            loadTeamMembers(sender, e);
        }

        private void cboTeam_SelectedIndexChanging(object sender, EventArgs e)
        {
            ///TODO: If fouls or entered players have been recorded, confirm team change first
            if (formHasBeenUsed())
            {
                if (MessageBox.Show("You have recorded fouls and players who have entered the game.  If you change the team now, you will lose that information.\r\n\r\nAre you sure you want to proceed?", "Discard changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                {
                    mostRecentAwayTeamID = int.Parse(((ComboBoxItem)cboTeam2.SelectedItem).Value);
                    loadTeamMembers(sender, e);
                }
                else
                {
                    setComboBoxValue(cboTeam2, mostRecentHomeTeamID);
                }
            }
            else
            {
                mostRecentAwayTeamID = int.Parse(((ComboBoxItem)cboTeam2.SelectedItem).Value);
                loadTeamMembers(sender, e);
            }

            loadTeamMembers(sender, e);
        }

        private void loadTeamMembers(object sender, EventArgs e)
        {
            // Upon team change, reset fouls and totals
            resetFoulsAndActivePlayers(false);

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

        private bool formHasBeenUsed()
        {
            bool rtn = false;

            if ((HomeFoulFirstTotal.Text != string.Empty) && (HomeFoulFirstTotal.Text != "0")) { return true; }
            if ((AwayFoulFirstTotal.Text != string.Empty) && (AwayFoulFirstTotal.Text != "0")) { return true; }
            if ((HomeFoulSecondTotal.Text != string.Empty) && (HomeFoulSecondTotal.Text != "0")) { return true; }
            if ((AwayFoulSecondTotal.Text != string.Empty) && (AwayFoulSecondTotal.Text != "0")) { return true; }

            for (int i = 1; i <= 18; i++)
            {
                if (((CheckBox)Controls.Find("HomeEntered" + i, true)[0]).Checked) { return true; }
                if (((CheckBox)Controls.Find("AwayEntered" + i, true)[0]).Checked) { return true; }
            }

            return rtn;
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

            if (!formHasBeenUsed())
            {
                // Resort the players numerically

                ComboBox cboTeam;
                if (homeOrAway == "Home")
                {
                    cboTeam = cboTeam1;
                }
                else
                {
                    cboTeam = cboTeam2;
                }
                EventArgs eventArgs = new EventArgs();

                loadTeamMembers(cboTeam, eventArgs);
            }
        }

        private void resetFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset all fouls and entered players?", "Reset form?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                resetFoulsAndActivePlayers(true);
            }
        }

        private void resetFoulsAndActivePlayers(bool requestConfirmation)
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

        private void resetAllForm()
        {
            for (int i = 1; i <= 18; i++)
            {
                ((CheckBox)Controls.Find("HomeEntered" + i, true)[0]).Checked = false;
                ((TextBox)Controls.Find("HomeNumber" + i, true)[0]).Text = "";
                ((TextBox)Controls.Find("HomeName" + i, true)[0]).Text = "";
                ((TextBox)Controls.Find("HomeFoulFirst" + i, true)[0]).Text = "";
                ((TextBox)Controls.Find("HomeFoulSecond" + i, true)[0]).Text = "";
                ((TextBox)Controls.Find("HomeFoulTotal" + i, true)[0]).Text = "";

                ((CheckBox)Controls.Find("AwayEntered" + i, true)[0]).Checked = false;
                ((TextBox)Controls.Find("AwayNumber" + i, true)[0]).Text = "";
                ((TextBox)Controls.Find("AwayName" + i, true)[0]).Text = "";
                ((TextBox)Controls.Find("AwayFoulFirst" + i, true)[0]).Text = "";
                ((TextBox)Controls.Find("AwayFoulSecond" + i, true)[0]).Text = "";
                ((TextBox)Controls.Find("AwayFoulTotal" + i, true)[0]).Text = "";
            }

            //Reset home and away colors
            groupHome.BackColor = SystemColors.Control;
            groupVisitor.BackColor = SystemColors.Control;
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
            var strArrowDirection = "";

            if (btnPossession.Text == "←")
            {
                btnPossession.Text = "→";
                strArrowDirection = "right";
            }
            else
            {
                btnPossession.Text = "←";
                strArrowDirection = "left";
            }

            object objImage = Basketball_Roster_Manager.Properties.Resources.ResourceManager.GetObject(strArrowDirection);
            btnPossession.Image = (Image)objImage;
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

        private void exportDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            DialogResult dialogResult = saveFileDialog1.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                File.Copy(dataPath, saveFileDialog1.FileName, true);
            }
        }

        private void importDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            DialogResult dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (MessageBox.Show("Are you sure you want to import this data file?\r\nThis will replace your current data file.  This action can not be undone.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    File.Copy(openFileDialog1.FileName, dataPath, true);
                }
            }
        }
    }
}
