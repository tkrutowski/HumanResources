namespace Pracownicy.Settings.Forms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.gbPozyczka = new System.Windows.Forms.GroupBox();
            this.groupBox56 = new System.Windows.Forms.GroupBox();
            this.rbLoanNotPaid = new System.Windows.Forms.RadioButton();
            this.rbLoanPaid = new System.Windows.Forms.RadioButton();
            this.rbLoanAll = new System.Windows.Forms.RadioButton();
            this.label46 = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.groupBox57 = new System.Windows.Forms.GroupBox();
            this.label48 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.cbLoansSotyType = new System.Windows.Forms.ComboBox();
            this.cbLoansSortColumn = new System.Windows.Forms.ComboBox();
            this.label51 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.gbPracownik = new System.Windows.Forms.GroupBox();
            this.groupBox53 = new System.Windows.Forms.GroupBox();
            this.rbPracZwolnieni = new System.Windows.Forms.RadioButton();
            this.rbPracZatrudnieni = new System.Windows.Forms.RadioButton();
            this.rbPracWszystkie = new System.Windows.Forms.RadioButton();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.groupBox54 = new System.Windows.Forms.GroupBox();
            this.label38 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.cbEmployeeSortType = new System.Windows.Forms.ComboBox();
            this.cbEmployeeSortColumn = new System.Windows.Forms.ComboBox();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.btnAnuluj = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.gbPozyczka.SuspendLayout();
            this.groupBox56.SuspendLayout();
            this.groupBox57.SuspendLayout();
            this.gbPracownik.SuspendLayout();
            this.groupBox53.SuspendLayout();
            this.groupBox54.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPozyczka
            // 
            this.gbPozyczka.Controls.Add(this.groupBox56);
            this.gbPozyczka.Controls.Add(this.groupBox57);
            this.gbPozyczka.Location = new System.Drawing.Point(12, 238);
            this.gbPozyczka.Name = "gbPozyczka";
            this.gbPozyczka.Size = new System.Drawing.Size(640, 199);
            this.gbPozyczka.TabIndex = 11;
            this.gbPozyczka.TabStop = false;
            this.gbPozyczka.Text = "Tabela POŻYCZKI";
            // 
            // groupBox56
            // 
            this.groupBox56.Controls.Add(this.rbLoanNotPaid);
            this.groupBox56.Controls.Add(this.rbLoanPaid);
            this.groupBox56.Controls.Add(this.rbLoanAll);
            this.groupBox56.Controls.Add(this.label46);
            this.groupBox56.Controls.Add(this.label47);
            this.groupBox56.Location = new System.Drawing.Point(16, 20);
            this.groupBox56.Name = "groupBox56";
            this.groupBox56.Size = new System.Drawing.Size(297, 151);
            this.groupBox56.TabIndex = 6;
            this.groupBox56.TabStop = false;
            this.groupBox56.Text = "Opcje wyświetlania";
            // 
            // rbPozNiesplacone
            // 
            this.rbLoanNotPaid.AutoSize = true;
            this.rbLoanNotPaid.Location = new System.Drawing.Point(15, 104);
            this.rbLoanNotPaid.Name = "rbPozNiesplacone";
            this.rbLoanNotPaid.Size = new System.Drawing.Size(86, 17);
            this.rbLoanNotPaid.TabIndex = 15;
            this.rbLoanNotPaid.TabStop = true;
            this.rbLoanNotPaid.Text = "Niespłacone";
            this.rbLoanNotPaid.UseVisualStyleBackColor = true;
            // 
            // rbPozSplacone
            // 
            this.rbLoanPaid.AutoSize = true;
            this.rbLoanPaid.Location = new System.Drawing.Point(15, 81);
            this.rbLoanPaid.Name = "rbPozSplacone";
            this.rbLoanPaid.Size = new System.Drawing.Size(72, 17);
            this.rbLoanPaid.TabIndex = 14;
            this.rbLoanPaid.TabStop = true;
            this.rbLoanPaid.Text = "Spłacone";
            this.rbLoanPaid.UseVisualStyleBackColor = true;
            // 
            // rbPozWszystkie
            // 
            this.rbLoanAll.AutoSize = true;
            this.rbLoanAll.Location = new System.Drawing.Point(15, 58);
            this.rbLoanAll.Name = "rbPozWszystkie";
            this.rbLoanAll.Size = new System.Drawing.Size(73, 17);
            this.rbLoanAll.TabIndex = 13;
            this.rbLoanAll.TabStop = true;
            this.rbLoanAll.Text = "Wszystkie";
            this.rbLoanAll.UseVisualStyleBackColor = true;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(3, 36);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(163, 13);
            this.label46.TabIndex = 12;
            this.label46.Text = "podczas uruchamianie programu:";
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(4, 21);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(272, 13);
            this.label47.TabIndex = 11;
            this.label47.Text = "Określ rodzaj wyświetlanych danych w tabeli \'Pożyczka\' ";
            // 
            // groupBox57
            // 
            this.groupBox57.Controls.Add(this.label48);
            this.groupBox57.Controls.Add(this.label50);
            this.groupBox57.Controls.Add(this.cbLoansSotyType);
            this.groupBox57.Controls.Add(this.cbLoansSortColumn);
            this.groupBox57.Controls.Add(this.label51);
            this.groupBox57.Controls.Add(this.label52);
            this.groupBox57.Location = new System.Drawing.Point(319, 20);
            this.groupBox57.Name = "groupBox57";
            this.groupBox57.Size = new System.Drawing.Size(288, 151);
            this.groupBox57.TabIndex = 7;
            this.groupBox57.TabStop = false;
            this.groupBox57.Text = "Sposób wyświetlania";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(1, 36);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(163, 13);
            this.label48.TabIndex = 14;
            this.label48.Text = "podczas uruchamianie programu:";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(2, 21);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(278, 13);
            this.label50.TabIndex = 13;
            this.label50.Text = "Określ sposób wyświetlanych danych w tabeli \'Pożyczka\' ";
            // 
            // cbPozSortRosMal
            // 
            this.cbLoansSotyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoansSotyType.FormattingEnabled = true;
            this.cbLoansSotyType.Items.AddRange(new object[] {
            "malejacym",
            "rosnacym"});
            this.cbLoansSotyType.Location = new System.Drawing.Point(85, 104);
            this.cbLoansSotyType.Name = "cbPozSortRosMal";
            this.cbLoansSotyType.Size = new System.Drawing.Size(197, 21);
            this.cbLoansSotyType.TabIndex = 11;
            // 
            // cbPozSortWg
            // 
            this.cbLoansSortColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoansSortColumn.FormattingEnabled = true;
            this.cbLoansSortColumn.Location = new System.Drawing.Point(85, 72);
            this.cbLoansSortColumn.Name = "cbPozSortWg";
            this.cbLoansSortColumn.Size = new System.Drawing.Size(197, 21);
            this.cbLoansSortColumn.TabIndex = 9;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(20, 108);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(62, 13);
            this.label51.TabIndex = 10;
            this.label51.Text = "w porzadku";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(30, 75);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(51, 13);
            this.label52.TabIndex = 8;
            this.label52.Text = "Sortuj wg";
            // 
            // gbPracownik
            // 
            this.gbPracownik.Controls.Add(this.groupBox53);
            this.gbPracownik.Controls.Add(this.groupBox54);
            this.gbPracownik.Location = new System.Drawing.Point(12, 12);
            this.gbPracownik.Name = "gbPracownik";
            this.gbPracownik.Size = new System.Drawing.Size(640, 199);
            this.gbPracownik.TabIndex = 10;
            this.gbPracownik.TabStop = false;
            this.gbPracownik.Text = "Tabela PRACOWNICY";
            // 
            // groupBox53
            // 
            this.groupBox53.Controls.Add(this.rbPracZwolnieni);
            this.groupBox53.Controls.Add(this.rbPracZatrudnieni);
            this.groupBox53.Controls.Add(this.rbPracWszystkie);
            this.groupBox53.Controls.Add(this.label41);
            this.groupBox53.Controls.Add(this.label42);
            this.groupBox53.Location = new System.Drawing.Point(16, 20);
            this.groupBox53.Name = "groupBox53";
            this.groupBox53.Size = new System.Drawing.Size(297, 151);
            this.groupBox53.TabIndex = 6;
            this.groupBox53.TabStop = false;
            this.groupBox53.Text = "Opcje wyświetlania";
            // 
            // rbPracZwolnieni
            // 
            this.rbPracZwolnieni.AutoSize = true;
            this.rbPracZwolnieni.Location = new System.Drawing.Point(15, 104);
            this.rbPracZwolnieni.Name = "rbPracZwolnieni";
            this.rbPracZwolnieni.Size = new System.Drawing.Size(70, 17);
            this.rbPracZwolnieni.TabIndex = 15;
            this.rbPracZwolnieni.TabStop = true;
            this.rbPracZwolnieni.Text = "Zwolnieni";
            this.rbPracZwolnieni.UseVisualStyleBackColor = true;
            // 
            // rbPracZatrudnieni
            // 
            this.rbPracZatrudnieni.AutoSize = true;
            this.rbPracZatrudnieni.Location = new System.Drawing.Point(15, 81);
            this.rbPracZatrudnieni.Name = "rbPracZatrudnieni";
            this.rbPracZatrudnieni.Size = new System.Drawing.Size(78, 17);
            this.rbPracZatrudnieni.TabIndex = 14;
            this.rbPracZatrudnieni.TabStop = true;
            this.rbPracZatrudnieni.Text = "Zatrudnieni";
            this.rbPracZatrudnieni.UseVisualStyleBackColor = true;
            // 
            // rbPracWszystkie
            // 
            this.rbPracWszystkie.AutoSize = true;
            this.rbPracWszystkie.Location = new System.Drawing.Point(15, 58);
            this.rbPracWszystkie.Name = "rbPracWszystkie";
            this.rbPracWszystkie.Size = new System.Drawing.Size(73, 17);
            this.rbPracWszystkie.TabIndex = 13;
            this.rbPracWszystkie.TabStop = true;
            this.rbPracWszystkie.Text = "Wszystkie";
            this.rbPracWszystkie.UseVisualStyleBackColor = true;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(3, 36);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(163, 13);
            this.label41.TabIndex = 12;
            this.label41.Text = "podczas uruchamianie programu:";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(4, 21);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(276, 13);
            this.label42.TabIndex = 11;
            this.label42.Text = "Określ rodzaj wyświetlanych danych w tabeli \'Pracownik\' ";
            // 
            // groupBox54
            // 
            this.groupBox54.Controls.Add(this.label38);
            this.groupBox54.Controls.Add(this.label39);
            this.groupBox54.Controls.Add(this.cbEmployeeSortType);
            this.groupBox54.Controls.Add(this.cbEmployeeSortColumn);
            this.groupBox54.Controls.Add(this.label44);
            this.groupBox54.Controls.Add(this.label45);
            this.groupBox54.Location = new System.Drawing.Point(319, 20);
            this.groupBox54.Name = "groupBox54";
            this.groupBox54.Size = new System.Drawing.Size(288, 151);
            this.groupBox54.TabIndex = 7;
            this.groupBox54.TabStop = false;
            this.groupBox54.Text = "Sposób wyświetlania";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(1, 36);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(163, 13);
            this.label38.TabIndex = 14;
            this.label38.Text = "podczas uruchamianie programu:";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(2, 21);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(282, 13);
            this.label39.TabIndex = 13;
            this.label39.Text = "Określ sposób wyświetlanych danych w tabeli \'Pracownik\' ";
            // 
            // cbPracSortRosMal
            // 
            this.cbEmployeeSortType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEmployeeSortType.FormattingEnabled = true;
            this.cbEmployeeSortType.Items.AddRange(new object[] {
            "malejacym",
            "rosnacym"});
            this.cbEmployeeSortType.Location = new System.Drawing.Point(85, 104);
            this.cbEmployeeSortType.Name = "cbPracSortRosMal";
            this.cbEmployeeSortType.Size = new System.Drawing.Size(197, 21);
            this.cbEmployeeSortType.TabIndex = 11;
            // 
            // cbPracSortWg
            // 
            this.cbEmployeeSortColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEmployeeSortColumn.FormattingEnabled = true;
            this.cbEmployeeSortColumn.Location = new System.Drawing.Point(85, 72);
            this.cbEmployeeSortColumn.Name = "cbPracSortWg";
            this.cbEmployeeSortColumn.Size = new System.Drawing.Size(197, 21);
            this.cbEmployeeSortColumn.TabIndex = 9;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(20, 108);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(62, 13);
            this.label44.TabIndex = 10;
            this.label44.Text = "w porzadku";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(30, 75);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(51, 13);
            this.label45.TabIndex = 8;
            this.label45.Text = "Sortuj wg";
            // 
            // btnAnuluj
            // 
            this.btnAnuluj.Location = new System.Drawing.Point(577, 453);
            this.btnAnuluj.Name = "btnAnuluj";
            this.btnAnuluj.Size = new System.Drawing.Size(75, 23);
            this.btnAnuluj.TabIndex = 12;
            this.btnAnuluj.Text = "&Anuluj";
            this.btnAnuluj.UseVisualStyleBackColor = true;
            this.btnAnuluj.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnZatwierdz
            // 
            this.btnSave.Location = new System.Drawing.Point(496, 453);
            this.btnSave.Name = "btnZatwierdz";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "&Zatwierdz";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.EnabledChanged += new System.EventHandler(this.btnSave_EnabledChanged);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // UstawieniaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 488);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAnuluj);
            this.Controls.Add(this.gbPozyczka);
            this.Controls.Add(this.gbPracownik);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(682, 527);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(682, 527);
            this.Name = "UstawieniaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.gbPozyczka.ResumeLayout(false);
            this.groupBox56.ResumeLayout(false);
            this.groupBox56.PerformLayout();
            this.groupBox57.ResumeLayout(false);
            this.groupBox57.PerformLayout();
            this.gbPracownik.ResumeLayout(false);
            this.groupBox53.ResumeLayout(false);
            this.groupBox53.PerformLayout();
            this.groupBox54.ResumeLayout(false);
            this.groupBox54.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbPozyczka;
        private System.Windows.Forms.GroupBox groupBox56;
        private System.Windows.Forms.RadioButton rbLoanNotPaid;
        private System.Windows.Forms.RadioButton rbLoanPaid;
        private System.Windows.Forms.RadioButton rbLoanAll;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.GroupBox groupBox57;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.ComboBox cbLoansSotyType;
        private System.Windows.Forms.ComboBox cbLoansSortColumn;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.GroupBox gbPracownik;
        private System.Windows.Forms.GroupBox groupBox53;
        private System.Windows.Forms.RadioButton rbPracZwolnieni;
        private System.Windows.Forms.RadioButton rbPracZatrudnieni;
        private System.Windows.Forms.RadioButton rbPracWszystkie;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.GroupBox groupBox54;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.ComboBox cbEmployeeSortType;
        private System.Windows.Forms.ComboBox cbEmployeeSortColumn;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Button btnAnuluj;
        private System.Windows.Forms.Button btnSave;
    }
}