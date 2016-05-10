using System.Drawing;

namespace GitTutorial.CodeMonkeys
{
  class Test : CodeMonkey
  {
    //-------------------------------------------------------------------------

    public override string GetName()
    {
      return "T";
    }

    //-------------------------------------------------------------------------

    public override Color GetFavouriteColour()
    {
      return Color.Blue;
    }

    //-------------------------------------------------------------------------
  }
}
