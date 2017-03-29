namespace Forms_Control
{
    partial class PuppetForm
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
            this.components = new System.ComponentModel.Container();
            this.notifyBubble = new System.Windows.Forms.NotifyIcon(this.components);
            this.chatBubble = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // notifyBubble
            // 
            this.notifyBubble.Text = "Your totally not shady friend";
            this.notifyBubble.Visible = true;
            // 
            // chatBubble
            // 
            this.chatBubble.IsBalloon = true;
            // 
            // PuppetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PuppetForm";
            this.ShowInTaskbar = false;
            this.Text = "Forms Controller";
            this.TopMost = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PuppetForm_Paint);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.PuppetForm_MouseDoubleClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyBubble;
        private System.Windows.Forms.ToolTip chatBubble;
    }
}

