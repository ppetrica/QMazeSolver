using System;
using System.Collections.Generic;


namespace QMazeSolver
{
    public class QLearning
    {
        static Random rnd = new Random(1);

        public static double[][] CreateQuality(int ns)
        {
            double[][] Q = new double[ns][];
            for (int i = 0; i < ns; ++i)
            {
                Q[i] = new double[ns];
                for (int j = 0; j < ns; ++j) {
                    //Q[i][j] = double.MinValue;
                }
            }
            return Q;
        }

        static List<int> GetPossNextStates(int s, double[][] R)
        {
            List<int> result = new List<int>();
            for (int j = 0; j < R.Length; ++j)
                if (R[s][j] != double.MinValue) result.Add(j);
            return result;
        }

        static int GetRandNextState(int s, double[][] R)
        {
            List<int> possNextStates = GetPossNextStates(s, R);
            int ct = possNextStates.Count;
            int idx = rnd.Next(0, ct);
            return possNextStates[idx];
        }

        public static void Train(double[][] R, double[][] Q, int goal, double gamma, double lrnRate, int maxEpochs, int maxIterations)
        {
            List<int> validStates = new List<int>();
            for (int i = 0; i < R.Length; ++i) {
                bool found = false;
                for (int j = 0; j < R.Length; ++j) {
                    if (R[i][j] != double.MinValue) {
                        found = true;
                        break;
                    }
                }

                if (found)
                    validStates.Add(i);
            }

            for (int epoch = 0; epoch < maxEpochs; ++epoch)
            {
                int idx = rnd.Next(0, validStates.Count);
                int currState = validStates[idx];
                for (int it = 0; it < maxIterations && currState != goal; ++it)
                {
                    int nextState = GetRandNextState(currState, R);
                    List<int> possNextNextStates = GetPossNextStates(nextState, R);
                    double maxQ = double.MinValue;
                    for (int j = 0; j < possNextNextStates.Count; ++j)
                    {
                        int nns = possNextNextStates[j];  // short alias
                        double q = Q[nextState][nns];
                        if (q > maxQ) maxQ = q;
                    }
                    Q[currState][nextState] =
        ((1 - lrnRate) * Q[currState][nextState]) +
        (lrnRate * (R[currState][nextState] + (gamma * maxQ)));
                    currState = nextState;
                } // while
            } // for
        } // Train

        public static List<int> Walk(int start, int goal, double[][] Q, double[][] R)
        {
            List<int> steps = new List<int>();
            int curr = start;
            while (curr != goal)
            {
                int next = -1;
                List<int> maxs = ArgMax(Q[curr], R[curr]);
                for (int i = 0; i < maxs.Count; ++i) {
                    if (!steps.Contains(maxs[i])) {
                        next = maxs[i];
                        break;
                    }
                }

                if (next == -1)
                    throw new Exception("Nu");

                steps.Add(curr);
                curr = next;
            }

            return steps;
        }

        static List<int> ArgMax(double[] vector, double[] R)
        {
            double maxVal = double.MinValue;
            List<int> maxs = new List<int>();
            for (int i = 0; i < vector.Length; ++i)
            {
                if (R[i] != double.MinValue)
                {
                    if (vector[i] > maxVal)
                    {
                        maxs.Clear();
                        maxVal = vector[i];
                        maxs.Add(i);
                    } else if (vector[i] == maxVal) {
                        maxs.Add(i);
                    }
                }
            }
            return maxs;
        }
    }
}
