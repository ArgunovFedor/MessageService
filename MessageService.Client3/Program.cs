public class Visitor
{
    // static Semaphore _semaphore = new Semaphore(3, 3);
    Thread myThread;
    // int _count = 1; //счётчик посещений. Один раз сходить в клуб - достаточно
    public Visitor(int i)
    {
        myThread = new Thread(Fun);
        myThread.Name = $"Посетитель #{i}";
        myThread.Start();
    }
    public void Fun()
    {
        // _semaphore.WaitOne();
        Console.WriteLine($"{Thread.CurrentThread.Name} входит в клуб");
        Console.WriteLine($"{Thread.CurrentThread.Name} веселится");
        Thread.Sleep(1000); //безудержное веселье
        Console.WriteLine($"{Thread.CurrentThread.Name} покидает клуб");
       //  _semaphore.Release();
    }
}
internal class Program
{
    static void Main(string[] args)
    {
        for (int i = 1; i < 7; i++)
        {
            Visitor visitor = new Visitor(i);
        }
    }
}