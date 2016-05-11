using System.Drawing;

namespace GitTutorial
{
  public abstract class CodeMonkey
  {
    //-------------------------------------------------------------------------
    // Properties.

    // Position.
    public float X { get; set; }
    public float Y { get; set; }

    // Velocity.
    public float VX { get; set; }
    public float VY { get; set; }

    // Force.
    public float FX { get; set; }
    public float FY { get; set; }

    // Is smashed?
    public bool IsSmashed { get; set; }

    // Monkey's radius.
    public int Radius { get; set; }

    //-------------------------------------------------------------------------
    // Abstract methods.

    protected abstract string GetName();
    public abstract Color GetFavouriteColour();

    //-------------------------------------------------------------------------

    public string GetSafeName()
    {
      string name = GetName();

      if( name.Length == 0 )
      {
        return "The Nameless One";
      }

      return name;
    }

    //-------------------------------------------------------------------------
  }
}
