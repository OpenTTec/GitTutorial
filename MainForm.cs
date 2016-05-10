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

    private const float c_repelFromEdgesForce = 0f;//.25f;

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
      // Create form controls for each monkey.
      Random rnd = new Random();
      MonkeyControls = new List<CodeMonkeyControl>();

      foreach( CodeMonkey monkey in Smasher.CodeMonkeys )
      {
        monkey.X = 50 + (float)( rnd.NextDouble() * ( Width * 0.75 ) );
        monkey.Y = 50 + (float)( rnd.NextDouble() * ( Height * 0.75 ) );

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

    private void MainForm_FormClosed( object sender, FormClosedEventArgs e )
    {
      if( Runner != null )
      {
        Runner.Abort();
        Runner.Join();
      }
    }

    //-------------------------------------------------------------------------

    private void Run()
    {
      UpdateMonkeysDelegate updateDelegate = new UpdateMonkeysDelegate( UpdateMonkeys );

      while( Runner.IsAlive )
      {
        try
        {
          Smasher.SmashTheCodeMonkeys();

          Invoke( updateDelegate );

          Thread.Sleep( 10 );
        }
        catch
        {
          // Ignore the interrupted exception.
        }
      }
    }

    //-------------------------------------------------------------------------

    private delegate void UpdateMonkeysDelegate();

    private void UpdateMonkeys()
    {
      Point point = new Point();
      List<CodeMonkeyControl> controlsToRemove = new List<CodeMonkeyControl>();

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
          monkey.VX += c_repelFromEdgesForce;
        }
        if( point.X + control.Width > Width - 20 )
        {
          monkey.VX = -monkey.VX;
          monkey.VX += -c_repelFromEdgesForce;
        }
        if( point.Y < 0 )
        {
          monkey.VY = -monkey.VY;
          monkey.VY += c_repelFromEdgesForce;
        }
        if( point.Y + control.Height > Height - 40 )
        {
          monkey.VY = -monkey.VY;
          monkey.VY += -c_repelFromEdgesForce;
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

      // Is there a winner?
      if( MonkeyControls.Count == 1 )
      {
        MessageBox.Show(
          ((CodeMonkey)MonkeyControls[ 0 ].Tag).GetSafeName() + " is victorious!",
          "Victory!",
          MessageBoxButtons.OK,
          MessageBoxIcon.Exclamation );

        Close();
      }
      // Everyone is dead?
      else if( MonkeyControls.Count == 0 )
      {
        MessageBox.Show(
          "Huh, fancy that... all the monkeys were smashed!",
          "Dang!",
          MessageBoxButtons.OK,
          MessageBoxIcon.Exclamation );

        Close();
      }
    }

    //-------------------------------------------------------------------------
  }
}
