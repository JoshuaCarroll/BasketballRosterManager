namespace Basketball_Roster_Manager
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.cboTeam1 = new System.Windows.Forms.ComboBox();
            this.teamContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.setHomeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAwayColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.switchSidesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeHalfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupHome = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnSaveHome = new System.Windows.Forms.Button();
            this.HomeFoulSecondTotal = new System.Windows.Forms.TextBox();
            this.HomeFoulFirstTotal = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setHomeColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.setAwayColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsMenuItemSwitchSides = new System.Windows.Forms.ToolStripMenuItem();
            this.resetFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.homeTeamWhiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exportDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leagueToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsCboLeague = new System.Windows.Forms.ToolStripComboBox();
            this.halfToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsCboHalf = new System.Windows.Forms.ToolStripComboBox();
            this.groupVisitor = new System.Windows.Forms.GroupBox();
            this.btnSaveAway = new System.Windows.Forms.Button();
            this.AwayFoulSecondTotal = new System.Windows.Forms.TextBox();
            this.AwayFoulFirstTotal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.cboTeam2 = new System.Windows.Forms.ComboBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnPossession = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.teamContextMenuStrip.SuspendLayout();
            this.groupHome.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupVisitor.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboTeam1
            // 
            this.cboTeam1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTeam1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTeam1.FormattingEnabled = true;
            this.cboTeam1.Location = new System.Drawing.Point(12, 37);
            this.cboTeam1.Margin = new System.Windows.Forms.Padding(6);
            this.cboTeam1.Name = "cboTeam1";
            this.cboTeam1.Size = new System.Drawing.Size(996, 63);
            this.cboTeam1.TabIndex = 2;
            this.cboTeam1.SelectedIndexChanged += new System.EventHandler(this.cboTeam1_SelectedIndexChanged);
            // 
            // teamContextMenuStrip
            // 
            this.teamContextMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.teamContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setHomeColorToolStripMenuItem,
            this.setAwayColorToolStripMenuItem,
            this.toolStripSeparator3,
            this.switchSidesToolStripMenuItem,
            this.changeHalfToolStripMenuItem});
            this.teamContextMenuStrip.Name = "teamContextMenuStrip";
            this.teamContextMenuStrip.Size = new System.Drawing.Size(254, 162);
            // 
            // setHomeColorToolStripMenuItem
            // 
            this.setHomeColorToolStripMenuItem.Name = "setHomeColorToolStripMenuItem";
            this.setHomeColorToolStripMenuItem.Size = new System.Drawing.Size(253, 38);
            this.setHomeColorToolStripMenuItem.Text = "Set home color";
            this.setHomeColorToolStripMenuItem.Click += new System.EventHandler(this.setTeamColor);
            // 
            // setAwayColorToolStripMenuItem
            // 
            this.setAwayColorToolStripMenuItem.Name = "setAwayColorToolStripMenuItem";
            this.setAwayColorToolStripMenuItem.Size = new System.Drawing.Size(253, 38);
            this.setAwayColorToolStripMenuItem.Text = "Set away color";
            this.setAwayColorToolStripMenuItem.Click += new System.EventHandler(this.setTeamColor);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(250, 6);
            // 
            // switchSidesToolStripMenuItem
            // 
            this.switchSidesToolStripMenuItem.Name = "switchSidesToolStripMenuItem";
            this.switchSidesToolStripMenuItem.Size = new System.Drawing.Size(253, 38);
            this.switchSidesToolStripMenuItem.Text = "Switch sides";
            this.switchSidesToolStripMenuItem.Click += new System.EventHandler(this.switchSidesToolStripMenuItem_Click);
            // 
            // changeHalfToolStripMenuItem
            // 
            this.changeHalfToolStripMenuItem.Name = "changeHalfToolStripMenuItem";
            this.changeHalfToolStripMenuItem.Size = new System.Drawing.Size(253, 38);
            this.changeHalfToolStripMenuItem.Text = "Change half";
            this.changeHalfToolStripMenuItem.Click += new System.EventHandler(this.changeHalfToolStripMenuItem_Click);
            // 
            // groupHome
            // 
            this.groupHome.Controls.Add(this.label22);
            this.groupHome.Controls.Add(this.label19);
            this.groupHome.Controls.Add(this.label20);
            this.groupHome.Controls.Add(this.label21);
            this.groupHome.Controls.Add(this.label18);
            this.groupHome.Controls.Add(this.label15);
            this.groupHome.Controls.Add(this.btnSaveHome);
            this.groupHome.Controls.Add(this.HomeFoulSecondTotal);
            this.groupHome.Controls.Add(this.HomeFoulFirstTotal);
            this.groupHome.Controls.Add(this.label9);
            this.groupHome.Controls.Add(this.label8);
            this.groupHome.Controls.Add(this.label7);
            this.groupHome.Controls.Add(this.label6);
            this.groupHome.Controls.Add(this.label4);
            this.groupHome.Controls.Add(this.label3);
            this.groupHome.Controls.Add(this.label2);
            this.groupHome.Controls.Add(this.cboTeam1);
            this.groupHome.Location = new System.Drawing.Point(24, 63);
            this.groupHome.Margin = new System.Windows.Forms.Padding(6, 0, 6, 2);
            this.groupHome.Name = "groupHome";
            this.groupHome.Padding = new System.Windows.Forms.Padding(6);
            this.groupHome.Size = new System.Drawing.Size(1294, 1431);
            this.groupHome.TabIndex = 4;
            this.groupHome.TabStop = false;
            this.groupHome.Text = "Home";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(12, 154);
            this.label15.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(30, 26);
            this.label15.TabIndex = 105;
            this.label15.Text = "In";
            // 
            // btnSaveHome
            // 
            this.btnSaveHome.Location = new System.Drawing.Point(59, 1366);
            this.btnSaveHome.Margin = new System.Windows.Forms.Padding(6);
            this.btnSaveHome.Name = "btnSaveHome";
            this.btnSaveHome.Size = new System.Drawing.Size(150, 44);
            this.btnSaveHome.TabIndex = 104;
            this.btnSaveHome.Text = "Save team";
            this.btnSaveHome.UseVisualStyleBackColor = true;
            this.btnSaveHome.Visible = false;
            this.btnSaveHome.Click += new System.EventHandler(this.SavePlayers);
            // 
            // HomeFoulSecondTotal
            // 
            this.HomeFoulSecondTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HomeFoulSecondTotal.Location = new System.Drawing.Point(775, 1369);
            this.HomeFoulSecondTotal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 2);
            this.HomeFoulSecondTotal.Name = "HomeFoulSecondTotal";
            this.HomeFoulSecondTotal.ReadOnly = true;
            this.HomeFoulSecondTotal.Size = new System.Drawing.Size(72, 44);
            this.HomeFoulSecondTotal.TabIndex = 102;
            // 
            // HomeFoulFirstTotal
            // 
            this.HomeFoulFirstTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HomeFoulFirstTotal.Location = new System.Drawing.Point(688, 1369);
            this.HomeFoulFirstTotal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 2);
            this.HomeFoulFirstTotal.Name = "HomeFoulFirstTotal";
            this.HomeFoulFirstTotal.ReadOnly = true;
            this.HomeFoulFirstTotal.Size = new System.Drawing.Size(72, 44);
            this.HomeFoulFirstTotal.TabIndex = 101;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(567, 1373);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(105, 37);
            this.label9.TabIndex = 100;
            this.label9.Text = "Totals";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(864, 154);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 26);
            this.label8.TabIndex = 9;
            this.label8.Text = "Total";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(769, 154);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 26);
            this.label7.TabIndex = 8;
            this.label7.Text = "2nd Half";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(681, 154);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 26);
            this.label6.TabIndex = 7;
            this.label6.Text = "1st Half";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(681, 114);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(95, 0, 95, 0);
            this.label4.Size = new System.Drawing.Size(257, 27);
            this.label4.TabIndex = 6;
            this.label4.Text = "Fouls";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(154, 154);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 26);
            this.label3.TabIndex = 5;
            this.label3.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(54, 154);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 26);
            this.label2.TabIndex = 4;
            this.label2.Text = "Number";
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.leagueToolStripMenuItem1,
            this.halfToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2824, 40);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setHomeColorToolStripMenuItem1,
            this.setAwayColorToolStripMenuItem1,
            this.toolStripSeparator1,
            this.tsMenuItemSwitchSides,
            this.resetFormToolStripMenuItem,
            this.toolStripSeparator2,
            this.homeTeamWhiteToolStripMenuItem,
            this.toolStripSeparator4,
            this.exportDatabaseToolStripMenuItem,
            this.importDatabaseToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(119, 36);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // setHomeColorToolStripMenuItem1
            // 
            this.setHomeColorToolStripMenuItem1.Name = "setHomeColorToolStripMenuItem1";
            this.setHomeColorToolStripMenuItem1.Size = new System.Drawing.Size(367, 44);
            this.setHomeColorToolStripMenuItem1.Text = "Set home color";
            // 
            // setAwayColorToolStripMenuItem1
            // 
            this.setAwayColorToolStripMenuItem1.Name = "setAwayColorToolStripMenuItem1";
            this.setAwayColorToolStripMenuItem1.Size = new System.Drawing.Size(367, 44);
            this.setAwayColorToolStripMenuItem1.Text = "Set away color";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(364, 6);
            // 
            // tsMenuItemSwitchSides
            // 
            this.tsMenuItemSwitchSides.Name = "tsMenuItemSwitchSides";
            this.tsMenuItemSwitchSides.Size = new System.Drawing.Size(367, 44);
            this.tsMenuItemSwitchSides.Text = "Switch sides";
            this.tsMenuItemSwitchSides.Click += new System.EventHandler(this.tsMenuItemSwitchSides_Click);
            // 
            // resetFormToolStripMenuItem
            // 
            this.resetFormToolStripMenuItem.Name = "resetFormToolStripMenuItem";
            this.resetFormToolStripMenuItem.Size = new System.Drawing.Size(367, 44);
            this.resetFormToolStripMenuItem.Text = "Reset form";
            this.resetFormToolStripMenuItem.Click += new System.EventHandler(this.resetFormToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(364, 6);
            // 
            // homeTeamWhiteToolStripMenuItem
            // 
            this.homeTeamWhiteToolStripMenuItem.Checked = true;
            this.homeTeamWhiteToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.homeTeamWhiteToolStripMenuItem.Name = "homeTeamWhiteToolStripMenuItem";
            this.homeTeamWhiteToolStripMenuItem.Size = new System.Drawing.Size(367, 44);
            this.homeTeamWhiteToolStripMenuItem.Text = "Home team in white";
            this.homeTeamWhiteToolStripMenuItem.CheckedChanged += new System.EventHandler(this.homeTeamWhiteToolStripMenuItem_CheckedChanged);
            this.homeTeamWhiteToolStripMenuItem.Click += new System.EventHandler(this.homeTeamWhiteToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(364, 6);
            // 
            // exportDatabaseToolStripMenuItem
            // 
            this.exportDatabaseToolStripMenuItem.Name = "exportDatabaseToolStripMenuItem";
            this.exportDatabaseToolStripMenuItem.Size = new System.Drawing.Size(367, 44);
            this.exportDatabaseToolStripMenuItem.Text = "Export database";
            this.exportDatabaseToolStripMenuItem.Click += new System.EventHandler(this.exportDatabaseToolStripMenuItem_Click);
            // 
            // importDatabaseToolStripMenuItem
            // 
            this.importDatabaseToolStripMenuItem.Name = "importDatabaseToolStripMenuItem";
            this.importDatabaseToolStripMenuItem.Size = new System.Drawing.Size(367, 44);
            this.importDatabaseToolStripMenuItem.Text = "Import database";
            this.importDatabaseToolStripMenuItem.Click += new System.EventHandler(this.importDatabaseToolStripMenuItem_Click);
            // 
            // leagueToolStripMenuItem1
            // 
            this.leagueToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsCboLeague});
            this.leagueToolStripMenuItem1.Name = "leagueToolStripMenuItem1";
            this.leagueToolStripMenuItem1.Size = new System.Drawing.Size(112, 36);
            this.leagueToolStripMenuItem1.Text = "League";
            // 
            // tsCboLeague
            // 
            this.tsCboLeague.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsCboLeague.Name = "tsCboLeague";
            this.tsCboLeague.Size = new System.Drawing.Size(300, 40);
            this.tsCboLeague.ToolTipText = "League";
            this.tsCboLeague.SelectedIndexChanged += new System.EventHandler(this.tsCboLeague_SelectedIndexChanged);
            // 
            // halfToolStripMenuItem1
            // 
            this.halfToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsCboHalf});
            this.halfToolStripMenuItem1.Name = "halfToolStripMenuItem1";
            this.halfToolStripMenuItem1.Size = new System.Drawing.Size(78, 36);
            this.halfToolStripMenuItem1.Text = "Half";
            // 
            // tsCboHalf
            // 
            this.tsCboHalf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsCboHalf.Name = "tsCboHalf";
            this.tsCboHalf.Size = new System.Drawing.Size(121, 40);
            this.tsCboHalf.SelectedIndexChanged += new System.EventHandler(this.tsCboHalf_SelectedIndexChanged);
            // 
            // groupVisitor
            // 
            this.groupVisitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupVisitor.Controls.Add(this.label5);
            this.groupVisitor.Controls.Add(this.label10);
            this.groupVisitor.Controls.Add(this.label11);
            this.groupVisitor.Controls.Add(this.label12);
            this.groupVisitor.Controls.Add(this.label23);
            this.groupVisitor.Controls.Add(this.label24);
            this.groupVisitor.Controls.Add(this.label25);
            this.groupVisitor.Controls.Add(this.label26);
            this.groupVisitor.Controls.Add(this.label27);
            this.groupVisitor.Controls.Add(this.btnSaveAway);
            this.groupVisitor.Controls.Add(this.AwayFoulSecondTotal);
            this.groupVisitor.Controls.Add(this.AwayFoulFirstTotal);
            this.groupVisitor.Controls.Add(this.label1);
            this.groupVisitor.Controls.Add(this.label16);
            this.groupVisitor.Controls.Add(this.label13);
            this.groupVisitor.Controls.Add(this.label14);
            this.groupVisitor.Controls.Add(this.cboTeam2);
            this.groupVisitor.Location = new System.Drawing.Point(1510, 63);
            this.groupVisitor.Margin = new System.Windows.Forms.Padding(6, 0, 6, 2);
            this.groupVisitor.Name = "groupVisitor";
            this.groupVisitor.Padding = new System.Windows.Forms.Padding(6);
            this.groupVisitor.Size = new System.Drawing.Size(1299, 1431);
            this.groupVisitor.TabIndex = 10;
            this.groupVisitor.TabStop = false;
            this.groupVisitor.Text = "Visitor";
            // 
            // btnSaveAway
            // 
            this.btnSaveAway.Location = new System.Drawing.Point(65, 1366);
            this.btnSaveAway.Margin = new System.Windows.Forms.Padding(6);
            this.btnSaveAway.Name = "btnSaveAway";
            this.btnSaveAway.Size = new System.Drawing.Size(150, 44);
            this.btnSaveAway.TabIndex = 103;
            this.btnSaveAway.Text = "Save team";
            this.btnSaveAway.UseVisualStyleBackColor = true;
            this.btnSaveAway.Visible = false;
            this.btnSaveAway.Click += new System.EventHandler(this.SavePlayers);
            // 
            // AwayFoulSecondTotal
            // 
            this.AwayFoulSecondTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AwayFoulSecondTotal.Location = new System.Drawing.Point(775, 1369);
            this.AwayFoulSecondTotal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 2);
            this.AwayFoulSecondTotal.Name = "AwayFoulSecondTotal";
            this.AwayFoulSecondTotal.ReadOnly = true;
            this.AwayFoulSecondTotal.Size = new System.Drawing.Size(76, 44);
            this.AwayFoulSecondTotal.TabIndex = 102;
            // 
            // AwayFoulFirstTotal
            // 
            this.AwayFoulFirstTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AwayFoulFirstTotal.Location = new System.Drawing.Point(688, 1369);
            this.AwayFoulFirstTotal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 2);
            this.AwayFoulFirstTotal.Name = "AwayFoulFirstTotal";
            this.AwayFoulFirstTotal.ReadOnly = true;
            this.AwayFoulFirstTotal.Size = new System.Drawing.Size(76, 44);
            this.AwayFoulFirstTotal.TabIndex = 101;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(567, 1373);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 37);
            this.label1.TabIndex = 100;
            this.label1.Text = "Totals";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(12, 154);
            this.label16.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 26);
            this.label16.TabIndex = 123;
            this.label16.Text = "In";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(160, 154);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 26);
            this.label13.TabIndex = 5;
            this.label13.Text = "Name";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(60, 154);
            this.label14.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(90, 26);
            this.label14.TabIndex = 4;
            this.label14.Text = "Number";
            // 
            // cboTeam2
            // 
            this.cboTeam2.ContextMenuStrip = this.teamContextMenuStrip;
            this.cboTeam2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTeam2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTeam2.FormattingEnabled = true;
            this.cboTeam2.Location = new System.Drawing.Point(12, 37);
            this.cboTeam2.Margin = new System.Windows.Forms.Padding(6);
            this.cboTeam2.Name = "cboTeam2";
            this.cboTeam2.Size = new System.Drawing.Size(996, 63);
            this.cboTeam2.TabIndex = 2;
            this.cboTeam2.SelectedIndexChanged += new System.EventHandler(this.cboTeam2_SelectedIndexChanged);
            // 
            // colorDialog1
            // 
            this.colorDialog1.SolidColorOnly = true;
            // 
            // btnPossession
            // 
            this.btnPossession.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPossession.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPossession.Image = global::Basketball_Roster_Manager.Properties.Resources.left;
            this.btnPossession.Location = new System.Drawing.Point(1330, 100);
            this.btnPossession.Margin = new System.Windows.Forms.Padding(6);
            this.btnPossession.Name = "btnPossession";
            this.btnPossession.Size = new System.Drawing.Size(168, 85);
            this.btnPossession.TabIndex = 11;
            this.btnPossession.Text = "←";
            this.btnPossession.UseVisualStyleBackColor = true;
            this.btnPossession.Click += new System.EventHandler(this.btnPossession_Click);
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(1357, 69);
            this.label17.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(123, 25);
            this.label17.TabIndex = 12;
            this.label17.Text = "Possession";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "brm";
            this.saveFileDialog1.FileName = "rosters.brm";
            this.saveFileDialog1.Filter = "Basketball Roster Manager data file(*.brm)|*.brm";
            this.saveFileDialog1.RestoreDirectory = true;
            this.saveFileDialog1.Title = "Export file name and location";
            this.saveFileDialog1.ValidateNames = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "brm";
            this.openFileDialog1.Filter = "Basketball Roster Manager data file(*.brm)|*.brm";
            this.openFileDialog1.Title = "Select data file to import";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label18.Location = new System.Drawing.Point(958, 114);
            this.label18.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label18.Name = "label18";
            this.label18.Padding = new System.Windows.Forms.Padding(120, 0, 120, 0);
            this.label18.Size = new System.Drawing.Size(314, 27);
            this.label18.TabIndex = 147;
            this.label18.Text = "Points";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(1140, 154);
            this.label19.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(37, 26);
            this.label19.TabIndex = 150;
            this.label19.Text = "FT";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(1048, 154);
            this.label20.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(42, 26);
            this.label20.TabIndex = 149;
            this.label20.Text = "3pt";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(962, 154);
            this.label21.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(42, 26);
            this.label21.TabIndex = 148;
            this.label21.Text = "FG";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(1213, 154);
            this.label22.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(59, 26);
            this.label22.TabIndex = 151;
            this.label22.Text = "Total";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1213, 154);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 26);
            this.label5.TabIndex = 160;
            this.label5.Text = "Total";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(1140, 154);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 26);
            this.label10.TabIndex = 159;
            this.label10.Text = "FT";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(1048, 154);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 26);
            this.label11.TabIndex = 158;
            this.label11.Text = "3pt";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(962, 154);
            this.label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(42, 26);
            this.label12.TabIndex = 157;
            this.label12.Text = "FG";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label23.Location = new System.Drawing.Point(958, 114);
            this.label23.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label23.Name = "label23";
            this.label23.Padding = new System.Windows.Forms.Padding(120, 0, 120, 0);
            this.label23.Size = new System.Drawing.Size(314, 27);
            this.label23.TabIndex = 156;
            this.label23.Text = "Points";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(864, 154);
            this.label24.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(59, 26);
            this.label24.TabIndex = 155;
            this.label24.Text = "Total";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(769, 154);
            this.label25.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(93, 26);
            this.label25.TabIndex = 154;
            this.label25.Text = "2nd Half";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(681, 154);
            this.label26.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(86, 26);
            this.label26.TabIndex = 153;
            this.label26.Text = "1st Half";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label27.Location = new System.Drawing.Point(681, 114);
            this.label27.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label27.Name = "label27";
            this.label27.Padding = new System.Windows.Forms.Padding(95, 0, 95, 0);
            this.label27.Size = new System.Drawing.Size(257, 27);
            this.label27.TabIndex = 152;
            this.label27.Text = "Fouls";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2824, 1517);
            this.ContextMenuStrip = this.teamContextMenuStrip;
            this.Controls.Add(this.label17);
            this.Controls.Add(this.btnPossession);
            this.Controls.Add(this.groupVisitor);
            this.Controls.Add(this.groupHome);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Basketball Roster Manager";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.teamContextMenuStrip.ResumeLayout(false);
            this.groupHome.ResumeLayout(false);
            this.groupHome.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupVisitor.ResumeLayout(false);
            this.groupVisitor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTeam1;
        private System.Windows.Forms.GroupBox groupHome;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.TextBox HomeFoulSecondTotal;
        private System.Windows.Forms.TextBox HomeFoulFirstTotal;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem tsMenuItemSwitchSides;
        private System.Windows.Forms.GroupBox groupVisitor;
        private System.Windows.Forms.TextBox AwayFoulSecondTotal;
        private System.Windows.Forms.TextBox AwayFoulFirstTotal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cboTeam2;
        private System.Windows.Forms.ContextMenuStrip teamContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem setHomeColorToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripMenuItem setAwayColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem switchSidesToolStripMenuItem;
        private System.Windows.Forms.Button btnSaveHome;
        private System.Windows.Forms.Button btnSaveAway;
        private System.Windows.Forms.ToolStripMenuItem leagueToolStripMenuItem1;
        private System.Windows.Forms.ToolStripComboBox tsCboLeague;
        private System.Windows.Forms.ToolStripMenuItem halfToolStripMenuItem1;
        private System.Windows.Forms.ToolStripComboBox tsCboHalf;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ToolStripMenuItem resetFormToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setHomeColorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setAwayColorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem homeTeamWhiteToolStripMenuItem;
        private System.Windows.Forms.Button btnPossession;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ToolStripMenuItem changeHalfToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem exportDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importDatabaseToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
    }
}

