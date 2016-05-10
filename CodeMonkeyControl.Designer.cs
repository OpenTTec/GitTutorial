namespace GitTutorial
{
  partial class CodeMonkeyControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if( disposing && ( components != null ) )
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.uiName = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // uiName
      // 
      this.uiName.AutoSize = true;
      this.uiName.BackColor = System.Drawing.Color.Transparent;
      this.uiName.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.uiName.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
      this.uiName.Location = new System.Drawing.Point( 0, 108 );
      this.uiName.Name = "uiName";
      this.uiName.Size = new System.Drawing.Size( 73, 20 );
      this.uiName.TabIndex = 0;
      this.uiName.Text = "<name>";
      this.uiName.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      // 
      // CodeMonkeyControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Transparent;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.Controls.Add( this.uiName );
      this.DoubleBuffered = true;
      this.Name = "CodeMonkeyControl";
      this.Size = new System.Drawing.Size( 128, 128 );
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label uiName;
  }
}
