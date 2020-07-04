namespace HumanResources.Loans.Forms
{
    partial class LoanInstallmentDetailsForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvLoanUnstallment = new System.Windows.Forms.DataGridView();
            this.data_wplaty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kwota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCount = new System.Windows.Forms.DataGridView();
            this.razem = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoanUnstallment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCount)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvRaty
            // 
            this.dgvLoanUnstallment.AllowUserToAddRows = false;
            this.dgvLoanUnstallment.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLoanUnstallment.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLoanUnstallment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoanUnstallment.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.data_wplaty,
            this.kwota});
            this.dgvLoanUnstallment.Location = new System.Drawing.Point(24, 12);
            this.dgvLoanUnstallment.MultiSelect = false;
            this.dgvLoanUnstallment.Name = "dgvRaty";
            this.dgvLoanUnstallment.ReadOnly = true;
            this.dgvLoanUnstallment.RowHeadersVisible = false;
            this.dgvLoanUnstallment.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvLoanUnstallment.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLoanUnstallment.Size = new System.Drawing.Size(202, 260);
            this.dgvLoanUnstallment.TabIndex = 14;
            // 
            // data_wplaty
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.data_wplaty.DefaultCellStyle = dataGridViewCellStyle2;
            this.data_wplaty.HeaderText = "Data wpłaty";
            this.data_wplaty.Name = "data_wplaty";
            this.data_wplaty.ReadOnly = true;
            // 
            // kwota
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.kwota.DefaultCellStyle = dataGridViewCellStyle3;
            this.kwota.HeaderText = "Kwota";
            this.kwota.Name = "kwota";
            this.kwota.ReadOnly = true;
            // 
            // gridPodliczenia
            // 
            this.dgvCount.AllowUserToAddRows = false;
            this.dgvCount.AllowUserToDeleteRows = false;
            this.dgvCount.AllowUserToResizeColumns = false;
            this.dgvCount.AllowUserToResizeRows = false;
            this.dgvCount.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCount.ColumnHeadersVisible = false;
            this.dgvCount.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.razem});
            this.dgvCount.Location = new System.Drawing.Point(144, 272);
            this.dgvCount.MultiSelect = false;
            this.dgvCount.Name = "gridPodliczenia";
            this.dgvCount.ReadOnly = true;
            this.dgvCount.RowHeadersVisible = false;
            this.dgvCount.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvCount.Size = new System.Drawing.Size(82, 23);
            this.dgvCount.TabIndex = 15;
            // 
            // razem
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.razem.DefaultCellStyle = dataGridViewCellStyle4;
            this.razem.HeaderText = "Razem";
            this.razem.Name = "razem";
            this.razem.ReadOnly = true;
            this.razem.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.razem.Width = 80;
            // 
            // btnZamknij
            // 
            this.btnClose.Location = new System.Drawing.Point(83, 312);
            this.btnClose.Name = "btnZamknij";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Zamknij";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // PozyczkaRataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 339);
            this.Controls.Add(this.dgvLoanUnstallment);
            this.Controls.Add(this.dgvCount);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(267, 378);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(267, 378);
            this.Name = "PozyczkaRataForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Shown += new System.EventHandler(this.LoanInstallmentDetailsForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoanUnstallment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLoanUnstallment;
        private System.Windows.Forms.DataGridViewTextBoxColumn data_wplaty;
        private System.Windows.Forms.DataGridViewTextBoxColumn kwota;
        private System.Windows.Forms.DataGridView dgvCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn razem;
        private System.Windows.Forms.Button btnClose;
    }
}