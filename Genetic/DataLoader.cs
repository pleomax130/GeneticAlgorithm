using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TspLibNet;

namespace Genetic
{
    class DataLoader
    {
        public int Size { get; set; }
        public string FilePath { get; set; }
        private TspLib95 _library;

        public void LoadData(out int[][] array, out int optimal, out int size, string fileName)
        {
            FilePath = fileName;
            optimal = 0;
            InitializeLibrary();
            var instance = Convert(_library.Items[0]);
            var info = instance[0].Split(' ');
            optimal = int.Parse(info[1]);
            Size = int.Parse(info[0]);
            size = Size;
            array = new int[Size][];
            for (int i = 0; i < Size; i++)
            {
                array[i] = new int[Size];
            }
            for (int i = 1; i < Size + 1; i++)
            {
                string[] numbers = instance[i].Split(' ').ToArray();
                for (int j = 0; j < Size; j++)
                {
                    array[i - 1][j] = int.Parse(numbers[j]);
                }
            }
        }

        private void InitializeLibrary()
        {
            List<string> atsp = new List<string> { "br17", "ft53", "ft70", "ftv33", "ftv35", "ftv38", "ftv44", "ftv47", "ftv55", "ftv64", "ftv70", "ftv170", "kro124p", "p43", "rbg323", "rbg358", "rbg403", "rbg443", "ry48p" };
            var name = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string tspDir = name + "\\packages\\TSPLib.Net.1.1.5\\TSPLIB95";
            _library = new TspLib95(tspDir);
            if (atsp.Contains(FilePath))
            {
                _library.LoadATSP(FilePath);
            }
            else
            {
                _library.LoadTSP(FilePath);
            }
        }

        private List<string> Convert(TspLib95Item item)
        {
            int size = item.Problem.NodeProvider.CountNodes(); // Pierwsza liczba rozmiar problemu
            int solution = (int)item.OptimalTourDistance; // Druga liczba rozwiązanie
            var nodes = item.Problem.NodeProvider.GetNodes();
            List<string> lines = new List<string>();
            lines.Add(size + " " + solution);

            // Pobieranie wag krawędzi.
            for (int i = 0; i < nodes.Count; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < nodes.Count; j++)
                {
                    var edge = item.Problem.EdgeWeightsProvider.GetWeight(nodes[i], nodes[j]);
                    sb.Append(edge + " ");
                }
                lines.Add(sb.ToString());
            }
            return lines;
        }
    }
}
