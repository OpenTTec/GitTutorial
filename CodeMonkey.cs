using System.Drawing;

namespace GitTutorial
{
  abstract class CodeMonkey
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

    //-------------------------------------------------------------------------
    // Abstract methods.

    public abstract string GetName();
    public abstract Color GetFavouriteColour();

    //-------------------------------------------------------------------------
  }
}
