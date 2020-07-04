namespace HumanResources.Employees.Forms
{
    partial class ChangeWorkTypeForm
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
            this.tbWorkTo = new System.Windows.Forms.MaskedTextBox();
            this.tbWorkFrom = new System.Windows.Forms.MaskedTextBox();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.btnZatwierdz = new System.Windows.Forms.Button();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.lblMainText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // maskedTextBoxPracaDoZamiana
            // 
            this.tbWorkTo.Location = new System.Drawing.Point(44, 66);
            this.tbWorkTo.Mask = "00:00";
            this.tbWorkTo.Name = "maskedTextBoxPracaDoZamiana";
            this.tbWorkTo.Size = new System.Drawing.Size(100, 20);
            this.tbWorkTo.TabIndex = 16;
            this.tbWorkTo.ValidatingType = typeof(System.DateTime);
            this.tbWorkTo.Visible = false;
            this.tbWorkTo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbWorkTo_MouseClick);
            // 
            // maskedTextBoxPracaOdZamiana
            // 
            this.tbWorkFrom.Location = new System.Drawing.Point(44, 33);
            this.tbWorkFrom.Mask = "00:00";
            this.tbWorkFrom.Name = "maskedTextBoxPracaOdZamiana";
            this.tbWorkFrom.Size = new System.Drawing.Size(100, 20);
            this.tbWorkFrom.TabIndex = 15;
            this.tbWorkFrom.ValidatingType = typeof(System.DateTime);
            this.tbWorkFrom.Visible = false;
            this.tbWorkFrom.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbWorFrom_MouseClick);
            // 
            // lblOd
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(16, 36);
            this.lblFrom.Name = "lblOd";
            this.lblFrom.Size = new System.Drawing.Size(22, 13);
            this.lblFrom.TabIndex = 17;
            this.lblFrom.Text = "od:";
            this.lblFrom.Visible = false;
            // 
            // lblDo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(16, 70);
            this.lblTo.Name = "lblDo";
            this.lblTo.Size = new System.Drawing.Size(22, 13);
            this.lblTo.TabIndex = 14;
            this.lblTo.Text = "do:";
            this.lblTo.Visible = false;
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnuluj.Location = new System.Drawing.Point(174, 92);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(75, 23);
            this.btnAnuluj.TabIndex = 13;
            this.btnAnuluj.Text = "&Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            this.btnAnuluj.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnZatwierdz
            // 
            this.btnZatwierdz.Location = new System.Drawing.Point(93, 92);
            this.btnZatwierdz.Name = "btnZatwierdz";
            this.btnZatwierdz.Size = new System.Drawing.Size(75, 23);
            this.btnZatwierdz.TabIndex = 12;
            this.btnZatwierdz.Text = "&Zatwierdz";
            this.btnZatwierdz.UseVisualStyleBackColor = true;
            this.btnZatwierdz.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbRodzaj
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(70, 46);
            this.cbType.Name = "cbRodzaj";
            this.cbType.Size = new System.Drawing.Size(158, 21);
            this.cbType.TabIndex = 11;
            this.cbType.Visible = false;
            // 
            // lblTekst
            // 
            this.lblMainText.AutoSize = true;
            this.lblMainText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblMainText.Location = new System.Drawing.Point(16, 14);
            this.lblMainText.Name = "lblTekst";
            this.lblMainText.Size = new System.Drawing.Size(35, 13);
            this.lblMainText.TabIndex = 10;
            this.lblMainText.Text = "label1";
            // 
            // ZamianaRodzajuPracyForm
            // 
            this.AcceptButton = this.btnZatwierdz;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnuluj;
            this.ClientSize = new System.Drawing.Size(264, 128);
            this.Controls.Add(this.tbWorkTo);
            this.Controls.Add(this.tbWorkFrom);
            this.Controls.Add(this.lblFrom);
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.btnAnuluj);
            this.Controls.Add(this.btnZatwierdz);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.lblMainText);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(280, 167);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 167);
            this.Name = "ZamianaRodzajuPracyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox tbWorkTo;
        private System.Windows.Forms.MaskedTextBox tbWorkFrom;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Button btnZatwierdz;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.Label lblMainText;
    }
}