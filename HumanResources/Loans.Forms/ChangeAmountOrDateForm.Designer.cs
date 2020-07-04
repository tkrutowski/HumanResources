namespace HumanResources.Loans.Forms
{
    partial class ChangeAmountOrDateForm
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
            this.btnZatwierdz = new System.Windows.Forms.Button();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.tbAmount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnZatwierdz
            // 
            this.btnZatwierdz.Location = new System.Drawing.Point(73, 118);
            this.btnZatwierdz.Name = "btnZatwierdz";
            this.btnZatwierdz.Size = new System.Drawing.Size(75, 23);
            this.btnZatwierdz.TabIndex = 14;
            this.btnZatwierdz.Text = "&Zatwierdz";
            this.btnZatwierdz.UseVisualStyleBackColor = true;
            this.btnZatwierdz.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnuluj.Location = new System.Drawing.Point(165, 118);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(75, 23);
            this.btnAnuluj.TabIndex = 13;
            this.btnAnuluj.Text = "&Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            // 
            // tbAmount
            // 
            this.tbAmount.Enabled = false;
            this.tbAmount.Location = new System.Drawing.Point(78, 58);
            this.tbAmount.Name = "tbAmount";
            this.tbAmount.Size = new System.Drawing.Size(147, 20);
            this.tbAmount.TabIndex = 12;
            this.tbAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_KeyPress);
            this.tbAmount.Leave += new System.EventHandler(this.tbZmianaKwoty_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Zmień kwotę:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Zmień datę:";
            // 
            // dtpDate
            // 
            this.dtpDate.CustomFormat = "dd MMMM yyyy";
            this.dtpDate.Enabled = false;
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDate.Location = new System.Drawing.Point(78, 23);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(147, 20);
            this.dtpDate.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbAmount);
            this.groupBox1.Location = new System.Drawing.Point(8, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 100);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edycja danych";
            // 
            // ZmianaKwotyDatyForm
            // 
            this.AcceptButton = this.btnZatwierdz;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnuluj;
            this.ClientSize = new System.Drawing.Size(252, 151);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnZatwierdz);
            this.Controls.Add(this.btnAnuluj);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(268, 190);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(268, 190);
            this.Name = "ZmianaKwotyDatyForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnZatwierdz;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.TextBox tbAmount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}