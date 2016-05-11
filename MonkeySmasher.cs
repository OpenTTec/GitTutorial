using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace GitTutorial
{
  class MonkeySmasher
  {
    //-------------------------------------------------------------------------

    // List of all the instantiated monkeys.
    public List< CodeMonkey > CodeMonkeys { get; private set; }

    //-------------------------------------------------------------------------

    struct Vector
    {
      public float x;
      public float y;

      public void NormaliseVector()
      {
        float m = Magnitude();

        if( m != 0f )
        {
          x /= m;
          y /= m;
        }
      }

      public float Magnitude()
      {
        return (float)Math.Sqrt( ( x * x ) + ( y * y ) );            
      }
    }

    //-------------------------------------------------------------------------

    public MonkeySmasher()
    {
      CodeMonkeys = new List<CodeMonkey>();

      LetThereBeCodeMonkeys();
    }

    //-------------------------------------------------------------------------

    private void LetThereBeCodeMonkeys()
    {
      // Lookup all the monkeys.
      Type[] types =
        Assembly.GetExecutingAssembly().GetTypes().Where(
          t => String.Equals(
              t.Namespace,
              "GitTutorial.CodeMonkeys",
              StringComparison.Ordinal ) ).ToArray();

      // Instantiate all of the monkeys.
      foreach( Type type in types )
      {
        CodeMonkeys.Add(
          (CodeMonkey)Activator.CreateInstance( type ) );

        // For debugging purposes, add some extra monkeys.
        for( int i = 0; i < Constants.c_debugging_extraMonkeyCount; i++ )
        {
          CodeMonkeys.Add(
            (CodeMonkey)Activator.CreateInstance( type ) );
        }
      }
    }

    //-------------------------------------------------------------------------

    public void SmashTheCodeMonkeys( float deltaTime )
    {
      ZeroAllForces();
      CalculateAttraction();
      MoveMonkeys( deltaTime );
    }

    //-------------------------------------------------------------------------

    private void ZeroAllForces()
    {
      foreach( CodeMonkey monkey in CodeMonkeys )
      {
        monkey.FX = 0.0f;
        monkey.FY = 0.0f;
      }
    }

    //-------------------------------------------------------------------------

    private void CalculateAttraction()
    {
      Vector from1To2 = new Vector();
      float mass1;
      float mass2;
      float totalMass;
      float distance;

      foreach( CodeMonkey monkey1 in CodeMonkeys )
      {
        if( monkey1.IsSmashed )
        {
          continue;
        }

        foreach( CodeMonkey monkey2 in CodeMonkeys )
        {
          // Don't be attracted to one's self or dead monkeys.
          if( monkey1 == monkey2 ||
              monkey2.IsSmashed )
          {
            continue;
          }

          // Get some required values.
          mass1 = monkey1.GetSafeName().Length;
          mass2 = monkey2.GetSafeName().Length;
          totalMass = mass1 + mass2;

          distance =
            (float)Math.Sqrt(
              ( ( monkey1.X - monkey2.X ) * ( monkey1.X - monkey2.X ) ) +
              ( ( monkey1.Y - monkey2.Y ) * ( monkey1.Y - monkey2.Y ) ) );

          from1To2.x = monkey2.X - monkey1.X;
          from1To2.y = monkey2.Y - monkey1.Y;
          from1To2.NormaliseVector();

          // Calculate attractive force.
          float f = -Constants.c_gravitationalConstant * ( ( mass1 * mass2 ) / distance );

          // Apply force to monkeys.
          monkey1.FX += from1To2.x * ( -f * ( totalMass / mass1 ) );
          monkey1.FY += from1To2.y * ( -f * ( totalMass / mass1 ) );

          monkey2.FX += from1To2.x * ( f * ( totalMass / mass2 ) );
          monkey2.FY += from1To2.y * ( f * ( totalMass / mass2 ) );

          // Smash the monkeys if they get too close together.
          if( distance < ( monkey1.Radius + monkey2.Radius ) * 0.5 )
          {
            float speedSqr1 = ( monkey1.VX * monkey1.VX ) + ( monkey1.VY * monkey1.VY );
            float speedSqr2 = ( monkey2.VX * monkey2.VX ) + ( monkey2.VY * monkey2.VY );

            // Faster monkey wins.
            monkey1.IsSmashed = speedSqr2 > speedSqr1;
            monkey2.IsSmashed = !monkey1.IsSmashed;
          }
        }
      }
    }

    //-------------------------------------------------------------------------

    private void MoveMonkeys( float deltaTime )
    {
      foreach( CodeMonkey monkey in CodeMonkeys )
      {
        if( monkey.IsSmashed == false )
        {
          monkey.VX += monkey.FX * deltaTime;
          monkey.VY += monkey.FY * deltaTime;

          monkey.X += monkey.VX * deltaTime;
          monkey.Y += monkey.VY * deltaTime;

          // Apply some drag, this helps to keep the rounds from dragging on.
          monkey.VX *= Constants.c_dragFactor;
          monkey.VY *= Constants.c_dragFactor;
        }
      }
    }

    //-------------------------------------------------------------------------
  }
}
