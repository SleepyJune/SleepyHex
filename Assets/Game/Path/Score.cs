using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class Score
{
    public static Score current = null;

    public Level level;
    public int points;

    public float time;

    public int stars;

    public Score(Level level, int points)
    {
        this.level = level;
        this.points = points;

        GetStars();
    }

    public void GetStars()
    {
        if (level.hasSolution)
        {
            var worst = level.solution.worstScore;
            var best = level.solution.bestScore;

            if(points == best)
            {
                stars = 3;
            }
            else if(points == worst)
            {
                stars = 1;
            }
            else
            {
                var percent = (float)(points - worst) / (best - worst);

                stars = percent >= .5f ? 2 : 1;
            }

            Debug.Log(stars);
        }
    }
}
