namespace HumanResources.Loans.Forms
{
    partial class LoanNewForm
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
            this.tbAmount = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbOther = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbInstallmentLoan = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpData = new System.Windows.Forms.DateTimePicker();
            this.groupBoxPozyczka = new System.Windows.Forms.GroupBox();
            this.cbEmployee = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBoxPozyczka.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbAmount
            // 
            this.tbAmount.Location = new System.Drawing.Point(107, 118);
            this.tbAmount.Name = "tbAmount";
            this.tbAmount.Size = new System.Drawing.Size(87, 20);
            this.tbAmount.TabIndex = 3;
            this.tbAmount.WordWrap = false;
            this.tbAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_KeyPress);
            this.tbAmount.Leave += new System.EventHandler(this.tbAmount_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(61, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Kwota:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Pracownik:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Wysokość raty:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(159, 249);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "&Zapisz";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.EnabledChanged += new System.EventHandler(this.btnSave_EnabledChanged);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(110, 53);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(161, 20);
            this.tbName.TabIndex = 1;
            this.tbName.WordWrap = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(64, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Nazwa:";
            // 
            // tbOther
            // 
            this.tbOther.Location = new System.Drawing.Point(107, 193);
            this.tbOther.Name = "tbOther";
            this.tbOther.Size = new System.Drawing.Size(164, 20);
            this.tbOther.TabIndex = 5;
            this.tbOther.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 196);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Dodatkowe info:";
            // 
            // tbInstallmentLoan
            // 
            this.tbInstallmentLoan.Location = new System.Drawing.Point(107, 156);
            this.tbInstallmentLoan.Name = "tbInstallmentLoan";
            this.tbInstallmentLoan.Size = new System.Drawing.Size(87, 20);
            this.tbInstallmentLoan.TabIndex = 4;
            this.tbInstallmentLoan.WordWrap = false;
            this.tbInstallmentLoan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Data:";
            // 
            // dtpData
            // 
            this.dtpData.CustomFormat = "dd MMMM yyyy";
            this.dtpData.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpData.Location = new System.Drawing.Point(110, 84);
            this.dtpData.Name = "dtpData";
            this.dtpData.Size = new System.Drawing.Size(161, 20);
            this.dtpData.TabIndex = 2;
            // 
            // groupBoxPozyczka
            // 
            this.groupBoxPozyczka.Controls.Add(this.tbName);
            this.groupBoxPozyczka.Controls.Add(this.label5);
            this.groupBoxPozyczka.Controls.Add(this.tbOther);
            this.groupBoxPozyczka.Controls.Add(this.label2);
            this.groupBoxPozyczka.Controls.Add(this.tbInstallmentLoan);
            this.groupBoxPozyczka.Controls.Add(this.label1);
            this.groupBoxPozyczka.Controls.Add(this.dtpData);
            this.groupBoxPozyczka.Controls.Add(this.cbEmployee);
            this.groupBoxPozyczka.Controls.Add(this.tbAmount);
            this.groupBoxPozyczka.Controls.Add(this.label6);
            this.groupBoxPozyczka.Controls.Add(this.label4);
            this.groupBoxPozyczka.Controls.Add(this.label3);
            this.groupBoxPozyczka.Location = new System.Drawing.Point(18, 12);
            this.groupBoxPozyczka.Name = "groupBoxPozyczka";
            this.groupBoxPozyczka.Size = new System.Drawing.Size(297, 231);
            this.groupBoxPozyczka.TabIndex = 18;
            this.groupBoxPozyczka.TabStop = false;
            this.groupBoxPozyczka.Text = "Pożyczka";
            // 
            // cbEmployee
            // 
            this.cbEmployee.DisplayMember = "razem";
            this.cbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEmployee.FormattingEnabled = true;
            this.cbEmployee.Location = new System.Drawing.Point(110, 19);
            this.cbEmployee.Name = "cbEmployee";
            this.cbEmployee.Size = new System.Drawing.Size(161, 21);
            this.cbEmployee.TabIndex = 0;
            this.cbEmployee.ValueMember = "id_pracownika";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(240, 249);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Anuluj";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnAnuluj_Click);
            // 
            // LoanNewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 279);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBoxPozyczka);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(354, 318);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(354, 318);
            this.Name = "LoanNewForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.groupBoxPozyczka.ResumeLayout(false);
            this.groupBoxPozyczka.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbAmount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbOther;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbInstallmentLoan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpData;
        private System.Windows.Forms.GroupBox groupBoxPozyczka;
        private System.Windows.Forms.ComboBox cbEmployee;
        private System.Windows.Forms.Button btnCancel;
    }
}