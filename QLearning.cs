using System;
using System.Collections.Generic;


namespace QMazeSolver
{
    public class QLearning
    {
        static Random rnd = new Random(1);

        static List<int> GetNextStates(int s, double[][] R)
        {
            List<int> result = new List<int>();
            for (int j = 0; j < R.Length; ++j)
                if (R[s][j] != double.MinValue) result.Add(j);
            return result;
        }

        public static void Solve(double[][] R, double[][] Q, int goal, double gamma,
                                 double alpha, int nEpochs, int maxIterations)
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

            for (int e = 0; e < nEpochs; ++e)
            {
                int idx = rnd.Next(0, validStates.Count);
                int cs = validStates[idx];
                for (int it = 0; it < maxIterations; ++it)
                {
                    if (cs == goal)
                        break;

                    List<int> possNextStates = GetNextStates(cs, R);

                    int ns = possNextStates[rnd.Next(0, possNextStates.Count)];

                    List<int> ps = GetNextStates(ns, R);

                    double maxQ = double.MinValue;
                    for (int j = 0; j < ps.Count; ++j)
                    {
                        int nns = ps[j];
                        double q = Q[ns][nns];
                        if (q > maxQ) maxQ = q;
                    }

                    Q[cs][ns] = ((1 - alpha) * Q[cs][ns]) + (alpha * (R[cs][ns] + (gamma * maxQ)));
                    cs = ns;
                }
            }
        }

        public static List<int> Walk(int start, int goal, double[][] Q, double[][] R)
        {
            List<int> steps = new List<int>();
            int curr = start;
            while (curr != goal)
            {
                int next = -1;
                List<int> maxs = Max(Q[curr], R[curr]);
                for (int i = 0; i < maxs.Count; ++i) {
                    if (!steps.Contains(maxs[i])) {
                        next = maxs[i];
                        break;
                    }
                }

                if (next == -1)
                    throw new Exception("Nu se poate gasi destinatia");

                steps.Add(curr);
                curr = next;
            }

            return steps;
        }

        static List<int> Max(double[] vector, double[] R)
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
