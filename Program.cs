using System;
using System.Collections.Generic;


namespace Project2
{
    class CartesianProduct
    {
        public List<List<int>> directions;

        public CartesianProduct(int n)
        {
            this.directions = new List<List<int>>();
            calculateCartesianProduct(n);
            if(directions.Count == Math.Pow(3, n)-1) 
            Console.WriteLine("CP sucessfuly made");
        }

        public void calculateCartesianProduct(int n)
        {
            int dimension = 0;
            int[] offsets = { -1, 0, 1 };

            List<int> currentVector = new List<int>();

            Stack<(List<int>, int)> stack = new Stack<(List<int>, int)>();
            stack.Push((currentVector, dimension));


            while (stack.Count > 0)
            {
                var topItem = stack.Pop();
                currentVector = topItem.Item1;
                dimension = topItem.Item2;


                if (dimension == n)//i.e. once the vector has all the correct dimensions, add to directions
                {
                    //skip the vector to nowhere
                    if (currentVector.All(x => x == 0)) continue;

                    directions.Add(new List<int>(currentVector));
                }
                else //create all possible directions for this dimension and push it to the stack
                { 
                    foreach (int offset in offsets)
                    { 
                        List<int> newVector = new List<int>(currentVector) { offset };
                        stack.Push((newVector, dimension + 1));
                    }
                }
            }
        }

        public void show()
        {
            foreach (var direction in directions)
            {
                Console.WriteLine(string.Join(", ", direction));
            }
        }
    }
    class Matrix
    {
        public Array matrix;

        public List<int[]> entrances { get; set; }
        public int[] dimensions { get; }

        public Matrix(Array matrix)
        {
            this.matrix = matrix;
            this.entrances = new List<int[]>();
            dimensions = new int[matrix.Rank];
            for (int i = 0; i < matrix.Rank; i++)
            {
                dimensions[i] = matrix.GetLength(i);
            }
        }
        public bool isWithinBoundary(int[] vector)
        {
            for (int i = 0; i < vector.Length; i++)
            {
                if (vector[i] < 0 || vector[i] >= dimensions[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void show()
        {
            // Iterate over the rows (the first dimension)
            for (int i = 0; i < dimensions[0]; i++)
            {
                string line = ""; // Reset the line for each row
                                      // Iterate over the columns (subsequent dimensions)
                for (int j = 0; j < dimensions[1]; j++)
                {
                    var position = matrix.GetValue(i, j); // Get the value at position (i, j)
                    switch (position)
                    {
                        case 0:
                            line += "  ";  // Two spaces for 0
                            break;
                        case 1:
                            line += "# ";  // Add "# " for 1
                            break;
                        case -1:
                            line += ". ";  // Add ". " for -1
                            break;
                        default:
                            Console.Error.WriteLine($"Error unexpected value in matrix at position ({i},{j})");
                            return;
                    }
                }
                Console.WriteLine(line); // Print the complete line (row) after processing it.
            }
            Console.ReadLine();
            //Console.Clear();
        }
    }

    class BFS
    {
        Matrix MatrixFunctions;
        public Array maze { get; set; }
        public int[] start { get; }
        public int[] end { get; }
        List<int[]> directions;
        public LinkedList<int[]> solution { get; }
        public class PathTree
        {
            public int[] Value { get; set; }
            public PathTree Parent { get; set; }

            public PathTree(int[] value)
            {
                Value = value;
            }
        }

        public BFS(Matrix maze, List<List<int>> directions,int[] start, int[]end)
        {
            this.MatrixFunctions = maze;
            this.maze = maze.matrix;//make a copy of the matrix for mutation
            this.start = start;
            this.end = end;
            
            this.directions = new List<int[]>();
            // initialise directions and convert type
            foreach (var direction in directions)
            {
                this.directions.Add(direction.ToArray());
            }
            this.solution = new LinkedList<int[]>();
            run();
        }
        void run()
        {
            Queue<(int[], PathTree)> queue = new Queue<(int[], PathTree)>();
            PathTree root = new PathTree(start);


            queue.Enqueue((this.start, root));


            //maze.SetValue(1, this.start); // Mark start as visited


            while (queue.Count > 0)
            {
                //MatrixFunctions.show();
                var item = queue.Dequeue();
                var current = item.Item1;
                var parent = item.Item2;


                // Check if we have reached the end
                if (current.SequenceEqual(end))
                {
                    Console.WriteLine("Solution found.");
                    // Trace back the path from the end node to the start node
                    while (parent != null)
                    {
                        solution.AddFirst(parent.Value);
                        parent = parent.Parent;
                    }
                    return;
                }


                foreach (var direction in directions)
                {
                    // Calculate new position
                    int[] newPosition = new int[maze.Rank];
                    for (int i = 0; i < maze.Rank; i++)
                    {
                        newPosition[i] = current[i] + direction[i];
                    }


                    if (MatrixFunctions.isWithinBoundary(newPosition))
                    {
                        if ((int)maze.GetValue(newPosition) == 0)
                        {
                            PathTree here = new PathTree(newPosition); // Create a new tree node
                            here.Parent = parent;
                            queue.Enqueue((newPosition, here));
                            maze.SetValue(1, newPosition); // Mark as visited
                        }
                    }
                }
            }
            Console.WriteLine("No Path found.");
            return;
        }
    }

    class Program
    {
        static int[,] maze1 = new int[,]{
           {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
           {0,0,1,0,0,0,1,1,0,0,0,1,1,1,1,1,1},
           {1,1,0,0,0,1,1,0,1,1,1,0,0,1,1,1,1},
           {1,0,1,1,0,0,0,0,1,1,1,1,0,0,1,1,1},
           {1,1,1,0,1,1,1,1,0,1,1,0,1,1,0,0,1},
           {1,1,1,0,1,0,0,1,0,1,1,1,1,1,1,1,1},
           {1,0,0,1,1,0,1,1,1,0,1,0,0,1,0,1,1},
           {1,0,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1},
           {1,0,0,1,1,0,1,1,0,1,1,1,1,1,1,0,1},
           {1,1,1,0,0,0,1,1,0,1,1,0,0,0,0,0,1},
           {1,0,0,1,1,1,1,1,0,0,0,1,1,1,1,0,1},
           {1,0,1,0,0,1,1,1,1,1,0,1,1,1,1,0,0},
           {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};

        static int[,] maze2 = new int[,]{
           {1,0,1,1,1,1,1,1},
           {1,0,0,0,0,0,0,1},
           {1,1,1,1,1,1,0,1},
           {1,0,0,0,0,0,0,1},
           {1,0,1,1,1,1,1,1},
           {1,0,0,0,0,0,0,1},
           {1,1,1,1,1,1,0,1},
           {1,0,0,0,0,0,0,1},
           {1,0,1,1,1,1,1,1},
           {1,0,0,0,0,0,0,1},
           {1,1,1,1,1,1,0,0},
           {1,1,1,1,1,1,1,1}};
        static void searchMaze(int[,]maze, int[] start, int[] end)
        {
            Matrix matrix = new Matrix(maze);
            CartesianProduct movementBehaviour = new CartesianProduct(matrix.dimensions.Length);

            BFS algorithm = new BFS(matrix, movementBehaviour.directions, start, end);

            if (algorithm.solution.Count > 0)
            {
                foreach (int[] position in algorithm.solution)
                {
                    matrix.matrix.SetValue(-1, position);
                }
            }
            matrix.show();
        }
        static void Main(string[] args)
        {
            int[] start = { 1, 0 };
            int[] end = { 11, 16 };
            searchMaze(maze1, start, end);

            Console.WriteLine();
            int[] start2 = { 0, 1 };
            int[] end2 = { 10, 7 };
            searchMaze(maze2, start2, end2);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}