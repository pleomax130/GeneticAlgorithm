using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    class Program
    {
        static void Main(string[] args)
        {

            var parser = new DataLoader();
            Data data = null;
            var timeLimit = new int();
            int sizePopulation;
            double pC, pM;
            Solver solver = null;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(
                    $"\n1.Wczytanie danych z pliku\n2.Wprowadzenie kryterium stopu\n3.Wprowadzenie wielkości populacji\n4.Wprowadzenie współczynnika krzyżowania\n5.Wprowadzenie współczynnika mutacji\n6.Uruchom algorytm");
                var decision = Convert.ToInt32(Console.ReadLine());
                Console.Clear();
                switch (decision)
                {
                    case 1:
                        Console.WriteLine($"Podaj nazwe pliku: ");
                        var fileName = Console.ReadLine();
                        data = new Data();
                        parser.LoadData(out data.TspArray, out data.optimal, out data.Cities, fileName);
                        solver = new Solver(data);
                        Console.Clear();
                        break;
                    case 2:
                        Console.WriteLine($"Ile czasu ma dzialac algorytm [s]: ");
                        var time = Convert.ToInt32(Console.ReadLine());
                        timeLimit = time;
                        Console.Clear();
                        break;
                    case 3:
                        Console.WriteLine($"Wielkość populacji : ");
                        var size = Convert.ToInt32(Console.ReadLine());
                        sizePopulation = size;
                        solver.PopulationSize = sizePopulation;
                        Console.Clear();
                        break;
                    case 4:
                        Console.WriteLine($"Wspolczynnik krzyzowania: ");
                        var pc = Convert.ToDouble(Console.ReadLine());
                        pC = pc;
                        if (solver != null) solver.Pc = pC;
                        Console.Clear();
                        break;
                    case 5:
                        Console.WriteLine($"Wspolczynnik mutacji: ");
                        var pm = Convert.ToDouble(Console.ReadLine());
                        pM = pm;
                        if (solver != null) solver.Pm = pM;
                        Console.Clear();
                        break;
                    case 6:
                        if (solver != null)
                        {
                            solver.TimeLimit = timeLimit;
                            solver.Solve();
                        }
                        Console.ReadKey();
                        break;

                }
                //var parser = new DataLoader();
                //var data = new Data();
                //parser.LoadData(out data.TspArray, out data.optimal, out data.Cities, "eil51");
                //var solver = new Solver(data);
                //solver.Solve();
            }
        }
    }
}
