namespace HITUI
{
    partial class UIDisplayForm
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
            this.TextControl = new System.Windows.Forms.Label();
            this.butOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextControl
            // 
            this.TextControl.AutoSize = true;
            this.TextControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextControl.Location = new System.Drawing.Point(29, 26);
            this.TextControl.Name = "TextControl";
            this.TextControl.Size = new System.Drawing.Size(0, 24);
            this.TextControl.TabIndex = 0;
            // 
            // butOk
            // 
            this.butOk.Location = new System.Drawing.Point(150, 86);
            this.butOk.Name = "butOk";
            this.butOk.Size = new System.Drawing.Size(90, 42);
            this.butOk.TabIndex = 1;
            this.butOk.Text = "Ok";
            this.butOk.UseVisualStyleBackColor = true;
            this.butOk.Click += new System.EventHandler(this.butOk_Click);
            // 
            // UIDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 141);
            this.Controls.Add(this.butOk);
            this.Controls.Add(this.TextControl);
            this.Name = "UIDisplayForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button butOk;
        public System.Windows.Forms.Label TextControl;
    }
}