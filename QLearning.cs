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
                Q[i] = new double[ns];
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

        public static void Train(double[][] R, double[][] Q, int goal, double gamma, double lrnRate, int maxEpochs)
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
                while (true)
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
                    if (currState == goal) break;
                } // while
            } // for
        } // Train

        public static List<int> Walk(int start, int goal, double[][] Q)
        {
            List<int> steps = new List<int>();
            int curr = start;
            int next;
            while (curr != goal)
            {
                next = ArgMax(Q[curr]);
                steps.Add(curr);
                curr = next;
            }

            return steps;
        }

        static int ArgMax(double[] vector)
        {
            double maxVal = vector[0]; int idx = 0;
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > maxVal)
                {
                    maxVal = vector[i]; idx = i;
                }
            }
            return idx;
        }
    }
}
