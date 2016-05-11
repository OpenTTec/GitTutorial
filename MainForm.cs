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
    private volatile bool _stopThread = false;
    private List<CodeMonkeyControl> MonkeyControls { get; set; }

    //-------------------------------------------------------------------------

    public MainForm()
    {
      DoubleBuffered = true;

      InitializeComponent();

      BackgroundImage = Resources.Background;

      WindowState = FormWindowState.Normal;
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

    private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
    {
      EndRound();
    }

    //-------------------------------------------------------------------------

    private void NewRound()
    {
      // If there was a prev round running, end the runner thread.
      EndRound();

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

        const float halfMaxV = Constants.c_initialMaxV * 0.5f;
        monkey.VX = halfMaxV - (float)rnd.NextDouble() * Constants.c_initialMaxV;
        monkey.VY = halfMaxV - (float)rnd.NextDouble() * Constants.c_initialMaxV;

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
      System.Diagnostics.Debug.Assert( Runner == null );

      _stopThread = false;
      Runner = new Thread( new ThreadStart( Run ) );
      Runner.Start();
    }

    //-------------------------------------------------------------------------

    private void EndRound()
    {
      _stopThread = true;

      if( Runner != null )
      {
        Runner.Join();
        Runner = null;
      }
    }

    //-------------------------------------------------------------------------

    private void Run()
    {
      PerformGameStepDelegate updateDelegate = new PerformGameStepDelegate( PerformGameStep );

      while( _stopThread == false )
      {
        try
        {
          Smasher.SmashTheCodeMonkeys( Constants.c_timeStep );

          if( InvokeRequired )
          {
            Invoke( updateDelegate );
          }
          else
          {
            PerformGameStep();
          }

          Thread.Sleep( (int)( Constants.c_timeStep * 1000 ) );
        }
        catch
        {
          // Ignore the thread interrupted exception and any others.
        }
      }
    }

    //-------------------------------------------------------------------------

    private delegate void PerformGameStepDelegate();

    private void PerformGameStep()
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
          monkey.VX = Math.Abs( monkey.VX );
          monkey.VX += Constants.c_boundryRepulsionForce;
        }
        if( point.X + control.Width > Width - 20 )
        {
          monkey.VX = -Math.Abs( monkey.VX );
          monkey.VX += -Constants.c_boundryRepulsionForce;
        }
        if( point.Y < 0 )
        {
          monkey.VY = Math.Abs( monkey.VY );
          monkey.VY += Constants.c_boundryRepulsionForce;
        }
        if( point.Y + control.Height > Height - 40 )
        {
          monkey.VY = -Math.Abs( monkey.VY );
          monkey.VY += -Constants.c_boundryRepulsionForce;
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

    private void MainForm_Resize( object sender, EventArgs e )
    {
      if( MonkeyControls == null )
      {
        return;
      }

      // If any monkey falls beyond the new window edges, clip it back.
      foreach( CodeMonkeyControl control in MonkeyControls )
      {
        CodeMonkey monkey = (CodeMonkey)control.Tag;

        if( monkey.X + control.Width > Width )
        {
          monkey.X = Width - control.Width;
        }

        if( monkey.Y + control.Height > Height )
        {
          monkey.Y = Height - control.Height;
        }
      }
    }

    //-------------------------------------------------------------------------
  }
}
