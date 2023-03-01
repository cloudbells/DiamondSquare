using System.Drawing;

namespace cloudbells.TerrainGeneration
{
    internal class HeightmapGenerator
    {
        private readonly int SIZE = 257; // This many rows and columns.
        private readonly int ROUGHNESS = 16; // Between 0 and 256. Anything >256 will be the same a 256 due to normalization.
        private readonly int MAX_HEIGHT = 8;
        private double[][] values;
        private Random rng;

        /// <summary>
        /// Sets the values of the top middle, bottom middle, left middle, and right middle cells of the square formed by the given constraints to the average
        /// of the middle cell and two neighbouring corners in that square.
        /// </summary>
        /// <param name="row1">the first row index of the square (y-coordinate)</param>
        /// <param name="col1">the first column index of the square (x-coordinate)</param>
        /// <param name="row2">the second row of the square (y-coordinate)</param>
        /// <param name="col2">the second column of the square (y-coordinate)</param>
        private void Square(int row1, int col1, int row2, int col2, double rand)
        {
            int middleRowIndex = row2 - (row2 - row1) / 2;
            int middleColIndex = col2 - (col2 - col1) / 2;
            double middleValue = values[middleRowIndex][middleColIndex];
            double topLeftValue = values[row1][col1];
            double topRightValue = values[row1][col2];
            double bottomLeftValue = values[row2][col1];
            double bottomRightValue = values[row2][col2];
            if (values[row1][middleColIndex] < 0) // Need to check this because in a previous recursion this cell might've already been assigned a value.
            {
                values[row1][middleColIndex] = (topLeftValue + topRightValue + middleValue) / 3 + (rng.NextDouble() * rand * 2) - rand; // Top middle cell.
                values[row1][middleColIndex] = values[row1][middleColIndex] < 0 ? 0 : values[row1][middleColIndex];
            }
            if (values[row2][middleColIndex] < 0)
            {
                values[row2][middleColIndex] = (bottomLeftValue + bottomRightValue + middleValue) / 3 + (rng.NextDouble() * rand * 2) - rand; // Bottom middle cell.
                values[row2][middleColIndex] = values[row2][middleColIndex] < 0 ? 0 : values[row2][middleColIndex];
            }
            if (values[middleRowIndex][col1] < 0)
            {
                values[middleRowIndex][col1] = (topLeftValue + bottomLeftValue + middleValue) / 3 + (rng.NextDouble() * rand * 2) - rand; // Left middle cell.
                values[middleRowIndex][col1] = values[middleRowIndex][col1] < 0 ? 0 : values[middleRowIndex][col1];
            }
            if (values[middleRowIndex][col2] < 0)
            {
                values[middleRowIndex][col2] = (topRightValue + bottomRightValue + middleValue) / 3 + (rng.NextDouble() * rand * 2) - rand; // Right middle cell
                values[middleRowIndex][col2] = values[middleRowIndex][col2] < 0 ? 0 : values[middleRowIndex][col2];
            }
            // Stop when we reach a 2x2 square.
            if (row1 + 2 != row2)
            {
                rand /= 2;
                Diamond(row1, col1, middleRowIndex, middleColIndex, rand); // Top left square.
                Diamond(row1, middleColIndex, middleRowIndex, col2, rand); // Top right square.
                Diamond(middleRowIndex, col1, row2, middleColIndex, rand); // Bottom left square.
                Diamond(middleRowIndex, middleColIndex, row2, col2, rand); // Bottom right square.
            }
        }

        /// <summary>
        /// Sets the value of the cell in the middle of the square formed by the given constraints to the average of the values of the corners of that square.
        /// </summary>
        /// <param name="row1">the first row index of the square (y-coordinate)</param>
        /// <param name="col1">the first column index of the square (x-coordinate)</param>
        /// <param name="row2">the second row of the square (y-coordinate)</param>
        /// <param name="col2">the second column of the square (y-coordinate)</param>
        private void Diamond(int row1, int col1, int row2, int col2, double rand)
        {
            int rowIndex = row2 - (row2 - row1) / 2;
            int colIndex = col2 - (col2 - col1) / 2;
            if (values[rowIndex][colIndex] < 0)
            {
                values[rowIndex][colIndex] =
                    (values[row1][col1] + values[row1][col2] + values[row2][col1] + values[row2][col2]) / 4 + (rng.NextDouble() * rand * 2) - rand;
                values[rowIndex][colIndex] = values[rowIndex][colIndex] < 0 ? 0 : values[rowIndex][colIndex];
            }
            Square(row1, col1, row2, col2, rand);
        }

        /// <summary>
        /// Generates a bitmap from the given values.
        /// </summary>
        /// <param name="values">the values to generate a bitmap from</param>
        /// <returns>the bitmap</returns>
        private Bitmap GenerateBitmap(double[][] values)
        {
            Bitmap bitmap = new Bitmap(SIZE, SIZE);
            double max = 0;
            double min = 0;
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    double value = values[row][col];
                    if (value > max)
                    {
                        max = value;
                    }
                    if (value < min)
                    {
                        min = value;
                    }
                }
            }
            // Normalize heights to color values (0-255).
            double heightMultiplier = 256 / (max - min);
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    int colorValue = (int)Math.Round(values[row][col] * heightMultiplier - 1);
                    colorValue = colorValue < 0 ? 0 : colorValue;
                    bitmap.SetPixel(col, row, Color.FromArgb(255, colorValue, colorValue, colorValue));
                }
            }
            return bitmap;
        }

        /// <summary>
        /// Generates the height map values using the Diamond-square algorithm.
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public Bitmap GenerateHeightmap()
        {
            rng = new Random(Environment.TickCount);
            values = new double[SIZE][];
            for (int i = 0; i < SIZE; i++)
            {
                values[i] = new double[SIZE];
                for (int j = 0; j < SIZE; j++)
                {
                    values[i][j] = double.MinValue;
                }
            }
            // Initialize the corners.
            values[0][0] = rng.NextDouble() * MAX_HEIGHT;
            values[0][SIZE - 1] = rng.NextDouble() * MAX_HEIGHT;
            values[SIZE - 1][0] = rng.NextDouble() * MAX_HEIGHT;
            values[SIZE - 1][SIZE - 1] = rng.NextDouble() * MAX_HEIGHT;
            // Recursively calculate the values.
            Diamond(0, 0, SIZE - 1, SIZE - 1, ROUGHNESS);
            return GenerateBitmap(values);
        }
    }
}
