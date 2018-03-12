using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    class Data
    {
        public int Cities;  // Liczba miast
        public int[][] TspArray;    // Tablica dwuwymiarowa przechowująca macierz kosztów
        public int optimal;
        public Data(int cities, int[][] tspArray)
        {
            Cities = cities;
            TspArray = tspArray;
        }

        public Data()
        {
            
        }
    }
}
