namespace GitTutorial
{
  class Constants
  {
    //-------------------------------------------------------------------------

    // Initial max speed monkey can have.
    public const float c_initialMaxV = 50000f;

    // Force applied to a monkey to repel it from the window boundry.
    public const float c_boundryRepulsionForce = 100f;

    // Time-step.
    public const float c_timeStep = 0.001f;

    // We're fudging a gravitational constant that works for us.
    public const float c_gravitationalConstant = 1e5f;

    // Drag force.
    public const float c_dragFactor = 0.99f;

    //-------------------------------------------------------------------------
    // Debugging.

    // Should be zero, adds duplicate monkeys for debugging purposes.
    public const int c_debugging_extraMonkeyCount = 5;

    //-------------------------------------------------------------------------
  }
}
