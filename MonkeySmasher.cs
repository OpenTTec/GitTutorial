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

        // TODO: Remove extra adds, just for testing.
        CodeMonkeys.Add(
          (CodeMonkey)Activator.CreateInstance( type ) );
        CodeMonkeys.Add(
          (CodeMonkey)Activator.CreateInstance( type ) );
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
      float mass1;
      float mass2;
      float distance;

      foreach( CodeMonkey monkey1 in CodeMonkeys )
      {
        if( monkey1.IsSmashed )
        {
          continue;
        }

        foreach( CodeMonkey monkey2 in CodeMonkeys )
        {
          // Don't be attracted to one's self.
          if( monkey1 == monkey2 ||
              monkey2.IsSmashed )
          {
            continue;
          }

          // Calculate attraction between both monkeys.
          mass1 = monkey1.GetName().Length;
          mass2 = monkey2.GetName().Length;

          distance =
            (float)Math.Sqrt(
              ( ( monkey1.X - monkey2.X ) * ( monkey1.X - monkey2.X ) ) +
              ( ( monkey1.Y - monkey2.Y ) * ( monkey1.Y - monkey2.Y ) ) );

          from1To2.x = monkey2.X - monkey1.X;
          from1To2.y = monkey2.Y - monkey1.Y;

          NormaliseVector( ref from1To2 );

          float f = -1e-2f * ( ( mass1 * mass2 ) / distance );

          monkey2.FX += from1To2.x * f;
          monkey2.FY += from1To2.y * f;

          // Smash the monkeys if they get too close together.
          if( distance < 10f )
          {
            monkey1.IsSmashed = mass2 >= mass1;
            monkey2.IsSmashed = mass1 >= mass2;
            continue;
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

    static private void NormaliseVector( ref Vector vector )
    {
      float len =
        (float)Math.Sqrt(
          ( vector.x * vector.x ) + ( vector.y * vector.y ) );

      if( len != 0f )
      {
        vector.x /= len;
        vector.y /= len;
      }
    }

    //-------------------------------------------------------------------------
  }
}
