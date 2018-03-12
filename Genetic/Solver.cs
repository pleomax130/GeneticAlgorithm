using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Genetic
{
    class Solver
    {
        public Data Data { get; set; }
        public double Pc { get; set; } = 0.8;
        public double Pm { get; set; } = 0.01;
        public int PopulationSize { get; set; } = 60;
        public List<Tour> Population { get; set; }
        public Tour BestTour { get; set; }
        private static Random rng = new Random();
        private static Stopwatch Sw = new Stopwatch();
        public int TimeLimit { get; set; } = 60;

        private List<Tour> CreateFirstPopulation()  // tworzenie populacji poczatkowej 
        {
            var firstPopulation = new List<Tour>(); // lista wszystkich osobnikow populacji
            for (int i = 0; i < PopulationSize; i++)
            {
                var list = new List<int>();
                for (int j = 0; j < Data.TspArray.Length; j++)
                {
                    list.Add(j);
                }
                list.Shuff();   // wymieszanie zawartosci drogi osobnika dodawanego do populacji
                firstPopulation.Add(new Tour(list, Data));
            }
            return firstPopulation;
        }

        private Tour TournamentSelection(List<Tour> pop)    // selekcja turniejowa
        {
            var list = new List<Tour>();
            for (int i = 0; i < 5; i++) // losowo wybierz 5 osobnikow z populacji
            {
                var r = rng.Next(pop.Count);
                list.Add(pop[r]);
            }
            var min = list[0];
            foreach (var tour in list.Skip(1))  // wybierz najlepszego z 5
            {
                if (tour.Cost < min.Cost)
                    min = tour;
            }
            return min;
        }

        private Tour Pmxcrossover(Tour parent1, Tour parent2)   // operacja krzyzowania PMX
        {
            
            var crossElement = new int[Data.Cities];
            int startPos, endPos;   // indexy oznaczajace poczzatek i koniec kopiowanego segmentu
            while (true)    // losowanie indexow
            {
                var x = rng.Next(parent1.TourList.Count);
                var y = rng.Next(parent1.TourList.Count);
                if (x < y)
                {
                     startPos = x;
                     endPos = y;
                    break;
                }
                if (y < x)
                {
                    startPos = y;
                    endPos = x;
                    break;
                } 
            }
            
                for (var i = startPos; i <= endPos; i++)    // skopiowanie wylosowanego segmentu do potomka
                {
                    crossElement[i] = parent1.TourList[i];
                }
                for (var i = 0; i < startPos; i++)
                    crossElement[i] = -1;
                for (var i = endPos+1; i < crossElement.Length; i++)    // wypelnienie reszty poprzez '-1' 
                    crossElement[i] = -1;
                for (var i = startPos; i <= endPos; i++)    // dla kazdego elementu w skopiowanym segmencie
                {
                    if (!crossElement.Contains(parent2.TourList[i]))    // sprawdzenie czy element z rodzica drugiego został juz skopiowany
                    {
                        int index = parent2.TourList.FindIndex(x => x == crossElement[i]);  // sprawdzenie pod ktorym indexem w drugim rodzicu znajduje sie element ktory zostal skopiowany na jego miejsce
                        while (true)
                        {
                            var end = true;
                            for (int j = startPos; j <= endPos; j++)
                            {
                                if (index==j)
                                {
                                    if (crossElement[index] != -1)  // Jezeli element ktory jest na miejscu danego elementu z rodzica drugiego zostal juz skopiowany
                                                                    // musimy znalezc element ktory zostal skopiowany na miejsce tego elementu az nie znajdziemy takiego elementu ktory mozemy skopiowac
                                    {
                                        index = parent2.TourList.FindIndex(x => x == crossElement[index]);
                                        end = false;
                                    }
                                    break;
                                }
                            }
                            if (end) break;
                        }
                        crossElement[index] = parent2.TourList[i];
                    }
                }
                for(int i = 0; i<crossElement.Length;i++)
                {
                    if (crossElement[i] == -1)
                        crossElement[i] = parent2.TourList[i];
                }

            var child = new Tour(crossElement.ToList(), Data);
            return child;
        }

        private Tour CrossoverSelection(List<Tour> selectionList)   // selekcja po krosowaniu osobnikow
        {
            var min = selectionList[0];
            foreach (var tour in selectionList)
            {
                if (tour.Cost < min.Cost)
                    min = tour;
            }
            return min;
        }
        private Tour InvertMutation(Tour child) // mutacja invert
        {
            int startPos, endPos;   // indexy zamienianego segmentu
            while (true)
            {
                var x = rng.Next(child.TourList.Count);
                var y = rng.Next(child.TourList.Count);
                if (x < y)
                {
                    startPos = x;
                    endPos = y;
                    break;
                }
                if (y < x)
                {
                    startPos = y;
                    endPos = x;
                    break;
                }
            }
            var newChild = new Tour();
            foreach (var i in child.TourList)
                newChild.TourList.Add(i);
            var diff = endPos - startPos;
            for (var i = 0; i <= diff; i++) // inwersja wskazanego segmentu
                newChild.TourList[startPos + i] = child.TourList[endPos - i];
            newChild.Cost = newChild.CalculateCost(Data);
            return newChild;

        }
        public void Solve()
        {
            BestTour = null;
            Sw.Restart();
            Sw.Start();
            Population = CreateFirstPopulation();   // stworzenie populacji poczatkowej
            

            while (Sw.ElapsedMilliseconds / 1000 < TimeLimit)   // warunek zakonczenia
            {
                for (int i = 0; i < PopulationSize; i++)
                {
                    var pc = rng.Next(1, 100);
                    if (pc <= Pc * 100) // prawdopodobienstwo krzyzowania
                    {
                        
                        var parent1 = TournamentSelection(Population);  // wybranie rodzicow selekcja turniejowa
                        var parent2 = TournamentSelection(Population);
                        while (parent2 == parent1) parent2 = TournamentSelection(Population);
                        var pmm = rng.Next(1, 100);
                        if (pmm <= Pm * 100)    // prawdopodobienstwo mutacji przed krzyzowaniem
                        {
                            var mut1 = InvertMutation(parent1);
                            Population[Population.FindIndex(x => x == parent1)] = mut1;
                            parent1 = mut1;
                            var mut2 = InvertMutation(parent1);
                            Population[Population.FindIndex(x => x == parent2)] = mut2;
                            parent2 = mut2;
                        }
                        var child1 = Pmxcrossover(parent1, parent2);    // krzyzowanie
                        var child2 = Pmxcrossover(parent2, parent1);
                        var pm = rng.Next(1, 100);
                        if (pm <= Pm * 100) // pradopodobienstwo mutacji po krzyzowaniu
                        {
                            child1 = InvertMutation(child1);
                            child2 = InvertMutation(child2);
                        }
                        var selectionList = new List<Tour> {parent1, parent2, child1, child2};
                        var best1 = CrossoverSelection(selectionList);
                        selectionList.Remove(best1);
                        var best2 = CrossoverSelection(selectionList);
                        Population[Population.FindIndex(x => x == parent1)] = best1;
                        Population[Population.FindIndex(x => x == parent2)] = best2;
                    }
                    
                }
                var min = Population[0];
                foreach (var tour in Population.Skip(1))
                {
                    if (tour.Cost < min.Cost)
                        min = tour;
                }
                if (BestTour == null || min.Cost<BestTour.Cost)
                    BestTour = min;
            }
            Console.WriteLine($"Czas: {Sw.Elapsed}");
            Console.WriteLine($"Wynik: {BestTour.Cost}");
            Sw.Stop();
        }

        public Solver(Data data)
        {
            Data = data;
        }
    }
}
