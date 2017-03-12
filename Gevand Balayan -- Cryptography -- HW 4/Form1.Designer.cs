namespace Gevand_Balayan____Cryptography____HW_4
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
            this.btnStart = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.radEnc = new System.Windows.Forms.RadioButton();
            this.radDec = new System.Windows.Forms.RadioButton();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(147, 36);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(187, 12);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(1251, 340);
            this.txtInput.TabIndex = 1;
            this.txtInput.Text = "Hello my name is Gevand. I woud like to send you a secret message!";
            // 
            // radEnc
            // 
            this.radEnc.AutoSize = true;
            this.radEnc.Checked = true;
            this.radEnc.Location = new System.Drawing.Point(12, 59);
            this.radEnc.Name = "radEnc";
            this.radEnc.Size = new System.Drawing.Size(61, 17);
            this.radEnc.TabIndex = 2;
            this.radEnc.TabStop = true;
            this.radEnc.Text = "Encrypt";
            this.radEnc.UseVisualStyleBackColor = true;
            // 
            // radDec
            // 
            this.radDec.AutoSize = true;
            this.radDec.Location = new System.Drawing.Point(12, 82);
            this.radDec.Name = "radDec";
            this.radDec.Size = new System.Drawing.Size(62, 17);
            this.radDec.TabIndex = 3;
            this.radDec.Text = "Decrypt";
            this.radDec.UseVisualStyleBackColor = true;
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(184, 367);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(66, 13);
            this.lblOutput.TabIndex = 4;
            this.lblOutput.Text = "Output here:";
            // 
            // txtKey
            // 
            this.txtKey.Enabled = false;
            this.txtKey.Location = new System.Drawing.Point(12, 136);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(147, 20);
            this.txtKey.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "16 Byte Key";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1450, 631);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.radDec);
            this.Controls.Add(this.radEnc);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "Gevand Balayan -- Homework 2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.RadioButton radEnc;
        private System.Windows.Forms.RadioButton radDec;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label label2;
    }
}

