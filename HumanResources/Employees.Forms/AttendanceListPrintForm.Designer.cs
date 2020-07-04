namespace HumanResources.Employees.Forms
{
    partial class AttendanceListPrintForm

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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDataWydruku = new System.Windows.Forms.DateTimePicker();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.btnDrukuj = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpDataWydruku);
            this.groupBox1.Location = new System.Drawing.Point(14, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(224, 77);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lista obecności";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data:";
            // 
            // dtpDataWydruku
            // 
            this.dtpDataWydruku.CustomFormat = "MMMM yyyy";
            this.dtpDataWydruku.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDataWydruku.Location = new System.Drawing.Point(56, 33);
            this.dtpDataWydruku.Name = "dtpDataWydruku";
            this.dtpDataWydruku.Size = new System.Drawing.Size(144, 20);
            this.dtpDataWydruku.TabIndex = 0;
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnuluj.Location = new System.Drawing.Point(163, 93);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(75, 23);
            this.btnAnuluj.TabIndex = 5;
            this.btnAnuluj.Text = "Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            this.btnAnuluj.Click += new System.EventHandler(this.btnAnuluj_Click);
            // 
            // btnDrukuj
            // 
            this.btnDrukuj.Location = new System.Drawing.Point(82, 93);
            this.btnDrukuj.Name = "btnDrukuj";
            this.btnDrukuj.Size = new System.Drawing.Size(75, 23);
            this.btnDrukuj.TabIndex = 4;
            this.btnDrukuj.Text = "Drukuj";
            this.btnDrukuj.UseVisualStyleBackColor = true;
            this.btnDrukuj.Click += new System.EventHandler(this.btnDrukuj_Click);
            // 
            // ListaObecnosciWydrukForm
            // 
            this.AcceptButton = this.btnDrukuj;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnAnuluj;
            this.ClientSize = new System.Drawing.Size(252, 126);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAnuluj);
            this.Controls.Add(this.btnDrukuj);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(268, 165);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(268, 165);
            this.Name = "ListaObecnosciWydrukForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpDataWydruku;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Button btnDrukuj;
    }
}