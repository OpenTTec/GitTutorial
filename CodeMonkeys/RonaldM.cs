using System.Drawing;

namespace GitTutorial.CodeMonkeys
{
  internal class RonaldM : CodeMonkey
  {
    //-------------------------------------------------------------------------

    protected override string GetName()
    {
      return "RonaldM";
    }

    //-------------------------------------------------------------------------

    public override Color GetFavouriteColour()
    {
      return Color.BlanchedAlmond;
    }

    //-------------------------------------------------------------------------
  }
}