namespace DeliverySystem
{
    partial class FMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.bOpenFile = new System.Windows.Forms.Button();
            this.tFilePath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // bOpenFile
            // 
            resources.ApplyResources(this.bOpenFile, "bOpenFile");
            this.bOpenFile.Name = "bOpenFile";
            this.bOpenFile.UseVisualStyleBackColor = true;
            this.bOpenFile.Click += new System.EventHandler(this.bOpenFile_Click);
            // 
            // tFilePath
            // 
            resources.ApplyResources(this.tFilePath, "tFilePath");
            this.tFilePath.Name = "tFilePath";
            // 
            // FMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tFilePath);
            this.Controls.Add(this.bOpenFile);
            this.Name = "FMain";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button bOpenFile;
        private System.Windows.Forms.TextBox tFilePath;
    }
}

