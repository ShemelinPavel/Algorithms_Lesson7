using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra
{
    /// <summary>
    /// перечисление цветов вершин графа для алгоритам поиска в глубину
    /// White - начальный цвет всех вершин
    /// Grey - найденная вершина
    /// Black - обработанная вершина
    /// </summary>
    public enum NodesColours
    {
        White, Grey, Black
    }

    /// <summary>
    /// класс объекта Граф
    /// </summary>
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
        /// рекурсивный обход в глубину
        /// </summary>
        /// <param name="node">имя узла</param>
        /// <param name="nodesStatus">словарь с цветами вершин</param>
        /// <param name="graphSubsets">словарь номеров подмножеств</param>
        /// <param name="graphSubsetsCount">счетчик подмножеств</param>
        private void DFS_Recurs(string node, ref Dictionary<string, NodesColours> nodesStatus, ref Dictionary<string, uint> graphSubsets, ref uint graphSubsetsCount )
        {
            nodesStatus[node] = NodesColours.Grey;
            
            //коллекция "соседей" вершины
            Dictionary<string, uint> neighbors = Nodes[node];

            foreach (var neigh_item in neighbors)
            {
                if (nodesStatus[neigh_item.Key] == NodesColours.White) DFS_Recurs( neigh_item.Key, ref nodesStatus, ref graphSubsets, ref graphSubsetsCount );
            }

            graphSubsets[node] = graphSubsetsCount;
            nodesStatus[node] = NodesColours.Black;
        }

        /// <summary>
        /// реализация алгоритма обход в глубину
        /// </summary>
        /// <returns>количество подмножеств - 1 = граф связанный</returns>
        public int DFS()
        {
            Dictionary<string, NodesColours> nodesStatus = new Dictionary<string, NodesColours>();
            Dictionary<string, uint> graphSubsets = new Dictionary<string, uint>();
            foreach (var item in Nodes)
            {
                nodesStatus.Add( item.Key, NodesColours.White );
                graphSubsets.Add( item.Key, 0 );
            }

            uint graphSubsetsCounter = 0;
            foreach (var node in Nodes)
            {
                if (nodesStatus[node.Key] == NodesColours.White)
                {
                    graphSubsetsCounter++;
                    DFS_Recurs( node.Key, ref nodesStatus, ref graphSubsets, ref graphSubsetsCounter );
                }
            }
            return graphSubsets.Values.Distinct().Count();
        }

        /// <summary>
        /// алгоритм Дейкстры
        /// </summary>
        /// <param name="nodeA">исходный узел</param>
        /// <param name="nodeB">конечный узел</param>
        /// <returns>список узлов - ма</returns>
        public bool Dijkstra( string nodeA, string nodeB, out int numGraphSubsets, out string[] bestPath, out string txtLog)
        {

            #region перехват ошибок аргументов
            if (!( Nodes.ContainsKey( nodeA ) )) throw new ArgumentException( $"Узел {nodeA} отсутствует в графе!" );
            if (!( Nodes.ContainsKey( nodeB ) )) throw new ArgumentException( $"Узел {nodeB} отсутствует в графе!" );
            #endregion  перехват ошибок аргументов

            numGraphSubsets = DFS();

            if (numGraphSubsets != 1)
            {
                bestPath = new string[0];
                txtLog = $"Проверка графа на связанность обходом в грубину -> неудачно.\nОбнаружено {numGraphSubsets} подможеств связанных графов";
                return false;
            }

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
            List<string> bestPathList = new List<string> { nextPathNode };

            while (nextPathNodeCost != Costs[nodeA])
            {
                Dictionary<string, uint> neighbors = Nodes[nextPathNode];
                foreach (var negh_item in neighbors)
                {
                    if (nextPathNodeCost - negh_item.Value == Costs[negh_item.Key])
                    {
                        nextPathNode = negh_item.Key;
                        nextPathNodeCost = Costs[negh_item.Key];
                        bestPathList.Add( nextPathNode );
                        break;
                    }
                }
            }

            #endregion реализация выбора лучшего маршрута

            bestPathList.Reverse();
            bestPath = bestPathList.ToArray();

            StringBuilder txt = new StringBuilder();
            txt.AppendLine( "Проверка графа на связанность обходом в грубину->успешно." );
            txt.AppendLine( $"Лучший маршрут из точки {nodeA} в точку {nodeB}:" );
            foreach (var item in bestPath)
            {
                txt.Append( $" {item}" );
            }

            txtLog = txt.ToString();
            return true;
        }
    }

    class Program
    {
        static void Main( string[] args )
        {

            Dictionary<string, uint> neighbors = null;

            Graph gr = new Graph();

            #region заполнение связанного графа

            //за основу возьмем связанный граф из вэбинара

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

            #endregion заполнение связанного графа

            //Дейкстра
            bool result = gr.Dijkstra( "x2", "x6", out int numberOfGraphSubsets, out string[] bestPath, out string txtLog );

            Console.WriteLine( txtLog );
            Console.WriteLine();

            //новый граф

            gr = new Graph();

            #region заполнение несвязанного графа

            //за основу возьмем несвязанный граф из вэбинара, но добавим точку x11 без соседей

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

            //добавляем вершины не связанные с предыдущими

            //соседи "x8"
            neighbors = new Dictionary<string, uint>
            {
                { "x9", 15 }
            };
            gr.Nodes.Add( "x8", neighbors );

            //соседи "x9"
            neighbors = new Dictionary<string, uint>
            {
                { "x8", 15 },
                { "x10", 17 }
            };
            gr.Nodes.Add( "x9", neighbors );

            //соседи "x10"
            neighbors = new Dictionary<string, uint>
            {
                { "x9", 17 }
            };
            gr.Nodes.Add( "x10", neighbors );

            //соседи "x11"
            neighbors = new Dictionary<string, uint>();
            gr.Nodes.Add( "x11", neighbors );

            #endregion заполнение несвязанного графа

            //Дейкстра
            bool resul1 = gr.Dijkstra( "x2", "x6", out int numberOfGraphSubsets1, out string[] bestPath1, out string txtLog1 );

            Console.WriteLine( txtLog1 );

            Console.ReadKey();
        }
    }
}
