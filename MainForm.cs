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
    private List<CodeMonkeyControl> MonkeyControls { get; set; }

    //-------------------------------------------------------------------------

    public MainForm()
    {
      DoubleBuffered = true;

      InitializeComponent();

      BackgroundImage = Resources.Background;
    }

    //-------------------------------------------------------------------------

    protected override CreateParams CreateParams
    {
      get
      {
        // We want full double-buffering.
        CreateParams cp = base.CreateParams;
        cp.ExStyle |= 0x02000000;  // WS_EX_COMPOSITED
        return cp;
      }
    }

    //-------------------------------------------------------------------------

    private void MainForm_Load( object sender, EventArgs e )
    {
      NewRound();
    }

    //-------------------------------------------------------------------------

    private void MainForm_FormClosed( object sender, FormClosedEventArgs e )
    {
      if( Runner != null )
      {
        Runner.Abort();
        Runner.Join();
      }
    }

    //-------------------------------------------------------------------------

    private void NewRound()
    {
      // If there was a prev round running, end the runner thread.
      if( Runner != null )
      {
        Runner.Abort();
        Runner.Join();
      }

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

      // Create form controls for each monkey.
      Controls.Clear();

      Random rnd = new Random();
      MonkeyControls = new List<CodeMonkeyControl>();

      foreach( CodeMonkey monkey in Smasher.CodeMonkeys )
      {
        monkey.X = 50 + (float)( rnd.NextDouble() * ( Width * 0.75 ) );
        monkey.Y = 50 + (float)( rnd.NextDouble() * ( Height * 0.75 ) );

        //monkey.VX = (float)( rnd.NextDouble() * 100.5 );
        //monkey.VY = (float)( rnd.NextDouble() * 100.5 );

        CodeMonkeyControl monkeyControl = new CodeMonkeyControl( monkey );
        monkeyControl.Tag = monkey;

        monkey.Radius = monkeyControl.Radius;

        MonkeyControls.Add( monkeyControl );
        Controls.Add( monkeyControl );
      }

      // Only 1 monkey?
      if( MonkeyControls.Count < 2 )
      {
        MessageBox.Show(
          "There are not enough monkeys present, there will be no smashing!",
          "Give me mooaaar monkeys!",
          MessageBoxButtons.OK,
          MessageBoxIcon.Exclamation );

        Close();
        return;
      }

      // Fire up the runner thread.
      Runner = new Thread( new ThreadStart( Run ) );
      Runner.Start();
    }

    //-------------------------------------------------------------------------

    private void Run()
    {
      UpdateMonkeysDelegate updateDelegate = new UpdateMonkeysDelegate( UpdateMonkeys );

      while( Runner.IsAlive )
      {
        try
        {
          Smasher.SmashTheCodeMonkeys( 0.001f );

          Invoke( updateDelegate );

          Thread.Sleep( 10 );
        }
        catch
        {
          // Ignore the thread interrupted exception.
        }
      }
    }

    //-------------------------------------------------------------------------

    private delegate void UpdateMonkeysDelegate();

    private void UpdateMonkeys()
    {
      Point point = new Point();
      List<CodeMonkeyControl> controlsToRemove = new List<CodeMonkeyControl>();
      bool roundIsComplete = false;
      string roundCompleteMsg = "";

      // Update each monkey.
      foreach( CodeMonkeyControl control in MonkeyControls )
      {
        CodeMonkey monkey = (CodeMonkey)control.Tag;

        // Update position.
        point.X = (int)( monkey.X - ( control.Width * 0.5 ) );
        point.Y = (int)( monkey.Y - ( control.Height * 0.5 ) );

        // Bounce back off sides of the screen.
        if( point.X < 0 )
        {
          monkey.VX = -monkey.VX;
        }
        if( point.X + control.Width > Width - 20 )
        {
          monkey.VX = -monkey.VX;
        }
        if( point.Y < 0 )
        {
          monkey.VY = -monkey.VY;
        }
        if( point.Y + control.Height > Height - 40 )
        {
          monkey.VY = -monkey.VY;
        }

        control.Location = point;
        control.DoLogic();

        // Monkey died?
        if( monkey.IsSmashed )
        {
          control.SendToBack();
          controlsToRemove.Add( control );
        }
      }

      // Clear out the dead bodies.
      foreach( CodeMonkeyControl control in controlsToRemove )
      {
        MonkeyControls.Remove( control );
      }

      // Round is complete?
      if( MonkeyControls.Count == 1 )
      {
        roundIsComplete = true;
        roundCompleteMsg =
          ((CodeMonkey)MonkeyControls[ 0 ].Tag).GetSafeName() + " is victorious!";
      }
      else if( MonkeyControls.Count == 0 )
      {
        roundIsComplete = true;
        roundCompleteMsg = "Huh, fancy that... all the monkeys were smashed!";
      }

      // If round is complete display a message and ask the user
      // if another round is in order.
      if( roundIsComplete )
      {
        DialogResult result =
          MessageBox.Show(
            roundCompleteMsg +
              Environment.NewLine + Environment.NewLine +
              "Is more smashing in order?",
            "Round Complete!",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Exclamation );

        if( result == DialogResult.No )
        {
          Close();
          return;
        }

        NewRound();
      }
    }

    //-------------------------------------------------------------------------
  }
}
