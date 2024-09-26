using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimesTable.Mobile.Models;

public class DifficultyLevel
{
    public long GivenTimePerQuestion { get; set; }

    public int Level
    {
        get
        {
            return GivenTimePerQuestion switch
            {
                >= 30000 => 1,
                >= 20000 => 2,
                >= 15000 => 3,
                >= 10000 => 5,
                >= 7000 => 8,
                >= 5000 => 14,
                >= 3000 => 21,
                _ => 0
            };
        }
    }

    public string LevelDescription
    {
        get
        {
            return GivenTimePerQuestion switch
            {
                >= 30000 => "Bebek işi (30sn)",
                >= 20000 => "Çocuk işi (20sn)",
                >= 15000 => "Kolay (15sn)",
                >= 10000 => "Orta (10sn)",
                >= 7000 => "Zorlayıcı (7sn)",
                >= 5000 => "Zor (5sn)",
                >= 3000 => "Çok zor (3sn)",
                _ => "İmkansız"
            };
        }
    }

    public override string ToString()
    {
        return LevelDescription;
    }

    public DifficultyLevel()
    {
        GivenTimePerQuestion = 30000;
    }

    public DifficultyLevel(long givenTimePerQuestion)
    {
        GivenTimePerQuestion = givenTimePerQuestion;
    }
}