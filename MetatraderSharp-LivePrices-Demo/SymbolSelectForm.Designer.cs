namespace LivePrices
{
    partial class SymbolSelectForm
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
            groupBox1 = new GroupBox();
            cbx_Symbol2 = new ComboBox();
            cbx_Symbol1 = new ComboBox();
            label2 = new Label();
            label1 = new Label();
            btn_Ok = new Button();
            btn_Cancel = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbx_Symbol2);
            groupBox1.Controls.Add(cbx_Symbol1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 22);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(232, 113);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Select Symbols";
            // 
            // cbx_Symbol2
            // 
            cbx_Symbol2.FormattingEnabled = true;
            cbx_Symbol2.Location = new Point(88, 65);
            cbx_Symbol2.Name = "cbx_Symbol2";
            cbx_Symbol2.Size = new Size(120, 25);
            cbx_Symbol2.TabIndex = 1;
            // 
            // cbx_Symbol1
            // 
            cbx_Symbol1.FormattingEnabled = true;
            cbx_Symbol1.Location = new Point(88, 32);
            cbx_Symbol1.Name = "cbx_Symbol1";
            cbx_Symbol1.Size = new Size(120, 25);
            cbx_Symbol1.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 68);
            label2.Name = "label2";
            label2.Size = new Size(65, 17);
            label2.TabIndex = 0;
            label2.Text = "Symbol 2:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 33);
            label1.Name = "label1";
            label1.Size = new Size(65, 17);
            label1.TabIndex = 0;
            label1.Text = "Symbol 1:";
            // 
            // btn_Ok
            // 
            btn_Ok.Location = new Point(36, 150);
            btn_Ok.Name = "btn_Ok";
            btn_Ok.Size = new Size(86, 30);
            btn_Ok.TabIndex = 2;
            btn_Ok.Text = "Ok";
            btn_Ok.UseVisualStyleBackColor = true;
            btn_Ok.Click += btn_Ok_Click;
            // 
            // btn_Cancel
            // 
            btn_Cancel.Location = new Point(138, 150);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new Size(86, 30);
            btn_Cancel.TabIndex = 2;
            btn_Cancel.Text = "Cancel";
            btn_Cancel.UseVisualStyleBackColor = true;
            btn_Cancel.Click += btn_Cancel_Click;
            // 
            // SymbolSelectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(259, 197);
            Controls.Add(btn_Cancel);
            Controls.Add(btn_Ok);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SymbolSelectForm";
            Text = "Start Tracking";
            FormClosing += SymbolSelectForm_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ComboBox cbx_Symbol2;
        private ComboBox cbx_Symbol1;
        private Label label2;
        private Label label1;
        private Button btn_Ok;
        private Button btn_Cancel;
    }
}