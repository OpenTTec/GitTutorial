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
    private volatile bool _isRoundActive = false;
    private volatile bool _allowWindowClose = false;
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

      Runner = new Thread( new ThreadStart( Run ) );
      Runner.Start();
    }

    //-------------------------------------------------------------------------

    private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
    {
      EndGame();

      e.Cancel = _allowWindowClose;
    }

    //-------------------------------------------------------------------------

    private void NewRound()
    {
      _isRoundActive = false;

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

      _isRoundActive = true;
    }

    //-------------------------------------------------------------------------

    private void EndGame()
    {
      _stopThread = true;
    }

    //-------------------------------------------------------------------------

    private void Run()
    {
      PerformGameStepDelegate gameStepDelegate = new PerformGameStepDelegate( PerformGameStep );

      while( _stopThread == false )
      {
        try
        {
          // Perform a game step if there's currently an active round.
          if( _isRoundActive )
          {
            if( InvokeRequired )
            {
              Invoke( gameStepDelegate );
            }
            else
            {
              PerformGameStep();
            }
          }

          // Sleep for a bit.
          Thread.Sleep( (int)( Constants.c_timeStep * 1000 ) );
        }
        catch
        {
          // Ignore the thread interrupted exception and any others.
        }
      }

      // Close the window.
      if( InvokeRequired )
      {
        BeginInvoke( new CloseWindowDelegate( CloseWindow ) );
      }
      else
      {
        CloseWindow();
      }
    }

    //-------------------------------------------------------------------------

    // Returns 'false' when game is over.

    private delegate void PerformGameStepDelegate();

    private void PerformGameStep()
    {
      Point point = new Point();
      List<CodeMonkeyControl> controlsToRemove = new List<CodeMonkeyControl>();
      string roundCompleteMsg = "";

      // Let smasher run to update the monkeys.
      Smasher.SmashTheCodeMonkeys( Constants.c_timeStep );

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
        _isRoundActive = false;
        roundCompleteMsg =
          ((CodeMonkey)MonkeyControls[ 0 ].Tag).GetSafeName() + " is victorious!";
      }
      else if( MonkeyControls.Count == 0 )
      {
        _isRoundActive = false;
        roundCompleteMsg = "Huh, fancy that... all the monkeys were smashed!";
      }

      // If round is complete display a message and ask the user
      // if another round is in order.
      if( _isRoundActive == false )
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

      return;
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

    private delegate void CloseWindowDelegate();

    private void CloseWindow()
    {
      _allowWindowClose = true;
      Close();
    }

    //-------------------------------------------------------------------------
  }
}
