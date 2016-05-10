using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;

namespace GitTutorial
{
  public partial class MainForm : Form
  {
    //-------------------------------------------------------------------------

    private MonkeySmasher Smasher { get; set; }
    private Thread Runner { get; set; }
    private List<Label> Labels { get; set; }

    //-------------------------------------------------------------------------

    public MainForm()
    {
      // Init the UI.
      InitializeComponent();

      // Fire up the smasher.
      try
      {
        Smasher = new MonkeySmasher();
      }
      catch( Exception ex )
      {
        Debug.WriteLine( ex.Message );

        MessageBox.Show(
          "The Monkey-Smasher appears to be broken." +
            Environment.NewLine + Environment.NewLine +
            "What have you done?",
          ":(",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error );

        throw ex;
      }

      // Create labels for each monkey.
      Random rnd = new Random();
      Labels = new List<Label>();

      foreach( CodeMonkey monkey in Smasher.CodeMonkeys )
      {
        monkey.X = (float)( rnd.NextDouble() * 800 );
        monkey.Y = (float)( rnd.NextDouble() * 800 );

        Label lbl = new Label();
        lbl.Text = monkey.GetName();
        lbl.Tag = monkey;
        lbl.AutoSize = true;
        lbl.Dock = DockStyle.None;
        lbl.ForeColor = monkey.GetFavouriteColour();
        lbl.TextAlign = ContentAlignment.MiddleCenter;

        Labels.Add( lbl );
        Controls.Add( lbl );
      }
      
      // Fire up the runner thread.
      Runner = new Thread( new ThreadStart( Run ) );
      Runner.Start();
    }

    //-------------------------------------------------------------------------

    private void MainForm_FormClosed( object sender, FormClosedEventArgs e )
    {
      Runner.Abort();
      Runner.Join();
    }

    //-------------------------------------------------------------------------

    private void Run()
    {
      UpdateLabelsDelegate updateDelegate = new UpdateLabelsDelegate( UpdateLabels );

      while( Runner.IsAlive )
      {
        try
        {
          Smasher.SmashTheCodeMonkeys();

          Invoke( updateDelegate );

          Thread.Sleep( 1 );
        }
        catch
        {
          // Ignore the interrupted exception.
        }
      }
    }

    //-------------------------------------------------------------------------

    private delegate void UpdateLabelsDelegate();

    private void UpdateLabels()
    {
      Point point = new Point();

      foreach( Label lbl in Labels )
      {
        CodeMonkey monkey = (CodeMonkey)lbl.Tag;

        // Update position.
        point.X = (int)monkey.X;
        point.Y = (int)monkey.Y;

        if( point.X < 0 )
        {
          point.X = Width;
        }
        if( point.X > Width - 5 )
        {
          point.X = 0;
        }
        if( point.Y < 0 )
        {
          point.Y = Height - 50;
        }
        if( point.Y > Height - 5 )
        {
          point.Y = 0;
        }

        lbl.Location = point;

        // Update colour.
        if( monkey.IsSmashed )
        {
          lbl.BackColor = Color.Red;
        }
      }
    }

    //-------------------------------------------------------------------------
  }
}
