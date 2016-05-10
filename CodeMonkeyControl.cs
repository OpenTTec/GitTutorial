using System;
using System.Windows.Forms;

namespace GitTutorial
{
  public partial class CodeMonkeyControl : UserControl
  {
    //-------------------------------------------------------------------------

    private CodeMonkey Monkey { get; set; }

    //-------------------------------------------------------------------------

    public CodeMonkeyControl( CodeMonkey monkey )
    {
      InitializeComponent();

      BackgroundImage = Resources.CodeMonkey;

      Monkey = monkey;

      uiName.Text = Monkey.GetSafeName();
      uiName.ForeColor = Monkey.GetFavouriteColour();

      if( uiName.Text.Length == 0 )
      {
        uiName.Text = "The Nameless One";
      }

      this.Width = uiName.Width;
      this.Height = uiName.Width + 20;    // Leave space for the name under the pic.
    }

    //-------------------------------------------------------------------------

    public void DoLogic()
    {
      if( Monkey.IsSmashed )
      {
        BackgroundImage = Resources.DeadCodeMonkey;
      }
    }

    //-------------------------------------------------------------------------

    public int Radius
    {
      get
      {
        return Width;
      }
    }

    //-------------------------------------------------------------------------
  }
}
