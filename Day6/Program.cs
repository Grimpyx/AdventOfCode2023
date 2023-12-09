using Day6;

PartTwo();

void PartOne()
{
    string[] data = File.ReadAllLines("./data_complete.txt");
    int[] raceTimes = DataInterpret.GetNumbersArrayFrom(data[0], ' ');
    int[] distanceRecords = DataInterpret.GetNumbersArrayFrom(data[1], ' ');
    foreach (string d in data)
    {
        Console.WriteLine(d);
    }
    Console.WriteLine("--------------------------------\n");

    // The assignment describes a race. You win the further you get in the same amount of time.
    // You start with v = 0. (t=0 -> v=0, origin of function)
    // You can spend time of the race to increase your speed in a linear ratio. Spend 4 time units to gain 4 velocity units. (v = t_offset)
    // Because you spend time from the race, we need to move the x-offset of the function. ( y(t) = v(t - t_offset) ) where v is velocity=time_spent.
    // We simplify to f(t) = v(t - v). We call this function f(x), where v is a constant.
    // But if we can, we would like to not test a bunch of numeric options for v to get our max value. So we will try different things.
    // If we plot this for values v=[0:5] we see it looks like a parable (x^2 something).
    // Because it goes through the origin and is not offset at all, it is y=kx^2, where k is some factor.
    // Testing a few values, 1/4 = 0.25 leads to a perfect parable.
    // We call this parable function g(t) = 0.25t^2.
    //
    // Two key observations:
    // (1) For all even numbers t the value of g(t) will be exactly that of f(t)
    // (2) for all odd numbers t the value of g(t) will be exactly that of f(t) + 0.25
    // Now we should be able to use g(t) to find the max value, and then reverse f(t) to find v.

    // Question is, what value for v maximizes the value of g(t) at the point t=t_totalRaceTime?

    int[] waysWeCanWinEachRace = new int[raceTimes.Length];
    for (int timesIndex = 0; timesIndex < raceTimes.Length; timesIndex++)
    {
        int time = raceTimes[timesIndex];
        int recordDistance = distanceRecords[timesIndex];


        // We have the time. We use g(t) to get the maximum distance we can obtain for that race
        // if t == odd or even ,respectively
        float maximumDistance = 0.25f * time * time;
        if (time % 2 == 1) maximumDistance -= 0.25f;

        // f(t, v) = 0 should give our time spent charging, which is the same as our starting speed
        // We need to reverse f(t, v) first though: s_max = v(t-v) = vt-v^2  =>  v^2 - tv + s_max = 0
        // using PQ formula, and we know it's the positive
        float p, q;
        p = -time;
        q = maximumDistance;
        double v_1 = (double)(-p / 2) + Math.Sqrt((p * p / 4) - q); // (p / 2)^2 = (p / 2) * (p / 2) = (p*p/4)
        double v_2 = (double)(-p / 2) - Math.Sqrt((p * p / 4) - q);

        Console.WriteLine("v1 = " + v_1 + "   v2 = " + v_2);

        double v = Math.Max(v_1, v_2);

        // In the assignment, they ask how many ways we can win each race.
        // So we start out with the max value of v, and decrement it until our speed from f(t, v)
        // f(t, v) = distanceReached = v(t-v)
        waysWeCanWinEachRace[timesIndex] = 0;
        for (int i = 0; i < 99999; i++)
        {
            double new_v = v - i;
            double distance = new_v * (time - new_v);
            Console.WriteLine("Comparing: [{0}, {1}]", distance, recordDistance);

            if (distance <= recordDistance) break;
            else waysWeCanWinEachRace[timesIndex]++;
        }
        for (int i = 1; i < 99999; i++)
        {
            double new_v = v + i;
            double distance = new_v * (time - new_v);
            Console.WriteLine("Comparing: [{0}, {1}]", distance, recordDistance);

            if (distance <= recordDistance) break;
            else waysWeCanWinEachRace[timesIndex]++;
        }
    }

    double product = 1;
    for (int i = 0; i < waysWeCanWinEachRace.Length; i++)
    {
        product *= waysWeCanWinEachRace[i];
        Console.WriteLine("Times we can win: " + waysWeCanWinEachRace[i]);
    }
    Console.WriteLine(" > Product: " + product);
}
void PartTwo()
{
    bool writeDebug = false;

    // We change the way we interpret the data
    string[] data = File.ReadAllLines("./data_complete.txt");
    long[] raceTimes = { long.Parse(DataInterpret.GetNumbersCharsFrom(data[0])) };
    long[] distanceRecords = { long.Parse(DataInterpret.GetNumbersCharsFrom(data[1])) };
    Console.WriteLine("Race time: " + raceTimes[0]);
    Console.WriteLine("   Record: " + distanceRecords[0]);
    Console.WriteLine("---------------------------------------\n");

    // The assignment describes a race. You win the further you get in the same amount of time.
    // You start with v = 0. (t=0 -> v=0, origin of function)
    // You can spend time of the race to increase your speed in a linear ratio. Spend 4 time units to gain 4 velocity units. (v = t_offset)
    // Because you spend time from the race, we need to move the x-offset of the function. ( y(t) = v(t - t_offset) ) where v is velocity=time_spent.
    // We simplify to f(t) = v(t - v). We call this function f(x), where v is a constant.
    // But if we can, we would like to not test a bunch of numeric options for v to get our max value. So we will try different things.
    // If we plot this for values v=[0:5] we see it looks like a parable (x^2 something).
    // Because it goes through the origin and is not offset at all, it is y=kx^2, where k is some factor.
    // Testing a few values, 1/4 = 0.25 leads to a perfect parable.
    // We call this parable function g(t) = 0.25t^2.
    //
    // Two key observations:
    // (1) For all even numbers t the value of g(t) will be exactly that of f(t)
    // (2) for all odd numbers t the value of g(t) will be exactly that of f(t) + 0.25
    // Now we should be able to use g(t) to find the max value, and then reverse f(t) to find v.

    // Question is, what value for v maximizes the value of g(t) at the point t=t_totalRaceTime?

    int[] waysWeCanWinEachRace = new int[raceTimes.Length];
    for (int timesIndex = 0; timesIndex < raceTimes.Length; timesIndex++)
    {
        long time = raceTimes[timesIndex];
        long recordDistance = distanceRecords[timesIndex];


        // We have the time. We use g(t) to get the maximum distance we can obtain for that race
        // if t == odd or even ,respectively
        float maximumDistance = 0.25f * time * time;
        if (time % 2 == 1) maximumDistance -= 0.25f;

        // f(t, v) = 0 should give our time spent charging, which is the same as our starting speed
        // We need to reverse f(t, v) first though: s_max = v(t-v) = vt-v^2  =>  v^2 - tv + s_max = 0
        // using PQ formula, and we know it's the positive
        float p, q;
        p = -time;
        q = maximumDistance;
        double v_1 = (double)(-p / 2) + Math.Sqrt((p * p / 4) - q); // (p / 2)^2 = (p / 2) * (p / 2) = (p*p/4)
        double v_2 = (double)(-p / 2) - Math.Sqrt((p * p / 4) - q);

        Console.WriteLine("v1 = " + v_1 + "   v2 = " + v_2);

        double v = Math.Max(v_1, v_2);

        // In the assignment, they ask how many ways we can win each race.
        // So we start out with the max value of v, and decrement it until our speed from f(t, v)
        // f(t, v) = distanceReached = v(t-v)
        waysWeCanWinEachRace[timesIndex] = 0;
        long timesGoneDown = -1;
        long timesGoneUp = 0; // had to change the for loop to a while loop
        while (true)
        {
            timesGoneDown++;

            double new_v = v - timesGoneDown;
            double distance = new_v * (time - new_v);
            if (writeDebug) Console.WriteLine("Comparing: [{0}, {1}]", distance, recordDistance);

            if (distance <= recordDistance) break;
            else waysWeCanWinEachRace[timesIndex]++;
        }
        while (true)
        {
            timesGoneUp++;

            double new_v = v + timesGoneUp;
            double distance = new_v * (time - new_v);
            if (writeDebug) Console.WriteLine("Comparing: [{0}, {1}]", distance, recordDistance);

            if (distance <= recordDistance) break;
            else waysWeCanWinEachRace[timesIndex]++;
        }
        Console.WriteLine(" -   timesGoneUp: " + timesGoneUp);
        Console.WriteLine(" - timesGoneDown: " + timesGoneDown);
    }

    double product = 1;
    for (int i = 0; i < waysWeCanWinEachRace.Length; i++)
    {
        product *= waysWeCanWinEachRace[i];
        Console.WriteLine("Times we can win: " + waysWeCanWinEachRace[i]);
    }
    Console.WriteLine(" > Product: " + product);
}