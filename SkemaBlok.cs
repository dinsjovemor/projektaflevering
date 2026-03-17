using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace projektaflevering
{
    public class SkemaBlok
    {
        public int DayIndex { get; }
        public int StartHour { get; }
        public int StartMinute { get; }
        public int EndHour { get; }
        public int EndMinute { get; }
        public string Title { get; }

        public SkemaBlok(int day, int startH, int startM, int endH, int endM, string title)
        {
            DayIndex = day;
            StartHour = startH;
            StartMinute = startM;
            EndHour = endH;
            EndMinute = endM;
            Title = title;
        }

    }
}
