using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IProjectile
{
    float Speed
    {
        get; set;
    }
    float Lifetime
    {
        get; set;
    }
    int HitDamage
    {
        get; set;
    }
}

