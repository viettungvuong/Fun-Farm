using System;

[Serializable]
public class Pair{
    public Pair() {
        this.First = 0;
        this.Second = 0;
    }

    public Pair(int first, int second) {
        this.First = first;
        this.Second = second;
    }

    public int First;
    public int Second;

    public new string ToString(){
        return First.ToString() + " - " + Second.ToString();
    }

    public static int CalculateTimeDifference(Pair time1, Pair time2)
    {
        int totalMinutesInDay = 24 * 60;

        int totalMinutes1 = time1.First * 60 + time1.Second;
        int totalMinutes2 = time2.First * 60 + time2.Second;

        int difference = (totalMinutes2 - totalMinutes1 + totalMinutesInDay) % totalMinutesInDay;

        return difference;
    }
};


