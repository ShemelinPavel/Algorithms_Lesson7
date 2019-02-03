using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra
{
    public class Graph
    {
        /// <summary>
        /// вершины графа (словарь)
        /// ключ - имя
        /// значение - ребра (словарь)
        /// ключ - имя вершины графа
        /// значение - вес ребра
        /// </summary>
        Dictionary<string, Dictionary<string, uint>> nodes;

        /// <summary>
        /// словарь весов вершин
        /// </summary>
        Dictionary<string, uint> costs;

        /// <summary>
        /// вершины графа (словарь)
        /// ключ - имя
        /// значение - ребра (словарь)
        /// ключ - имя вершины графа
        /// значение - вес ребра
        /// </summary>
        public Dictionary<string, Dictionary<string, uint>> Nodes
        {
            get { return this.nodes; }
            set { this.nodes = value; }
        }

        /// <summary>
        /// словарь весов вершин
        /// </summary>
        public Dictionary<string, uint> Costs
        {
            get { return this.costs; }
            set { this.costs = value; }
        }

        /// <summary>
        /// инициализация словаря весов вершин
        /// </summary>
        private void InitCosts()
        {
            Costs.Clear();
            foreach (var item in Nodes.Keys)
            {
                Costs.Add( item, uint.MaxValue );
            }
        }

        /// <summary>
        /// конструктор объекта Граф
        /// </summary>
        public Graph()
        {
            Nodes = new Dictionary<string, Dictionary<string, uint>>();
            Costs = new Dictionary<string, uint>();
        }

        /// <summary>
        /// алгоритм Дейкстры
        /// </summary>
        /// <param name="nodeA">исходный узел</param>
        /// <param name="nodeB">конечный узел</param>
        /// <returns>список узлов - ма</returns>
        public string[] Dijkstra( string nodeA, string nodeB )
        {
            #region перехват ошибок аргументов
            if (!( Nodes.ContainsKey( nodeA ) )) throw new ArgumentException( $"Узел {nodeA} отсутствует в графе!" );
            if (!( Nodes.ContainsKey( nodeB ) )) throw new ArgumentException( $"Узел {nodeB} отсутствует в графе!" );
            #endregion  перехват ошибок аргументов

            InitCosts();

            #region реализация поиска весов вершин

            //очередь обработки вершин
            Queue<string> qu = new Queue<string>();

            qu.Enqueue( nodeA );
            Costs[nodeA] = 0;

            while (qu.Count != 0)
            {
                //обрабатываемая вершина
                string tmpNode = qu.Dequeue();
                
                //вес вершины
                uint nodeCost = Costs[tmpNode];

                //коллекция "соседей" вершины
                Dictionary<string, uint> neighbors = Nodes[tmpNode];

                foreach (var negh_item in neighbors)
                {
                    if (negh_item.Value + nodeCost < Costs[negh_item.Key])
                    {
                        Costs[negh_item.Key] = negh_item.Value + nodeCost;
                        qu.Enqueue( negh_item.Key );
                    }
                }
            }

            #endregion реализация поиска весов вершин

            #region реализация выбора лучшего маршрута

            string nextPathNode = nodeB;
            uint nextPathNodeCost = Costs[nextPathNode];

            //коллекция для сбора маршрута
            List<string> bestPath = new List<string> { nextPathNode };

            while (nextPathNodeCost != Costs[nodeA])
            {
                Dictionary<string, uint> neighbors = Nodes[nextPathNode];
                foreach (var negh_item in neighbors)
                {
                    if (nextPathNodeCost - negh_item.Value == Costs[negh_item.Key])
                    {
                        nextPathNode = negh_item.Key;
                        nextPathNodeCost = Costs[negh_item.Key];
                        bestPath.Add( nextPathNode );
                        break;
                    }
                }
            }

            #endregion реализация выбора лучшего маршрута

            bestPath.Reverse();
            return bestPath.ToArray();
        }
    }

    class Program
    {
        static void Main( string[] args )
        {

            Dictionary<string, uint> neighbors = null;

            Graph gr = new Graph();

            #region заполнение графа

            //за основу возьмем граф из вэбинара

            //соседи "x0"
            neighbors = new Dictionary<string, uint>
            {
                { "x1", 4 },
                { "x2", 3 },
                { "x3", 3 }
            };
            gr.Nodes.Add( "x0", neighbors );

            //соседи "x1"
            neighbors = new Dictionary<string, uint>
            {
                { "x0", 4 },
                { "x2", 1 },
                { "x4", 8 },
                { "x5", 6 }
            };
            gr.Nodes.Add( "x1", neighbors );

            //соседи "x2"
            neighbors = new Dictionary<string, uint>
            {
                { "x0", 3 },
                { "x1", 1 },
                { "x3", 8 },
                { "x4", 2 }
            };
            gr.Nodes.Add( "x2", neighbors );

            //соседи "x3"
            neighbors = new Dictionary<string, uint>
            {
                { "x0", 3 },
                { "x2", 8 },
                { "x6", 4 }
            };
            gr.Nodes.Add( "x3", neighbors );

            //соседи "x4"
            neighbors = new Dictionary<string, uint>
            {
                { "x2", 2 },
                { "x1", 8 },
                { "x5", 2 },
                { "x7", 5 }
            };
            gr.Nodes.Add( "x4", neighbors );

            //соседи "x5"
            neighbors = new Dictionary<string, uint>
            {
                { "x1", 6 },
                { "x4", 2 },
                { "x7", 3 }
            };
            gr.Nodes.Add( "x5", neighbors );

            //соседи "x6"
            neighbors = new Dictionary<string, uint>
            {
                { "x3", 4 },
                { "x7", 2 }
            };
            gr.Nodes.Add( "x6", neighbors );

            //соседи "x7"
            neighbors = new Dictionary<string, uint>
            {
                { "x5", 3 },
                { "x4", 5 },
                { "x6", 2 }
            };
            gr.Nodes.Add( "x7", neighbors );

            #endregion заполнение графа

            //Дейкстра
            string[] bestPath = gr.Dijkstra( "x2", "x6" );

            foreach (var item in bestPath)
            {
                Console.Write( $" {item}" );
            }

            Console.ReadKey();
        }
    }
}
