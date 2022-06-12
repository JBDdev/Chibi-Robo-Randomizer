namespace WindowsFormsApp1
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
            this.isoFilePath = new System.Windows.Forms.TextBox();
            this.title = new System.Windows.Forms.Label();
            this.openISO = new System.Windows.Forms.Button();
            this.openDestination = new System.Windows.Forms.Button();
            this.destinationPath = new System.Windows.Forms.TextBox();
            this.settingsLabel = new System.Windows.Forms.Label();
            this.logicSettings = new System.Windows.Forms.ComboBox();
            this.openDownstairs = new System.Windows.Forms.CheckBox();
            this.openUpstairs = new System.Windows.Forms.CheckBox();
            this.seedLabel = new System.Windows.Forms.Label();
            this.seed = new System.Windows.Forms.TextBox();
            this.statusDialog = new System.Windows.Forms.RichTextBox();
            this.randomizeButton = new System.Windows.Forms.Button();
            this.batteryCharge = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // isoFilePath
            // 
            this.isoFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isoFilePath.Location = new System.Drawing.Point(123, 60);
            this.isoFilePath.Name = "isoFilePath";
            this.isoFilePath.ReadOnly = true;
            this.isoFilePath.Size = new System.Drawing.Size(490, 26);
            this.isoFilePath.TabIndex = 0;
            this.isoFilePath.Text = "<- Select path to Chibi-Robo NTSC-U ISO";
            this.isoFilePath.TextChanged += new System.EventHandler(this.isoFilePath_TextChanged);
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(12, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(304, 26);
            this.title.TabIndex = 2;
            this.title.Text = "Chibi-Robo Randomizer v0.6a";
            // 
            // openISO
            // 
            this.openISO.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openISO.Location = new System.Drawing.Point(17, 56);
            this.openISO.Name = "openISO";
            this.openISO.Size = new System.Drawing.Size(100, 35);
            this.openISO.TabIndex = 3;
            this.openISO.Text = "Open ISO";
            this.openISO.UseVisualStyleBackColor = true;
            this.openISO.Click += new System.EventHandler(this.openISO_Click);
            // 
            // openDestination
            // 
            this.openDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openDestination.Location = new System.Drawing.Point(17, 97);
            this.openDestination.Name = "openDestination";
            this.openDestination.Size = new System.Drawing.Size(100, 35);
            this.openDestination.TabIndex = 4;
            this.openDestination.Text = "Browse";
            this.openDestination.UseVisualStyleBackColor = true;
            this.openDestination.Click += new System.EventHandler(this.openDestination_Click);
            // 
            // destinationPath
            // 
            this.destinationPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.destinationPath.Location = new System.Drawing.Point(123, 101);
            this.destinationPath.Name = "destinationPath";
            this.destinationPath.ReadOnly = true;
            this.destinationPath.Size = new System.Drawing.Size(490, 26);
            this.destinationPath.TabIndex = 5;
            this.destinationPath.Text = "<- Set destination path";
            // 
            // settingsLabel
            // 
            this.settingsLabel.AutoSize = true;
            this.settingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsLabel.Location = new System.Drawing.Point(18, 160);
            this.settingsLabel.Name = "settingsLabel";
            this.settingsLabel.Size = new System.Drawing.Size(107, 20);
            this.settingsLabel.TabIndex = 7;
            this.settingsLabel.Text = "Mode / Logic: ";
            this.settingsLabel.Click += new System.EventHandler(this.settingsLabel_Click);
            // 
            // logicSettings
            // 
            this.logicSettings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logicSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logicSettings.FormattingEnabled = true;
            this.logicSettings.Items.AddRange(new object[] {
            "Glitchless",
            "Glitched",
            "No Logic"});
            this.logicSettings.Location = new System.Drawing.Point(123, 157);
            this.logicSettings.Name = "logicSettings";
            this.logicSettings.Size = new System.Drawing.Size(121, 28);
            this.logicSettings.TabIndex = 8;
            // 
            // openDownstairs
            // 
            this.openDownstairs.AutoSize = true;
            this.openDownstairs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openDownstairs.Location = new System.Drawing.Point(267, 159);
            this.openDownstairs.Name = "openDownstairs";
            this.openDownstairs.Size = new System.Drawing.Size(150, 24);
            this.openDownstairs.TabIndex = 10;
            this.openDownstairs.Text = "Open Downstairs";
            this.openDownstairs.UseVisualStyleBackColor = true;
            // 
            // openUpstairs
            // 
            this.openUpstairs.AutoSize = true;
            this.openUpstairs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openUpstairs.Location = new System.Drawing.Point(423, 160);
            this.openUpstairs.Name = "openUpstairs";
            this.openUpstairs.Size = new System.Drawing.Size(130, 24);
            this.openUpstairs.TabIndex = 11;
            this.openUpstairs.Text = "Open Upstairs";
            this.openUpstairs.UseVisualStyleBackColor = true;
            // 
            // seedLabel
            // 
            this.seedLabel.AutoSize = true;
            this.seedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seedLabel.Location = new System.Drawing.Point(74, 199);
            this.seedLabel.Name = "seedLabel";
            this.seedLabel.Size = new System.Drawing.Size(51, 20);
            this.seedLabel.TabIndex = 12;
            this.seedLabel.Text = "Seed:";
            this.seedLabel.Click += new System.EventHandler(this.seed_Click);
            // 
            // seed
            // 
            this.seed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seed.Location = new System.Drawing.Point(123, 196);
            this.seed.Name = "seed";
            this.seed.Size = new System.Drawing.Size(121, 26);
            this.seed.TabIndex = 13;
            // 
            // statusDialog
            // 
            this.statusDialog.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusDialog.Location = new System.Drawing.Point(12, 283);
            this.statusDialog.Name = "statusDialog";
            this.statusDialog.ReadOnly = true;
            this.statusDialog.Size = new System.Drawing.Size(601, 155);
            this.statusDialog.TabIndex = 14;
            this.statusDialog.Text = "";
            // 
            // randomizeButton
            // 
            this.randomizeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.randomizeButton.Location = new System.Drawing.Point(12, 228);
            this.randomizeButton.Name = "randomizeButton";
            this.randomizeButton.Size = new System.Drawing.Size(161, 49);
            this.randomizeButton.TabIndex = 15;
            this.randomizeButton.Text = "Randomize";
            this.randomizeButton.UseVisualStyleBackColor = true;
            this.randomizeButton.Click += new System.EventHandler(this.randomizeButton_Click);
            // 
            // batteryCharge
            // 
            this.batteryCharge.AutoSize = true;
            this.batteryCharge.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.batteryCharge.Location = new System.Drawing.Point(423, 190);
            this.batteryCharge.Name = "batteryCharge";
            this.batteryCharge.Size = new System.Drawing.Size(144, 24);
            this.batteryCharge.TabIndex = 16;
            this.batteryCharge.Text = "Charged Battery";
            this.batteryCharge.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(625, 450);
            this.Controls.Add(this.batteryCharge);
            this.Controls.Add(this.randomizeButton);
            this.Controls.Add(this.statusDialog);
            this.Controls.Add(this.seed);
            this.Controls.Add(this.seedLabel);
            this.Controls.Add(this.openUpstairs);
            this.Controls.Add(this.openDownstairs);
            this.Controls.Add(this.logicSettings);
            this.Controls.Add(this.settingsLabel);
            this.Controls.Add(this.destinationPath);
            this.Controls.Add(this.openDestination);
            this.Controls.Add(this.openISO);
            this.Controls.Add(this.title);
            this.Controls.Add(this.isoFilePath);
            this.Name = "Form1";
            this.Text = "Chibi-Robo Randomizer v0.6a";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox isoFilePath;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Button openISO;
        private System.Windows.Forms.Button openDestination;
        private System.Windows.Forms.TextBox destinationPath;
        private System.Windows.Forms.Label settingsLabel;
        private System.Windows.Forms.ComboBox logicSettings;
        private System.Windows.Forms.CheckBox openDownstairs;
        private System.Windows.Forms.CheckBox openUpstairs;
        private System.Windows.Forms.Label seedLabel;
        private System.Windows.Forms.TextBox seed;
        private System.Windows.Forms.RichTextBox statusDialog;
        private System.Windows.Forms.Button randomizeButton;
        private System.Windows.Forms.CheckBox batteryCharge;
    }
}

