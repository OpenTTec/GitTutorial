using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace GitTutorial
{
  class MonkeySmasher
  {
    //-------------------------------------------------------------------------

    public List< CodeMonkey > CodeMonkeys { get; private set; }

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

        //// TODO: Remove extra adds, just for testing.
        //CodeMonkeys.Add(
        //  (CodeMonkey)Activator.CreateInstance( type ) );
        //CodeMonkeys.Add(
        //  (CodeMonkey)Activator.CreateInstance( type ) );
      }
    }

    //-------------------------------------------------------------------------

    public void SmashTheCodeMonkeys()
    {
      ZeroAllForces();
      CalculateAttraction();
      MoveMonkeys();
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
      Vector from2To1 = new Vector();
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

          from2To1.x = monkey1.X - monkey2.X;
          from2To1.y = monkey1.Y - monkey2.Y;
          from2To1.NormaliseVector();

          // Calculate attractive force.
          // We're fudging a gravitational constant that works for us.
          float f = -1e-1f * ( ( mass1 * mass2 ) / distance );

          // Apply force to monkeys.
          monkey1.FX += from2To1.x * ( f * ( totalMass / mass1 ) );
          monkey1.FY += from2To1.y * ( f * ( totalMass / mass1 ) );

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

    private void MoveMonkeys()
    {
      foreach( CodeMonkey monkey in CodeMonkeys )
      {
        if( monkey.IsSmashed == false )
        {
          monkey.VX += monkey.FX;
          monkey.VY += monkey.FY;

          monkey.X += monkey.VX;
          monkey.Y += monkey.VY;
        }
      }
    }

    //-------------------------------------------------------------------------
  }
}
