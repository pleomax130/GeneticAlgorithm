using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    class Tour 
    {
        public int Cost { get; set; }
        public List<int> TourList { get; set; }

        public int CalculateCost(Data data)
        {
            var cost = 0;
            for (int i = 0; i < TourList.Count - 1; i++)
                cost += data.TspArray[TourList[i]][TourList[i + 1]];
            cost += data.TspArray[TourList[TourList.Count - 1]][TourList[0]];
            return cost;
        }

        public Tour()
        {
            TourList= new List<int>();
        }

        public Tour(List<int> list, Data data)
        {
            TourList = list;
            Cost = CalculateCost(data);
        }
    }
}
