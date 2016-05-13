using System.Drawing;

namespace GitTutorial.CodeMonkeys
{
  class TestUser : CodeMonkey
  {
    //-------------------------------------------------------------------------

    protected override string GetName()
    {
      return "TestUser";
    }

    //-------------------------------------------------------------------------

    public override Color GetFavouriteColour()
    {
      return Color.Red;
    }

    //-------------------------------------------------------------------------
  }
}
