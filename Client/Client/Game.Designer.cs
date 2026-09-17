namespace Bomber.Client
{
    partial class Game
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
            gamePanel = new Panel();
            SuspendLayout();
            // 
            // gamePanel
            // 
            gamePanel.Location = new Point(81, 36);
            gamePanel.Name = "gamePanel";
            gamePanel.Size = new Size(609, 333);
            gamePanel.TabIndex = 0;
            gamePanel.Paint += this.GamePanel_Paint;
            // 
            // Game
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(gamePanel);
            Name = "Game";
            Text = "Bomber";
            ResumeLayout(false);
        }

        #endregion

        private Panel gamePanel;
    }
}