using etl_service.services;

ETLService service = new ETLService("ETLServiceSettings.json"); ;

string answer = "";
while (!answer.Equals("exit"))
{
    Console.WriteLine("1 - start");
    Console.WriteLine("2 - reset");
    Console.WriteLine("3 - stop");
    Console.WriteLine("4 - exit\n");

    answer = Console.ReadLine();
    Console.WriteLine();

    if (answer.Equals("start") || answer.Equals("1"))
    {
        service.Start();
    }
    if (answer.Equals("reset") || answer.Equals("2"))
    {
        service.Dispose();
        service.Start();
    }
    if (answer.Equals("stop") || answer.Equals("3"))
    {
        service.Dispose();
    }
    if (answer.Equals("exit") || answer.Equals("4"))
    {
        service.Dispose();
        Environment.Exit(0);
    }
}














