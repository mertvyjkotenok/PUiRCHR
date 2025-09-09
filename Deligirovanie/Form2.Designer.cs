namespace Deligirovanie
{
    partial class Form2
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
            label1 = new Label();
            label2 = new Label();
            popa4 = new CheckedListBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Courier New", 25.8000011F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(307, 9);
            label1.Name = "label1";
            label1.Size = new Size(178, 50);
            label1.TabIndex = 0;
            label1.Text = "ТЕСТ 1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Courier New", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label2.Location = new Point(321, 59);
            label2.Name = "label2";
            label2.Size = new Size(150, 31);
            label2.TabIndex = 1;
            label2.Text = "ВОПРОС 1";
            // 
            // popa4
            // 
            popa4.FormattingEnabled = true;
            popa4.Location = new Point(272, 167);
            popa4.Name = "popa4";
            popa4.Size = new Size(267, 158);
            popa4.TabIndex = 2;
            popa4.SelectedIndexChanged += checkedListBox2_SelectedIndexChanged;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 128, 255);
            ClientSize = new Size(800, 450);
            Controls.Add(popa4);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form2";
            Text = "ТЕСТ 1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private CheckedListBox popa4;
    }
}